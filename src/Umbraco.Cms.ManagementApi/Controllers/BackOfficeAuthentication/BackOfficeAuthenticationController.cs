﻿using System.Security.Claims;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Web.BackOffice.Security;
using Umbraco.Extensions;
using Umbraco.New.Cms.Web.Common.Routing;

namespace Umbraco.Cms.ManagementApi.Controllers.BackOfficeAuthentication;

[ApiController]
[BackOfficeRoute("api/v{version:apiVersion}/back-office-authentication")]
[OpenApiTag("BackOfficeAuthentication")]
public class BackOfficeAuthenticationController : ManagementApiControllerBase
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IBackOfficeSignInManager _backOfficeSignInManager;
    private readonly IBackOfficeUserManager _backOfficeUserManager;

    public BackOfficeAuthenticationController(IHttpContextAccessor httpContextAccessor, IBackOfficeSignInManager backOfficeSignInManager, IBackOfficeUserManager backOfficeUserManager)
    {
        _httpContextAccessor = httpContextAccessor;
        _backOfficeSignInManager = backOfficeSignInManager;
        _backOfficeUserManager = backOfficeUserManager;
    }

    [HttpPost("authorize")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> Authorize()
    {
        HttpContext context = _httpContextAccessor.GetRequiredHttpContext();
        OpenIddictRequest? request = context.GetOpenIddictServerRequest();
        if (request == null)
        {
            return BadRequest("Unable to obtain OpenID data from the current request");
        }

        if (request.Username != null && request.Password != null)
        {
            Microsoft.AspNetCore.Identity.SignInResult result = await _backOfficeSignInManager.PasswordSignInAsync(request.Username, request.Password, true, true);
            if (result.Succeeded)
            {
                BackOfficeIdentityUser backOfficeUser = await _backOfficeUserManager.FindByNameAsync(request.Username);
                // yes, back office user can be null despite nullable reference types saying otherwise.
                // it is highly unlikely though, since we just managed to sign in the user above.
                if (backOfficeUser != null)
                {
                    ClaimsPrincipal backOfficePrincipal = await _backOfficeSignInManager.CreateUserPrincipalAsync(backOfficeUser);
                    backOfficePrincipal.SetClaim(OpenIddictConstants.Claims.Subject, backOfficeUser.Key.ToString());

                    // TODO: it is not optimal to append all claims to the token.
                    // the token size grows with each claim, although it is still smaller than the old cookie.
                    // see if we can find a better way so we do not risk leaking sensitive data in bearer tokens.
                    // maybe work with scopes instead?
                    Claim[] backOfficeClaims = backOfficePrincipal.Claims.ToArray();
                    foreach (Claim backOfficeClaim in backOfficeClaims)
                    {
                        backOfficeClaim.SetDestinations(OpenIddictConstants.Destinations.AccessToken);
                    }

                    return new SignInResult(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme, backOfficePrincipal);
                }
            }
        }

        return new ChallengeResult(new[] { OpenIddictServerAspNetCoreDefaults.AuthenticationScheme });
    }
}
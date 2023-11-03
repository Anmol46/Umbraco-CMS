﻿using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Api.Management.Factories;
using Umbraco.Cms.Api.Management.Security.Authorization.Content;
using Umbraco.Cms.Api.Management.ViewModels.Document;
using Umbraco.Cms.Core.Actions;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Web.Common.Authorization;

namespace Umbraco.Cms.Api.Management.Controllers.Document;

[ApiVersion("1.0")]
public class NotificationsController : DocumentControllerBase
{
    private readonly IAuthorizationService _authorizationService;
    private readonly IContentEditingService _contentEditingService;
    private readonly IDocumentNotificationPresentationFactory _documentNotificationPresentationFactory;

    public NotificationsController(
        IAuthorizationService authorizationService,
        IContentEditingService contentEditingService,
        IDocumentNotificationPresentationFactory documentNotificationPresentationFactory)
    {
        _authorizationService = authorizationService;
        _contentEditingService = contentEditingService;
        _documentNotificationPresentationFactory = documentNotificationPresentationFactory;
    }

    [MapToApiVersion("1.0")]
    [HttpGet("{id:guid}/notifications")]
    [ProducesResponseType(typeof(IEnumerable<DocumentNotificationResponseModel>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Notifications(Guid id)
    {
        var resource = new ContentPermissionResource(id, ActionBrowse.ActionLetter);
        AuthorizationResult authorizationResult = await _authorizationService.AuthorizeAsync(User, resource,
            $"New{AuthorizationPolicies.ContentPermissionByResource}");

        if (!authorizationResult.Succeeded)
        {
            return Forbidden();
        }

        IContent? content = await _contentEditingService.GetAsync(id);
        return content != null
            ? Ok(await _documentNotificationPresentationFactory.CreateNotificationModelsAsync(content))
            : DocumentNotFound();
    }
}

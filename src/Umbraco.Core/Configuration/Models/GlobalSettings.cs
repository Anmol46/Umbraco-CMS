// Copyright (c) Umbraco.
// See LICENSE for more details.

using System;
using System.ComponentModel;

namespace Umbraco.Cms.Core.Configuration.Models
{
    /// <summary>
    /// Typed configuration options for global settings.
    /// </summary>
    [UmbracoOptions(Constants.Configuration.ConfigGlobal)]
    public class GlobalSettings
    {
        internal const string StaticReservedPaths = "~/app_plugins/,~/install/,~/mini-profiler-resources/,~/umbraco/,"; // must end with a comma!
        internal const string StaticReservedUrls = "~/.well-known,"; // must end with a comma!
        internal const string StaticTimeOut = "00:20:00";
        internal const string StaticDefaultUILanguage = "en-US";
        internal const bool StaticHideTopLevelNodeFromPath = true;
        internal const bool StaticUseHttps = false;
        internal const int StaticVersionCheckPeriod = 7;
        internal const string StaticUmbracoPath = "~/umbraco";
        internal const string StaticIconsPath = "~/umbraco/assets/icons";
        internal const string StaticUmbracoCssPath = "~/css";
        internal const string StaticUmbracoScriptsPath = "~/scripts";
        internal const string StaticUmbracoMediaPath = "~/media";
        internal const bool StaticInstallMissingDatabase = false;
        internal const bool StaticDisableElectionForSingleServer = false;
        internal const string StaticNoNodesViewPath = "~/umbraco/UmbracoWebsite/NoNodes.cshtml";
        internal const string StaticSqlWriteLockTimeOut = "00:00:05";

        /// <summary>
        /// Gets or sets a value for the reserved URLs.
        /// It must end with a comma
        /// </summary>
        [DefaultValue(StaticReservedUrls)]
        public string ReservedUrls { get; set; } = StaticReservedUrls;

        /// <summary>
        /// Gets or sets a value for the reserved paths.
        /// It must end with a comma
        /// </summary>
        [DefaultValue(StaticReservedPaths)]
        public string ReservedPaths { get; set; } = StaticReservedPaths;

        /// <summary>
        /// Gets or sets a value for the timeout
        /// </summary>
        [DefaultValue(StaticTimeOut)]
        public TimeSpan TimeOut { get; set; } = TimeSpan.Parse(StaticTimeOut);

        /// <summary>
        /// Gets or sets a value for the default UI language.
        /// </summary>
        [DefaultValue(StaticDefaultUILanguage)]
        public string DefaultUILanguage { get; set; } = StaticDefaultUILanguage;

        /// <summary>
        /// Gets or sets a value indicating whether to hide the top level node from the path.
        /// </summary>
        [DefaultValue(StaticHideTopLevelNodeFromPath)]
        public bool HideTopLevelNodeFromPath { get; set; } = StaticHideTopLevelNodeFromPath;

        /// <summary>
        /// Gets or sets a value indicating whether HTTPS should be used.
        /// </summary>
        [DefaultValue(StaticUseHttps)]
        public bool UseHttps { get; set; } = StaticUseHttps;

        /// <summary>
        /// Gets or sets a value for the version check period in days.
        /// </summary>
        [DefaultValue(StaticVersionCheckPeriod)]
        public int VersionCheckPeriod { get; set; } = StaticVersionCheckPeriod;

        /// <summary>
        /// Gets or sets a value for the Umbraco back-office path.
        /// </summary>
        [DefaultValue(StaticUmbracoPath)]
        public string UmbracoPath { get; set; } = StaticUmbracoPath;

        /// <summary>
        /// Gets or sets a value for the Umbraco icons path.
        /// </summary>
        /// <remarks>
        /// TODO: Umbraco cannot be hard coded here that is what UmbracoPath is for
        ///       so this should not be a normal get set it has to have dynamic ability to return the correct
        ///       path given UmbracoPath if this hasn't been explicitly set.
        /// </remarks>
        [DefaultValue(StaticIconsPath)]
        public string IconsPath { get; set; } = StaticIconsPath;

        /// <summary>
        /// Gets or sets a value for the Umbraco CSS path.
        /// </summary>
        [DefaultValue(StaticUmbracoCssPath)]
        public string UmbracoCssPath { get; set; } = StaticUmbracoCssPath;

        /// <summary>
        /// Gets or sets a value for the Umbraco scripts path.
        /// </summary>
        [DefaultValue(StaticUmbracoScriptsPath)]
        public string UmbracoScriptsPath { get; set; } = StaticUmbracoScriptsPath;

        /// <summary>
        /// Gets or sets a value for the Umbraco media path.
        /// </summary>
        [DefaultValue(StaticUmbracoMediaPath)]
        public string UmbracoMediaPath { get; set; } = StaticUmbracoMediaPath;

        /// <summary>
        /// Gets or sets a value indicating whether to install the database when it is missing.
        /// </summary>
        [DefaultValue(StaticInstallMissingDatabase)]
        public bool InstallMissingDatabase { get; set; } = StaticInstallMissingDatabase;

        /// <summary>
        /// Gets or sets a value indicating whether to disable the election for a single server.
        /// </summary>
        [DefaultValue(StaticDisableElectionForSingleServer)]
        public bool DisableElectionForSingleServer { get; set; } = StaticDisableElectionForSingleServer;

        /// <summary>
        /// Gets or sets a value for the database factory server version.
        /// </summary>
        public string DatabaseFactoryServerVersion { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value for the main dom lock.
        /// </summary>
        public string MainDomLock { get; set; } = string.Empty;
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value for the path to the no content view.
        /// </summary>
        [DefaultValue(StaticNoNodesViewPath)]
        public string NoNodesViewPath { get; set; } = StaticNoNodesViewPath;

        /// <summary>
        /// Gets or sets a value for the database server registrar settings.
        /// </summary>
        public DatabaseServerRegistrarSettings DatabaseServerRegistrar { get; set; } = new DatabaseServerRegistrarSettings();

        /// <summary>
        /// Gets or sets a value for the database server messenger settings.
        /// </summary>
        public DatabaseServerMessengerSettings DatabaseServerMessenger { get; set; } = new DatabaseServerMessengerSettings();

        /// <summary>
        /// Gets or sets a value for the SMTP settings.
        /// </summary>
        public SmtpSettings Smtp { get; set; }

        /// <summary>
        /// Gets a value indicating whether SMTP is configured.
        /// </summary>
        public bool IsSmtpServerConfigured => !string.IsNullOrWhiteSpace(Smtp?.Host);

        /// <summary>
        /// An int value representing the time in milliseconds to lock the database for a write operation
        /// </summary>
        /// <remarks>
        /// The default value is 5000 milliseconds
        /// </remarks>
        /// <value>The timeout in milliseconds.</value>
        [DefaultValue(StaticSqlWriteLockTimeOut)]
        public TimeSpan SqlWriteLockTimeOut { get; } = TimeSpan.Parse(StaticSqlWriteLockTimeOut);
    }
}
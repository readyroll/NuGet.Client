// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using NuGet.Commands;
using NuGet.Common;
using NuGet.Frameworks;
using NuGet.LibraryModel;
using NuGet.Packaging;
using NuGet.Packaging.Core;
using NuGet.ProjectModel;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;

namespace NuGet.ProjectManagement.Projects
{
    /// <summary>
    /// A NuGet integrated MSBuild project.k
    /// These projects contain a project.json
    /// </summary>
    public abstract class NuGetScriptProject : NuGetProject
    {
        private readonly string _projectName;

        /// <summary>
        /// Project.json based project system.
        /// </summary>
        /// <param name="msBuildProjectPath">Path to the msbuild project file.</param>
        /// <param name="msbuildProjectSystem">Underlying msbuild project system.</param>
        public NuGetScriptProject(
            string msBuildProjectPath,
            IMSBuildNuGetProjectSystem msbuildProjectSystem)
        {
            if (msBuildProjectPath == null)
            {
                throw new ArgumentNullException(nameof(msBuildProjectPath));
            }

            MSBuildNuGetProjectSystem = msbuildProjectSystem;
            MSBuildProjectPath = msBuildProjectPath;

            _projectName = Path.GetFileNameWithoutExtension(msBuildProjectPath);

            if (string.IsNullOrEmpty(_projectName))
            {
                throw new ArgumentException(
                    string.Format(CultureInfo.CurrentCulture, Strings.InvalidProjectName, msBuildProjectPath));
            }

            var targetFramework = GetTargetFramework();
            InternalMetadata.Add(NuGetProjectMetadataKeys.TargetFramework, targetFramework);
            InternalMetadata.Add(NuGetProjectMetadataKeys.Name, msbuildProjectSystem.ProjectName);
            InternalMetadata.Add(NuGetProjectMetadataKeys.FullPath, msbuildProjectSystem.ProjectFullPath);

            var supported = new List<FrameworkName>
            {
                new FrameworkName(targetFramework.DotNetFrameworkName)
            };

            InternalMetadata.Add(NuGetProjectMetadataKeys.SupportedFrameworks, supported);
        }

        public string MSBuildProjectPath { get; }

        /// <summary>
        /// The underlying msbuild project system
        /// </summary>
        public IMSBuildNuGetProjectSystem MSBuildNuGetProjectSystem { get; }

        public override async Task<bool> InstallPackageAsync(
            PackageIdentity packageIdentity,
            DownloadResourceResult downloadResourceResult,
            INuGetProjectContext nuGetProjectContext,
            CancellationToken token)
        {
            var dependency = new PackageDependency(packageIdentity.Id, new VersionRange(packageIdentity.Version));

            return await AddDependency(dependency, token);
        }

        /// <summary>
        /// Install a package using the global packages folder.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801")]
        public abstract Task<bool> AddDependency(PackageDependency dependency, CancellationToken token);

        /// <summary>
        /// Uninstall a package from the config file.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801")]
        public abstract Task<bool> RemoveDependency(string packageId, INuGetProjectContext nuGetProjectContext, CancellationToken token);

        public override async Task<bool> UninstallPackageAsync(PackageIdentity packageIdentity, INuGetProjectContext nuGetProjectContext, CancellationToken token)
        {
            return await RemoveDependency(packageIdentity.Id, nuGetProjectContext, token);
        }

        /// <summary>
        /// Project name
        /// </summary>
        public virtual string ProjectName
        {
            get
            {
                return _projectName;
            }
        }

        /// <summary>
        /// Script executor hook
        /// </summary>
        public virtual Task<bool> ExecuteInitScriptAsync(
            PackageIdentity identity,
            string packageInstallPath,
            INuGetProjectContext projectContext,
            bool throwOnFailure)
        {
            return Task.FromResult(false);
        }

        /// <summary>
        /// Fetches target framework for project from available config files (varies based on project type)
        /// </summary>
        /// <returns>Project's target framework</returns>
        protected abstract NuGetFramework GetTargetFramework();
    }
}

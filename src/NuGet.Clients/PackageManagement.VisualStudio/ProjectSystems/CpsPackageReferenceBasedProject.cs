// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Shell;
using NuGet.Packaging;
using NuGet.Packaging.Core;
using NuGet.ProjectManagement;
using NuGet.ProjectManagement.Projects;
using NuGet.Protocol.Core.Types;
using NuGet.VisualStudio.Facade.ProjectSystem;
using EnvDTEProject = EnvDTE.Project;
using Microsoft.VisualStudio.ProjectSystem;
using Microsoft.VisualStudio.ProjectSystem.Properties;

namespace NuGet.PackageManagement.VisualStudio
{
    /// <summary>
    /// An implementation of <see cref="NuGetProject"/> that shells out to MSBuild to execute the NuGet.Build.Tasks
    /// on an arbitrary project. The build task constructs the dependency graph spec on disk. This implementation
    /// reads that file, extracting the needed information for restore.
    /// </summary>
    public class CpsPackageReferenceBasedProject : NuGetProject, INuGetIntegratedProject
    {
        private UnconfiguredProject _unconfiguredProject;
        public static CpsPackageReferenceBasedProject Create(EnvDTEProject project)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            // The project must be an IVSHierarchy.
            var hierarchy = VsHierarchyUtility.ToVsHierarchy(project);
            if (hierarchy == null)
            {
                return null;
            }

            if(!hierarchy.IsCapabilityMatch("CPS"))
            {
                return null;
            }
            var unconfiguredProject = GetUnconfiguredProject(project);
            return new CpsPackageReferenceBasedProject(unconfiguredProject, project.Name, EnvDTEProjectUtility.GetCustomUniqueName(project));
        }

        private static UnconfiguredProject GetUnconfiguredProject(EnvDTE.Project project)
        {
            IVsBrowseObjectContext context = project as IVsBrowseObjectContext;
            if (context == null && project != null)
            { // VC implements this on their DTE.Project.Object
                context = project.Object as IVsBrowseObjectContext;
            }

            return context != null ? context.UnconfiguredProject : null;
        }

        private CpsPackageReferenceBasedProject(UnconfiguredProject unconfiguredProject, string projectName, string uniqueName)
        {
            _unconfiguredProject = unconfiguredProject;
            InternalMetadata.Add(NuGetProjectMetadataKeys.Name, projectName);
            InternalMetadata.Add(NuGetProjectMetadataKeys.UniqueName, uniqueName);
        }

        public override async Task<IEnumerable<PackageReference>> GetInstalledPackagesAsync(CancellationToken token)
        {
            //TODO: Figure out the right API to call the list of packages installed.
            var configuredProject = await _unconfiguredProject.GetSuggestedConfiguredProjectAsync();
            var list = new List<PackageReference>();
            return list;
        }

        public override async Task<Boolean> InstallPackageAsync(PackageIdentity packageIdentity, DownloadResourceResult downloadResourceResult, INuGetProjectContext nuGetProjectContext, CancellationToken token)
        {
            var configuredProject = await _unconfiguredProject.GetSuggestedConfiguredProjectAsync();
            var result = await 
                configuredProject.Services.PackageReferences.AddAsync
                (packageIdentity.Id, packageIdentity.Version.ToString());
            if(!result.Added)
            {
                var existingReference = result.Reference;
                await existingReference.Metadata.SetPropertyValueAsync("Version", packageIdentity.Version.ToString());
            }

            //TODO: Set additional metadata here.
            return true;
        }

        public override async Task<Boolean> UninstallPackageAsync(PackageIdentity packageIdentity, INuGetProjectContext nuGetProjectContext, CancellationToken token)
        {
            var configuredProject = await _unconfiguredProject.GetSuggestedConfiguredProjectAsync();
            await configuredProject.Services.PackageReferences.RemoveAsync(packageIdentity.Id);
            return true;
        }
    }

}

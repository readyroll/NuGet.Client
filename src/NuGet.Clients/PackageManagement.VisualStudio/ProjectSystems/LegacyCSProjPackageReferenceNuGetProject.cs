// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.ProjectSystem;
using Microsoft.VisualStudio.ProjectSystem.Properties;
using Microsoft.VisualStudio.Shell;
using NuGet.Packaging;
using NuGet.Packaging.Core;
using NuGet.ProjectManagement;
using NuGet.Protocol.Core.Types;
using EnvDTEProject = EnvDTE.Project;

#if VS15
namespace NuGet.PackageManagement.VisualStudio.ProjectSystems
{
    /// <summary>
    /// An implementation of <see cref="NuGetProject"/> that interfaces with VS project APIs to coordinate
    /// packages in a legacy CSProj with package references.
    /// </summary>
    public class LegacyCSProjPackageReferenceNuGetProject : NuGetProject
    {
        private UnconfiguredProject _unconfiguredProject;

        public static LegacyCSProjPackageReferenceNuGetProject Create(EnvDTEProject project)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            // The project must be an IVsHierarchy
            var hierarchy = VsHierarchyUtility.ToVsHierarchy(project);
            if (hierarchy == null)
            {
                return null;
            }
            
            if (hierarchy.IsCapabilityMatch("CPS"))
            {
                return null;
            }

            if (!hierarchy.IsCapabilityMatch("PackageReference"))
            {
                return null;
            }

            var unconfiguredProject = GetUnconfiguredProject(project);
            return new LegacyCSProjPackageReferenceNuGetProject(unconfiguredProject, project.Name, EnvDTEProjectUtility.GetCustomUniqueName(project));
        }

        private static UnconfiguredProject GetUnconfiguredProject(EnvDTE.Project project)
        {
            IVsBrowseObjectContext context = project as IVsBrowseObjectContext;
            if (context == null && project != null)
            { // VC implements this on their DTE.Project.Object
                context = project.Object as IVsBrowseObjectContext;
            }

            return context?.UnconfiguredProject;
        }

        private LegacyCSProjPackageReferenceNuGetProject(UnconfiguredProject unconfiguredProject, string projectName, string uniqueName)
        {
            _unconfiguredProject = unconfiguredProject;
            InternalMetadata.Add(NuGetProjectMetadataKeys.Name, projectName);
            InternalMetadata.Add(NuGetProjectMetadataKeys.UniqueName, uniqueName);
        }

        public override async Task<IEnumerable<PackageReference>> GetInstalledPackagesAsync(CancellationToken token)
        {
            //TODO: Figure out the right API to call the list of packages installed.
            ConfiguredProject configuredProject = await _unconfiguredProject.GetSuggestedConfiguredProjectAsync();
            var list = new List<PackageReference>();
            return list;
        }

        public override async Task<Boolean> InstallPackageAsync(PackageIdentity packageIdentity, DownloadResourceResult downloadResourceResult, INuGetProjectContext nuGetProjectContext, CancellationToken token)
        {
            var configuredProject = await _unconfiguredProject.GetSuggestedConfiguredProjectAsync();
            var result = await
                configuredProject.Services.PackageReferences.AddAsync
                (packageIdentity.Id, packageIdentity.Version.ToString());
            if (!result.Added)
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
#endif

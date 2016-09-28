// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using NuGet.LibraryModel;
using NuGet.ProjectModel;
using System;
using System.Linq;

namespace NuGet.SolutionRestoreManager
{
    public static class ProjectRestoreInfoBuilder
    {
        public static IVsProjectRestoreInfo Build(PackageSpec packageSpec)
        {
            if (packageSpec == null)
            {
                throw new ArgumentNullException(nameof(packageSpec));
            }

            if (packageSpec.TargetFrameworks == null)
            {
                return null;
            }

            var targetFrameworks = new VsTargetFrameworks(
                packageSpec
                    .TargetFrameworks
                    .Select(ToTargetFrameworkInfo));

            return new VsProjectRestoreInfo(
                packageSpec.RestoreMetadata.OutputPath,
                targetFrameworks);
        }

        private static VsTargetFrameworkInfo ToTargetFrameworkInfo(TargetFrameworkInformation tfm)
        {
            var packageReferences = new VsReferenceItems(
                tfm.Dependencies
                    .Where(d => d.LibraryRange.TypeConstraint == LibraryDependencyTarget.Package)
                    .Select(ToPackageReference));

            var projectReferences = new VsReferenceItems(
                tfm.Dependencies
                    .Where(d => d.LibraryRange.TypeConstraint == LibraryDependencyTarget.ExternalProject)
                    .Select(ToProjectReference));

            return new VsTargetFrameworkInfo(
                tfm.FrameworkName.ToString(),
                packageReferences,
                projectReferences);
        }

        private static IVsReferenceItem ToPackageReference(LibraryDependency library)
        {
            var properties = new VsReferenceProperties(
                new[] { new VsReferenceProperty("Version", library.LibraryRange.VersionRange.OriginalString) }
            );
            return new VsReferenceItem(library.Name, properties);
        }

        private static IVsReferenceItem ToProjectReference(LibraryDependency library)
        {
            var properties = new VsReferenceProperties(
                new[] { new VsReferenceProperty("ProjectFileFullPath", library.LibraryRange.Name) }
            );
            return new VsReferenceItem(library.Name, properties);
        }
    }
}

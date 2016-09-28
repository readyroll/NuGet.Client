// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using NuGet.ProjectManagement;

namespace NuGet.PackageManagement.VisualStudio
{
    public interface IProjectSystemProvider
    {
        bool TryCreateNuGetProject(EnvDTE.Project project, ProjectSystemProviderContext context, out NuGetProject result);
    }
}

// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using NuGet.ProjectManagement;
using NuGet.ProjectModel;

namespace NuGet.PackageManagement.VisualStudio
{
    /// <summary>
    /// Project system data cache that stores project metadata indexed by multiple names,
    /// e.g. EnvDTE.Project can be retrieved by name (if non-conflicting), unique name or 
    /// custom unique name.
    /// </summary>
    public interface IProjectSystemCache
    {
        bool TryGetNuGetProject(string name, out NuGetProject nuGetProject);

        bool TryGetDTEProject(string name, out EnvDTE.Project project);

        bool TryGetProjectRestoreInfo(string name, out PackageSpec packageSpec);

        /// <summary>
        /// Finds a project name by short name, unique name or custom unique name.
        /// </summary>
        /// <param name="name">name of the project</param>
        /// <param name="projectNames">project name instance</param>
        /// <returns>true if the project name with the specified name is found.</returns>
        bool TryGetProjectNames(string name, out ProjectNames EnvDTEProjectName);

        /// <summary>
        /// Tries to find a project by short name. Returns the project name if and only if it is non-ambiguous.
        /// </summary>
        bool TryGetProjectNameByShortName(string name, out ProjectNames EnvDTEProjectName);

        bool ContainsKey(string name);

        IReadOnlyList<NuGetProject> GetNuGetProjects();

        /// <summary>
        /// Returns all cached projects.
        /// </summary>
        IReadOnlyList<EnvDTE.Project> GetEnvDTEProjects();

        /// <summary>
        /// Determines if a short name is ambiguous
        /// </summary>
        /// <param name="shortName">short name of the project</param>
        /// <returns>true if there are multiple projects with the specified short name.</returns>
        bool IsAmbiguous(string shortName);

        /// <summary>
        /// Adds a project to the project cache. If the project already exists in the cache, this
        /// this operation does nothing.
        /// </summary>
        /// <param name="projectNames">The project name.</param>
        /// <param name="dteProject">The VS project.</param>
        /// <param name="nuGetProject">The NuGet project.</param>
        /// <returns>
        /// Returns true if the project was newly added to the cache. Returns false if the project
        /// was already in the cache.
        /// </returns>
        bool AddProject(ProjectNames projectName, EnvDTE.Project dteProject, NuGetProject nuGetProject);

        bool AddProjectRestoreInfo(ProjectNames projectName, PackageSpec packageSpec);

        /// <summary>
        /// Removes a project and returns the project name instance of the removed project.
        /// </summary>
        /// <param name="name">name of the project to remove.</param>
        void RemoveProject(string name);

        void Clear();
    }
}

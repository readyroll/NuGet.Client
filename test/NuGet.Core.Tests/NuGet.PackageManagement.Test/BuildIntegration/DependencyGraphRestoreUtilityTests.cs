// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using NuGet.Common;
using NuGet.Configuration;
using NuGet.Frameworks;
using NuGet.ProjectManagement;
using NuGet.ProjectModel;
using NuGet.Test.Utility;
using Test.Utility;
using Xunit;

namespace NuGet.PackageManagement.Test
{
    public class DependencyGraphRestoreUtilityTests
    {
        [Fact]
        public async Task DependencyGraphRestoreUtility_NoopIsRestoreRequiredAsyncTest()
        {
            // Arrange
            var projectName = "testproj";
            var logger = new TestLogger();

            using (var rootFolder = TestDirectory.Create())
            {
                var projectFolder = new DirectoryInfo(Path.Combine(rootFolder, projectName));
                projectFolder.Create();
                var projectConfig = new FileInfo(Path.Combine(projectFolder.FullName, "project.json"));
                var msbuildProjectPath = new FileInfo(Path.Combine(projectFolder.FullName, $"{projectName}.csproj"));

                var sources = new List<string>
                {
                    "https://www.nuget.org/api/v2/"
                };

                var targetFramework = NuGetFramework.Parse("net46");

                var msBuildNuGetProjectSystem = new TestMSBuildNuGetProjectSystem(targetFramework, new TestNuGetProjectContext());
                var project = new TestMSBuildNuGetProject(msBuildNuGetProjectSystem, rootFolder, projectFolder.FullName);

                var effectiveGlobalPackagesFolder = SettingsUtility.GetGlobalPackagesFolder(NullSettings.Instance);

                var externalReferenceContext = new ExternalProjectReferenceContext(logger);

                var projects = new List<IDependencyGraphProject>() { project };

                var cache = await DependencyGraphProjectCacheUtility.CreateDependencyGraphProjectCache(
                    projects,
                    externalReferenceContext);

                externalReferenceContext.ProjectCache = cache;

                var pathContext = NuGetPathContext.Create(NullSettings.Instance);

                // Act
                var result = await DependencyGraphRestoreUtility.IsRestoreRequiredAsync(
                    projects,
                    forceRestore: false,
                    pathContext: pathContext,
                    referenceContext: externalReferenceContext);

                // Assert
                Assert.Equal(false, result);
                Assert.Equal(0, logger.Errors);
                Assert.Equal(0, logger.Warnings);
                Assert.Equal(0, logger.MinimalMessages.Count);
            }
        }

        [Fact]
        public async Task DependencyGraphRestoreUtility_NoopRestoreTest()
        {
            // Arrange
            var projectName = "testproj";
            var logger = new TestLogger();

            using (var rootFolder = TestDirectory.Create())
            {
                var projectFolder = new DirectoryInfo(Path.Combine(rootFolder, projectName));
                projectFolder.Create();
                var projectConfig = new FileInfo(Path.Combine(projectFolder.FullName, "project.json"));
                var msbuildProjectPath = new FileInfo(Path.Combine(projectFolder.FullName, $"{projectName}.csproj"));

                var sources = new List<string>
                {
                    "https://www.nuget.org/api/v2/"
                };

                var targetFramework = NuGetFramework.Parse("net46");

                var msBuildNuGetProjectSystem = new TestMSBuildNuGetProjectSystem(targetFramework, new TestNuGetProjectContext());
                var project = new TestMSBuildNuGetProject(msBuildNuGetProjectSystem, rootFolder, projectFolder.FullName);

                var effectiveGlobalPackagesFolder = SettingsUtility.GetGlobalPackagesFolder(NullSettings.Instance);

                var externalReferenceContext = new ExternalProjectReferenceContext(logger);

                var projects = new List<IDependencyGraphProject>() { project };

                // Act
                await DependencyGraphRestoreUtility.RestoreAsync(
                    projects,
                    sources,
                    NullSettings.Instance,
                    externalReferenceContext,
                    null);

                // Assert
                Assert.Equal(0, logger.Errors);
                Assert.Equal(0, logger.Warnings);
            }
        }

        [Fact]
        public async Task DependencyGraphRestoreUtility_Restore_RaiseTelemetryEvents()
        {
            // Arrange
            var projectName = "testproj";
            var logger = new TestLogger();

            // set up telemetry service
            var operationId = Guid.NewGuid().ToString();
            var telemetryEvents = new List<TelemetryEvent>();
            var telemetryService = TestTelemetryServiceUtility.CreateRestoreTelemetryService(operationId, telemetryEvents);

            using (var rootFolder = TestDirectory.Create())
            {
                var projectFolder = new DirectoryInfo(Path.Combine(rootFolder, projectName));
                projectFolder.Create();
                var projectConfig = Path.Combine(rootFolder, "project.json");
                BuildIntegrationTestUtility.CreateConfigJson(projectConfig);

                var sources = new List<string>
                {
                    "https://www.nuget.org/api/v2/"
                };

                var targetFramework = NuGetFramework.Parse("net46");

                var projectTargetFramework = NuGetFramework.Parse("netcore50");
                var testNuGetProjectContext = new TestNuGetProjectContext();

                var msBuildNuGetProjectSystem = new TestMSBuildNuGetProjectSystem(
                    projectTargetFramework,
                    testNuGetProjectContext,
                    rootFolder,
                    projectName);

                var project = new TestBuildIntegratedNuGetProject(projectConfig, msBuildNuGetProjectSystem);
                project.ProjectClosure = new List<ExternalProjectReference>
                {
                    BuildIntegrationTestUtility.CreateReference(projectName)
                };

                var effectiveGlobalPackagesFolder = SettingsUtility.GetGlobalPackagesFolder(NullSettings.Instance);

                var externalReferenceContext = new ExternalProjectReferenceContext(logger);

                var projects = new List<IDependencyGraphProject>() { project };

                // Act
                await DependencyGraphRestoreUtility.RestoreAsync(
                    projects,
                    sources,
                    NullSettings.Instance,
                    externalReferenceContext,
                    telemetryService);

                // Assert
                Assert.Equal(4, telemetryEvents.Count);
                Assert.Equal(operationId, telemetryEvents[0].Properties[TelemetryConstants.OperationIdPropertyName].ToString());
                Assert.Equal(
                    string.Format(TelemetryConstants.RestoreGraphStepName, projectName),
                    telemetryEvents[0].Properties[TelemetryConstants.StepNamePropertyName].ToString());
                Assert.Equal(operationId, telemetryEvents[1].Properties[TelemetryConstants.OperationIdPropertyName].ToString());
                Assert.Equal(
                    string.Format(TelemetryConstants.CreateAssetsFileStepName, projectName),
                    telemetryEvents[1].Properties[TelemetryConstants.StepNamePropertyName].ToString());
                Assert.Equal(operationId, telemetryEvents[2].Properties[TelemetryConstants.OperationIdPropertyName].ToString());
                Assert.Equal(
                    string.Format(TelemetryConstants.PackageCompatibilityStepName, projectName),
                    telemetryEvents[2].Properties[TelemetryConstants.StepNamePropertyName].ToString());
                Assert.Equal(operationId, telemetryEvents[3].Properties[TelemetryConstants.OperationIdPropertyName].ToString());
                Assert.Equal(
                    string.Format(TelemetryConstants.CreateTargetPropFileStepName, projectName),
                    telemetryEvents[3].Properties[TelemetryConstants.StepNamePropertyName].ToString());
            }
        }
    }
}
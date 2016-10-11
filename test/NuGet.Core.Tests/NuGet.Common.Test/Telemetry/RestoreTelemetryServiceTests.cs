// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Moq;
using Xunit;
using System.Collections.Generic;

namespace NuGet.Common.Test.Telemetry
{
    public class RestoreTelemetryServiceTests
    {
        [Theory]
        [InlineData(RestoreOperationSource.OnBuild, NuGetOperationStatus.Succeeded)]
        [InlineData(RestoreOperationSource.Explicit, NuGetOperationStatus.Succeeded)]
        [InlineData(RestoreOperationSource.OnBuild, NuGetOperationStatus.NoOp)]
        [InlineData(RestoreOperationSource.Explicit, NuGetOperationStatus.NoOp)]
        [InlineData(RestoreOperationSource.OnBuild, NuGetOperationStatus.Failed)]
        [InlineData(RestoreOperationSource.Explicit, NuGetOperationStatus.Failed)]
        public void RestoreTelemetryService_EmitRestoreEvent_OperationSucceed(RestoreOperationSource source, NuGetOperationStatus status)
        {
            // Arrange
            var telemetrySession = new Mock<ITelemetrySession>();
            TelemetryEvent lastTelemetryEvent = null;
            telemetrySession
                .Setup(x => x.PostEvent(It.IsAny<TelemetryEvent>()))
                .Callback<TelemetryEvent>(x => lastTelemetryEvent = x);

            string operationId = Guid.NewGuid().ToString();
            var telemetrySessionContext = new TelemetrySessionContext(telemetrySession.Object, operationId);

            var stausMessage = status == NuGetOperationStatus.Failed ? "Operation Failed" : string.Empty;
            var restoreTelemetryData = new RestoreTelemetryEvent(
                operationId: operationId,
                projectIds: new[] { Guid.NewGuid().ToString() },
                source: source,
                startTime: DateTime.Now.AddSeconds(-3),
                status: status,
                statusMessage: stausMessage,
                packageCount: 2,
                endTime: DateTime.Now,
                duration: 2.10);
            var service = new RestoreTelemetryService(telemetrySessionContext);

            // Act
            service.EmitRestoreEvent(restoreTelemetryData);

            // Assert
            VerifyTelemetryEventData(restoreTelemetryData, lastTelemetryEvent);
        }

        [Theory]
        [MemberData(nameof(GetStepNames))]
        public void RestoreTelemetryService_EmitActionStepsEvent(string stepName)
        {
            // Arrange
            var telemetrySession = new Mock<ITelemetrySession>();
            TelemetryEvent lastTelemetryEvent = null;
            telemetrySession
                .Setup(x => x.PostEvent(It.IsAny<TelemetryEvent>()))
                .Callback<TelemetryEvent>(x => lastTelemetryEvent = x);

            string operationId = Guid.NewGuid().ToString();
            var telemetrySessionContext = new TelemetrySessionContext(telemetrySession.Object, operationId);
            var duration = 1.12;
            var stepNameWithProject = string.Format(stepName, "testProject");
            var service = new RestoreTelemetryService(telemetrySessionContext);

            // Act
            service.EmitActionStepsEvent(stepNameWithProject, duration);

            // Assert
            Assert.NotNull(lastTelemetryEvent);
            Assert.Equal(TelemetryConstants.NugetActionStepsEventName, lastTelemetryEvent.Name);
            Assert.Equal(3, lastTelemetryEvent.Properties.Count);

            Assert.Equal(operationId, lastTelemetryEvent.Properties[TelemetryConstants.OperationIdPropertyName].ToString());
            Assert.Equal(stepNameWithProject, lastTelemetryEvent.Properties[TelemetryConstants.StepNamePropertyName].ToString());
            Assert.Equal(duration, (double)lastTelemetryEvent.Properties[TelemetryConstants.DurationPropertyName]);
        }

        public static IEnumerable<object[]> GetStepNames()
        {
            yield return new[] { TelemetryConstants.RestorePackagesConfigStepName };
            yield return new[] { TelemetryConstants.IsRequiredRequiredStepName };
            yield return new[] { TelemetryConstants.RestoreGraphStepName };
            yield return new[] { TelemetryConstants.ToolsRestoreStepName };
            yield return new[] { TelemetryConstants.CreateAssetsFileStepName };
            yield return new[] { TelemetryConstants.PackageCompatibilityStepName };
            yield return new[] { TelemetryConstants.CreateTargetPropFileStepName };
        }

        private void VerifyTelemetryEventData(RestoreTelemetryEvent expected, TelemetryEvent actual)
        {
            Assert.NotNull(actual);
            Assert.Equal(TelemetryConstants.RestoreActionEventName, actual.Name);
            Assert.Equal(9, actual.Properties.Count);

            Assert.Equal(expected.Source.ToString(), actual.Properties[TelemetryConstants.OperationSourcePropertyName].ToString());

            TelemetryTestUtility.VerifyTelemetryEventData(expected, actual);
        }
    }
}

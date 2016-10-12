// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Moq;
using Xunit;

namespace NuGet.Common.Test
{
    public class ActionsTelemetryServiceTests
    {
        [Theory]
        [InlineData(NuGetOperationType.Install, OperationSource.UI)]
        [InlineData(NuGetOperationType.Update, OperationSource.UI)]
        [InlineData(NuGetOperationType.Uninstall, OperationSource.UI)]
        [InlineData(NuGetOperationType.Install, OperationSource.PMC)]
        [InlineData(NuGetOperationType.Update, OperationSource.PMC)]
        [InlineData(NuGetOperationType.Uninstall, OperationSource.PMC)]
        public void ActionsTelemetryService_EmitActionEvent_OperationSucceed(NuGetOperationType operationType, OperationSource source)
        {
            // Arrange
            var telemetrySession = new Mock<ITelemetrySession>();
            TelemetryEvent lastTelemetryEvent = null;
            telemetrySession
                .Setup(x => x.PostEvent(It.IsAny<TelemetryEvent>()))
                .Callback<TelemetryEvent>(x => lastTelemetryEvent = x);

            string operationId = Guid.NewGuid().ToString();
            var telemetrySessionContext = new TelemetrySessionContext(telemetrySession.Object, operationId);

            var actionTelemetryData = new ActionsTelemetryEvent(
                operationId: operationId,
                projectIds: new[] { Guid.NewGuid().ToString() },
                operationType: operationType,
                source: source,
                startTime: DateTime.Now.AddSeconds(-3),
                status: NuGetOperationStatus.Succeeded,
                statusMessage: string.Empty,
                packageCount: 1,
                endTime: DateTime.Now,
                duration: 2.10);
            var service = new ActionsTelemetryService(telemetrySessionContext);

            // Act
            service.EmitActionEvent(actionTelemetryData);

            // Assert
            VerifyTelemetryEventData(actionTelemetryData, lastTelemetryEvent);
        }

        [Theory]
        [InlineData(NuGetOperationType.Install, OperationSource.UI)]
        [InlineData(NuGetOperationType.Update, OperationSource.UI)]
        [InlineData(NuGetOperationType.Uninstall, OperationSource.UI)]
        [InlineData(NuGetOperationType.Install, OperationSource.PMC)]
        [InlineData(NuGetOperationType.Update, OperationSource.PMC)]
        [InlineData(NuGetOperationType.Uninstall, OperationSource.PMC)]
        public void ActionsTelemetryService_EmitActionEvent_OperationFailed(NuGetOperationType operationType, OperationSource source)
        {
            // Arrange
            var telemetrySession = new Mock<ITelemetrySession>();
            TelemetryEvent lastTelemetryEvent = null;
            telemetrySession
                .Setup(x => x.PostEvent(It.IsAny<TelemetryEvent>()))
                .Callback<TelemetryEvent>(x => lastTelemetryEvent = x);

            string operationId = Guid.NewGuid().ToString();
            var telemetrySessionContext = new TelemetrySessionContext(telemetrySession.Object, operationId);

            var actionTelemetryData = new ActionsTelemetryEvent(
                operationId: operationId,
                projectIds: new[] { Guid.NewGuid().ToString() },
                operationType: operationType,
                source: source,
                startTime: DateTime.Now.AddSeconds(-2),
                status: NuGetOperationStatus.Failed,
                statusMessage: "Operation Failed.",
                packageCount: 1,
                endTime: DateTime.Now,
                duration: 1.30);
            var service = new ActionsTelemetryService(telemetrySessionContext);

            // Act
            service.EmitActionEvent(actionTelemetryData);

            // Assert
            VerifyTelemetryEventData(actionTelemetryData, lastTelemetryEvent);
        }

        [Theory]
        [InlineData(NuGetOperationType.Install)]
        [InlineData(NuGetOperationType.Update)]
        [InlineData(NuGetOperationType.Uninstall)]
        public void ActionsTelemetryService_EmitActionEvent_OperationNoOp(NuGetOperationType operationType)
        {
            // Arrange
            var telemetrySession = new Mock<ITelemetrySession>();
            TelemetryEvent lastTelemetryEvent = null;
            telemetrySession
                .Setup(x => x.PostEvent(It.IsAny<TelemetryEvent>()))
                .Callback<TelemetryEvent>(x => lastTelemetryEvent = x);

            string operationId = Guid.NewGuid().ToString();
            var telemetrySessionContext = new TelemetrySessionContext(telemetrySession.Object, operationId);

            var actionTelemetryData = new ActionsTelemetryEvent(
                operationId: operationId,
                projectIds: new[] { Guid.NewGuid().ToString() },
                operationType: operationType,
                source: OperationSource.PMC,
                startTime: DateTime.Now.AddSeconds(-1),
                status: NuGetOperationStatus.NoOp,
                statusMessage: string.Empty,
                packageCount: 1,
                endTime: DateTime.Now,
                duration: .40);
            var service = new ActionsTelemetryService(telemetrySessionContext);

            // Act
            service.EmitActionEvent(actionTelemetryData);

            // Assert
            VerifyTelemetryEventData(actionTelemetryData, lastTelemetryEvent);
        }

        [Theory]
        [MemberData(nameof(GetStepNames))]
        public void ActionsTelemetryService_EmitActionStepsEvent(string stepName)
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
            var service = new ActionsTelemetryService(telemetrySessionContext);

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
            yield return new[] { TelemetryConstants.PreviewBuildIntegratedStepName };
            yield return new[] { TelemetryConstants.WritingLockFileStepName };
            yield return new[] { TelemetryConstants.ExecuteInitScriptStepName };
            yield return new[] { TelemetryConstants.ParentRestoreStepName };
            yield return new[] { TelemetryConstants.GatherDependencyStepName };
            yield return new[] { TelemetryConstants.ResolveDependencyStepName };
            yield return new[] { TelemetryConstants.ResolvedActionsStepName };
            yield return new[] { TelemetryConstants.ExecuteActionStepName };
            yield return new[] { TelemetryConstants.PreviewUninstallStepName };
        }

        private void VerifyTelemetryEventData(ActionsTelemetryEvent expected, TelemetryEvent actual)
        {
            Assert.NotNull(actual);
            Assert.Equal(TelemetryConstants.NugetActionEventName, actual.Name);
            Assert.Equal(10, actual.Properties.Count);

            Assert.Equal(expected.OperationType.ToString(), actual.Properties[TelemetryConstants.OperationTypePropertyName].ToString());
            Assert.Equal(expected.Source.ToString(), actual.Properties[TelemetryConstants.OperationSourcePropertyName].ToString());

            TelemetryUtilityTest.VerifyTelemetryEventData(expected, actual);
        }
    }
}

// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Moq;
using Xunit;

namespace NuGet.Common.Test.Telemetry
{
    public class ActionsTelemetryServiceTests
    {
        [Theory]
        [InlineData(NugetOperationType.Install, OperationSource.UI)]
        [InlineData(NugetOperationType.Update, OperationSource.UI)]
        [InlineData(NugetOperationType.Uninstall, OperationSource.UI)]
        [InlineData(NugetOperationType.Install, OperationSource.PMC)]
        [InlineData(NugetOperationType.Update, OperationSource.PMC)]
        [InlineData(NugetOperationType.Uninstall, OperationSource.PMC)]
        public void ActionsTelemetryService_EmitActionEvent_OperationSucceed(NugetOperationType operationType, OperationSource source)
        {
            // Arrange
            var telemetrySession = new Mock<ITelemetrySession>();
            TelemetryEvent lastTelemetryEvent = null;
            telemetrySession
                .Setup(x => x.PostEvent(It.IsAny<TelemetryEvent>()))
                .Callback<TelemetryEvent>(x => lastTelemetryEvent = x);

            var actionTelemetryData = new ActionsTelemetryEvent(
                Guid.NewGuid().ToString(),
                new[] { Guid.NewGuid().ToString() },
                operationType,
                source,
                DateTime.Now.AddSeconds(-3),
                NugetOperationStatus.Succeed,
                string.Empty,
                1,
                DateTime.Now,
                2.10);
            var service = new ActionsTelemetryService(telemetrySession.Object);

            // Act
            service.EmitActionEvent(actionTelemetryData);

            // Assert
            VerifyTelemetryEventData(actionTelemetryData, lastTelemetryEvent);
        }

        [Theory]
        [InlineData(NugetOperationType.Install, OperationSource.UI)]
        [InlineData(NugetOperationType.Update, OperationSource.UI)]
        [InlineData(NugetOperationType.Uninstall, OperationSource.UI)]
        [InlineData(NugetOperationType.Install, OperationSource.PMC)]
        [InlineData(NugetOperationType.Update, OperationSource.PMC)]
        [InlineData(NugetOperationType.Uninstall, OperationSource.PMC)]
        public void ActionsTelemetryService_EmitActionEvent_OperationFailed(NugetOperationType operationType, OperationSource source)
        {
            // Arrange
            var telemetrySession = new Mock<ITelemetrySession>();
            TelemetryEvent lastTelemetryEvent = null;
            telemetrySession
                .Setup(x => x.PostEvent(It.IsAny<TelemetryEvent>()))
                .Callback<TelemetryEvent>(x => lastTelemetryEvent = x);

            var actionTelemetryData = new ActionsTelemetryEvent(
                Guid.NewGuid().ToString(),
                new[] { Guid.NewGuid().ToString() },
                operationType,
                source,
                DateTime.Now.AddSeconds(-2),
                NugetOperationStatus.Failed,
                "Operation Failed.",
                1,
                DateTime.Now,
                1.30);
            var service = new ActionsTelemetryService(telemetrySession.Object);

            // Act
            service.EmitActionEvent(actionTelemetryData);

            // Assert
            VerifyTelemetryEventData(actionTelemetryData, lastTelemetryEvent);
        }

        [Theory]
        [InlineData(NugetOperationType.Install)]
        [InlineData(NugetOperationType.Update)]
        [InlineData(NugetOperationType.Uninstall)]
        public void ActionsTelemetryService_EmitActionEvent_OperationNoOp(NugetOperationType operationType)
        {
            // Arrange
            var telemetrySession = new Mock<ITelemetrySession>();
            TelemetryEvent lastTelemetryEvent = null;
            telemetrySession
                .Setup(x => x.PostEvent(It.IsAny<TelemetryEvent>()))
                .Callback<TelemetryEvent>(x => lastTelemetryEvent = x);

            var actionTelemetryData = new ActionsTelemetryEvent(
                Guid.NewGuid().ToString(),
                new[] { Guid.NewGuid().ToString() },
                operationType,
                OperationSource.PMC,
                DateTime.Now.AddSeconds(-1),
                NugetOperationStatus.NoOp,
                string.Empty,
                1,
                DateTime.Now,
                .40);
            var service = new ActionsTelemetryService(telemetrySession.Object);

            // Act
            service.EmitActionEvent(actionTelemetryData);

            // Assert
            VerifyTelemetryEventData(actionTelemetryData, lastTelemetryEvent);
        }

        private void VerifyTelemetryEventData(ActionsTelemetryEvent expected, TelemetryEvent actual)
        {
            Assert.NotNull(actual);
            Assert.Equal(Constants.NugetActionEventName, actual.Name);
            Assert.Equal(10, actual.Properties.Count);

            Assert.Equal(expected.OperationType.ToString(), actual.Properties[Constants.OperationTypePropertyName].ToString());
            Assert.Equal(expected.Source.ToString(), actual.Properties[Constants.OperationSourcePropertyName].ToString());

            Assert.Equal(expected.Duration, (double)actual.Properties[Constants.DurationPropertyName]);
        }
    }
}

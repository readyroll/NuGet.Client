// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Moq;
using Xunit;

namespace NuGet.Common.Test.Telemetry
{
    public class RestoreTelemetryServiceTests
    {
        [Theory]
        [InlineData(RestoreOperationSource.OnBuild, NugetOperationStatus.Succeed)]
        [InlineData(RestoreOperationSource.Explicit, NugetOperationStatus.Succeed)]
        [InlineData(RestoreOperationSource.OnBuild, NugetOperationStatus.NoOp)]
        [InlineData(RestoreOperationSource.Explicit, NugetOperationStatus.NoOp)]
        [InlineData(RestoreOperationSource.OnBuild, NugetOperationStatus.Failed)]
        [InlineData(RestoreOperationSource.Explicit, NugetOperationStatus.Failed)]
        public void RestoreTelemetryService_EmitRestoreEvent_OperationSucceed(RestoreOperationSource source, NugetOperationStatus status)
        {
            // Arrange
            var telemetrySession = new Mock<ITelemetrySession>();
            TelemetryEvent lastTelemetryEvent = null;
            telemetrySession
                .Setup(x => x.PostEvent(It.IsAny<TelemetryEvent>()))
                .Callback<TelemetryEvent>(x => lastTelemetryEvent = x);

            var stausMessage = status == NugetOperationStatus.Failed ? "Operation Failed" : string.Empty;
            var restoreTelemetryData = new RestoreTelemetryEvent(
                Guid.NewGuid().ToString(),
                new[] { Guid.NewGuid().ToString() },
                source,
                DateTime.Now.AddSeconds(-3),
                status,
                stausMessage,
                2,
                DateTime.Now,
                2.10);
            var service = new RestoreTelemetryService(telemetrySession.Object);

            // Act
            service.EmitRestoreEvent(restoreTelemetryData);

            // Assert
            VerifyTelemetryEventData(restoreTelemetryData, lastTelemetryEvent);
        }

        private void VerifyTelemetryEventData(RestoreTelemetryEvent expected, TelemetryEvent actual)
        {
            Assert.NotNull(actual);
            Assert.Equal(Constants.RestoreActionEventName, actual.Name);
            Assert.Equal(9, actual.Properties.Count);

            Assert.Equal(expected.Source.ToString(), actual.Properties[Constants.OperationSourcePropertyName].ToString());

            TelemetryTestUtility.VerifyTelemetryEventData(expected, actual);
        }
    }
}

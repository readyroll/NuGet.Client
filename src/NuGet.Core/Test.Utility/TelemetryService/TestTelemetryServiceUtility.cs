// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using Moq;
using NuGet.Common;

namespace Test.Utility
{
    public class TestTelemetryServiceUtility
    {
        public static ActionsTelemetryService CreateActionsTelemetryService(string operationId, List<TelemetryEvent> telemetryEvents)
        {
            var telemetrySession = new Mock<ITelemetrySession>();
            telemetrySession
                .Setup(x => x.PostEvent(It.IsAny<TelemetryEvent>()))
                .Callback<TelemetryEvent>(x => telemetryEvents.Add(x));

            var telemetrySessionContext = new TelemetrySessionContext(telemetrySession.Object, operationId);
            return new ActionsTelemetryService(telemetrySessionContext);
        }

        public static RestoreTelemetryService CreateRestoreTelemetryService(string operationId, List<TelemetryEvent> telemetryEvents)
        {
            var telemetrySession = new Mock<ITelemetrySession>();
            telemetrySession
                .Setup(x => x.PostEvent(It.IsAny<TelemetryEvent>()))
                .Callback<TelemetryEvent>(x => telemetryEvents.Add(x));

            var telemetrySessionContext = new TelemetrySessionContext(telemetrySession.Object, operationId);
            return new RestoreTelemetryService(telemetrySessionContext);
        }
    }
}

// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

namespace NuGet.Common
{
    /// <summary>
    /// Base class for nuget telemetry services
    /// </summary>
    public class NuGetTelemetryService
    {
        private readonly ITelemetrySession _telemetrySession;

        public NuGetTelemetryService(ITelemetrySession telemetrySession)
        {
            if (telemetrySession == null)
            {
                throw new ArgumentNullException(nameof(telemetrySession));
            }

            _telemetrySession = telemetrySession;
        }

        public void EmitEvent(string eventName, Dictionary<string, object> properties)
        {
            var telemetryEvent = new TelemetryEvent(eventName);

            foreach (var pair in properties)
            {
                telemetryEvent.Properties[pair.Key] = pair.Value;
            }

            _telemetrySession.PostEvent(telemetryEvent);
        }
    }
}

// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

namespace NuGet.Common
{
    /// <summary>
    /// Perf telemetry service class for restore operation.
    /// </summary>
    public class RestoreTelemetryService : ActionsTelemetryService
    {
        public RestoreTelemetryService(ITelemetrySessionContext telemetrySessionContext) : 
            base (telemetrySessionContext)
        {
        }

        public void EmitRestoreEvent(RestoreTelemetryEvent restoreTelemetryData)
        {
            if (restoreTelemetryData == null)
            {
                throw new ArgumentNullException(nameof(restoreTelemetryData));
            }

            var telemetryEvent = new TelemetryEvent(
                TelemetryConstants.RestoreActionEventName,
                new Dictionary<string, object>
                {
                    { TelemetryConstants.OperationIdPropertyName, restoreTelemetryData.OperationId },
                    { TelemetryConstants.ProjectIdsPropertyName, string.Join(",", restoreTelemetryData.ProjectIds) },
                    { TelemetryConstants.OperationSourcePropertyName, restoreTelemetryData.Source },
                    { TelemetryConstants.PackagesCountPropertyName, restoreTelemetryData.PackagesCount },
                    { TelemetryConstants.OperationStatusPropertyName, restoreTelemetryData.Status },
                    { TelemetryConstants.StatusMessagePropertyName, restoreTelemetryData.StatusMessage },
                    { TelemetryConstants.StartTimePropertyName, restoreTelemetryData.StartTime.ToString() },
                    { TelemetryConstants.EndTimePropertyName, restoreTelemetryData.EndTime.ToString() },
                    { TelemetryConstants.DurationPropertyName, restoreTelemetryData.Duration }
                }
            );
            telemetrySession.PostEvent(telemetryEvent);
        }
    }
}

// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

namespace NuGet.Common
{
    /// <summary>
    /// Perf telemetry service class for restore operation.
    /// </summary>
    public class RestoreTelemetryService
    {
       private readonly NuGetTelemetryService _nuGetTelemetryService;

        public RestoreTelemetryService(ITelemetrySession telemetrySession)
        {
            if (telemetrySession == null)
            {
                throw new ArgumentNullException(nameof(telemetrySession));
            }

            _nuGetTelemetryService = new NuGetTelemetryService(telemetrySession);
        }

        public void EmitRestoreEvent(RestoreTelemetryEvent restoreTelemetryData)
        {
            if (restoreTelemetryData == null)
            {
                throw new ArgumentNullException(nameof(restoreTelemetryData));
            }

            _nuGetTelemetryService.EmitEvent(
                Constants.RestoreActionEventName,
                new Dictionary<string, object>
                {
                    { Constants.OperationIdPropertyName, restoreTelemetryData.OperationId },
                    { Constants.ProjectIdsPropertyName, string.Join(",", restoreTelemetryData.ProjectIds) },
                    { Constants.OperationSourcePropertyName, restoreTelemetryData.Source },
                    { Constants.PackagesCountPropertyName, restoreTelemetryData.PackagesCount },
                    { Constants.OperationStatusPropertyName, restoreTelemetryData.Status },
                    { Constants.StatusMessagePropertyName, restoreTelemetryData.StatusMessage },
                    { Constants.StartTimePropertyName, restoreTelemetryData.StartTime.ToString() },
                    { Constants.EndTimePropertyName, restoreTelemetryData.EndTime.ToString() },
                    { Constants.DurationPropertyName, restoreTelemetryData.Duration }
                }
            );
        }
    }
}

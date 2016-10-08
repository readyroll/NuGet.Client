// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

namespace NuGet.Common
{
    /// <summary>
    /// Nuget actions performance telemetry service class.
    /// </summary>
    public class ActionsTelemetryService
    {
        private readonly NuGetTelemetryService _nuGetTelemetryService;

        public ActionsTelemetryService(ITelemetrySession telemetrySession)
        {
            if (telemetrySession == null)
            {
                throw new ArgumentNullException(nameof(telemetrySession));
            }

            _nuGetTelemetryService = new NuGetTelemetryService(telemetrySession);
        }

        public void EmitActionEvent(ActionsTelemetryEvent actionTelemetryData)
        {
            if (actionTelemetryData == null)
            {
                throw new ArgumentNullException(nameof(actionTelemetryData));
            }

            _nuGetTelemetryService.EmitEvent(
                Constants.NugetActionEventName,
                new Dictionary<string, object>
                {
                    { Constants.OperationIdPropertyName, actionTelemetryData.OperationId },
                    { Constants.ProjectIdsPropertyName, string.Join(",", actionTelemetryData.ProjectIds) },
                    { Constants.OperationTypePropertyName, actionTelemetryData.OperationType },
                    { Constants.OperationSourcePropertyName, actionTelemetryData.Source },
                    { Constants.PackagesCountPropertyName, actionTelemetryData.PackagesCount },
                    { Constants.OperationStatusPropertyName, actionTelemetryData.Status },
                    { Constants.StatusMessagePropertyName, actionTelemetryData.StatusMessage },
                    { Constants.StartTimePropertyName, actionTelemetryData.StartTime.ToString() },
                    { Constants.EndTimePropertyName, actionTelemetryData.EndTime.ToString() },
                    { Constants.DurationPropertyName, actionTelemetryData.Duration }
                }
            );
        }

        public void EmitActionStepsEvent(ActionStepsTelemetryEvent actionStepsData)
        {
            if (actionStepsData == null)
            {
                throw new ArgumentNullException(nameof(actionStepsData));
            }

            _nuGetTelemetryService.EmitEvent(
                Constants.NugetActionStepsEventName,
                new Dictionary<string, object>
                {
                    { Constants.OperationIdPropertyName, actionStepsData.OperationId },
                    { Constants.StepNamePropertyName, actionStepsData.StepName },
                    { Constants.DurationPropertyName, actionStepsData.Duration }
                }
            );
        }
    }
}

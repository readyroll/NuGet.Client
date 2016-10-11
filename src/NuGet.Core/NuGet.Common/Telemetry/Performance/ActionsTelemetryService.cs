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
        protected readonly ITelemetrySession telemetrySession;

        private readonly string _operationId;

        public ActionsTelemetryService(ITelemetrySessionContext telemetrySessionContext)
        {
            if (telemetrySessionContext == null)
            {
                throw new ArgumentNullException(nameof(telemetrySessionContext));
            }

            telemetrySession = telemetrySessionContext.TelemetrySession;
            _operationId = telemetrySessionContext.OperationId;
        }

        public void EmitActionEvent(ActionsTelemetryEvent actionTelemetryData)
        {
            if (actionTelemetryData == null)
            {
                throw new ArgumentNullException(nameof(actionTelemetryData));
            }

            var telemetryEvent = new TelemetryEvent(
                TelemetryConstants.NugetActionEventName,
                new Dictionary<string, object>
                {
                    { TelemetryConstants.OperationIdPropertyName, actionTelemetryData.OperationId },
                    { TelemetryConstants.ProjectIdsPropertyName, string.Join(",", actionTelemetryData.ProjectIds) },
                    { TelemetryConstants.OperationTypePropertyName, actionTelemetryData.OperationType },
                    { TelemetryConstants.OperationSourcePropertyName, actionTelemetryData.Source },
                    { TelemetryConstants.PackagesCountPropertyName, actionTelemetryData.PackagesCount },
                    { TelemetryConstants.OperationStatusPropertyName, actionTelemetryData.Status },
                    { TelemetryConstants.StatusMessagePropertyName, actionTelemetryData.StatusMessage },
                    { TelemetryConstants.StartTimePropertyName, actionTelemetryData.StartTime.ToString() },
                    { TelemetryConstants.EndTimePropertyName, actionTelemetryData.EndTime.ToString() },
                    { TelemetryConstants.DurationPropertyName, actionTelemetryData.Duration }
                }
            );
            telemetrySession.PostEvent(telemetryEvent);

        }

        public void EmitActionStepsEvent(string stepName, double duration)
        {
            if (stepName == null)
            {
                throw new ArgumentNullException(nameof(stepName));
            }

            var telemetryEvent = new TelemetryEvent(
                TelemetryConstants.NugetActionStepsEventName,
                new Dictionary<string, object>
                {
                    { TelemetryConstants.OperationIdPropertyName, _operationId },
                    { TelemetryConstants.StepNamePropertyName, stepName },
                    { TelemetryConstants.DurationPropertyName, duration }
                }
            );
            telemetrySession.PostEvent(telemetryEvent);
        }
    }
}

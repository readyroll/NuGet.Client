// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace NuGet.Common
{
    /// <summary>
    /// Telemetry event data for nuget operation sub-steps
    /// </summary>
    public class ActionStepsTelemetryEvent
    {
        public ActionStepsTelemetryEvent(
            string operationId,
            string stepName,
            string duration)
        {
            OperationId = operationId;
            StepName = stepName;
            Duration = duration;
        }

        public string OperationId { get; }

        public string StepName { get; }

        public string Duration { get; }
    }
}

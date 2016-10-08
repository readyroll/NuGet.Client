// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace NuGet.Common
{
    /// <summary>
    /// Telemetry event data for nuget operations like install, update, or uninstall.
    /// </summary>
    public class ActionsTelemetryEvent : ActionEventBase
    {
        public ActionsTelemetryEvent(
            string operationId,
            string[] projectIds,
            NugetOperationType operationType,
            OperationSource source,
            DateTime startTime,
            NugetOperationStatus status,
            string statusMessage,
            int packageCount,
            DateTime endTime,
            double duration) : base(operationId, projectIds, startTime, status, statusMessage, packageCount, endTime, duration)
        {
            OperationType = operationType;
            Source = source;
        }

        public NugetOperationType OperationType { get; set; }

        public OperationSource Source { get; set; }
    }

    /// <summary>
    /// Define nuget operation type values.
    /// </summary>
    public enum NugetOperationType
    {
        /// <summary>
        /// Install package action.
        /// </summary>
        Install = 0,

        /// <summary>
        /// Update package action.
        /// </summary>
        Update = 1,

        /// <summary>
        /// Uninstall package action.
        /// </summary>
        Uninstall = 2,
    }

    /// <summary>
    /// Define different sources to trigger nuget action.
    /// </summary>
    public enum OperationSource
    {
        /// <summary>
        /// When nuget action is trigger from Package Management Console.
        /// </summary>
        PMC = 0,

        /// <summary>
        /// When nuget action is trigger from Nuget Manager UI.
        /// </summary>
        UI = 1,

        /// <summary>
        /// When nuget action is trigger from nuget public api.
        /// </summary>
        API = 2,
    }
}

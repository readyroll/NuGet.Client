// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace NuGet.Common
{
    /// <summary>
    /// Telemetry event data for restore operation.
    /// </summary>
    public class RestoreTelemetryEvent : ActionEventBase
    {
        public RestoreTelemetryEvent(
            string operationId,
            string[] projectIds,
            RestoreOperationSource source,
            DateTime startTime,
            NugetOperationStatus status,
            string statusMessage,
            int packageCount,
            DateTime endTime,
            double duration) : base(operationId, projectIds, startTime, status, statusMessage, packageCount, endTime, duration)
        {
            Source = source;
        }

        public RestoreOperationSource Source { get; set; }
    }

    /// <summary>
    /// Define multiple sources to trigger restore.
    /// </summary>
    public enum RestoreOperationSource
    {
        /// <summary>
        /// When restore is trigger through OnBuild event.
        /// </summary>
        OnBuild =0,

        /// <summary>
        /// When restore is trigger through manually from UI.
        /// </summary>
        Explicit =1,

        /// <summary>
        /// Auto restore with nuget restore manager.
        /// </summary>
        Implicit =2,
    }

}

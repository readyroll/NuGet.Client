// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace NuGet.Common
{
    /// <summary>
    /// Base class to generate telemetry data for nuget operations like install, udpate or restore.
    /// </summary>
    public abstract class ActionEventBase
    {
        public ActionEventBase(
            string operationId,
            string[] projectIds,
            DateTime startTime,
            NugetOperationStatus status,
            string statusMessage,
            int packageCount,
            DateTime endTime,
            double duration)
        {
            OperationId = operationId;
            ProjectIds = projectIds;
            PackagesCount = packageCount;
            Status = status;
            StatusMessage = statusMessage;
            StartTime = startTime;
            EndTime = endTime;
            Duration = duration;
        }

        public string OperationId { get; set; }

        public string[] ProjectIds { get; set; }

        public int PackagesCount { get; set; }

        public NugetOperationStatus Status { get; set; }

        public string StatusMessage { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public double Duration { get; set; }
    }

    /// <summary>
    /// Define different states for nuget operation status.
    /// </summary>
    public enum NugetOperationStatus
    {
        /// <summary>
        /// no operation performed.
        /// </summary>
        NoOp = 0,

        /// <summary>
        /// operation was successful.
        /// </summary>
        Succeed = 1,

        /// <summary>
        /// operation failed.
        /// </summary>
        Failed = 2,
    }
}

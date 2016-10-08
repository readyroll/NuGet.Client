// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace NuGet.Common
{
    /// <summary>
    /// This class contains telemetry events name and properties name.
    /// </summary>
    public static class Constants
    {
        public const string EventNamePrefix = "VS/NuGet/";
        public const string PropertyNamePrefix = "VS.NuGet.";

        public const string NuGetVersionPropertyName = PropertyNamePrefix + "NuGetVersion";
        public const string ProjectIdPropertyName = PropertyNamePrefix + "ProjectId";

        // nuget telemetry event names
        public const string ProjectInformationEventName = EventNamePrefix + "ProjectInformation";
        public const string ProjectDependencyStatisticsEventName = EventNamePrefix + "DependencyStatistics";
        public const string NugetActionEventName = EventNamePrefix + "NugetAction";
        public const string NugetActionStepsEventName = EventNamePrefix + "NugetActionSteps";
        public const string RestoreActionEventName = EventNamePrefix + "RestoreInformation";
        public const string RestoreStepsEventName = EventNamePrefix + "RestoreStepsInformation";

        // project information event data
        public const string NuGetProjectTypePropertyName = PropertyNamePrefix + "NuGetProjectType";

        // dependency statistics event data
        public const string InstalledPackageCountPropertyName = PropertyNamePrefix + "InstalledPackageCount";

        // nuget action event data
        public const string OperationIdPropertyName = PropertyNamePrefix + "OperationId";
        public const string ProjectIdsPropertyName = PropertyNamePrefix + "ProjectIds";
        public const string OperationTypePropertyName = PropertyNamePrefix + "OperationType";
        public const string OperationSourcePropertyName = PropertyNamePrefix + "OperationSource";
        public const string PackagesCountPropertyName = PropertyNamePrefix + "PackagesCount";
        public const string OperationStatusPropertyName = PropertyNamePrefix + "OperationStatus";
        public const string StatusMessagePropertyName = PropertyNamePrefix + "StatusMessage";
        public const string StartTimePropertyName = PropertyNamePrefix + "StartTime";
        public const string EndTimePropertyName = PropertyNamePrefix + "EndTime";
        public const string DurationPropertyName = PropertyNamePrefix + "Duration";

        // nuget action step event data
        public const string StepNamePropertyName = PropertyNamePrefix + "StepName";

    }
}

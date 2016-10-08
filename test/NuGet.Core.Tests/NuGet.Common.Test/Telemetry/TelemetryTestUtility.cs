// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Xunit;

namespace NuGet.Common.Test.Telemetry
{
    public static class TelemetryTestUtility
    {
        public static void VerifyTelemetryEventData(ActionEventBase expected, TelemetryEvent actual)
        {
            Assert.Equal(expected.OperationId, actual.Properties[Constants.OperationIdPropertyName].ToString());
            Assert.Equal(string.Join(",", expected.ProjectIds), actual.Properties[Constants.ProjectIdsPropertyName].ToString());
            Assert.Equal(expected.PackagesCount, (int)actual.Properties[Constants.PackagesCountPropertyName]);
            Assert.Equal(expected.Status.ToString(), actual.Properties[Constants.OperationStatusPropertyName].ToString());
            Assert.Equal(expected.StatusMessage, actual.Properties[Constants.StatusMessagePropertyName].ToString());
            Assert.Equal(expected.StartTime.ToString(), actual.Properties[Constants.StartTimePropertyName].ToString());
            Assert.Equal(expected.EndTime.ToString(), actual.Properties[Constants.EndTimePropertyName].ToString());
            Assert.Equal(expected.Duration, (double)actual.Properties[Constants.DurationPropertyName]);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NuGet.Common
{
    public class TelemetrySessionContext : ITelemetrySessionContext
    {
        public TelemetrySessionContext(ITelemetrySession telemetrySession, string operationId)
        {
            if (telemetrySession == null)
            {
                throw new ArgumentNullException(nameof(telemetrySession));
            }

            if (operationId == null)
            {
                throw new ArgumentNullException(nameof(operationId));
            }

            TelemetrySession = telemetrySession;
            OperationId = operationId;
        }

        public ITelemetrySession TelemetrySession { get; set; }

        public string OperationId { get; set; }
    }
}

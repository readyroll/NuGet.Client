using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NuGet.Common
{
    public interface ITelemetrySessionContext
    {
        ITelemetrySession TelemetrySession { get; set; }

        string OperationId { get; set; }
    }
}

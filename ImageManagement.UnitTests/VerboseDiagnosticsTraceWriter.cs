using Microsoft.Azure.WebJobs.Host;
using System.Diagnostics;

namespace UnitTestProject1
{
    internal class VerboseDiagnosticsTraceWriter : TraceWriter
    {
        public VerboseDiagnosticsTraceWriter() : base(TraceLevel.Verbose)
        {
        }

        public override void Trace(TraceEvent traceEvent)
        {
            Debug.WriteLine(traceEvent.Message);
        }
    }
}

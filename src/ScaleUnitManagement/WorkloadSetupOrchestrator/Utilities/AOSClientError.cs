using System;
using System.Net;

namespace ScaleUnitManagement.WorkloadSetupOrchestrator.Utilities
{
    public sealed class AOSClientError : Exception
    {
        public AOSClientError(HttpStatusCode statusCode, string message)
            : base(message)
        {
            StatusCode = statusCode;
        }

        public HttpStatusCode StatusCode { get; }
    }
}

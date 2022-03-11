using System;
using System.Runtime.Serialization;

namespace MatplotlibCS.PlotItems
{
    public interface IHealthCheck
    {
        void HealthCheck();
    }

    [Serializable]
    public class HealthCheckException : Exception
    {
        public HealthCheckException()
        {
        }

        public HealthCheckException(string message)
            : base(message)
        {
        }

        public HealthCheckException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected HealthCheckException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}

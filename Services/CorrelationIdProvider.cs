using System;

namespace NET.GettingStarted.Services
{
    public class CorrelationIdProvider
    {
        private readonly string _correlationId;

        public CorrelationIdProvider()
        {
            _correlationId = Guid.NewGuid().ToString();
        }

        public string CorrelationId => _correlationId;
    }
}
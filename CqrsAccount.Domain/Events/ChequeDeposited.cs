namespace CqrsAccount.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Newtonsoft.Json;
    using ReactiveDomain.Messaging;



    public class ChequeDeposited : Event
    {
        public ChequeDeposited(CorrelatedMessage source)
            : base(source)
        { }

        [JsonConstructor]
        public ChequeDeposited(CorrelationId correlationId, SourceId sourceId)
            : base(correlationId, sourceId)
        { }

        public Guid AccountId { get; set; }

        public decimal Amount { get; set; }
    }
}

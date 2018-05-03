namespace CqrsAccount.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Newtonsoft.Json;
    using ReactiveDomain.Messaging;



    public class CashDeposited : Event
    {
        public CashDeposited(CorrelatedMessage source)
            : base(source)
        { }

        [JsonConstructor]
        public CashDeposited(CorrelationId correlationId, SourceId sourceId)
            : base(correlationId, sourceId)
        { }

        public Guid AccountId { get; set; }

        public decimal Amount { get; set; }
    }
}

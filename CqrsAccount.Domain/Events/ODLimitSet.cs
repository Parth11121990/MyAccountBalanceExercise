namespace CqrsAccount.Domain
{ 
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Newtonsoft.Json;
    using ReactiveDomain.Messaging;



    public class ODLimitSet : Event
    {
        public ODLimitSet(CorrelatedMessage source)
            : base(source)
        { }

        [JsonConstructor]
        public ODLimitSet(CorrelationId correlationId, SourceId sourceId)
            : base(correlationId, sourceId)
        { }

        public Guid AccountId { get; set; }

        public decimal ODLimit { get; set; }
    }
}

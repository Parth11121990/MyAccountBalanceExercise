namespace CqrsAccount.Domain
{
    using System;
    using Newtonsoft.Json;
    using ReactiveDomain.Messaging;

    public sealed class DailyWireTransferLimitSet : Event
    {
        public DailyWireTransferLimitSet(CorrelatedMessage source)
            : base(source)
        { }

        [JsonConstructor]
        public DailyWireTransferLimitSet(CorrelationId correlationId, SourceId sourceId)
            : base(correlationId, sourceId)
        { }

        public Guid AccountId { get; set; }

        public decimal DailyWireTransferLimit { get; set; }
    }
}
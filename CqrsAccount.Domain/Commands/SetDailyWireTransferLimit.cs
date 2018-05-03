namespace CqrsAccount.Domain
{
    using System;
    using ReactiveDomain.Messaging;

    public class SetDailyWireTransferLimit : Command
    {
        public SetDailyWireTransferLimit()
            : base(NewRoot())
        { }

        public Guid AccountId { get; set; }

        public decimal DailyWireTransferLimit { get; set; }
    }
}

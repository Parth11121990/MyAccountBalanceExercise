using System;
using System.Collections.Generic;
using System.Text;
using ReactiveDomain.Messaging;

namespace CqrsAccount.Domain.Commands
{
    public class SetODLimit : Command
    {
        public SetODLimit() : base(NewRoot()) {

        }
        public Guid AccountId { get; set; }

        public decimal ODLimit { get; set; }

    }
}

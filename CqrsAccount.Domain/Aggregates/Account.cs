namespace CqrsAccount.Domain
{
    using System;
    using ReactiveDomain;
    using ReactiveDomain.Messaging;
    using CqrsAccount.Domain;
    using CqrsAccount.Domain.Commands;

    public sealed class Account : EventDrivenStateMachine
    {
        decimal _accountBalance;
        decimal _accountOverdraftLimit;
        decimal _accountDailyWireTransferLimit;
        Account()
        {
            Register<AccountCreated>(e => { Id = e.AccountId; });
            Register<SetODLimit>(e => { _accountOverdraftLimit = e.ODLimit; });
            Register<SetDailyWireTransferLimit>(e => { _accountDailyWireTransferLimit = e.DailyWireTransferLimit; });
            Register<ChequeDeposited>(e => { });
            Register<CashDeposited>(e => { _accountBalance += e.Amount; });
            
        }

        public static Account Create(Guid id, string AccountHolderName, CorrelatedMessage source)
        {
            if (string.IsNullOrWhiteSpace(AccountHolderName))
                throw new ValidationException("A valid account owner name must be provided");

            var account = new Account();

            account.Raise(new AccountCreated(source)
            {
                AccountId = id,
                AccountHolderName = AccountHolderName
            });

            return account;
        }

        public void SetODLimit(decimal ODLimit,CorrelatedMessage source)
        {
            if (ODLimit < 0)
            {
                throw new ValidationException("Overdraft limit must be a Positive amount");
            }

            Raise(new ODLimitSet(source)
            {
                AccountId = Id,
                ODLimit = ODLimit

            });

        }

        public void SetDailyWireTransferLimit(decimal dailyWireTransferLimit, CorrelatedMessage source)
        {
            if (dailyWireTransferLimit < 0)
                throw new ValidationException("Daily wire transfer limit must be a Positive amount");

            Raise(new DailyWireTransferLimitSet(source)
            {
                AccountId = Id,
                DailyWireTransferLimit = dailyWireTransferLimit
            });
        }

        public void DepositCash(decimal amount, CorrelatedMessage source)
        {
            if (amount <= 0)
                throw new ValidationException("Cash deposited must be a positive amount");

            Raise(new CashDeposited(source)
            {
                AccountId = Id,
                Amount = amount
            });
        }

        public void DepositCheque(decimal amount, CorrelatedMessage source)
        {
            if (amount <= 0)
                throw new ValidationException("Cheque deposited must be a positive amount");

            Raise(new ChequeDeposited(source)
            {
                AccountId = Id,
                Amount = amount
            });
        }
    }
}

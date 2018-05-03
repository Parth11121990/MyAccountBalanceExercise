namespace CqrsAccount.Domain.Tests
{
    using System;
    using System.Threading.Tasks;
    using CqrsAccount.Domain.Tests.Common;
    using ReactiveDomain.Messaging;
    using Xunit;
    using Xunit.ScenarioReporting;

    [Collection("AggregateTest")]
    public sealed class UC5CashDepositedTestCases : IDisposable
    {
        readonly Guid _accountId;
        readonly EventStoreScenarioRunner<Account> _runner;

        public UC5CashDepositedTestCases(EventStoreFixture fixture)
        {
            _accountId = Guid.NewGuid();
            _runner = new EventStoreScenarioRunner<Account>(
                _accountId,
                fixture,
                (repository, dispatcher) => new AccountCommandHandler(repository, dispatcher));
        }

        public void Dispose()
        {
            _runner.Dispose();
        }

        [Theory]
        [InlineData(0.01)]
        [InlineData(100)]
        [InlineData(100000000)]
        public async Task CanDepositCash(double amount)
        {
            var created = new AccountCreated(CorrelatedMessage.NewRoot())
            {
                AccountId = _accountId,
                AccountHolderName = "Parth Sheth"
            };

            var cmd = new DepositCash
            {
                AccountId = _accountId,
                Amount = Convert.ToDecimal(amount)
            };

            var limitSet = new CashDeposited(cmd)
            {
                AccountId = _accountId,
                Amount = cmd.Amount
            };

            await _runner.Run(
                def => def.Given(created).When(cmd).Then(limitSet)
            );
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-0.01)]
        [InlineData(-100)]
        [InlineData(-100000000)]
        public async Task CannotDepositCashWithNonPositiveAmount(double amount)
        {
            var created = new AccountCreated(CorrelatedMessage.NewRoot())
            {
                AccountId = _accountId,
                AccountHolderName = "Parth Sheth"
            };

            var cmd = new DepositCash
            {
                AccountId = _accountId,
                Amount = Convert.ToDecimal(amount)
            };

            await _runner.Run(
                def => def.Given(created).When(cmd).Throws(new ValidationException("Cash deposited must be a positive amount"))
            );
        }

        [Fact]
        public async Task CannotDepositCashIntoInvalidAccount()
        {
            var cmd = new DepositCash
            {
                AccountId = _accountId,
                Amount = Convert.ToDecimal(100m)
            };

            await _runner.Run(
                def => def.Given().When(cmd).Throws(new ValidationException("No account with this ID exists"))
            );
        }
    }
}

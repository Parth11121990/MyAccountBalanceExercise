namespace CqrsAccount.Domain.Tests
{
    using System;
    using System.Threading.Tasks;
    using CqrsAccount.Domain.Tests.Common;
    using ReactiveDomain.Messaging;
    using Xunit;
    using Xunit.ScenarioReporting;

    [Collection("AggregateTest")]
    public sealed class UC6CashWithDrawTestCases : IDisposable
    {
        readonly Guid _accountId;
        readonly EventStoreScenarioRunner<Account> _runner;

        public UC6CashWithDrawTestCases(EventStoreFixture fixture)
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
        [InlineData(0.05)]
        [InlineData(50)]
        [InlineData(500)]
        public async Task CanWithdrawCash(double amount)
        {
            var created = new AccountCreated(CorrelatedMessage.NewRoot())
            {
                AccountId = _accountId,
                AccountHolderName = "Parth Sheth"
            };

            var cmd = new WithdrawCash
            {
                AccountId = _accountId,
                Amount = Convert.ToDecimal(amount)
            };

            var limitSet = new CashWithdrawn(cmd)
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
        [InlineData(-0.05)]
        [InlineData(-50)]
        [InlineData(-500)]
        public async Task CannotWithdrawCashWithNonPositiveAmount(double amount)
        {
            var created = new AccountCreated(CorrelatedMessage.NewRoot())
            {
                AccountId = _accountId,
                AccountHolderName = "Parth Sheth"
            };

            var cmd = new WithdrawCash
            {
                AccountId = _accountId,
                Amount = Convert.ToDecimal(amount)
            };

            await _runner.Run(
                def => def.Given(created).When(cmd).Throws(new ValidationException("Cash withdrawn must be a positive amount"))
            );
        }

        [Fact]
        public async Task CannotWithdrawCashIntoInvalidAccount()
        {
            var cmd = new WithdrawCash
            {
                AccountId = _accountId,
                Amount = Convert.ToDecimal(500)
            };

            await _runner.Run(
                def => def.Given().When(cmd).Throws(new ValidationException("No account with this ID exists"))
            );
        }
    }
}

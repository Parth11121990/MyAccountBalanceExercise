namespace CqrsAccount.Domain.Tests
{
    using System;
    using System.Threading.Tasks;
    using CqrsAccount.Domain.Tests.Common;
    using ReactiveDomain.Messaging;
    using Xunit;
    using Xunit.ScenarioReporting;

    [Collection("AggregateTest")]
    public sealed class UC3DailyWireTransferLimitTestCases : IDisposable
    {
        readonly Guid _accountId;
        readonly EventStoreScenarioRunner<Account> _runner;

        public UC3DailyWireTransferLimitTestCases(EventStoreFixture fixture)
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
        [InlineData(0)]
        [InlineData(0.01)]
        [InlineData(100)]
        [InlineData(100000000)]
        public async Task CanSetDailyWireTransferLimit(double limit)
        {
            var created = new AccountCreated(CorrelatedMessage.NewRoot())
            {
                AccountId = _accountId,
                AccountHolderName = "Parth Sheth"
            };

            var cmd = new SetDailyWireTransferLimit
            {
                AccountId = _accountId,
                DailyWireTransferLimit = Convert.ToDecimal(limit)
            };

            var limitSet = new DailyWireTransferLimitSet(cmd)
            {
                AccountId = _accountId,
                DailyWireTransferLimit = cmd.DailyWireTransferLimit
            };

            await _runner.Run(
                def => def.Given(created).When(cmd).Then(limitSet)
            );
        }

        [Theory]
        [InlineData(-0.01)]
        [InlineData(-100)]
        [InlineData(-100000000)]
        public async Task CannotSetNegativeLimit(double limit)
        {
            var created = new AccountCreated(CorrelatedMessage.NewRoot())
            {
                AccountId = _accountId,
                AccountHolderName = "Parth Sheth"
            };

            var cmd = new SetDailyWireTransferLimit
            {
                AccountId= _accountId,
                DailyWireTransferLimit = Convert.ToDecimal(limit)
            };

            await _runner.Run(
                def => def.Given(created).When(cmd).Throws(new ValidationException("Daily wire transfer limit must be a Positive amount"))
            );
        }

        [Fact]
        public async Task CannotSetLimitOnInvalidAccount()
        {
            var cmd = new SetDailyWireTransferLimit
            {
                AccountId = _accountId,
                DailyWireTransferLimit = Convert.ToDecimal(400)
            };

            await _runner.Run(
                def => def.Given().When(cmd).Throws(new ValidationException("No account with this ID exists"))
            );
        }
    }
}

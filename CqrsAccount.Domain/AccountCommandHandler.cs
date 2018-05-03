namespace CqrsAccount.Domain
{
    using System;
    using CqrsAccount.Domain.Commands;
    using ReactiveDomain.Foundation;
    using ReactiveDomain.Messaging;
    using ReactiveDomain.Messaging.Bus;

    public sealed class AccountCommandHandler
        : IHandleCommand<CreateAccount>
        , IHandleCommand<SetODLimit>
        , IHandleCommand<SetDailyWireTransferLimit>
        , IHandleCommand<DepositCheque>
        , IHandleCommand<DepositCash>
        , IHandleCommand<WithdrawCash>
        , IDisposable
    {
        readonly IRepository _repository;
        readonly IDisposable _disposable;

        public AccountCommandHandler(IRepository repository, ICommandSubscriber dispatcher)
        {
            _repository = repository;

            _disposable = new CompositeDisposable
            {
                dispatcher.Subscribe<CreateAccount>(this),
                dispatcher.Subscribe<SetODLimit>(this),
                dispatcher.Subscribe<SetDailyWireTransferLimit>(this),
                dispatcher.Subscribe<DepositCheque>(this),
                dispatcher.Subscribe<DepositCash>(this),
                dispatcher.Subscribe<WithdrawCash>(this),
            };
        }

        public void Dispose()
        {
            _disposable?.Dispose();
        }

        public CommandResponse Handle(CreateAccount command)
        {
            try
            {
                if (_repository.TryGetById<Account>(command.AccountId, out var _))
                    throw new ValidationException("An account with this ID already exists");

                var account = Account.Create(command.AccountId, command.AccountHolderName, command);

                _repository.Save(account);
                return command.Succeed();
            }
            catch (Exception e)
            {
                return command.Fail(e);
            }
        }

        public CommandResponse Handle(SetODLimit command)
        {
            try
            {
                if (!_repository.TryGetById<Account>(command.AccountId, out var account))
                    throw new ValidationException("No account with this ID exists");

                account.SetODLimit(command.ODLimit, command);

                _repository.Save(account);
                return command.Succeed();
            }
            catch (Exception e)
            {
                return command.Fail(e);
            }
        }

        public CommandResponse Handle(SetDailyWireTransferLimit command)
        {
            try
            {
                if (!_repository.TryGetById<Account>(command.AccountId, out var account))
                    throw new ValidationException("No account with this ID exists");

                account.SetDailyWireTransferLimit(command.DailyWireTransferLimit , command);

                _repository.Save(account);
                return command.Succeed();
            }
            catch (Exception e)
            {
                return command.Fail(e);
            }
        }

        public CommandResponse Handle(DepositCash command)
        {
            try
            {
                if (!_repository.TryGetById<Account>(command.AccountId, out var account))
                    throw new ValidationException("No account with this ID exists");

                account.DepositCash(command.Amount, command);

                _repository.Save(account);
                return command.Succeed();
            }
            catch (Exception e)
            {
                return command.Fail(e);
            }
        }

        public CommandResponse Handle(DepositCheque command)
        {
            try
            {
                if (!_repository.TryGetById<Account>(command.AccountId, out var account))
                    throw new ValidationException("No account with this ID exists");

                account.DepositCheque(command.Amount, command);

                _repository.Save(account);
                return command.Succeed();
            }
            catch (Exception e)
            {
                return command.Fail(e);
            }
        }

        public CommandResponse Handle(WithdrawCash command)
        {
            try
            {
                if (!_repository.TryGetById<Account>(command.AccountId, out var account))
                    throw new ValidationException("No account with this ID exists");

                account.WithdrawCash(command.Amount, command);

                _repository.Save(account);
                return command.Succeed();
            }
            catch (Exception e)
            {
                return command.Fail(e);
            }
        }
    }
}

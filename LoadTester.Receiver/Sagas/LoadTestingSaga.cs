using NServiceBus;
using Receiver.Sagas.SagaDatas;
using LoadTester.Shared.Commands;
using LoadTester.Shared.Commands.Responses;

namespace Receiver.Sagas
{
    public class LoadTestingSaga : Saga<LoadTestingSagaData>, 
                    IAmStartedByMessages<LoadTestingCommand>,
                    IHandleMessages<LogPaymentResponse>,
                    IHandleMessages<CheckPermissionsResponse>,
                    IHandleMessages<CheckBalanceResponse>,
                    IHandleMessages<ExecutePaymentResponse>,
                    IHandleMessages<StoreExecutedPaymentResponse>
    {
        protected override void ConfigureHowToFindSaga(SagaPropertyMapper<LoadTestingSagaData> mapper)
        {
            mapper.MapSaga(saga => saga.CorrelationId)
                .ToMessage<LoadTestingCommand>(msg => msg.Id);
        }

        public async Task Handle(LoadTestingCommand message, IMessageHandlerContext context)
        {
            await context.SendLocal(new LogPaymentCommand());
        }

        public async Task Handle(LogPaymentResponse message, IMessageHandlerContext context)
        {
            await context.SendLocal(new CheckPermissionsCommand());
        }

        public async Task Handle(CheckPermissionsResponse message, IMessageHandlerContext context)
        {
            await context.SendLocal(new CheckBalanceCommand());
        }

        public async Task Handle(CheckBalanceResponse message, IMessageHandlerContext context)
        {
            await context.SendLocal(new ExecutePaymentCommand());
        }

        public async Task Handle(ExecutePaymentResponse message, IMessageHandlerContext context)
        {
            await context.SendLocal(new StoreExecutedPaymentCommand());
        }

        public Task Handle(StoreExecutedPaymentResponse message, IMessageHandlerContext context)
        {
            MarkAsComplete();

            return Task.CompletedTask;
        }
    }
}

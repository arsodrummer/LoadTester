using NServiceBus;

namespace Receiver.Sagas.SagaDatas
{
    public class LoadTestingSagaData : ContainSagaData
    {
        public string CorrelationId { get; set; }
    }
}

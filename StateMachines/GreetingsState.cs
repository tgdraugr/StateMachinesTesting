using System;
using Automatonymous;

namespace StateMachines 
{
    public class GreetingsState : SagaStateMachineInstance
    {
        public string CurrentState { get; set; }

        public string Sender { get; set; }

        public DateTime? Created { get; set; }

        public DateTime? Updated { get; set; }

        public int Occurrences { get; set; }

        //public byte[] RowVersion { get; set; }

        public Guid CorrelationId { get; set; }
    }
}

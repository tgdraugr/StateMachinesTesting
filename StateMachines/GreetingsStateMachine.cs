using Automatonymous;
using MassTransit;

namespace StateMachines 
{
    public class GreetingsStateMachine : MassTransitStateMachine<GreetingsState>
    {
        public GreetingsStateMachine()
        {
            InstanceState(state => state.CurrentState);

            Event(() => HelloSaid, @event =>
            {
                @event.CorrelateBy((state, context) => state.Sender == context.Message.Sender)
                    .SelectId(context => context.CorrelationId ?? InVar.CorrelationId);

                @event.InsertOnInitial = true;

                var timestamp = InVar.Timestamp;
                @event.SetSagaFactory(context => new GreetingsState
                {
                    CorrelationId = context.CorrelationId ?? InVar.CorrelationId,
                    Sender = context.Message.Sender,
                    Created = timestamp,
                    Updated = timestamp,
                    Occurrences = 1
                });
            });

            Event(() => GoodbyeSaid, @event =>
            {
                @event.CorrelateBy((state, context) => state.Sender == context.Message.Sender)
                    .SelectId(context => context.CorrelationId ?? InVar.CorrelationId);;
            });

            Initially(
                When(HelloSaid)
                    .TransitionTo(Greeted),
                Ignore(GoodbyeSaid)
                );

            During(Greeted,
                When(HelloSaid)
                    .Then(context =>
                    {
                        var timestamp = InVar.Timestamp;

                        if (timestamp > context.Instance.Updated)
                            context.Instance.Updated = timestamp;

                        context.Instance.Occurrences += 1;
                    }),
                When(GoodbyeSaid)
                    .Finalize()
            );
        }

        public State Greeted { get; set; }

        public Event<ISayHello> HelloSaid { get; set; }

        public Event<ISayGoodbye> GoodbyeSaid { get; set; }
    }
}

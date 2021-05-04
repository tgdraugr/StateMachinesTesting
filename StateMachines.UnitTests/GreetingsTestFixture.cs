using System;
using System.Threading.Tasks;
using MassTransit;
using MassTransit.Testing;
using NUnit.Framework;

namespace StateMachines.UnitTests 
{
    public class GreetingsTestFixture 
    {
        protected InMemoryTestHarness TestHarness { get; private set; }

        private GreetingsStateMachine StateMachine { get; set; }

        protected IStateMachineSagaTestHarness<GreetingsState, GreetingsStateMachine> SagaTestHarness { get; private set; }

        protected ISendEndpoint InputQueueSendEndpoint => TestHarness.InputQueueSendEndpoint;

        [SetUp]
        public virtual async Task BeforeEachTest()
        {
            TestHarness = new InMemoryTestHarness {TestTimeout = TimeSpan.FromSeconds(4)};
            StateMachine = new GreetingsStateMachine();
            SagaTestHarness =
                TestHarness.StateMachineSaga<GreetingsState, GreetingsStateMachine>(StateMachine);

            await TestHarness.Start();
        }

        [TearDown]
        public virtual async Task AfterEachTest()
        {
            await TestHarness.Stop();
        }
    }
}

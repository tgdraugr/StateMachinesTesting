using System;
using System.Linq;
using System.Threading.Tasks;
using MassTransit.Testing;
using NUnit.Framework;

namespace StateMachines.UnitTests 
{
    public class TestNonExistentState 
        : GreetingsTestFixture
    {
        [Test]
        public async Task WhenHelloReceived_ShouldCreateState()
        {
            await InputQueueSendEndpoint.Send<ISayHello>(new { Sender = "aSender" });

            Assert.That(await TestHarness.Consumed.Any<ISayHello>(), Is.True);
            Assert.That(SagaTestHarness.Created.Count(), Is.EqualTo(1));

            var stateIds =
                await SagaTestHarness.Exists(state => state.Sender == "aSender", machine => machine.Greeted);
            var instance = SagaTestHarness.Sagas.Contains(stateIds.FirstOrDefault());

            Assert.That(instance, Is.Not.Null);
            Assert.That(instance.CorrelationId, Is.Not.EqualTo(Guid.Empty));
            Assert.That(instance.Sender, Is.EqualTo("aSender"));
            Assert.That(instance.Occurrences, Is.EqualTo(1));
            Assert.IsTrue(instance.Created == instance.Updated);
        }
    }

    public class TestGreetedState 
        : GreetingsTestFixture
    {
        private async Task SayManyHellos(string sender, int count)
        {
            var tasks = Enumerable.Range(0, count)
                .Select(_ => InputQueueSendEndpoint.Send<ISayHello>(new {Sender = sender}))
                .ToList();

            await Task.WhenAll(tasks);
        }

        [Test]
        public async Task WhenHelloReceivedFromAnotherUser_ShouldRemainUnchanged()
        {
            await SayManyHellos("aSender", 3);

            await InputQueueSendEndpoint.Send<ISayHello>(new {Sender = "anotherSender"});

            var stateIds =
                await SagaTestHarness.Exists(state => state.Sender == "aSender", machine => machine.Greeted);
            var instance = SagaTestHarness.Sagas.Contains(stateIds.FirstOrDefault());

            Assert.That(instance, Is.Not.Null);
            Assert.That(instance.CorrelationId, Is.Not.EqualTo(Guid.Empty));
            Assert.That(instance.CurrentState, Is.EqualTo(nameof(GreetingsStateMachine.Greeted)));
            Assert.That(instance.Sender, Is.EqualTo("aSender"));
            Assert.That(instance.Occurrences, Is.EqualTo(3));
        }

        [Test]
        public async Task WhenHelloReceivedFromSameUser_ShouldIncrementOccurrences()
        {
            await SayManyHellos("aSender", 3);

            await InputQueueSendEndpoint.Send<ISayHello>(new {Sender = "aSender"});

            Assert.That(await TestHarness.Consumed.Any<ISayHello>(), Is.True);

            var stateIds =
                await SagaTestHarness.Exists(state => state.Sender == "aSender", machine => machine.Greeted);
            var instance = SagaTestHarness.Sagas.Contains(stateIds.FirstOrDefault());

            Assert.That(instance, Is.Not.Null);
            Assert.That(instance.CorrelationId, Is.Not.EqualTo(Guid.Empty));
            Assert.That(instance.Sender, Is.EqualTo("aSender"));
            Assert.That(instance.Occurrences, Is.EqualTo(4));
            Assert.IsTrue(instance.Updated > instance.Created);
        }

        [Test]
        public async Task WhenGoodbyeReceivedFromAnotherUser_ShouldRemainUnchanged()
        {
            await SayManyHellos("aSender", 3);

            await InputQueueSendEndpoint.Send<ISayGoodbye>(new {Sender = "anotherSender"});

            var stateIds =
                await SagaTestHarness.Exists(state => state.Sender == "aSender", machine => machine.Greeted);
            var instance = SagaTestHarness.Sagas.Contains(stateIds.FirstOrDefault());

            Assert.That(instance, Is.Not.Null);
            Assert.That(instance.CorrelationId, Is.Not.EqualTo(Guid.Empty));
            Assert.That(instance.CurrentState, Is.EqualTo(nameof(GreetingsStateMachine.Greeted)));
            Assert.That(instance.Sender, Is.EqualTo("aSender"));
            Assert.That(instance.Occurrences, Is.EqualTo(3));
        }

        [Test]
        public async Task WhenGoodbyeReceivedFromSameUser_ShouldFinalize()
        {
            await SayManyHellos("aSender", 3);

            await InputQueueSendEndpoint.Send<ISayGoodbye>(new { Sender = "aSender" });

            Assert.That(await TestHarness.Consumed.Any<ISayGoodbye>(), Is.True);

            var stateIds =
                await SagaTestHarness.Exists(state => state.Sender == "aSender", machine => machine.Final);
            var instance = SagaTestHarness.Sagas.Contains(stateIds.FirstOrDefault());

            Assert.That(instance, Is.Not.Null);
            Assert.That(instance.CorrelationId, Is.Not.EqualTo(Guid.Empty));
            Assert.That(instance.Sender, Is.EqualTo("aSender"));
        }
    }
}
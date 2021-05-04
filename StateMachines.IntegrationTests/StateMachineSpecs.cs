using System;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace StateMachines.IntegrationTests 
{
    public class TestNonExistentState 
        : GreetingsTestFixture
    {
        public TestNonExistentState()
            : base(ServiceProviders.FromName(TestContext.Parameters["ServiceProvider"])) {
        }

        [Test]
        public async Task WhenHelloReceived_ShouldCreateState()
        {
            await Bus.Publish<ISayHello>(new {Sender="aSender"});
            await WaitForProcessingSaga();

            var instance = await DbContext.Set<GreetingsState>()
                .FirstOrDefaultAsync(state => state.Sender == "aSender");

            Assert.That(instance,Is.Not.Null);
            Assert.That(instance.CurrentState,Is.EqualTo(nameof(GreetingsStateMachine.Greeted)));
            Assert.That(instance.Sender,Is.EqualTo("aSender"));
            Assert.That(instance.Occurrences,Is.EqualTo(1));
            Assert.IsTrue(instance.Created == instance.Updated);
        }
    }

    public class TestGreetedState 
        : GreetingsTestFixture
    {
        public TestGreetedState()
            : base(ServiceProviders.FromName(TestContext.Parameters["ServiceProvider"])) {
        }

        [SetUp]
        public async Task BeforeEachTest()
        {
            await Bus.Publish<ISayHello>(new {Sender = "aSender"});
            await WaitForProcessingSaga();
        }

        [TearDown]
        public async Task AfterEachTest()
        {
            var instance =
                await DbContext.Set<GreetingsState>().FirstOrDefaultAsync(state => state.Sender == "aSender");

            DbContext.Remove(instance);
            await DbContext.SaveChangesAsync();
        }

        [Test]
        public async Task WhenHelloReceivedFromAnotherUser_ShouldRemainUnchanged()
        {
            await Bus.Publish<ISayHello>(new {Sender = "anotherSender"});
            await WaitForProcessingSaga();

            var instance = await DbContext.Set<GreetingsState>()
                .FirstOrDefaultAsync(state => state.Sender == "aSender");
            Assert.That(instance, Is.Not.Null);
            Assert.That(instance.CurrentState, Is.EqualTo(nameof(GreetingsStateMachine.Greeted)));
            Assert.That(instance.Sender,Is.EqualTo("aSender"));
            Assert.That(instance.Occurrences, Is.EqualTo(1));
        }

        [Test]
        public async Task WhenHelloReceivedFromSameUser_ShouldIncrementOccurrences()
        {
            await Bus.Publish<ISayHello>(new {Sender = "aSender"});
            await WaitForProcessingSaga();

            var instance = await DbContext.Set<GreetingsState>()
                .FirstOrDefaultAsync(state => state.Sender == "aSender");
            
            Assert.That(instance, Is.Not.Null);
            Assert.That(instance.CurrentState, Is.EqualTo(nameof(GreetingsStateMachine.Greeted)));
            Assert.That(instance.Sender,Is.EqualTo("aSender"));
            Assert.That(instance.Occurrences, Is.EqualTo(2));
        }

        [Test]
        public async Task WhenSeveralHellosReceivedFromSameUser_ShouldIncrementOccurrences()
        {
            var tasks = Enumerable.Range(0, 2)
                .Select(_ => Bus.Publish<ISayHello>(new {Sender = "aSender"}))
                .ToList();
            await Task.WhenAll(tasks);
            await WaitForProcessingSaga(TimeSpan.FromSeconds(2.5));
            
            var instance = await DbContext.Set<GreetingsState>()
                .FirstOrDefaultAsync(state => state.Sender == "aSender");
            
            Assert.That(instance, Is.Not.Null);
            Assert.That(instance.CurrentState, Is.EqualTo(nameof(GreetingsStateMachine.Greeted)));
            Assert.That(instance.Sender,Is.EqualTo("aSender"));
            Assert.That(instance.Occurrences, Is.EqualTo(3));
            Assert.That(instance.Updated > instance.Created);
        }

        [Test]
        public async Task WhenGoodbyeReceivedFromAnotherUser_ShouldRemainUnchanged()
        {
            await Bus.Publish<ISayGoodbye>(new {Sender = "anotherSender"});
            await WaitForProcessingSaga();
            
            var instance = await DbContext.Set<GreetingsState>()
                .FirstOrDefaultAsync(state => state.Sender == "aSender");

            Assert.That(instance, Is.Not.Null);
            Assert.That(instance.CurrentState, Is.EqualTo(nameof(GreetingsStateMachine.Greeted)));
            Assert.That(instance.Sender,Is.EqualTo("aSender"));
            Assert.That(instance.Occurrences, Is.EqualTo(1));
        }

        [Test]
        public async Task WhenGoodbyeReceivedFromSameUser_ShouldFinalize()
        {
            await Bus.Publish<ISayGoodbye>(new {Sender = "aSender"});
            await WaitForProcessingSaga();

            var instance = await DbContext.Set<GreetingsState>()
                .FirstOrDefaultAsync(state => state.Sender == "aSender");
            Assert.That(instance, Is.Not.Null);
            Assert.That(instance.CurrentState, Is.EqualTo(nameof(GreetingsStateMachine.Final)));
        }
    }
}
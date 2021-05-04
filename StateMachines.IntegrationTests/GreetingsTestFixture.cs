using System;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace StateMachines.IntegrationTests 
{
    public class GreetingsTestFixture : IDisposable
    {
        protected GreetingsTestFixture(IServiceProvider serviceProvider)
        {
            Bus = serviceProvider.GetRequiredService<IBusControl>();
            DbContext = serviceProvider.GetRequiredService<GreetingsDbContext>();
            DbContext.Database.OpenConnection();
        }

        protected IBusControl Bus { get; }

        protected GreetingsDbContext DbContext { get; }
        
        protected async Task WaitForProcessingSaga(TimeSpan? delay = default)
        {
            await Task.Delay(delay ?? TimeSpan.FromSeconds(1));
        }

        [OneTimeSetUp]
        public async Task BeforeAllTests()
        {
            await Bus.StartAsync();
        }

        [OneTimeTearDown]
        public async Task AfterAllTests()
        {
            await Bus.StopAsync();
            DbContext.RemoveRange(DbContext.Set<GreetingsState>());
            await DbContext.SaveChangesAsync();
        }
        
        public void Dispose()
        {
            DbContext.Database.CloseConnection();
        }
    }
}

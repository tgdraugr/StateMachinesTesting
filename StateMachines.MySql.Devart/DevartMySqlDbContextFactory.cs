using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace StateMachines.MySql.Devart 
{
    public class DevartMySqlDbContextFactory : IDesignTimeDbContextFactory<GreetingsDbContext>
    {
        private const string ConnectionString = "Server=localhost;Database=StateMachines.Devart;Uid=root;Pwd=p@ssw0rd;";

        public GreetingsDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<GreetingsDbContext>();
            Configure(optionsBuilder);
            return new GreetingsDbContext(optionsBuilder.Options);
        }
        
        public static void Configure(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(ConnectionString ,mySql => 
            {
                mySql.MigrationsAssembly(typeof(DevartMySqlDbContextFactory).GetTypeInfo().Assembly.GetName().Name);
            });
        }
    }
}

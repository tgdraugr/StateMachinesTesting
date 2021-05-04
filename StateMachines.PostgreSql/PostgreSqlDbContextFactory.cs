using System;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace StateMachines.PostgreSql 
{
    public class PostgreSqlDbContextFactory : IDesignTimeDbContextFactory<GreetingsDbContext>
    {
        private const string ConnectionString = "Server=127.0.0.1;Port=5432;Database=StateMachines;User ID=postgres;Password=p@ssw0rd;";

        public GreetingsDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<GreetingsDbContext>();
            Configure(optionsBuilder);
            return new GreetingsDbContext(optionsBuilder.Options);
        }
        
        public static void Configure(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(ConnectionString, sqlServer => 
            {
                sqlServer.MigrationsAssembly(typeof(PostgreSqlDbContextFactory).GetTypeInfo().Assembly.GetName().Name);
                sqlServer.EnableRetryOnFailure(15,TimeSpan.FromSeconds(30),null);
            });
        }
    }
}

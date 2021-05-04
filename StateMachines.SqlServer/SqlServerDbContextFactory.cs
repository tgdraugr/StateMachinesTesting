using System;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace StateMachines.SqlServer 
{
    public class SqlServerDbContextFactory : IDesignTimeDbContextFactory<GreetingsDbContext>
    {
        private const string ConnectionString = "Server=127.0.0.1;Database=StateMachines;User Id=sa;Password=p@ssw0rd;";

        public GreetingsDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<GreetingsDbContext>();
            Configure(optionsBuilder);
            return new GreetingsDbContext(optionsBuilder.Options);
        }
        
        public static void Configure(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(ConnectionString, sqlServer => 
            {
                sqlServer.MigrationsAssembly(typeof(SqlServerDbContextFactory).GetTypeInfo().Assembly.GetName().Name);
                sqlServer.EnableRetryOnFailure(15,TimeSpan.FromSeconds(30),null);
            });
        }
    }
}

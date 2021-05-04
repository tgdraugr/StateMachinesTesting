using System;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace StateMachines.MySql.PomeloFoundation 
{
    public class PomeloMySqlDbContextFactory : IDesignTimeDbContextFactory<GreetingsDbContext>
    {
        private const string ConnectionString = "Server=localhost;Database=StateMachines.Pomelo;Uid=root;Pwd=p@ssw0rd;";

        public GreetingsDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<GreetingsDbContext>();
            Configure(optionsBuilder);
            return new GreetingsDbContext(optionsBuilder.Options);
        }
        
        public static void Configure(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(ConnectionString, ServerVersion.AutoDetect(ConnectionString) ,mySql => 
            {
                mySql.MigrationsAssembly(typeof(PomeloMySqlDbContextFactory).GetTypeInfo().Assembly.GetName().Name);
                mySql.EnableRetryOnFailure(15,TimeSpan.FromSeconds(30),null);
            });
        }
    }
}

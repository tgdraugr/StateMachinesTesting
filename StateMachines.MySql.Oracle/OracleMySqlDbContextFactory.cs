using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace StateMachines.MySql.Oracle 
{
    public class OracleMySqlDbContextFactory : IDesignTimeDbContextFactory<GreetingsDbContext>
    {
        private const string ConnectionString = "Server=localhost;Database=StateMachines.Oracle;Uid=root;Pwd=p@ssw0rd;";

        public GreetingsDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<GreetingsDbContext>();
            Configure(optionsBuilder);
            return new GreetingsDbContext(optionsBuilder.Options);
        }
        
        public static void Configure(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySQL(ConnectionString ,mySql => 
            {
                mySql.MigrationsAssembly(typeof(OracleMySqlDbContextFactory).GetTypeInfo().Assembly.GetName().Name);
            });
        }
    }
}

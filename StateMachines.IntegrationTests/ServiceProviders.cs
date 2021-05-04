using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MassTransit;
using MassTransit.EntityFrameworkCoreIntegration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StateMachines.MySql.Oracle;
using StateMachines.MySql.PomeloFoundation;
using StateMachines.PostgreSql;
using StateMachines.SqlServer;

namespace StateMachines.IntegrationTests 
{
    public static class ServiceProviders
    {
        private static readonly IServiceProvider WithSqlServer = New(SqlServerDbContextFactory.Configure);

        private static readonly IServiceProvider WithPostgreSql = New(PostgreSqlDbContextFactory.Configure, new PostgresLockStatementProvider());

        private static readonly IServiceProvider WithPomeloMySqlAndDefaultLocking = New(PomeloMySqlDbContextFactory.Configure, new MySqlLockStatementProvider());

        private static readonly IServiceProvider WithOracleMySqlAndDefaultLocking = New(OracleMySqlDbContextFactory.Configure, new MySqlLockStatementProvider());

        private static readonly IServiceProvider WithPomeloMySqlAndChangedLocking = New(PomeloMySqlDbContextFactory.Configure, new MySqlLockStatementProvider2());

        private static readonly IServiceProvider WithOracleMySqlAndChangedLocking = New(OracleMySqlDbContextFactory.Configure, new MySqlLockStatementProvider2());

        public static IServiceProvider FromName(string name)
        {
            var serviceProvider = Fields
                .Where(field => string.Equals(field.Name, name, StringComparison.InvariantCultureIgnoreCase))
                .Select(field => field.GetValue(null))
                .Cast<IServiceProvider>()
                .FirstOrDefault();

            if (serviceProvider is null)
                throw new InvalidOperationException($"The parameter '{name}' does not match with any of the possible values: {PossibleValues}");

            return serviceProvider;
        }

        private static IEnumerable<FieldInfo> Fields => typeof(ServiceProviders)
            .GetFields(BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);

        private static string PossibleValues => string.Join(",", Fields.Select(field => field.Name));
        
        private static IServiceProvider New(Action<DbContextOptionsBuilder> configureDbContext, ILockStatementProvider lockStatementProvider = null)
        {
            var services = new ServiceCollection();

            services
                .AddLogging(logging => logging.AddConsole())
                .Configure<LoggerFilterOptions>(loggerConfig => loggerConfig.MinLevel = LogLevel.Information)
                .AddDbContext<GreetingsDbContext>(configureDbContext)
                .AddMassTransit(massTransit =>
                {
                    massTransit.AddSagaStateMachine<GreetingsStateMachine, GreetingsState>()
                        .EntityFrameworkRepository(entityFramework =>
                        {
                            if (lockStatementProvider != null)
                                entityFramework.LockStatementProvider = lockStatementProvider;

                            entityFramework.ExistingDbContext<GreetingsDbContext>();
                        });

                    massTransit.UsingInMemory((context, bus) =>
                    {
                        bus.ReceiveEndpoint("greetings", endpoint =>
                        {
                            endpoint.StateMachineSaga<GreetingsState>(context);
                        });
                    });
                });

            return services.BuildServiceProvider();
        }
    }
}

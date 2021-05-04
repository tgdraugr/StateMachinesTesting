using System;
using System.Collections.Generic;
using MassTransit.EntityFrameworkCoreIntegration;
using MassTransit.EntityFrameworkCoreIntegration.Mappings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace StateMachines 
{
    public class GreetingsDbContext : SagaDbContext 
    {
        public GreetingsDbContext(DbContextOptions options) 
            : base(options)
        {
        }

        protected override IEnumerable<ISagaClassMap> Configurations
        {
            get
            {
                yield return new GreetingsStateMachineMap();
            }
        }

        private class GreetingsStateMachineMap : SagaClassMap<GreetingsState>
        {
            protected override void Configure(EntityTypeBuilder<GreetingsState> entity, ModelBuilder model)
            {
                base.Configure(entity, model);
                entity.ToTable("Greetings");
                entity.Property(state => state.CurrentState)
                    .HasMaxLength(64);
            }
        }
    }
}

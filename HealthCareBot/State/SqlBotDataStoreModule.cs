using System;
using System.Collections.Generic;
using Autofac;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Builder.Internals.Fibers;
using Microsoft.Bot.Connector;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.Bot.Builder.Azure;
using Microsoft.Bot.Builder.Scorables.Internals;
using Module = Autofac.Module;

namespace HealthCareBot.State
{
    public class SqlBotDataStoreModule : Module
    {
        public static readonly object KeyDataStore = new object();

        private readonly Assembly assembly;
        public SqlBotDataStoreModule(Assembly assembly)
        {
           
            SetField.NotNull(out assembly, nameof(assembly), assembly);
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ConnectorStore>()
                .AsSelf()
                .InstancePerLifetimeScope();


            SqlBotDataContext.AssertDatabaseReady();

            var store = new SqlServerBotDataStore(ConfigurationManager.ConnectionStrings["BotDataContextConnectionString"]
                .ConnectionString);


            builder.Register(c => store)
                .Keyed<IBotDataStore<BotData>>(KeyDataStore)
                .AsSelf()
                .SingleInstance();

            builder.Register(c => new CachingBotDataStore(c.ResolveKeyed<IBotDataStore<BotData>>(KeyDataStore),
                    CachingBotDataStoreConsistencyPolicy.LastWriteWins))
                .As<IBotDataStore<BotData>>()
                .AsSelf()
                .InstancePerLifetimeScope();
            
        }
    }
}

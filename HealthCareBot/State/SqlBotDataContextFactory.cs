using System.Data.Entity.Infrastructure;

namespace HealthCareBot.State
{
    public class SqlBotDataContextFactory : IDbContextFactory<SqlBotDataContext>
    {
        public SqlBotDataContext Create()
        {
            return new SqlBotDataContext("BotDataContextConnectionString");
        }
    }
}
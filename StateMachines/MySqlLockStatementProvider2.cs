using MassTransit.EntityFrameworkCoreIntegration;

namespace StateMachines
{
    public class MySqlLockStatementProvider2 :
        SqlLockStatementProvider
    {
        private const string DefaultSchemaName = "";
        private const string DefaultRowLockStatement = "SELECT * FROM `{1}` WHERE `{2}` = @p0 FOR UPDATE";

        public MySqlLockStatementProvider2(bool enableSchemaCaching = true)
            : base(DefaultSchemaName, DefaultRowLockStatement, enableSchemaCaching)
        {
        }
    }
}
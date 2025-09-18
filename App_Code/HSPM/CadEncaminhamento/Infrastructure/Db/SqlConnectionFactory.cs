using System.Configuration;
using System.Data.SqlClient;

namespace Hspm.CadEncaminhamento.Infrastructure
{
    public interface IConnectionFactory
    {
        SqlConnection Create();
    }

    public sealed class SqlConnectionFactory : IConnectionFactory
    {
        private readonly string _cs;

        public SqlConnectionFactory(string connectionStringName)
        {
            _cs = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
        }

        public SqlConnection Create()
        {
            return new SqlConnection(_cs);
        }
    }
}

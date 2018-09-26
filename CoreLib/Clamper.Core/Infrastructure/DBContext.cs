using System;
using System.Data;
using System.Data.SqlClient;
using Clamper.Core.Infrastructure.Interfaces;
using Clamper.Core.Infrastructure.Querying;
using Clamper.Core.Infrastructure.Querying.Strategies;
using MySql.Data.MySqlClient;

namespace Clamper.Core.Infrastructure
{
	/// <summary>
    /// An Implementation that uses SqlConnection
    /// </summary>
	public class DBContext : IDBContext
    {
        private readonly Func<IDbConnection> _getConnection;


		/// <summary>
        /// Initialize  a new dapper context 
        /// </summary>
        public DBContext(IDatabaseMetadataProvider databaseMetadataProvider)
		{
		    var connectionString = databaseMetadataProvider.GetConnectionString();

		    switch (databaseMetadataProvider.GetDbms())
            {
                case Dbms.Mssql:
                    QueryStrategy = new MicrosoftSqlServerQueryStrategy();
                    _getConnection = () => new SqlConnection(connectionString);
                    break;
                case Dbms.Mysql:
                    QueryStrategy = new MysqlQueryStrategy();
                   _getConnection = () => new MySqlConnection(connectionString);
                    break;
            }
		}

        public IUnitOfWork Unit() 
        {
            return new UnitOfWork(this);
        }

        public QueryStrategy QueryStrategy { get; }

        public IDbConnection GetConnection()
        {
            return _getConnection();
        }
    }
}


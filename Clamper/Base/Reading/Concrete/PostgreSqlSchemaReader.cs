using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Clamper.Base.Configuration.Abstract;
using Clamper.Base.Reading.Abstract;
using Clamper.Base.Reading.Concrete.Models;
using Clamper.Models.Abstract;
using Npgsql;

namespace Clamper.Base.Reading.Concrete
{
    internal class PostgreSqlSchemaReader: SqlSchemaReader, IDatabaseSchemaReader
    {
        internal override void Setup(IConfiguration configuration)
        {
            QueryToReadColumns = @"
            SELECT
              c.COLUMN_NAME AS ""Name""
               ,'""' || t.TABLE_SCHEMA || '""' || '.""' || t.TABLE_NAME || '""' AS ""TableFullName""
               ,t.TABLE_Name ""TableName""
               ,t.TABLE_TYPE  AS TableType
               ,c.IS_NULLABLE AS ""Nullable""
               ,c.DATA_TYPE AS ""DataType""
               ,CASE WHEN pkc.CONSTRAINT_NAME IS NULL THEN 0 ELSE 1 END AS IsPrimaryKey
               ,CASE WHEN fkc.CONSTRAINT_NAME IS NULL THEN 0 ELSE 1 END AS IsForeignKey
               ,rct.TABLE_NAME AS ReferencedTableName
               ,rcuc.COLUMN_NAME AS ReferencedColumn
               ,descrtbl.DESCRIPTION AS TABLECOMMENT
               ,descrcol.DESCRIPTION AS COLUMNCOMMENT
               ,CASE WHEN seq.sequence_name IS NULL THEN 0 ELSE 1 END AS IsIdentity
            FROM INFORMATION_SCHEMA.COLUMNS c
                INNER JOIN INFORMATION_SCHEMA.TABLES t
                  ON c.TABLE_NAME = t.TABLE_NAME AND t.TABLE_SCHEMA = c.TABLE_SCHEMA
                LEFT OUTER JOIN INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE ccu
                  ON c.TABLE_NAME = ccu.TABLE_NAME AND c.COLUMN_NAME = ccu.COLUMN_NAME AND ccu.TABLE_SCHEMA = c.TABLE_SCHEMA
                LEFT OUTER JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS pkc
                  ON pkc.CONSTRAINT_TYPE = 'PRIMARY KEY' AND pkc.CONSTRAINT_NAME = ccu.CONSTRAINT_NAME AND pkc.TABLE_SCHEMA = c.TABLE_SCHEMA
                LEFT OUTER JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS fkc
                  ON fkc.CONSTRAINT_TYPE = 'FOREIGN KEY' AND fkc.CONSTRAINT_NAME = ccu.CONSTRAINT_NAME AND fkc.TABLE_SCHEMA = c.TABLE_SCHEMA
                LEFT OUTER JOIN INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS rc
                  ON fkc.CONSTRAINT_NAME = rc.CONSTRAINT_NAME AND rc.CONSTRAINT_SCHEMA = c.TABLE_SCHEMA
                LEFT OUTER JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS rct
                  ON rct.CONSTRAINT_NAME = rc.UNIQUE_CONSTRAINT_NAME AND rct.CONSTRAINT_SCHEMA = c.TABLE_SCHEMA
                LEFT OUTER JOIN INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE rcuc
                  ON rc.UNIQUE_CONSTRAINT_NAME = rcuc.CONSTRAINT_NAME  AND rcuc.CONSTRAINT_SCHEMA = c.TABLE_SCHEMA
                LEFT OUTER JOIN INFORMATION_SCHEMA.SEQUENCES seq
                  ON c.COLUMN_DEFAULT LIKE 'nextval(''%'|| seq.sequence_name ||'''%'
                LEFT OUTER JOIN PG_CATALOG.PG_STATIO_ALL_TABLES sat
                  ON t.TABLE_SCHEMA = sat.SCHEMANAME AND sat.RELNAME = t.TABLE_NAME
                LEFT OUTER JOIN PG_CATALOG.PG_DESCRIPTION AS descrcol
                  ON c.ORDINAL_POSITION = descrcol.OBJSUBID AND descrcol.OBJOID = sat.RELID
                LEFT OUTER JOIN PG_CATALOG.PG_DESCRIPTION AS descrtbl
                  ON 0 = descrtbl.OBJSUBID AND descrtbl.OBJOID = sat.RELID
            WHERE t.TABLE_SCHEMA = 'public'
            ORDER BY c.TABLE_NAME, c.ORDINAL_POSITION";
        }

        protected override IDbConnection GetConnection(string connectionString)
        {
            return new NpgsqlConnection(connectionString);
        }

        protected override IDbCommand GetCommand(string query, IDbConnection connection, IDbTransaction transaction)
        {
            return new NpgsqlCommand(query, connection as NpgsqlConnection, transaction as NpgsqlTransaction);
        }

        protected override DatabaseSchemaColumn ReadColumn(IDataReader reader)
        {
            var column = new DatabaseSchemaColumn
            {
                Name = reader.GetString(0),
                TableFullName = reader.GetString(1),
                TableName = reader.GetString(2),
                Type = reader.GetString(3),
                Nullable = reader.GetString(4) == "YES",
                DataType = reader.GetString(5),
                IsPrimaryKey = reader.GetInt32(6) == 1,
                IsForeignKey = reader.GetInt32(7) == 1,
                TableComment = reader.GetString(10),
                Comment = reader.GetString(11),
                IsIdentity = reader.GetBoolean(12)
            };

            if (column.IsForeignKey)
            {
                column.ReferencedTableName = reader.GetString(8);
                column.ReferencedColumnName = reader.GetString(9);
            }

            return column;
        }

        protected override DatabaseParameter ReadParameter(IDataReader reader)
        {
            return new DatabaseParameter
            {
                Procedure = reader.GetString(0),
                Name = reader.GetString(1),
                DataType = reader.GetString(2),
                Position = reader.GetInt32(3)
            };
        }

        protected override ExtendedPropertyInfo ReadExtendedProperty(IDataReader reader)
        {
            throw new NotImplementedException();
        }

        protected override string GetEnumValueQuery(IConfiguration configuration, IConfigurationEnumTable configurationEnumTable)
        {
            return $@"SELECT ""{configurationEnumTable.NameColumn}"" AS ""Name""," +
                   $@"       ""{configurationEnumTable.ValueColumn}"" AS ""Value""" +
                   $@" FROM ""{configuration.Schema}"".""{configurationEnumTable.Table}""";
        }

        protected override void ProcessProcedureParameters(IStoredProcedure storedProcedure)
        {
            throw new NotImplementedException();
        }

        protected override List<DatabaseParameter> GetAllProcedureParameters(IDbConnection connection,IConfiguration configuration)
        {
            var query = $@"
            SELECT
              ,p.proname
              ,pg_catalog.pg_get_function_identity_arguments(p.oid)
            FROM   pg_catalog.pg_proc p
            JOIN   pg_catalog.pg_namespace n ON n.oid = p.pronamespace
            WHERE p.proisagg = false and n.nspname ='{configuration.Schema}'";

            var parameters = new List<DatabaseParameter>();
            using (var command = GetCommand(query, connection, null))
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var spName = reader.GetString(1);
                        var paramList = reader.GetString(2).Split(new [] { ","}, StringSplitOptions.RemoveEmptyEntries)
                            .Select(s => s.Trim());

                        var position = 1;
                        foreach (var parameter in paramList)
                        {
                            parameters.Add(new DatabaseParameter
                            {
                                DataType = parameter,
                                Position = position,
                                Procedure = spName,
                            });
                            position++;
                        }
                    }
                }
            }

            return parameters;
        }
    }
}

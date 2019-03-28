#region Usings

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Clamper.Base.Configuration.Abstract;
using Clamper.Base.Reading.Abstract;
using Clamper.Base.Reading.Concrete.Models;
using Clamper.Models.Abstract;
using Clamper.Tools;

#endregion

namespace Clamper.Base.Reading.Concrete
{
    internal class SqlServerSchemaReader : SqlSchemaReader, IDatabaseSchemaReader
    {
        protected override IDbCommand GetCommand(string query, IDbConnection connection, IDbTransaction transaction)
        {
            return new SqlCommand(query, connection as SqlConnection, transaction as SqlTransaction);
        }

        protected override IDbConnection GetConnection(string connectionString)
        {
            return new SqlConnection(connectionString);
        }

        protected override string GetEnumValueQuery(IConfiguration configuration,
            IConfigurationEnumTable enumTable)
        {
            return $"SELECT [{enumTable.NameColumn}] AS [Name]," +
                   $"       [{enumTable.ValueColumn}] AS [Value]" +
                   $" FROM [dbo].[{enumTable.Table}]";
        }

        protected override void ProcessProcedureParameters(IStoredProcedure storedProcedure)
        {
            var addedNames = new List<string>();
            var parameterString = storedProcedure.Parameters.Aggregate("", (current, param) => {

                if (addedNames.Contains(param.Name))
                    return current;
                else addedNames.Add(param.Name);
                var dataType = CommonTools.GetCSharpDataType(param.DataType, true);

                // Sometimes it can be a table type
                if (dataType.Length < 1)
                    dataType = "object";

                return current + $"{dataType} {param.Name.Replace("@", "")} = null" + ",";

            });

            addedNames = new List<string>();

            var parameterPassString = storedProcedure.Parameters.Aggregate("", (current, param) => {

                if (addedNames.Contains(param.Name))
                    return current;
                else addedNames.Add(param.Name);

                return $"{current} {param.Name.Replace("@", "")},";
            });

            storedProcedure.ParamString = parameterString.TrimEnd(',');
            storedProcedure.PassString = $"{{ {parameterPassString.TrimEnd(',')} }}" ;
        }

        protected override DatabaseSchemaColumn ReadColumn(IDataReader reader)
        {
            var column = new DatabaseSchemaColumn();
            column.Name = reader.GetString(0);
            column.TableFullName = reader.GetString(1);
            column.TableName = reader.GetString(2);
            column.Type = reader.GetString(3);
            column.Nullable = reader.GetString(4) == "YES";
            column.DataType = reader.GetString(5);
            column.IsPrimaryKey = reader.GetInt32(6) == 1;
            column.IsForeignKey = reader.GetInt32(7) == 1;
            column.IsIdentity = reader.GetInt32(8) == 1;
            column.Schema = reader.GetString(11);

            if (column.IsForeignKey)
            {
                column.ReferencedTableName = reader.GetString(9);
                column.ReferencedColumnName = reader.GetString(10);
            }

            return column;
        }

        protected override ExtendedPropertyInfo ReadExtendedProperty(IDataReader reader)
        {
            return new ExtendedPropertyInfo
            {
                SchemaName = reader.GetString(0),
                ObjectName = reader.GetString(1),
                ColumnName = reader.GetString(2),
                Property = reader.GetString(3)
            };
        }

        protected override DatabaseParameter ReadParameter(IDataReader reader)
        {
            var parameter = new DatabaseParameter
            {
                Procedure = reader.GetString(0),
                Name = reader.GetString(1),
                DataType = reader.GetString(2)
            };

            if (char.IsDigit(parameter.Procedure[0]))
            {
                parameter.Procedure = $"Sp{parameter.Procedure}";
            }

            return parameter;
        }

        internal override void Setup(IConfiguration configuration)
        {
            QueryToReadColumns = $@"
                       SELECT 
                c.COLUMN_NAME AS [Name]
               ,'[' + t.TABLE_SCHEMA + ']' + '.[' + t.TABLE_NAME + ']' AS [TableFullName]
               ,t.TABLE_Name [TableName]
               ,t.TABLE_TYPE  AS TableType
               ,c.IS_NULLABLE AS [Nullable]
               ,c.DATA_TYPE AS [DataType]
               ,CASE WHEN pkc.CONSTRAINT_NAME IS NULL THEN 0 ELSE 1 END AS IsPrimaryKey
               ,CASE WHEN fkc.CONSTRAINT_NAME IS NULL THEN 0 ELSE 1 END AS IsForeignKey
               ,ISNULL(COLUMNPROPERTY(object_id('[' + t.TABLE_SCHEMA + ']' + '.[' + t.TABLE_NAME + ']'), c.COLUMN_NAME, 'IsIdentity'), 0) AS IsIdentity
               ,rct.TABLE_NAME AS ReferencedTableName
               ,rcuc.COLUMN_NAME AS ReferencedColumn
               ,t.TABLE_SCHEMA AS [Schema]
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
                  ON rct.CONSTRAINT_NAME = rc.UNIQUE_CONSTRAINT_NAME
                LEFT OUTER JOIN INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE rcuc
                  ON rc.UNIQUE_CONSTRAINT_NAME = rcuc.CONSTRAINT_NAME
            WHERE t.TABLE_CATALOG = '$databaseName$'
            ORDER BY c.TABLE_NAME, c.ORDINAL_POSITION";


            QueryToGetParameters = @"
                    SELECT DISTINCT
                        p.SPECIFIC_NAME AS SP
                        ,p.PARAMETER_NAME AS [Name]
                        ,p.DATA_TYPE AS DataType
                        ,p.SCOPE_NAME
                        ,p.ORDINAL_POSITION
                    FROM INFORMATION_SCHEMA.PARAMETERS p
                        INNER JOIN INFORMATION_SCHEMA.ROUTINES r
                            ON p.SPECIFIC_NAME = r.SPECIFIC_NAME
                    WHERE r.ROUTINE_TYPE = 'PROCEDURE' AND p.SPECIFIC_CATALOG = '$databaseName$'
                    ORDER BY p.SCOPE_NAME , p.ORDINAL_POSITION";


            QueryToGetExtendedProperties = $@"SELECT
                         s.[name] AS SchemaName
                        ,oo.[name] AS ObjectName
                        ,col.[name] AS ColumnName
                        ,ep.[value] AS Property
                        FROM sys.all_objects oo 
                    INNER JOIN sys.extended_properties ep ON ep.major_id = oo.object_id 
                    LEFT JOIN sys.schemas s on oo.schema_id = s.schema_id
                    INNER JOIN sys.columns AS col ON ep.major_id = col.object_id AND ep.minor_id = col.column_id
                    WHERE ep.[value] IS NOT NULL AND ep.[value] <> ''";
        }
    }
}
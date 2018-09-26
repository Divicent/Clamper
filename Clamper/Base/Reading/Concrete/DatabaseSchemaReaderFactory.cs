#region Usings

using Clamper.Base.Reading.Abstract;

#endregion

namespace Clamper.Base.Reading.Concrete
{
    internal static class DatabaseSchemaReaderFactory
    {
        public static IDatabaseSchemaReader GetReader(string databaseManagementSystemName)
        {
            switch (databaseManagementSystemName.ToLowerInvariant())
            {
                case "mssql":
                    return new SqlServerSchemaReader();
                case "mysql":
                    return new MySqlSchemaReader();
                default:
                    return null;
            }
        }
    }
}
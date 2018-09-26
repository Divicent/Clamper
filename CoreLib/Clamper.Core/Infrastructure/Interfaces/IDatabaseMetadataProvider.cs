namespace Clamper.Core.Infrastructure.Interfaces
{
    /// <summary>
    /// Type of the target database
    /// </summary>
    public enum Dbms
    {
        /// <summary>
        /// Microsoft SQL server
        /// </summary>
        Mssql = 1,

        /// <summary>
        /// MySql server
        /// </summary>
        Mysql = 2
    }

    /// <summary>
    /// This is a provider which should be used to give the
    /// target connection string to the context, this should be
    /// implemented inside the applicaiton.
    /// </summary>
    public interface IDatabaseMetadataProvider
    {
        /// <summary>
        /// Get the target connection string
        /// </summary>
        /// <returns>The connection string</returns>
        string GetConnectionString();

        /// <summary>
        /// Get the type of the database
        /// </summary>
        Dbms GetDbms();
    }
}


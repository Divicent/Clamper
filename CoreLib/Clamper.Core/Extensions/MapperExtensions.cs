using System;
using System.Data;
using System.Threading.Tasks;
using System.Linq;
using Clamper.Core.Infrastructure.Models;
using Clamper.Core.Mapper;
using QB = Clamper.Core.Infrastructure.Querying.QueryBuilder;

namespace Clamper.Core.Extensions
{
    public static class MapperExtensions
    {
        /// <summary>
        /// Inserts an entity into table "T" and returns identity id.
        /// </summary>
        /// <param name="connection">Open SqlConnection</param>
        /// <param name="entityToInsert">Entity to insert</param>
        /// <param name="queryBuilder"></param>
        /// <returns>Identity of inserted entity</returns>
        public static object Insert(this IDbConnection connection, BaseModel entityToInsert, QB queryBuilder)
        {
            using (connection)
            {
                connection.Open();
                var cmd = queryBuilder.Insert(entityToInsert);
                return connection.ExecuteScalar(cmd, entityToInsert);
            }
        }


        /// <summary>
        /// Inserts an entity into table "T" and returns identity id asynchronously.
        /// </summary>
        /// <param name="connection">Open SqlConnection</param>
        /// <param name="entityToInsert">Entity to insert</param>
        /// <param name="queryBuilder"></param>
        /// <returns>Identity of inserted entity</returns>
        public static async Task<object> InsertAsync(this IDbConnection connection, BaseModel entityToInsert, QB queryBuilder)
        {
            using (connection)
            {
                connection.Open();
                var cmd = queryBuilder.Insert(entityToInsert);
                return await connection.ExecuteScalarAsync(cmd, entityToInsert);
            }
        }

        /// <summary>
        /// Updates entity in table "Ts", checks if the entity is modified if the entity is tracked by the Get() extension.
        /// </summary>
        /// <param name="connection">Open SqlConnection</param>
        /// <param name="entityToUpdate">Entity to be updated</param>
        /// <param name="queryBuilder"></param>
        /// <returns>true if updated, false if not found or not modified (tracked entities)</returns>
        public static bool Update(this IDbConnection connection, BaseModel entityToUpdate, QB queryBuilder)
        {
            var query = queryBuilder.Update(entityToUpdate);
            if (query == null)
            {
                return false;
            }
            using (connection)
            {
                connection.Open();
                var updated = connection.Execute(query, entityToUpdate);
                return updated > 0;
            }
        }

        /// <summary>
        /// Updates entity in table "Ts", checks if the entity is modified if the entity is tracked by the Get() extension asynchronously.
        /// </summary>
        /// <param name="connection">Open SqlConnection</param>
        /// <param name="entityToUpdate">Entity to be updated</param>
        /// <param name="queryBuilder"></param>
        /// <returns>true if updated, false if not found or not modified (tracked entities)</returns>
        public static async Task<bool> UpdateAsync(this IDbConnection connection, BaseModel entityToUpdate, QB queryBuilder)
        {
            var query = queryBuilder.Update(entityToUpdate);
            if (query == null)
            {
                return false;
            }
            using (connection)
            {
                connection.Open();
                var updated = await connection.ExecuteAsync(query, entityToUpdate);
                return updated > 0;
            }
        }

        /// <summary>
        /// Delete entity in table "Ts".
        /// </summary>
        /// <param name="connection">Open SqlConnection</param>
        /// <param name="entity"></param>
        /// <param name="queryBuilder"></param>
        /// <returns>true if deleted, false if not found</returns>
        public static bool Delete(this IDbConnection connection, BaseModel entity, QB queryBuilder)
        {
            using (connection)
            {
                connection.Open();
                var deleted = connection.Execute(queryBuilder.Delete(entity), entity) > 0;
                return deleted;
            }
        }


        /// <summary>
        /// Delete entity in table "Ts" asynchronously.
        /// </summary>
        /// <param name="connection">Open SqlConnection</param>
        /// <param name="entity"></param>
        /// <param name="queryBuilder"></param>
        /// <returns>true if deleted, false if not found</returns>
        public static async Task<bool> DeleteAsync(this IDbConnection connection, BaseModel entity, QB queryBuilder)
        {
            using (connection)
            {
                connection.Open();
                var deleted = await connection.ExecuteAsync(queryBuilder.Delete(entity), entity) > 0;
                return deleted;
            }
        }
    }
}

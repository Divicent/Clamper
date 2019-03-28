using System.Collections.Generic;

namespace Clamper.Models.Abstract
{
    public interface IModel
    {
        /// <summary>
        ///     Get all attributes
        /// </summary>
        /// <returns></returns>
        IEnumerable<ISimpleAttribute> GetAttributes();

        /// <summary>
        /// Schema of the model
        /// </summary>
        string Schema { get; set; }


        /// <summary>
        ///     Get name of the model
        /// </summary>
        /// <returns></returns>
        string GetName();

        /// <summary>
        ///     Get all foreign key attributes
        /// </summary>
        /// <returns></returns>
        IEnumerable<IForeignKeyAttribute> GetForeignKeyAttributes();

        /// <summary>
        ///     Get reference lists if available
        /// </summary>
        /// <returns></returns>
        IEnumerable<IReferenceList> GetReferenceLists();
    }
}
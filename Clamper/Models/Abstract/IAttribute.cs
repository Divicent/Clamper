﻿namespace Clamper.Models.Abstract
{
    /// <summary>
    ///     Represents an attribute (column) of a relation
    /// </summary>
    public interface IAttribute : ISimpleAttribute
    {
        /// <summary>
        ///     Is this a primary key of the relation
        /// </summary>
        bool IsKey { get; set; }

        /// <summary>
        ///     Is this an identity column (auto increment)
        /// </summary>
        bool IsIdentity { get; set; }

        /// <summary>
        ///     Reference property name if available
        /// </summary>
        string RefPropName { get; set; }
    }
}
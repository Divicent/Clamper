using Clamper.Core.Infrastructure.Models;

namespace Clamper.Core.Infrastructure.Interfaces
{
    /// <summary>
    /// Type of an operation
    /// </summary>
    public enum OperationType
    {
        Add = 1,
        Remove = 2,
        Update = 3
    }

    /// <summary>
    /// A database operation
    /// </summary>
	public interface IOperation
    {
        OperationType Type { get; }
        BaseModel Object { get; }
    }
}


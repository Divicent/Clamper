namespace Clamper.Core.Infrastructure.Models
{
    /// <summary>
    /// A column represents a table column meta-data
    /// </summary>
    /// <typeparam name="T">Type of the column</typeparam>
    public interface IColumn<T>
    {
        string Name { get; }
    }
}

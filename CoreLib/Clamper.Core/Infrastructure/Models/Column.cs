namespace Clamper.Core.Infrastructure.Models
{
    public class Column<T> : IColumn<T>
    {
        public Column(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }
}

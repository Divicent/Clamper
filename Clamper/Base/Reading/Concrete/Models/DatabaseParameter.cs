namespace Clamper.Base.Reading.Concrete.Models
{
    internal class DatabaseParameter
    {
        public string Procedure { get; set; }
        public string Name { get; set; }
        public string DataType { get; set; }
        public int Position { get; set; }
    }
}
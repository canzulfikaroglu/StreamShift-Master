namespace StreamShift.Application.DTOs
{
    public class TableSchema
    {
        public string SchemaName { get; set; }
        public string TableName { get; set; }
        public string ColumnName { get; set; }
        public string DataType { get; set; }
        public int? MaxLength { get; set; }
        public string IsPrimaryKey { get; set; }
        public string IsNotNull { get; set; }
        public string? DefaultValue { get; set; }
    }

    public class TableExist
    {
        public bool Exist { get; set; }
    }
}
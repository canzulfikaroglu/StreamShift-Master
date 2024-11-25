using StreamShift.ApplicationContract.Enums;

namespace StreamShift.Persistence.ContextFactories.Models
{
    public class DbConnectionOptions
    {
        public eDatabaseSource SourceDbType { get; set; }
        public string SourceConnectionString { get; set; }
        public eDatabaseTarget DestinationDbType { get; set; }
        public string DestinationConnectionString { get; set; }
        public string DatabaseName { get; set; }
        public string tableName { get; set; }
    }
}

using StreamShift.ApplicationContract.Enums;

namespace StreamShift.Persistence.ContextFactories.Models
{
    public class DbConnectionOptions
    {
        public eDatabase SourceDbType { get; set; }
        public string SourceConnectionString { get; set; }
        public eDatabase DestinationDbType { get; set; }
        public string DestinationConnectionString { get; set; }
        public string DatabaseName { get; set; }
        public string tableName { get; set; }
    }
}

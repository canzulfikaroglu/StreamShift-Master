using StreamShift.Domain.DatabaseEnums;
using System.ComponentModel.DataAnnotations;

namespace StreamShift.Domain.Entities
{
    public class Transfer
    {
        public Transfer()
        {
            Id = Guid.NewGuid().ToString();
        }

        [Key]
        public string Id { get; set; }
        public string SourceConnectionString { get; set; }
        public string DestinationConnectionString { get; set; }
        public eDatabase SourceDatabase { get; set; }
        public eDatabase DestinationDatabase { get; set; }
        public bool IsFinished { get; set; }
        public List<TableSelection> SelectedTable { get; set; } = new List<TableSelection>();

        public virtual string? AppUserId { get; set; }
        public virtual AppUser? User { get; set; }
    }
}
public class TableSelection
{
    [Key] 
    public string TableName { get; set; }
    public bool IsSelected { get; set; }
}
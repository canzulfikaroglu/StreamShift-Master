using StreamShift.ApplicationContract.Enums;
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
        [Display(Name = "Kaynak Bağlantı Adresi")]
        public string SourceConnectionString { get; set; }
        [Display(Name = "Hedef Bağlantı Adresi")]
        public string DestinationConnectionString { get; set; }
        [Display(Name = "Kaynak Veri Tabanı")]
        public eDatabase SourceDatabase { get; set; }
        [Display(Name = "Hedef Veri Tabanı")]
        public eDatabase DestinationDatabase { get; set; }
        [Display(Name = "Bitti Mi")]
        public bool IsFinished { get; set; }
        public virtual string? AppUserId { get; set; }
        public virtual AppUser? User { get; set; }
        //table'i biz ekledik 
        public virtual string? TableName { get; set; }
    }
}
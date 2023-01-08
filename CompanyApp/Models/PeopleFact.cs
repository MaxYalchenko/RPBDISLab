using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CompanyApp.Models
{
    public class PeopleFact
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int peopleFactId { get; set; }
        public DateTime peopleFactDate { get; set; }
        public int peopleFactIndex { get; set; }
        public Workpeople workpeople { get; set; }
        public int workPeopleId { get; set; }
    }
}

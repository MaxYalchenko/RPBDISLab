using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CompanyApp.Models
{
    public class Workpeople
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int workpeopleId { get; set; }
        public string peopleName { get; set; }
        public int amountPeople { get; set; }
        public int subdivisionId { get; set; }
        public Subdivision subdivision { get; set; }
        public string Achievements { get; set; }

        public List<PeoplePlan> peoplePlans { get; set; }
        public List<PeopleFact> peopleFacts { get; set; }
        public Workpeople()
        {

        }
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CompanyApp.Models
{
    public class PeoplePlan
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int peoplePlanId { get; set; }
        public DateTime peoplePlanDate { get; set; }
        public int peoplePlanIndex { get; set; }
        public Workpeople workpeople { get; set; }
        public int workPeopleId { get; set; }
    }
}

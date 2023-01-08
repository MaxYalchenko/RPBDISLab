using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CompanyApp.Models
{
    public class PeoplePlanView
    {
        public int peoplePlanId { get; set; }
        public DateTime peoplePlanDate { get; set; }
        public int peoplePlanIndex { get; set; }
        public string workpeople { get; set; }
    }
}

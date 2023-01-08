using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CompanyApp.Models
{
    public class SubdivisionPlanView
    {
        public int subdivisionPlanId { get; set; }
        public DateTime subdivisionPlanDate { get; set; }
        public int subdivisionPlanIndex { get; set; }
        public string subdivision { get; set; }
    }
}

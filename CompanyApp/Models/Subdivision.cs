using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CompanyApp.Models
{
    public class Subdivision
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int subdivisionId { get; set; }
        public string subdivisionName { get; set; }
        public int amountSubdivision { get; set; }
        public List<SubdivisionFact> subdivisionFacts { get; set; }
        public List<SubdivisionPlan> subdivisionPlans { get; set; }
        public List<Workpeople> workpeoples { get; set; }
        public Subdivision()
        {
            
        }
    }
}

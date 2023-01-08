using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CompanyApp.Models
{
    public class SubdivisionView
    {
        public int subdivisionId { get; set; }
        public string subdivisionName { get; set; }
        public int amountSubdivision { get; set; }
    }
}

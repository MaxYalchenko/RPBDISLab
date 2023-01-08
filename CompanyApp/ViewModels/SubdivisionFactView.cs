using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CompanyApp.Models
{
    public class SubdivisionFactView
    {
        public int subdivisionFactId { get; set; }
        public DateTime subdivisionFactDate { get; set; }
        public int subdivisionFactIndex { get; set; }
        public string subdivision { get; set; }
    }
}

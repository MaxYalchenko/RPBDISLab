using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CompanyApp.Models
{
    public class PeopleFactView
    {
        public int peopleFactId { get; set; }
        public DateTime peopleFactDate { get; set; }
        public int peopleFactIndex { get; set; }
        public string workpeople { get; set; }
    }
}

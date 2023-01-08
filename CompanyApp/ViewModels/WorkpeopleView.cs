using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CompanyApp.Models
{
    public class WorkpeopleView
    {
        public int workpeopleId { get; set; }
        public string peopleName { get; set; }
        public int amountPeople { get; set; }
        public string subdivision { get; set; }
        public string Achievements { get; set; }
    }
}

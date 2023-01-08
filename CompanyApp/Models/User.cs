using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace CompanyApp.Models
{
    [Table("AspNetUsers")]
    public class User : IdentityUser
    {
        
    }
}

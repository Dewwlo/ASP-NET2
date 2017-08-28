using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace HäggesPizzeria.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public ICollection<Order> Orders { get; set; }
        public string Adress { get; set; }
        public string Zipcode { get; set; }
    }
}

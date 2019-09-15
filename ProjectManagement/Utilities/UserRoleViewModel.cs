using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectManagement.Utilities
{
    public class UserRoleViewModel
    {
        [Display(Name = "Select User")]
        [Required]
        public string UserId { get; set; }

        [Required]
        [Display(Name = "Select Role")]
        public string RoleId { get; set; }
    }
}

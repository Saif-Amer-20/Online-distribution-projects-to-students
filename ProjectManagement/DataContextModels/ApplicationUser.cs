using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectManagement.DataContextModels
{
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser()
        {

        }
        public ApplicationUser(string userName) : base(userName)
        {

        }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ThirdName { get; set; }
        public string ProfessorScientificDegree { get; set; }
        public string ProfessorSpecialization { get; set; }
        public bool IsProfessor { get; set; }
        public bool IsBachelorStudent { get; set; }
        public bool IsMasterStudent { get; set; }
        public bool IsDoctorStudent { get; set; }
        public float StudentAvgPreviousYear { get; set; }
        public bool IT { get; set; }
        public bool CS { get; set; }
      //  public List<string> Roles { get; set; }
    }
}

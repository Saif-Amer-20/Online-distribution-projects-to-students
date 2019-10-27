using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ProjectManagement.DataContextModels
{
    public class ProjectStudent:BaseDataModel
    {
        public int Id { get; set; }
        [Required]
        public int ProjectId { get; set; }
        [Required]
        public string ApplicationUserId { get; set; }

        [NotMapped]
        [Display(Name = "Project Name")]
        public string ProjectName { get; set; }
        [NotMapped]
        [Display(Name = "Student Name")]
        public string StudentName { get; set; }
        [NotMapped]
        [Display(Name = "Supervisor")]
        public string Professor { get; set; }
        [NotMapped]
        public string ProjectType { get; set; }
        [NotMapped]
        public string ProjectBranch { get; set; }
        public virtual Project Project { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }
    }
}

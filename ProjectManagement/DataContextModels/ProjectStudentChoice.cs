using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectManagement.DataContextModels
{
    public class ProjectStudentChoice: BaseDataModel
    {
        public int Id { get; set; }
        [Required]
        public int ProjectId { get; set; }
        [Required]
        public string ApplicationUserId { get; set; }
        public bool IsApproved { get; set; }
        [DataType(DataType.Date)]
        public DateTime ApprovalRejectionDate { get; set; }
        public string ApprovedRejectedBy { get; set; }

        [NotMapped]
        [Display(Name = "Student Name")]
        public string StudentName { get; set; }
        [NotMapped]
        [Display(Name = "Project Name")]
        public string ProjectName { get; set; }
        [NotMapped]
        [Display(Name = "Supervisor")]
        public string Professor { get; set; }
        [NotMapped]
        public string ApprovalSummary { get; set; }

        public int Sequence { get; set; }
        public virtual Project Project { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }
    }
}

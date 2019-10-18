using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectManagement.DataContextModels
{
    public class Project: BaseDataModel
    {

        public int Id { get; set; }
        [Required]
        [Display(Name = "Project Title")]
        public string Name { get; set; }
        [Required]
        [Display(Name = "Project Description")]
        public string Description { get; set; }
        [Required]
        [Display(Name = "Project Tools")]
        public string ProjectTools { get; set; }
        [Required]
        [Display(Name = "Project Type")]
        public string ProjectType { get; set; }
        [Display(Name ="Sub Type")]
        public string ProjectSubType { get; set; }

        [Display(Name="Approved")]
        
        public bool? IsApproved { get; set; }
        [Display(Name="Approval | Rejection Date")]
        [DataType(DataType.Date)]
        public DateTime ApprovalRejectionDate { get; set; }
        [Display(Name ="Approved | Rejected By")]
        public string ApprovedRejectedBy { get; set; }
        [Display(Name ="Assigned")]
        public bool IsClosed { get; set; }
        [Display(Name = "Assigned Date")]
        [DataType(DataType.Date)]
        public DateTime ClosingDate { get; set; }
        [Required]
        [Display(Name ="Students Count")]
        public int MaxApprovedStudents { get; set; }
        [NotMapped]
        public string ApprovalSummary { get; set; }
        [NotMapped]
        public string CloserSummary { get; set; }

        public virtual ICollection<ProjectStudent> ProjectStudents { get; set; }
        public virtual ICollection<ProjectStudentChoice> ProjectStudentChoices { get; set; }
    }
}

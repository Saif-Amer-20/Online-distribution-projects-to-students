using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectManagement.DataContextModels
{
    public class BaseDataModel
    {
        public string CreatedBy { get; set; }
        [DataType(DataType.Date)]
        public DateTime CreatedOn { get; set; }
        public string UpdatedBy { get; set; }
        [DataType(DataType.Date)]
        public DateTime? UpdatedOn { get; set; }

        [ForeignKey("CreatedBy")]
        public virtual ApplicationUser Creator { get; set; }

        [ForeignKey("UpdatedBy")]
        public virtual ApplicationUser Updater { get; set; }
    }
}

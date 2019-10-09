using Microsoft.EntityFrameworkCore;
using ProjectManagement.Data;
using ProjectManagement.DataContextModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectManagement.PDF
{
    public class Reports
    {
        private readonly  ApplicationDbContext _context;
        public Reports()
        {

        }

        public Reports(ApplicationDbContext  context)
        {
            _context = context;
        }
        public  List<ProjectStudent> GetAllStudents()
        {

            var records = _context.ProjectStudents
                  .Select(p => new ProjectStudent()
                  {
                      ProjectName = p.Project.Name,
                      StudentName = p.ApplicationUser.FirstName + " " + p.ApplicationUser.LastName,
                      Professor = (string.IsNullOrEmpty(p.Creator.FirstName) || string.IsNullOrEmpty(p.Creator.LastName)) ? p.Creator.UserName : (p.Creator.FirstName + " " + p.Creator.LastName),


                  });

            return records.ToList();

        }

    }
}

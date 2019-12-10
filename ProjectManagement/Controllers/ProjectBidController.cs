using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectManagement.Data;
using ProjectManagement.DataContextModels;
using ProjectManagement.Utilities;

namespace ProjectManagement.Controllers
{
    [Authorize(Roles = "Student")]
    public class ProjectBidController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public ProjectBidController(ApplicationDbContext context, UserManager<ApplicationUser> userManager) : base(userManager)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult GetProjects(int? page, int? limit, string sortBy, string direction, string projectName = null, string projectType = null, 
            string projectSubType = null, string Professor = null)
        {
            int total;
            var records = GetJsonData(page, limit, sortBy, direction, out total, projectName, projectType, projectSubType, Professor);

            var result = Json(new { records, total });

            return result;
        }

        public List<Project> GetJsonData(int? page, int? limit, string sortBy, string direction, out int total, string projectName = null, string projectType = null, 
            string projectSubType = null, string Professor = null)
        {
            var isBac = _context.ApplicationUsers.Where(p => p.Id == UserIdentity.Id).Select(p => p.IsBachelorStudent).SingleOrDefault();
            var records = _context.Projects.Include(p => p.Creator).Include(p => p.Updater)
                .Select(p => new Project()
                {
                    Id = p.Id,
                    Name = p.Name,
                    IsApproved = p.IsApproved,
                    IsClosed = p.IsClosed,
                    ApprovalSummary = p.IsApproved.Value ? "Yes" : "No",
                    CloserSummary = p.IsClosed?"Yes":"No",
                    MaxApprovedStudents = p.MaxApprovedStudents,
                    ProjectType = p.ProjectType,
                    ProjectSubType = p.ProjectSubType,
                    CreatedBy = (string.IsNullOrEmpty(p.Creator.FirstName) || string.IsNullOrEmpty(p.Creator.LastName)) ? p.Creator.UserName : (p.Creator.FirstName + " " + p.Creator.LastName)
                }).Where(p => p.IsApproved.Value && !p.IsClosed && !_context.ProjectStudents.Select(c => c.ApplicationUserId).Contains(UserIdentity.Id) && isBac==true)
                .AsQueryable();

            
            if (!string.IsNullOrEmpty(projectName))
            {
                records = records.Where(r => r.Name.ToLower().Contains(projectName.Trim().ToLower()));
            }
            if (!string.IsNullOrEmpty(projectType) && projectType != "Select")
            {
                records = records.Where(r => r.ProjectType.ToLower().Contains(projectType.Trim().ToLower()));
            }
            if (!string.IsNullOrEmpty(projectSubType) && projectSubType != "Select")
            {
                records = records.Where(r => r.ProjectSubType.ToLower().Contains(projectSubType.Trim().ToLower()));
            }
            if (!string.IsNullOrEmpty(Professor))
            {
                records = records.Where(r => r.CreatedBy.ToLower().Contains(Professor.Trim().ToLower()));
            }

           
            total = records.Count();

            if (!string.IsNullOrEmpty(sortBy) && !string.IsNullOrEmpty(direction))
            {
                if (direction.Trim().ToLower() == "asc")
                {
                    records = SortHelper.OrderBy(records, sortBy);
                }
                else
                {
                    records = SortHelper.OrderByDescending(records, sortBy);
                }
            }
            if (page.HasValue && limit.HasValue)
            {
                int start = (page.Value - 1) * limit.Value;
                records = records.Skip(start).Take(limit.Value);
            }

           
            return records.ToList();
        }

        // GET: Projects/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Projects
                .FirstOrDefaultAsync(m => m.Id == id);
            if (project == null)
            {
                return NotFound();
            }

            var isApplied = await _context.ProjectStudentChoices.FirstOrDefaultAsync(p => p.ProjectId == id && p.ApplicationUserId == UserIdentity.Id);
            ViewBag.IsApplied = isApplied;
            return View(project);
        }

        public IActionResult MyProjects()
        {
            ViewBag.StudentId = UserIdentity.Id;
            return View();
        }

    }
}
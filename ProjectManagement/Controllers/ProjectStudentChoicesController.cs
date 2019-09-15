using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjectManagement.Data;
using ProjectManagement.DataContextModels;
using ProjectManagement.Utilities;

namespace ProjectManagement.Controllers
{
    [Authorize(Roles = "SystemAdmin,Student")]
    public class ProjectStudentChoicesController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public ProjectStudentChoicesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager): base(userManager)
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult GetProjects(int? page, int? limit, string sortBy, string direction, int projectId, string studentId)
        {
            int total;
            var records = GetJsonData(page, limit, sortBy, direction, out total, projectId, studentId);

            var result = Json(new { records, total });

            return result;
        }

        public List<ProjectStudentChoice> GetJsonData(int? page, int? limit, string sortBy, string direction, out int total, int projectId, string studentId)
        {
            var records = _context.ProjectStudentChoices
                .Include(p => p.Project).ThenInclude(p => p.Creator)
                .Include(p => p.ApplicationUser)
                .Select(p => new ProjectStudentChoice()
                {
                    Id = p.Id,
                    ProjectId = p.ProjectId,
                    CreatedOn = p.CreatedOn,
                    IsApproved = p.IsApproved,
                    ApprovalSummary = p.IsApproved ? "Accepted" : "Rejected",
                    ApprovalRejectionDate = p.ApprovalRejectionDate,
                    ProjectName = p.Project.Name,
                    StudentName = (string.IsNullOrEmpty(p.ApplicationUser.FirstName) || string.IsNullOrEmpty(p.ApplicationUser.LastName)) ? p.ApplicationUser.UserName 
                    : (p.ApplicationUser.FirstName + " " + p.ApplicationUser.LastName),
                    ApplicationUserId = p.ApplicationUserId,
                    Professor = (string.IsNullOrEmpty(p.Project.Creator.FirstName) || string.IsNullOrEmpty(p.Project.Creator.LastName)) ? p.Project.Creator.UserName
                    : (p.Project.Creator.FirstName + " " + p.Project.Creator.LastName),
                })
                .AsQueryable();

            if (projectId != 0)
            {
                records = records.Where(r => r.ProjectId == projectId);
            }
            if (!string.IsNullOrEmpty(studentId))
            {
                records = records.Where(r => r.ApplicationUserId == studentId);
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


        // GET: ProjectStudentChoices
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.ProjectStudentChoices.Include(p => p.ApplicationUser).Include(p => p.Creator).Include(p => p.Project).Include(p => p.Updater);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: ProjectStudentChoices/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var projectStudentChoice = await _context.ProjectStudentChoices
                .Include(p => p.ApplicationUser)
                .Include(p => p.Creator)
                .Include(p => p.Project)
                .Include(p => p.Updater)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (projectStudentChoice == null)
            {
                return NotFound();
            }

            return View(projectStudentChoice);
        }

        // GET: ProjectStudentChoices/Create
        public IActionResult Create()
        {
            ViewData["ApplicationUserId"] = new SelectList(_context.Users, "Id", "Id");
            ViewData["CreatedBy"] = new SelectList(_context.Users, "Id", "Id");
            ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Description");
            ViewData["UpdatedBy"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: ProjectStudentChoices/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<IActionResult> Create(int projectId)
        {
            if (ModelState.IsValid)
            {
                var projectStudents = _context.ProjectStudentChoices.Include(p => p.ApplicationUser).Where( p => p.ApplicationUserId == UserIdentity.Id);
                if (projectStudents != null && projectStudents.Count() >= 6)
                {
                    return Json("Error: You have reached maximum attempt = 6");
                }

                if (projectStudents.FirstOrDefault(p => p.ProjectId == projectId) != null)
                {
                    return Json("Error: You have have applied on this project previously");
                }

                var projectStudentChoice = new ProjectStudentChoice
                {
                    ProjectId = projectId,
                    ApplicationUserId = UserIdentity.Id,
                    CreatedBy = UserIdentity.Id,
                    CreatedOn = DateTime.Now
                };
                _context.Add(projectStudentChoice);
                await _context.SaveChangesAsync();
                return Json("Success: Applied successfully");
            }

            return Json("Invalid");
        }

        // GET: ProjectStudentChoices/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var projectStudentChoice = await _context.ProjectStudentChoices.FindAsync(id);
            if (projectStudentChoice == null)
            {
                return NotFound();
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.Users, "Id", "Id", projectStudentChoice.ApplicationUserId);
            ViewData["CreatedBy"] = new SelectList(_context.Users, "Id", "Id", projectStudentChoice.CreatedBy);
            ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Description", projectStudentChoice.ProjectId);
            ViewData["UpdatedBy"] = new SelectList(_context.Users, "Id", "Id", projectStudentChoice.UpdatedBy);
            return View(projectStudentChoice);
        }

        // POST: ProjectStudentChoices/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ProjectId,ApplicationUserId,IsApproved,ApprovalRejectionDate,ApprovedRejectedBy,CreatedBy,CreatedOn,UpdatedBy,UpdatedOn")] ProjectStudentChoice projectStudentChoice)
        {
            if (id != projectStudentChoice.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(projectStudentChoice);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProjectStudentChoiceExists(projectStudentChoice.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.Users, "Id", "Id", projectStudentChoice.ApplicationUserId);
            ViewData["CreatedBy"] = new SelectList(_context.Users, "Id", "Id", projectStudentChoice.CreatedBy);
            ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Description", projectStudentChoice.ProjectId);
            ViewData["UpdatedBy"] = new SelectList(_context.Users, "Id", "Id", projectStudentChoice.UpdatedBy);
            return View(projectStudentChoice);
        }

        // GET: ProjectStudentChoices/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var projectStudentChoice = await _context.ProjectStudentChoices
                .Include(p => p.ApplicationUser)
                .Include(p => p.Creator)
                .Include(p => p.Project)
                .Include(p => p.Updater)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (projectStudentChoice == null)
            {
                return NotFound();
            }

            return View(projectStudentChoice);
        }

        // POST: ProjectStudentChoices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var projectStudentChoice = await _context.ProjectStudentChoices.FindAsync(id);
            _context.ProjectStudentChoices.Remove(projectStudentChoice);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProjectStudentChoiceExists(int id)
        {
            return _context.ProjectStudentChoices.Any(e => e.Id == id);
        }
    }
}

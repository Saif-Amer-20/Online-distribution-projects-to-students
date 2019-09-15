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
    public class ProjectStudentsController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public ProjectStudentsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager) : base(userManager)
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

        public List<ProjectStudent> GetJsonData(int? page, int? limit, string sortBy, string direction, out int total, int projectId, string studentId)
        {
            var records = _context.ProjectStudents
                .Include(p => p.Project)
                .Include(p => p.ApplicationUser)
                .Include(p => p.Creator)
                .Include(p => p.Updater)
                .Select(p => new ProjectStudent()
                {
                    Id = p.Id,
                    CreatedOn = p.CreatedOn,
                    ProjectName = p.Project.Name,
                    StudentName = (string.IsNullOrEmpty(p.ApplicationUser.FirstName) || string.IsNullOrEmpty(p.ApplicationUser.LastName)) ? p.ApplicationUser.UserName
                    : (p.ApplicationUser.FirstName + " " + p.ApplicationUser.LastName),
                    ApplicationUserId = p.ApplicationUserId,
                    Professor = (string.IsNullOrEmpty(p.Project.Creator.FirstName) || string.IsNullOrEmpty(p.Project.Creator.LastName)) ? p.Project.Creator.UserName
                    : (p.Project.Creator.FirstName + " " + p.Project.Creator.LastName),
                    ProjectId = p.ProjectId
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

        // GET: ProjectStudents
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.ProjectStudents.Include(p => p.ApplicationUser).Include(p => p.Creator).Include(p => p.Project).Include(p => p.Updater);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: ProjectStudents/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var projectStudent = await _context.ProjectStudents
                .Include(p => p.ApplicationUser)
                .Include(p => p.Creator)
                .Include(p => p.Project)
                .Include(p => p.Updater)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (projectStudent == null)
            {
                return NotFound();
            }

            return View(projectStudent);
        }

        // GET: ProjectStudents/Create
        public IActionResult Create()
        {
            ViewData["ApplicationUserId"] = new SelectList(_context.Users, "Id", "Id");
            ViewData["CreatedBy"] = new SelectList(_context.Users, "Id", "Id");
            ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Description");
            ViewData["UpdatedBy"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: ProjectStudents/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ProjectId,ApplicationUserId,CreatedBy,CreatedOn,UpdatedBy,UpdatedOn")] ProjectStudent projectStudent)
        {
            if (ModelState.IsValid)
            {
                _context.Add(projectStudent);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.Users, "Id", "Id", projectStudent.ApplicationUserId);
            ViewData["CreatedBy"] = new SelectList(_context.Users, "Id", "Id", projectStudent.CreatedBy);
            ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Description", projectStudent.ProjectId);
            ViewData["UpdatedBy"] = new SelectList(_context.Users, "Id", "Id", projectStudent.UpdatedBy);
            return View(projectStudent);
        }

        // GET: ProjectStudents/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var projectStudent = await _context.ProjectStudents.FindAsync(id);
            if (projectStudent == null)
            {
                return NotFound();
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.Users, "Id", "Id", projectStudent.ApplicationUserId);
            ViewData["CreatedBy"] = new SelectList(_context.Users, "Id", "Id", projectStudent.CreatedBy);
            ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Description", projectStudent.ProjectId);
            ViewData["UpdatedBy"] = new SelectList(_context.Users, "Id", "Id", projectStudent.UpdatedBy);
            return View(projectStudent);
        }

        // POST: ProjectStudents/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ProjectId,ApplicationUserId,CreatedBy,CreatedOn,UpdatedBy,UpdatedOn")] ProjectStudent projectStudent)
        {
            if (id != projectStudent.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(projectStudent);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProjectStudentExists(projectStudent.Id))
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
            ViewData["ApplicationUserId"] = new SelectList(_context.Users, "Id", "Id", projectStudent.ApplicationUserId);
            ViewData["CreatedBy"] = new SelectList(_context.Users, "Id", "Id", projectStudent.CreatedBy);
            ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Description", projectStudent.ProjectId);
            ViewData["UpdatedBy"] = new SelectList(_context.Users, "Id", "Id", projectStudent.UpdatedBy);
            return View(projectStudent);
        }

        // GET: ProjectStudents/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var projectStudent = await _context.ProjectStudents
                .Include(p => p.ApplicationUser)
                .Include(p => p.Creator)
                .Include(p => p.Project)
                .Include(p => p.Updater)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (projectStudent == null)
            {
                return NotFound();
            }

            return View(projectStudent);
        }

        // POST: ProjectStudents/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var projectStudent = await _context.ProjectStudents.FindAsync(id);
            _context.ProjectStudents.Remove(projectStudent);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProjectStudentExists(int id)
        {
            return _context.ProjectStudents.Any(e => e.Id == id);
        }
    }
}

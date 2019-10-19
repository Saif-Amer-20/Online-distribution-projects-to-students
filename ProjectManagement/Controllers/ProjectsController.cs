using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
    [Authorize(Roles = "SystemAdmin,Supervisor")]
    public class ProjectsController : BaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailService _emailService;

   
        public ProjectsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager,
            IEmailService emailService) : base(userManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _emailService = emailService;
        }



        // GET: Projects
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult NotFound()
        {
            return View();
        }
        [HttpGet]
        public ActionResult GetProjects(int? page, int? limit, string sortBy, string direction, string projectName = null, string projectType = null, bool? isApproved = null, bool? isClosed = null)
        {
            int total;
            var records = GetJsonData(page, limit, sortBy, direction, out total, projectName, projectType, isApproved, isClosed);

            var result = Json(new { records, total });

            return result;
        }

        public List<Project> GetJsonData(int? page, int? limit, string sortBy, string direction, out int total, string projectName = null, string projectType = null, bool? isApproved = null, bool? isClosed = null)
        {
            var records = _context.Projects.Include(p => p.Creator).Include(p => p.Updater)
                .Select(p => new Project()
                {
                    Id = p.Id,
                    Name = p.Name,
                    IsApproved = p.IsApproved,
                    IsClosed = p.IsClosed,
                    ApprovalSummary = p.IsApproved.Value ? "Yes" : "No",
                    CloserSummary = p.IsClosed ? "Yes" : "No",
                    MaxApprovedStudents = p.MaxApprovedStudents,
                    ProjectType = p.ProjectType,
                    CreatedBy = (string.IsNullOrEmpty(p.Creator.FirstName) || string.IsNullOrEmpty(p.Creator.LastName)) ? p.Creator.UserName : (p.Creator.FirstName + " " + p.Creator.LastName)
                })
                .AsQueryable();


            if (!string.IsNullOrEmpty(projectName))
            {
                records = records.Where(r => r.Name.ToLower().Contains(projectName.Trim().ToLower()));
            }
            if (!string.IsNullOrEmpty(projectType) && projectType != "Select")
            {
                records = records.Where(r => r.ProjectType.ToLower().Contains(projectType.Trim().ToLower()));
            }

            if (isApproved != null)
            {
                records = records.Where(r => r.IsApproved == isApproved);
            }
            if (isClosed != null)
            {
                records = records.Where(r => r.IsClosed == isClosed);
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

            return View(project);
        }

        // GET: Projects/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Projects/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description,ProjectTools,ProjectType,ProjectSubType,MaxApprovedStudents")] Project project)
        {
            if (ModelState.IsValid)
            {
                project.CreatedBy = UserIdentity.Id;
                project.CreatedOn = DateTime.Now;
                if (project.ProjectType != "Bachelor")
                {
                    project.ProjectSubType = string.Empty;
                }
                _context.Add(project);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(project);
        }

        // GET: Projects/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            if (User.IsInRole("SystemAdmin"))
            { 
                var allProject = await _context.Projects.FindAsync(id);
                if (allProject == null)
                {
                    return RedirectToAction(nameof(Index));
                }
                return View(allProject);

            }
            var project = await _context.Projects.Where(p => p.Creator.Id == UserIdentity.Id &&p.Id==id).SingleOrDefaultAsync();
            if (project == null)
            {
                return RedirectToAction(nameof(NotFound));
            }
            return View(project);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,ProjectTools,ProjectType,ProjectSubType,MaxApprovedStudents,CreatedBy,CreatedOn")] Project project)
        {
            if (id != project.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    project.UpdatedBy = UserIdentity.Id;
                    project.UpdatedOn = DateTime.Now;
                    if (project.ProjectType != "Bachelor")
                    {
                        project.ProjectSubType = string.Empty;
                    }
                    _context.Update(project);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProjectExists(project.Id))
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
            return View(project);
        }


        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> Confirmed(int id)
        {
            if (User.IsInRole("SystemAdmin"))
            {
                var projects = await _context.Projects
                    .Include(p => p.ProjectStudentChoices)
                    .Include(p => p.ProjectStudents)
                    .FirstOrDefaultAsync(p => p.Id == id);
              
                if (projects.ProjectStudents.Any())
                {
                    return Json("Warning: project have exist students approved.");
                }
                else if (projects.ProjectStudentChoices.Any())
                {
                    return Json("Warning: project has applied students in the queue.");
                }

                _context.Projects.Remove(projects);
                await _context.SaveChangesAsync();
                return Json("Success: Project is removed successfully.");

            }
            var project = await _context.Projects
                .Include(p => p.ProjectStudentChoices)
                .Include(p => p.ProjectStudents).Where(p => p.Creator.Id == UserIdentity.Id && p.Id == id)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (project==null)
            {
                return Json("Warning: You don't have permission to remove this project'.");
            }
            if (project.ProjectStudents.Any())
            {
                return Json("Warning: project have exist students approved.");
            }
            else if (project.ProjectStudentChoices.Any())
            {
                return Json("Warning: project has applied students in the queue.");
            }

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();
            return Json("Success: Project is removed successfully.");
        }

        [HttpPost]
        public async Task<IActionResult> Approve(int projectId)
        {
            var project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == projectId);
            if (project == null)
            {
                return Json("Warning: the project is not exist");
            }
            if (project.IsApproved != null && project.IsApproved.Value)
            {
                return Json("Warning: this project is already approved");
            }

            project.IsApproved = true;
            project.ApprovalRejectionDate = DateTime.Now;
            project.ApprovedRejectedBy = UserIdentity.Id;
            _context.Update(project);
            //var emailAddresses = new List<EmailAddress>();
            //await _emailService.Send(new EmailMessage()
            //{
            //    Content = string.Format("Approved Project Bid for project " + project.Name),
            //    ToAddresses = emailAddresses
            //});
            await _context.SaveChangesAsync();
            return Json("Success: the project is approved successfully.");


        }

        [HttpPost]
        public async Task<IActionResult> Reject(int projectId)
        {
            var project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == projectId);
            if (project == null)
            {
                return Json("Warning: the project is not exist");
            }
            if (project.IsApproved != null && project.IsApproved.Value)
            {
                return Json("Warning: this project is already approved");
            }

            project.IsApproved = false;
            project.ApprovalRejectionDate = DateTime.Now;
            project.ApprovedRejectedBy = UserIdentity.Id;
            _context.Update(project);
            await _context.SaveChangesAsync();
            return Json("Success: the project is approved successfully.");
        }

        [HttpPost]
        public async Task<IActionResult> Complete(int projectId)
        {
            var project = await _context.Projects
                .Include(p => p.ProjectStudentChoices)
                .FirstOrDefaultAsync(p => p.Id == projectId);
            if (project == null)
            {
                return Json("Warning: the project is not exist");
            }



            var selectedStudents = _context.ProjectStudentChoices
                .Include(p => p.ApplicationUser).Where(p => !_context.ProjectStudents.Select(c => c.ApplicationUserId).Contains(p.ApplicationUserId))
                .Where(p => p.ProjectId == projectId)
                .OrderByDescending(p => p.ApplicationUser.StudentAvgPreviousYear).Take(project.MaxApprovedStudents);
            selectedStudents.OrderBy(p => p.Sequence);
            var emailAddresses = new List<EmailAddress>();
            var approvedStudents = new List<ProjectStudent>();
            foreach (var item in selectedStudents)
            {
                item.IsApproved = true;
                item.ApprovedRejectedBy = UserIdentity.Id;
                item.ApprovalRejectionDate = DateTime.Now;

                approvedStudents.Add(new ProjectStudent()
                {
                    ProjectId = item.ProjectId,
                    ApplicationUserId = item.ApplicationUserId,
                    CreatedBy = item.Project.CreatedBy,
                    CreatedOn = DateTime.Now
                });

                emailAddresses.Add(new EmailAddress() { Name = item.ApplicationUser.Email, Address = item.ApplicationUser.Email });
            }

            _context.UpdateRange(selectedStudents);

            project.IsClosed = true;
            project.ClosingDate = DateTime.Now;
            _context.Update(project);

            await _context.ProjectStudents.AddRangeAsync(approvedStudents);
            await _context.SaveChangesAsync();

            //await _emailService.Send(new EmailMessage()
            //{
            //    Content = string.Format("Approved Project Bid for project " + project.Name),
            //    ToAddresses = emailAddresses
            //});

            return Json("Success: Project is complete and result is set properly.");
        }

        [HttpGet]
        public IActionResult NotAllocatedStudents()
        {
            return View();
        }

        [HttpGet]
        public ActionResult GetNotAllocatedStudents(int? page, int? limit, string sortBy, string direction)
        {
            int total;
            var records = GetNotAllocatedStudentsJsonData(page, limit, sortBy, direction, out total);

            var result = Json(new { records, total });

            return result;
        }

        public List<ApplicationUser> GetNotAllocatedStudentsJsonData(int? page, int? limit, string sortBy, string direction, out int total)
        {
            var approvedStudents = _context.ProjectStudents
                .Include(p => p.ApplicationUser)
                .Select(p => new ApplicationUser()
                {
                    FirstName = p.ApplicationUser.FirstName,
                    LastName = p.ApplicationUser.LastName,
                    Email = p.ApplicationUser.Email,
                    PhoneNumber = p.ApplicationUser.PhoneNumber,
                    UserName = p.ApplicationUser.UserName
                });


            var notApprovedStudents = _context.ApplicationUsers
               .Where(p => !_context.ProjectStudentChoices.Select(c => c.ApplicationUserId).Contains(p.Id)&& _context.UserRoles.Where(c=>c.RoleId== "1f8cd529-9587-48a9-8efe-f9a1ec3b6268").Select(c=>c.UserId).Contains(p.Id)).Select(p => new ApplicationUser()
                {

                    FirstName = p.FirstName,
                    LastName = p.LastName,
                    Email = p.Email,
                    PhoneNumber = p.PhoneNumber,
                    UserName = p.UserName
                });

            var records = notApprovedStudents.Except(approvedStudents);

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

        public ActionResult ManageUsers()
        {
            return View();
        }

        public async Task<IActionResult> DeleteUsers(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }
        [HttpPost, ActionName("DeleteUser")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            await _userManager.DeleteAsync(user);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(ManageUsers));
        }


      

        [HttpGet]
        public ActionResult AssignRole()
        {
            var usersList = _context.Users.Where(p => !_context.UserRoles.Select(c => c.UserId).Contains(p.Id)).ToList();
            ViewData["UserId"] = new SelectList(usersList, "Id", "UserName");


            var rolesList = _roleManager.Roles.Where(r => r.Name != "Administrator").ToList();
            ViewData["RoleId"] = new SelectList(rolesList, "Name", "Name");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AssignRole(UserRoleViewModel userRoleViewModel)
        {
            if (ModelState.IsValid)
            {
                var appUser = _userManager.Users.FirstOrDefault(u => u.Id == userRoleViewModel.UserId);
                var isInRole = await _userManager.IsInRoleAsync(appUser, userRoleViewModel.RoleId);
                if (isInRole)
                {
                    ViewData["Resultexist"] = "User already have the selected role";
                }
                else
                {
                    if (userRoleViewModel.RoleId == "Supervisor" || userRoleViewModel.RoleId == "SystemAdmin")
                    {
                        appUser.IsProfessor = true;
                        appUser.IsDoctorStudent = false;
                        appUser.IsBachelorStudent = false;
                        appUser.IsMasterStudent = false;
                    }
                    else
                    {
                        appUser.IsProfessor = false;
                        appUser.IsDoctorStudent = false;
                        appUser.IsBachelorStudent = false;
                        appUser.IsMasterStudent = false;
                    }

                    await _userManager.AddToRoleAsync(appUser, userRoleViewModel.RoleId);
                    ViewData["Result"] = "User is assigned successfully to the selected role";
                }


                var usersList = _context.Users.Where(p => !_context.UserRoles.Select(c => c.UserId).Contains(p.Id)).ToList();
                ViewData["UserId"] = new SelectList(usersList, "Id", "UserName");


                var rolesList = _roleManager.Roles.Where(r => r.Name != "Administrator").ToList();
                ViewData["RoleId"] = new SelectList(rolesList, "Name", "Name");

                return View(userRoleViewModel);
            }
            return View(userRoleViewModel);

        }

        public IActionResult ViewUserRoles(string id)
        {
            ViewBag.Id = id;
            return View();
        }

        [HttpGet]
        public ActionResult GetUsers(int? page, int? limit, string sortBy, string direction)
        {
            int total;
            var records = GetJsonData(page, limit, sortBy, direction, out total);

            var result = Json(new { records, total });

            return result;
        }

        public List<ApplicationUser> GetJsonData(int? page, int? limit, string sortBy, string direction, out int total)
        {
            var records = _userManager.Users.ToArray().AsQueryable();

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

        public async Task<IActionResult> UnAssignRole(string id, string roleName)
        {
            var appUser = await _userManager.FindByIdAsync(id);
            if (appUser == null)
            {
                return NotFound();
            }

            var result = _userManager.RemoveFromRoleAsync(appUser, roleName);
            if (result.Result.Succeeded)
            {
                return Json(appUser.UserName + " is successfully un assigned from role " + roleName);
            }

            return Json("An error has occured, please return back to system administrator");
        }
        private bool ProjectExists(int id)
        {
            return _context.Projects.Any(e => e.Id == id);
        }

        public IActionResult Reports()
        {
            return View();
        }

        [HttpGet]
        public  ActionResult GetReports(int? page, int? limit, string sortBy, string direction, string projectName = null, string projectType = null, bool? isApproved = null, bool? isClosed = null)
        {
            int total;
            var records = GetReportsJsonData(page, limit, sortBy, direction, out total, projectName, projectType, isApproved, isClosed);

            var result = Json(new { records, total });

            return result;
        }

        public List<ProjectStudent> GetReportsJsonData(int? page, int? limit, string sortBy, string direction, out int total, string projectName = null, string projectType = null, bool? isApproved = null, bool? isClosed = null)
        {
           
            var records = _context.ProjectStudents.Include(p => p.Creator).Include(p => p.Updater).Where(p=>p.Project.ProjectType== "Bachelor")
                .Select(p => new ProjectStudent()
                {
                    Id = p.Id,
                    ProjectName = p.Project.Name,
                    StudentName = p.ApplicationUser.FirstName + " " + p.ApplicationUser.LastName,
                    Professor = (string.IsNullOrEmpty(p.Creator.FirstName) || string.IsNullOrEmpty(p.Creator.LastName)) ? p.Creator.UserName : (p.Creator.FirstName + " " + p.Creator.LastName),
                    ProjectType = p.Project.ProjectType,
                    ProjectBranch = p.Project.ProjectSubType

                })
                .AsQueryable();

           
            if (!string.IsNullOrEmpty(projectName))
            {
                records = records.Where(r => r.ProjectName.ToLower().Contains(projectName.Trim().ToLower()));
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
        public IActionResult PostgraduateReports()
        {
            return View();
        }
        [HttpGet]
        public ActionResult GetPostgraduateReports(int? page, int? limit, string sortBy, string direction, string projectName = null, string projectType = null, bool? isApproved = null, bool? isClosed = null)
        {
            int total;
            var records = GetPostgraduateReportsJsonData(page, limit, sortBy, direction, out total, projectName, projectType, isApproved, isClosed);

            var result = Json(new { records, total });

            return result;
        }

        public List<ProjectStudent> GetPostgraduateReportsJsonData(int? page, int? limit, string sortBy, string direction, out int total, string projectName = null, string projectType = null, bool? isApproved = null, bool? isClosed = null)
        {

            var records = _context.ProjectStudents.Include(p => p.Creator).Include(p => p.Updater).Where(p => p.Project.ProjectType == "Master"|| p.Project.ProjectType == "Doctorate")
                .Select(p => new ProjectStudent()
                {
                    Id = p.Id,
                    ProjectName = p.Project.Name,
                    StudentName = p.ApplicationUser.FirstName + " " + p.ApplicationUser.LastName,
                    Professor = (string.IsNullOrEmpty(p.Creator.FirstName) || string.IsNullOrEmpty(p.Creator.LastName)) ? p.Creator.UserName : (p.Creator.FirstName + " " + p.Creator.LastName),
                    ProjectType = p.Project.ProjectType

                })
                .AsQueryable();


            if (!string.IsNullOrEmpty(projectName))
            {
                records = records.Where(r => r.ProjectName.ToLower().Contains(projectName.Trim().ToLower()));
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
        public IActionResult StudentsAverage()
        {
            return View();
        }

        [HttpGet]
        public ActionResult GetStudentsAverage(int? page, int? limit, string sortBy, string direction, string projectName = null, string projectType = null, bool? isApproved = null, bool? isClosed = null)
        {
            int total;
            var records = GetStudentsAverage(page, limit, sortBy, direction, out total, projectName, projectType, isApproved, isClosed);

            var result = Json(new { total, records });

            return result;
        }

        public List<ApplicationUser> GetStudentsAverage(int? page, int? limit, string sortBy, string direction, out int total, string projectName = null, string projectType = null, bool? isApproved = null, bool? isClosed = null)
        {

            var records = _userManager.Users.Where(p => p.IsProfessor == false &&(p.IsBachelorStudent||p.IsMasterStudent||p.IsMasterStudent))
                .AsQueryable();
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

        // GET: Projects/EditStudentsAverage/5
        public IActionResult EditStudentsAverage(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var projects = _userManager.Users.FirstOrDefault(p => p.Id == id);
            if (projects == null)
            {
                return NotFound();
            }
            return View(projects);
        }

        // POST: Projects/EditStudentsAverage/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditStudentsAverage(string id, [Bind("StudentAvgPreviousYear")] ApplicationUser project)
        {
            var user = await _userManager.FindByIdAsync(id);
            //if (id != UserIdentity.Id)
            //{
            //    return NotFound();
            //}

            if (ModelState.IsValid)
            {
                try
                {
                    user.StudentAvgPreviousYear = project.StudentAvgPreviousYear;

                    IdentityResult result = await _userManager.UpdateAsync(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    return NotFound();
                }
                return RedirectToAction(nameof(StudentsAverage));
            }
            return View(project);
        }
       
    }




}

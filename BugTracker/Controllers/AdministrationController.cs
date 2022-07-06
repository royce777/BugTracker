using BugTracker.Areas.Identity.Data;
using BugTracker.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BugTracker.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdministrationController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        public AdministrationController(UserManager<ApplicationUser> userManager,RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Roles()
        {
            RoleViewModel roleDTO = new RoleViewModel()
            {
                roles = _roleManager.Roles.ToList()

            };
            return View(roleDTO);
        }

        public async Task<IActionResult> CreateRole(RoleViewModel role)
        {
            if (ModelState.IsValid)
            {
                IdentityRole identityRole = new IdentityRole
                {
                    Name = role.RoleName
                };
                IdentityResult result = await _roleManager.CreateAsync(identityRole);
                if (result.Succeeded)
                {
                    TempData["success"] = "New role has been created !";
                    return RedirectToAction("Roles");
                }
                foreach(IdentityError error in result.Errors)
                {
                    ModelState.AddModelError("",error.Description);
                }
            }
            return RedirectToAction("Roles");
        }

        public async Task<IActionResult> ManageRole(string id)
        {
            IdentityRole role = await _roleManager.FindByIdAsync(id);
            if(role == null)
            {
                return NotFound();
            }
            RoleManageViewModel model = new RoleManageViewModel
            {
                Id = role.Id,
                RoleName = role.Name
            };
            foreach(var user in _userManager.Users.ToList())
            {
                if(await _userManager.IsInRoleAsync(user, role.Name))
                {
                    model.Users.Add(user);
                }
                else
                {
                    model.otherUsers.Add(user);
                }
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddUserRole(string[] userNames, string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                return NotFound();
            }
            foreach(string uname in userNames)
            {
                var user = await _userManager.FindByNameAsync(uname);
                if (user == null) return NotFound();
                IdentityResult result = await _userManager.AddToRoleAsync(user, role.Name);
                if (!result.Succeeded)
                {
                    // generate some error page
                }
            }
            return RedirectToAction("ManageRole", new { id = role.Id });
        }

        [HttpPost]
        public async Task<IActionResult> RemoveUserRole(string[] userNames, string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if(role == null)
            {
                return NotFound();
            }
            foreach(string uname in userNames)
            {
                var user = await _userManager.FindByNameAsync(uname);
                if (user == null) return NotFound();
                IdentityResult result = await _userManager.RemoveFromRoleAsync(user, role.Name);
                if (!result.Succeeded)
                {
                    // generate some error page
                }
            }
            return RedirectToAction("ManageRole", new { id = role.Id });
        }

        public async Task<IActionResult> DeleteRole(string id)
        {
            IdentityRole role = await _roleManager.FindByIdAsync(id);
            if(role == null)
            {
                return NotFound();
            }
            try
            {
                var result = await _roleManager.DeleteAsync(role);
                if (result.Succeeded)
                {
                    TempData["success"] = "Role deleted";
                    return RedirectToAction("Roles");
                }
                foreach(var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View("Roles");
            }
            catch(DbUpdateException ex)
            {
                ViewBag.ErrorTitle = $"{role.Name} role is still in use";
                ViewBag.ErrorMessage = $"{role.Name} role can't be deleted as there " +
                    $"are still users in this role. Please remove users from this " +
                    $"role first";
                return View("Error");
            }
        }

        public IActionResult Users()
        {
            List<ApplicationUser> users = _userManager.Users.ToList();
            return View(users);
        }

        public async Task<IActionResult> ManageUserRoles(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if(user == null)
            {
                return NotFound();
            }
            ViewBag.userId = id;
            ViewBag.userName = user.UserName; 
            var model = new List<UserRolesViewModel>();
            foreach(var role in _roleManager.Roles.ToList())
            {
                var roleModel = new UserRolesViewModel()
                {
                    RoleId = role.Id,
                    RoleName = role.Name
                };
                if(await _userManager.IsInRoleAsync(user, role.Name))
                {
                    roleModel.Selected = true;
                }
                else
                {
                    roleModel.Selected = false;
                }
                model.Add(roleModel);
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ManageUserRoles(List<UserRolesViewModel> model, string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if(user == null)
            {
                return NotFound();
            }
            var roles = await _userManager.GetRolesAsync(user);
            var result = await _userManager.RemoveFromRolesAsync(user, roles);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot remove user roles");
                return View(model);
            }
            result = await _userManager.AddToRolesAsync(user, model.Where(x => x.Selected).Select(y => y.RoleName));
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot add roles to user");
                return View(model);
            }
            TempData["success"] = "User roles have been modified!";
            return RedirectToAction("Users");
        }
    }
}

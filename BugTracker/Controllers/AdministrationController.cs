using BugTracker.Areas.Identity.Data;
using BugTracker.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BugTracker.Controllers
{
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
            foreach(var user in _userManager.Users)
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
            await _roleManager.DeleteAsync(role);
            return RedirectToAction("Roles");
        }
    }
}

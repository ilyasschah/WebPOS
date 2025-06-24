using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebPOS.Data;
using WebPOS.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebPOS.Pages.Admin
{
    public class UsersModel : PageModel
    {
        private readonly AppDbContext _ctx;
        public UsersModel(AppDbContext ctx) => _ctx = ctx;

        public List<User> Users { get; set; } = new();
        public List<Role> Roles { get; set; } = new();

        [BindProperty]
        public int EditUserId { get; set; }
        [BindProperty]
        public int EditRoleId { get; set; }

        public async Task OnGetAsync()
        {
            Users = await _ctx.Users.Include(u => u.Role).ToListAsync();
            Roles = await _ctx.Roles.ToListAsync();
        }

        public async Task<IActionResult> OnPostAssignRoleAsync(int userId, int roleId)
        {
            var user = await _ctx.Users.FindAsync(userId);
            if (user != null)
            {
                user.RoleId = roleId;
                await _ctx.SaveChangesAsync();
            }
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostRemoveUserAsync(int userId)
        {
            var user = await _ctx.Users.FindAsync(userId);
            if (user != null)
            {
                _ctx.Users.Remove(user);
                await _ctx.SaveChangesAsync();
            }
            return RedirectToPage();
        }
    }
}

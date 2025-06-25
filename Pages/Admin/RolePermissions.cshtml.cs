using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using WebPOS.Data;
using WebPOS.Models;

namespace WebPOS.Pages.Admin
{
    public class RolePermissionsModel : PageModel
    {
        private readonly AppDbContext _ctx;
        public RolePermissionsModel(AppDbContext ctx) => _ctx = ctx;

        public List<Role> Roles { get; set; } = new();
        
        public List<string> AllPermissions { get; set; } = new();

        public List<string> CurrentPermissions { get; set; } = new();

        public List<string> AvailablePermissions => AllPermissions.Except(CurrentPermissions).ToList();

        public Dictionary<int, List<string>> RolePerms = new();

        [BindProperty(SupportsGet = true)]
        public int? SelectedRoleId { get; set; }

        //public async Task OnGetAsync()
        //{
        //    Roles = await _ctx.Roles.ToListAsync();
        //    var perms = await _ctx.RolePermissions.ToListAsync();
        //
        //    foreach (var role in Roles)
        //    {
        //        RolePerms[role.RoleId] = perms
        //            .Where(p => p.RoleId == role.RoleId)
        //            .Select(p => p.Permission)
        //            .ToList();
        //
        //    }
        //}
        public async Task OnGetAsync()
        {
            // Charger tous les rôles
            Roles = await _ctx.Roles.ToListAsync();
            // Charger toutes les permissions distinctes dans la base
            AllPermissions = await _ctx.RolePermissions
                .Select(rp => rp.Permission)
                .Distinct()
                .ToListAsync();
            // Charger les permissions par rôle
            var perms = await _ctx.RolePermissions.ToListAsync();
            foreach (var role in Roles)
            {
                RolePerms[role.RoleId] = perms
                    .Where(p => p.RoleId == role.RoleId)
                    .Select(p => p.Permission)
                    .ToList();
            }
            // Si un rôle est sélectionné, charger ses permissions
            if (SelectedRoleId.HasValue)
            {
                CurrentPermissions = RolePerms.ContainsKey(SelectedRoleId.Value)
                    ? RolePerms[SelectedRoleId.Value]
                    : new List<string>();
            }
        }

        public async Task<IActionResult> OnPostAddPermAsync(int roleId, string permission)
        {
            if (!_ctx.RolePermissions.Any(rp => rp.RoleId == roleId && rp.Permission == permission))
            {
                _ctx.RolePermissions.Add(new RolePermission { RoleId = roleId, Permission = permission });
                await _ctx.SaveChangesAsync();
            }
            return RedirectToPage(new { SelectedRoleId = roleId });
        }

        public async Task<IActionResult> OnPostRemovePermAsync(int roleId, string permission)
        {
            var perm = await _ctx.RolePermissions
                .FirstOrDefaultAsync(rp => rp.RoleId == roleId && rp.Permission == permission);
            if (perm != null)
            {
                _ctx.RolePermissions.Remove(perm);
                await _ctx.SaveChangesAsync();
            }
            return RedirectToPage(new { SelectedRoleId = roleId });
        }
    }
}


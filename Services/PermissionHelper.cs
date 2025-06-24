using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using WebPOS.Data;

namespace WebPOS.Services
{
    public class PermissionHelper
    {
        private readonly AppDbContext _ctx;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PermissionHelper(AppDbContext ctx, IHttpContextAccessor httpContextAccessor)
        {
            _ctx = ctx;
            _httpContextAccessor = httpContextAccessor;
        }

        public bool UserHasPermission(string permission)
        {
            var userId = _httpContextAccessor.HttpContext?.Session.GetInt32("UserId");
            if (userId == null) return false;

            var roleId = _ctx.Users
                .Where(u => u.UserId == userId)
                .Select(u => u.RoleId)
                .FirstOrDefault();

            if (roleId == 0) return false;

            return _ctx.RolePermissions
                .Any(rp => rp.RoleId == roleId && rp.Permission == permission);
        }
    }
}

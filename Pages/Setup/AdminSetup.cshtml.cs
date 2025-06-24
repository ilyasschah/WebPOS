using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using WebPOS.Data;
using WebPOS.Models;

namespace WebPOS.Pages.Setup
{
    public class AdminSetupModel : PageModel
    {
        private readonly AppDbContext _ctx;
        private readonly ILogger<AdminSetupModel> _logger;

        public AdminSetupModel(AppDbContext ctx, ILogger<AdminSetupModel> logger)
        {
            _ctx = ctx;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public void OnGet()
        {
            if (_ctx.Users.Any())
            {
                Response.Redirect("/");
            }
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var business = _ctx.Businesses.FirstOrDefault();
            if (business == null)
            {
                ModelState.AddModelError("", "No business found. Please set up your business first.");
                return Page();
            }

            var user = new User
            {
                Username = Input.Username.Trim(),
                PasswordHash = Input.Password, // For demo only; use a hash in production!
                RoleId = 1, // assuming 1 is the admin role; adjust if needed
                BusinessId = business.BusinessId
            };

            _ctx.Users.Add(user);
            _ctx.SaveChanges();

            _logger.LogInformation("Initial admin user '{Username}' created.", user.Username);

            return RedirectToPage("/Index");
        }

        public class InputModel
        {
            [Required]
            [StringLength(64)]
            public string Username { get; set; } = "";

            [Required]
            [StringLength(64, MinimumLength = 6)]
            [DataType(DataType.Password)]
            public string Password { get; set; } = "";
        }
    }
}

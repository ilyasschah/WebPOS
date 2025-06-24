using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebPOS.Data;
using WebPOS.Models;
using System.ComponentModel.DataAnnotations;

namespace WebPOS.Pages.Account
{
    public class LoginModel(AppDbContext ctx, ILogger<LoginModel> logger) : PageModel
    {
        private readonly AppDbContext _ctx = ctx;
        private readonly ILogger<LoginModel> _logger = logger;

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public string? ErrorMessage { get; set; }

        public void OnGet()
        {
            // Optionally: clear session/cookie here for full logout
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
                return Page();

            var user = _ctx.Users.FirstOrDefault(u =>
                u.Username == Input.Username && u.PasswordHash == Input.Password);

            if (user == null)
            {
                ErrorMessage = "Invalid username or password.";
                return Page();
            }

            // Simple session authentication
            HttpContext.Session.SetInt32("UserId", user.UserId);
            HttpContext.Session.SetString("Username", user.Username);
            HttpContext.Session.SetInt32("BusinessId", user.BusinessId);

            // Redirect to home page after login
            return RedirectToPage("/Index");
        }

        public class InputModel
        {
            [Required]
            public string Username { get; set; } = "";

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; } = "";
        }
    }
}

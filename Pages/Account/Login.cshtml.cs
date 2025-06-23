using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebPOS.Models;

namespace WebPOS.Pages.Account
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public string Username { get; set; }

        [BindProperty]
        public string Password { get; set; }

        public string? ErrorMessage { get; set; }

        public IActionResult OnPost()
        {
            // TODO: Replace with database user check
            var fakeUsers = new List<User>
            {
                new User { Id = 1, Username = "admin", Password = "1234", Role = "Admin" },
                new User { Id = 2, Username = "cashier", Password = "1234", Role = "Cashier" }
            };

            var user = fakeUsers.FirstOrDefault(u => u.Username == Username && u.Password == Password);

            if (user == null)
            {
                ErrorMessage = "Invalid credentials.";
                return Page();
            }

            HttpContext.Session.SetString("Username", user.Username);
            HttpContext.Session.SetString("Role", user.Role);

            return RedirectToPage(user.Role == "Admin" ? "/Admin/Index" : "/Cashier/Index");
        }
    }
}

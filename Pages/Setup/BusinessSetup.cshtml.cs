using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebPOS.Data;
using WebPOS.Models;
using System.ComponentModel.DataAnnotations;

namespace WebPOS.Pages.Setup
{
    public class BusinessSetupModel : PageModel
    {
        private readonly AppDbContext _ctx;
        private readonly ILogger<BusinessSetupModel> _logger;

        public BusinessSetupModel(AppDbContext ctx, ILogger<BusinessSetupModel> logger)
        {
            _ctx = ctx;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public List<SelectListItem> TemplateOptions { get; set; } = new();

        public void OnGet()
        {
            // Redirect if already configured
            if (_ctx.Businesses.Any())
            {
                Response.Redirect("/");
                return;
            }

            // Load templates from the database
            TemplateOptions = _ctx.Templates
                .Select(t => new SelectListItem { Value = t.TemplateId.ToString(), Text = t.TemplateName })
                .ToList();
        }

        public IActionResult OnPost()
        {
            // Reload templates if POST fails
            TemplateOptions = _ctx.Templates
                .Select(t => new SelectListItem { Value = t.TemplateId.ToString(), Text = t.TemplateName })
                .ToList();

            if (!ModelState.IsValid) return Page();

            // Save new business
            var business = new Business
            {
                Name = Input.Name.Trim(),
                TemplateId = Input.TemplateId
            };
            _ctx.Businesses.Add(business);
            _ctx.SaveChanges();

            _logger.LogInformation("Business '{Name}' created (TemplateId = {TemplateId})", business.Name, business.TemplateId);
            return RedirectToPage("/Index");
        }

        public class InputModel
        {
            [Required, StringLength(128)]
            public string Name { get; set; } = "";

            [Required(ErrorMessage = "Please select an activity type.")]
            [Display(Name = "Activity Type")]
            public int TemplateId { get; set; }
        }
    }
}

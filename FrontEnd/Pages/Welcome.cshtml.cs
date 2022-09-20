using System.Threading.Tasks;
using FrontEnd.Services;
using FrontEnd.Pages.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using System.ComponentModel;
using Microsoft.Extensions.Hosting.Internal;

namespace FrontEnd
{
    [ValidateAntiForgeryToken]
    [SkipWelcome]
    public class WelcomeModel : PageModel
    {
        private readonly IApiClient _apiClient;

        private readonly string[] permittedExtensions = { ".jpg", ".jpeg" };
        public WelcomeModel(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        [BindProperty]
        public Attendee Attendee { get; set; }

        [BindProperty]
        [DisplayName("upload image")]
        public IFormFile ProfileImage { get; set; }

        public IActionResult OnGet()
        {
            // Redirect to home page if user is anonymous or already registered as attendee
            var isAttendee = User.IsAttendee();

            if (!User.Identity.IsAuthenticated || isAttendee)
            {
                return RedirectToPage("/Index");
            }

            return Page();
        }

        
        public async Task<IActionResult> OnPostAsync()
        {
            
            if(ProfileImage == null || !(ProfileImage.Length > 0))
            {
                ModelState.AddModelError("", "Profile image not uploaded");
                return Page();
            }
            else
            {
                var ext = Path.GetExtension(ProfileImage.FileName).ToLowerInvariant();

                if (!permittedExtensions.Contains(ext))
                {
                    ModelState.AddModelError("", "Permitted extensions .jpg or .jpeg");
                    return Page();
                }
                else
                {
                    Attendee.ImageName = Guid.NewGuid().ToString() + "_" + Attendee.UserName
                        + Path.GetExtension(ProfileImage.FileName);
                }
            }

            var success = await _apiClient.AddAttendeeAsync(Attendee);

            if (!success)
            {
                ModelState.AddModelError("", "There was an issue creating the attendee for this user.");
                return Page();
            }
            UploadedFile(ProfileImage, Attendee.ImageName);
            // Re-issue the auth cookie with the new IsAttendee claim
            User.MakeAttendee();
            await HttpContext.SignInAsync(IdentityConstants.ApplicationScheme, User);

            return RedirectToPage("/Index");
        }

        private async Task UploadedFile(IFormFile ProfileImage, string uniqueFileName)
        {
            

            if (ProfileImage != null)
            {

                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(),@"wwwroot\Images");
                
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                   await ProfileImage.CopyToAsync(fileStream);
                }
            }
        }
    }
}
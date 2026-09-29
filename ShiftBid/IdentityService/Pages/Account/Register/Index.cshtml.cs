using System.Security.Claims;
using IdentityModel;
using IdentityService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Duende.IdentityServer.Services;
using Duende.IdentityServer.Models;

namespace IdentityService.Pages.Register;

[SecurityHeaders]
[AllowAnonymous]
public class Index : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IIdentityServerInteractionService _interaction;

    [BindProperty]
    public InputModel Input { get; set; } = default!;

    public Index(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IIdentityServerInteractionService interaction)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _interaction = interaction;
    }

    public IActionResult OnGet(string? returnUrl)
    {
        Input = new InputModel { ReturnUrl = returnUrl };
        return Page();
    }

    public async Task<IActionResult> OnPost()
    {
        if (Input.Button != "register")
        {
            var context = await _interaction.GetAuthorizationContextAsync(Input.ReturnUrl);
            if (context != null)
            {
                await _interaction.DenyAuthorizationAsync(context, AuthorizationError.AccessDenied);
                if (context.IsNativeClient()) return this.LoadingPage(Input.ReturnUrl);
                return Redirect(Input.ReturnUrl ?? "~/");
            }
            return Redirect("~/");
        }

        if (ModelState.IsValid)
        {
            var user = new ApplicationUser
            {
                UserName = Input.Username,
                Email = Input.Email,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, Input.Password!);

            if (result.Succeeded)
            {
                var claims = new List<Claim>
                {
                    new Claim(JwtClaimTypes.Name, Input.FullName!)
                };

                await _userManager.AddClaimsAsync(user, claims);

                await _signInManager.SignInAsync(user, isPersistent: false);

                var context = await _interaction.GetAuthorizationContextAsync(Input.ReturnUrl);
                if (context != null)
                {
                    if (context.IsNativeClient()) return this.LoadingPage(Input.ReturnUrl);
                    return Redirect(Input.ReturnUrl ?? "~/");
                }

                if (Url.IsLocalUrl(Input.ReturnUrl)) return Redirect(Input.ReturnUrl);
                else if (string.IsNullOrEmpty(Input.ReturnUrl)) return Redirect("~/");
                else throw new ArgumentException("invalid return URL");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        return Page();
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyShop.BLL.Models.Dto.IdentityDto;
using MyShop.BLL.Services.IdentityServices;
using MyShop.DAL.Entities.IdentityEntity;
using MyShop.PL.ViewModels.Identity;

namespace MyShop.PL.Controllers.IdentityController
{
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IPasswordResetManager _passwordResetManager;
        private readonly ILogger<AccountController> _logger;
        private readonly IConfiguration _configuration;

        public AccountController(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IPasswordResetManager passwordResetManager,
            ILogger<AccountController> logger,
             IConfiguration configuration)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _passwordResetManager = passwordResetManager;
            _logger = logger;
            _configuration = configuration;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true)
                return RedirectToLocal(returnUrl);

            ViewData["ReturnUrl"] = returnUrl;
            return View(new LoginVM());
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email.Trim());
            if (user is null)
            {
                ModelState.AddModelError(string.Empty, "Invalid email or password.");
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, lockoutOnFailure: true);

            if (result.Succeeded)
                return RedirectToLocal(returnUrl);

            if (result.IsLockedOut)
                ModelState.AddModelError(string.Empty, "This account is temporarily locked. Please try again later.");
            else
                ModelState.AddModelError(string.Empty, "Invalid email or password.");

            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true)
                return RedirectToLocal(returnUrl);

            ViewData["ReturnUrl"] = returnUrl;
            return View(new RegisterVM());
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterVM model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
                return View(model);

            var hasUsers = await _userManager.Users.AnyAsync();
            var user = new ApplicationUser
            {
                FirstName = model.FirstName.Trim(),
                LastName = model.LastName.Trim(),
                UserName = model.UserName.Trim(),
                Email = model.Email.Trim(),
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                var roleName = hasUsers ? "Customer" : "Admin";
                await EnsureRoleAsync(roleName);
                await _userManager.AddToRoleAsync(user, roleName);
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToLocal(returnUrl);
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            return View(model);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Profile()
        {
            ViewBag.PageTitle = "Security";
            ViewBag.CardTitle = "My Profile";

            var user = await _userManager.GetUserAsync(User);
            if (user is null)
                return Challenge();

            var model = new ProfileRequestViewModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email ?? string.Empty,
                PhoneNumber = user.PhoneNumber
            };

            return View(model);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(ProfileRequestViewModel model)
        {
            ViewBag.PageTitle = "Security";
            ViewBag.CardTitle = "My Profile";

            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.GetUserAsync(User);
            if (user is null)
                return Challenge();

            user.FirstName = model.FirstName.Trim();
            user.LastName = model.LastName?.Trim();
            user.PhoneNumber = model.PhoneNumber?.Trim();

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                TempData["success"] = "Profile updated successfully.";
                return RedirectToAction(nameof(Profile));
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            return View(model);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(Login));
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }

        private async Task EnsureRoleAsync(string roleName)
        {
            if (!await _roleManager.RoleExistsAsync(roleName))
                await _roleManager.CreateAsync(new IdentityRole(roleName));
        }

        private IActionResult RedirectToLocal(string? returnUrl)
        {
            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
                return LocalRedirect(returnUrl);

            return RedirectToAction("Index", "Product");
        }
        #region Forget Password
        private bool IsMailSettingsConfigured()
        {
            return !string.IsNullOrWhiteSpace(_configuration["MailSettings:Host"])
                && !string.IsNullOrWhiteSpace(_configuration["MailSettings:Email"])
                && !string.IsNullOrWhiteSpace(_configuration["MailSettings:Password"]);
        }

        [HttpGet]
        public IActionResult ForgotPassword() => View();

        // Step 2: Send OTP to email
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            if (!IsMailSettingsConfigured())
            {
                _logger.LogWarning("ForgotPassword blocked because SMTP settings are not configured.");
                ModelState.AddModelError(string.Empty, "Email service is not configured. Please configure SMTP settings before sending OTP.");
                return View(dto);
            }

            try
            {
                await _passwordResetManager.SendOtpAsync(dto.Email);
                ViewBag.Message = "OTP sent to your email.";
                return View("VerifyOtp", new VerifyOtpDto { Email = dto.Email });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "ForgotPassword failed because email settings are not configured.");
                ModelState.AddModelError(string.Empty, "Email service is not configured. Please configure SMTP settings before sending OTP.");
                return View(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ForgotPassword failed to send OTP to {Email}", dto.Email);
                ModelState.AddModelError(string.Empty, "Unable to send OTP right now. Please try again later.");
                return View(dto);
            }
        }

        // Step 3: Verify OTP
        [HttpGet]
        public IActionResult VerifyOtp() => View();

        [HttpPost]
        public async Task<IActionResult> VerifyOtp(VerifyOtpDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            try
            {
                var token = await _passwordResetManager.VerifyOtpAsync(dto.Email, dto.Otp);

                return RedirectToAction("ResetPassword", new { token });

            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(dto);
            }
        }

        // Step 4: Reset Password
        [HttpGet]
        public IActionResult ResetPassword(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return BadRequest("Invalid reset token");

            return View(new ResetPasswordDto
            {
                Token = token
            });
        }



        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            try
            {
                if (string.IsNullOrWhiteSpace(dto.Token))
                {
                    ModelState.AddModelError("", "Invalid or expired reset token.");
                    return View(dto);
                }

                await _passwordResetManager.ResetPasswordAsync(dto.Token, dto.NewPassword);
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(dto);
            }
        } 
        #endregion

    }
}

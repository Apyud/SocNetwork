using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SocNetwork.Models.Db;
using SocNetwork.Models.Service;
using SocNetwork.Models.ViewModel;

namespace SocNetwork.Controllers
{
    public class AccountController : Controller
    {
        private IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ILogger<AccountController> _logger;
        private readonly IUserService _userService;
        public AccountController(IMapper mapper, UserManager<User> userManager, SignInManager<User> signInManager, 
            ILogger<AccountController> logger, IUserService userService)
        {
            _mapper = mapper;
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _userService = userService;
        }
        

        [Route("Login")]
        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View(new LoginViewModel { ReturnUrl = returnUrl ?? "" });
        }

        [Route("Login")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                // Найдем пользователя по email
                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Неверный email или пароль.");
                    return View(model);
                }

             
                // Используем UserName для входа
                var result = await _signInManager.PasswordSignInAsync(
                    user.UserName,
                    model.Password,
                    model.RememberMe,
                    lockoutOnFailure: true); // Включаем блокировку при множественных неудачных попытках

                if (result.Succeeded)
                {
                    _logger.LogInformation("Пользователь {Email} вошел в систему", model.Email);

                    if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }
                    return RedirectToAction("MyPage", "Account");
                }

                if (result.IsLockedOut)
                {
                    _logger.LogWarning("Аккаунт пользователя {Email} заблокирован", model.Email);
                    ModelState.AddModelError(string.Empty, "Аккаунт временно заблокирован. Попробуйте позже.");
                    return View(model);
                }

                if (result.RequiresTwoFactor)
                {
                    ModelState.AddModelError(string.Empty, "Требуется двухфакторная аутентификация.");
                    return View(model);
                }

                ModelState.AddModelError(string.Empty, "Неверный email или пароль.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при входе пользователя {Email}", model.Email);
                ModelState.AddModelError(string.Empty, "Произошла ошибка при входе в систему");
            }

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
           _logger.LogInformation("Пользователь вышел из системы");
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [Route("MyPage")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> MyPage()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            var userViewModel = _mapper.Map<UserViewModel>(user);

            // Устанавливаем время начала текущей сессии - настоящее время
            userViewModel.JoinDate = DateTime.Now;

            return View("User", userViewModel);
        }

        [Authorize]
        [Route("Update")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task <IActionResult> Update(UserEditViewModel model)
        {

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(model.Id);

                if (user == null)
                {
                    return NotFound();
                }

                // Проверяем, что пользователь редактирует свой профиль
                if (user.Id != _userManager.GetUserId(User))
                {
                    return Forbid();
                }

                // Обновляем данные пользователя
                _mapper.Map(model, user);

                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    _logger.LogInformation("Профиль пользователя {UserId} обновлен", model.Id);
                    return RedirectToAction("MyPage", "Account");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }

            // Если есть ошибки, возвращаем на форму редактирования
            return View("Edit", model);
        }

        [Authorize]
        [Route("Edit")]
        [HttpGet]
        public async Task<IActionResult>Edit()
        {
            var user = await _userManager.GetUserAsync(User);
                if(user == null)
            {
                return RedirectToAction("Login");
            }
                var userEditViewModel = _mapper.Map<UserEditViewModel>(user);
                return View(userEditViewModel);
        }

        [Authorize]
        [Route("Edit")]
        [HttpPost]
        public async Task<IActionResult> Edit(UserEditViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var currentUserId = _userManager.GetUserId(User);
                if (model.Id != currentUserId)
                    return Forbid();

                // ИСПОЛЬЗУЕМ USER SERVICE вместо прямого UserManager
                await _userService.UpdateUserProfileAsync(model);

                _logger.LogInformation("Профиль пользователя {UserId} обновлен", model.Id);
                return RedirectToAction("MyPage", "Account");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при обновлении профиля пользователя {UserId}", model.Id);
                ModelState.AddModelError(string.Empty, "Произошла ошибка при сохранении изменений");
                return View(model);
            }
        }
    }
    }

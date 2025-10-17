using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql.Replication.PgOutput.Messages;
using SocNetwork.Models.Db;
using SocNetwork.Models.ViewModel;

namespace SocNetwork.Controllers
{
    public class RegisterController:Controller
    {
        private IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        
        public RegisterController(UserManager<User> userManager, SignInManager<User> signInManager, IMapper mapper)
        {
            _userManager = userManager; 
            _signInManager = signInManager;
            _mapper = mapper;
        }

       // [Route("Register")]
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var user = _mapper.Map<User>(model);

                // Проверка даты рождения
                if (user.BirthDate > DateTime.Now.AddYears(-10) || user.BirthDate < DateTime.Now.AddYears(-100))
                {
                    ModelState.AddModelError("", "Некорректная дата рождения");
                    return View(model);
                }

                // Проверка уникальности email и username
                var existingUserByEmail = await _userManager.FindByEmailAsync(model.EmailReg);
                if (existingUserByEmail != null)
                {
                    ModelState.AddModelError("EmailReg", "Этот email уже зарегистрирован");
                    return View(model);
                }

                var existingUserByUsername = await _userManager.FindByNameAsync(model.Login);
                if (existingUserByUsername != null)
                {
                    ModelState.AddModelError("Login", "Этот никнейм уже занят");
                    return View(model);
                }

                var result = await _userManager.CreateAsync(user, model.PasswordReg);

                if (result.Succeeded)
                {
                    // Успешная регистрация - вход и перенаправление
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("MyPage", "Account");
                }

                // Обработка ошибок Identity
                foreach (var error in result.Errors)
                {
                    if (error.Code.Contains("Password"))
                    {
                        ModelState.AddModelError("PasswordReg", error.Description);
                    }
                    else if (error.Code.Contains("UserName"))
                    {
                        ModelState.AddModelError("Login", error.Description);
                    }
                    else if (error.Code.Contains("Email"))
                    {
                        ModelState.AddModelError("EmailReg", error.Description);
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            catch (DbUpdateException dbEx)
            {
                ModelState.AddModelError("", $"Ошибка базы данных: {dbEx.Message}");
            }
            catch (FormatException)
            {
                ModelState.AddModelError("", "Некорректный формат даты рождения");
            }
            catch (ArgumentOutOfRangeException)
            {
                ModelState.AddModelError("", "Некорректная дата рождения");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Ошибка при регистрации: {ex.Message}");
            }

            return View(model);
        }
       
    }
}

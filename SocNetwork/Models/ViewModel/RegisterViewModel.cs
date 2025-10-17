using System.ComponentModel.DataAnnotations;

namespace SocNetwork.Models.ViewModel
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Имя обязательно")]
        [Display(Name = "Имя")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Фамилия обязательна")]
        [Display(Name = "Фамилия")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Отчество обязательно")]
        [Display(Name = "Отчество")]
        public string MiddleName { get; set; }

        [Required(ErrorMessage = "Email обязателен")]
        [EmailAddress(ErrorMessage = "Некорректный формат email")]
        [Display(Name = "Email")]
        public string EmailReg { get; set; }

        [Required(ErrorMessage = "Год обязателен")]
        [Display(Name = "Год")]
        [Range(1900, 2020, ErrorMessage = "Год должен быть между 1900 и 2020")]
        public string Year { get; set; }

        [Required(ErrorMessage = "День обязателен")]
        [Display(Name = "День")]
        [Range(1, 31, ErrorMessage = "День должен быть от 1 до 31")]
        public int Date { get; set; }

        [Required(ErrorMessage = "Месяц обязателен")]
        [Display(Name = "Месяц")]
        [Range(1, 12, ErrorMessage = "Месяц должен быть от 1 до 12")]
        public int Month { get; set; }

        [Required(ErrorMessage = "Пароль обязателен")]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        [StringLength(100, ErrorMessage = "Пароль должен быть от {2} до {1} символов", MinimumLength = 5)]
        public string PasswordReg { get; set; }

        [Required(ErrorMessage = "Подтверждение пароля обязательно")]
        [Compare("PasswordReg", ErrorMessage = "Пароли не совпадают")]
        [DataType(DataType.Password)]
        [Display(Name = "Подтвердить пароль")]
        public string PasswordConfirm { get; set; }

        [Required(ErrorMessage = "Никнейм обязателен")]
        [Display(Name = "Никнейм")]
        [StringLength(20, ErrorMessage = "Никнейм должен быть до {1} символов")]
        public string Login { get; set; }
    }
}
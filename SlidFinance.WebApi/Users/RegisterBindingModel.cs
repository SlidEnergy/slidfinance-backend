using System.ComponentModel.DataAnnotations;

namespace SlidFinance.WebApi
{
    /// <summary>
    /// Модель для регистрации нового пользователя
    /// </summary>
    public class RegisterBindingModel
    {
        /// <summary>
        /// Уникальная электронная почта
        /// </summary>
        [Required]
        public string Email { get; set; }

        /// <summary>
        /// Пароль
        /// </summary>
        [Required]
        [StringLength(100, ErrorMessage = "Пароль должен быть польше 6 символов.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        /// <summary>
        /// Подтверждение пароля
        /// </summary>
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Пароль и подтверждение пароля не совпадают.")]
        public string ConfirmPassword { get; set; }
    }
}

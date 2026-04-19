using System.ComponentModel.DataAnnotations;

namespace TeacherWorkplace.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Логин обязателен")]
        [StringLength(50)]
        [Display(Name = "Логин")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Пароль обязателен")]
        [StringLength(255)]
        [Display(Name = "Пароль")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "ФИО обязательно")]
        [StringLength(200)]
        [Display(Name = "ФИО")]
        public string FullName { get; set; } = string.Empty;

        [Display(Name = "Должность")]
        [StringLength(100)]
        public string? Position { get; set; }

        [Display(Name = "Email")]
        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        [Display(Name = "Роль")]
        [StringLength(20)]
        public string Role { get; set; } = "Teacher";

        [Display(Name = "Активен")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "Дата создания")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Display(Name = "Последний вход")]
        public DateTime? LastLogin { get; set; }

        // Навигационные свойства
        public virtual ICollection<LogEntry> LogEntries { get; set; } = new List<LogEntry>();
    }
}

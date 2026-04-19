using System.ComponentModel.DataAnnotations;

namespace TeacherWorkplace.Models
{
    public class LogEntry
    {
        public int Id { get; set; }

        [Display(Name = "Пользователь")]
        public int? UserId { get; set; }

        [Required]
        [StringLength(255)]
        [Display(Name = "Действие")]
        public string Action { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Время")]
        public DateTime Timestamp { get; set; } = DateTime.Now;

        [StringLength(50)]
        [Display(Name = "IP-адрес")]
        public string? IpAddress { get; set; }

        [Display(Name = "Детали")]
        public string? Details { get; set; }

        // Навигационные свойства
        public virtual User? User { get; set; }
    }
}

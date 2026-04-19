using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TeacherWorkplace.Models
{
    public class Group
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Название группы обязательно")]
        [StringLength(20)]
        [Display(Name = "Название группы")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Курс")]
        [Range(1, 5, ErrorMessage = "Курс должен быть от 1 до 5")]
        public int Course { get; set; }

        [Required(ErrorMessage = "Специальность обязательна")]
        [StringLength(200)]
        [Display(Name = "Специальность")]
        public string Specialty { get; set; } = string.Empty;

        [Display(Name = "Год поступления")]
        public int EnrollmentYear { get; set; } = DateTime.Now.Year;

        [Display(Name = "Староста")]
        public int? MonitorId { get; set; }

        [Display(Name = "Примечания")]
        public string? Notes { get; set; }

        // Вычисляемые свойства
        [NotMapped]
        [Display(Name = "Количество студентов")]
        public int StudentsCount => Students?.Count ?? 0;

        [NotMapped]
        [Display(Name = "Полное название")]
        public string FullName => $"{Name} ({Specialty}, {Course} курс)";

        // Навигационные свойства
        public virtual ICollection<Student> Students { get; set; } = new List<Student>();
        public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
    }
}

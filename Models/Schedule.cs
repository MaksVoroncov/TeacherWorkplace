using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TeacherWorkplace.Models
{
    public class Schedule
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Дисциплина")]
        public int SubjectId { get; set; }

        [Required]
        [Display(Name = "Группа")]
        public int GroupId { get; set; }

        [Required]
        [Display(Name = "День недели")]
        [StringLength(20)]
        public string DayOfWeek { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Время начала")]
        [DataType(DataType.Time)]
        public TimeSpan StartTime { get; set; }

        [Required]
        [Display(Name = "Время окончания")]
        [DataType(DataType.Time)]
        public TimeSpan EndTime { get; set; }

        [Display(Name = "Аудитория")]
        [StringLength(20)]
        public string? Classroom { get; set; }

        [Display(Name = "Тип занятия")]
        [StringLength(50)]
        public string? LessonType { get; set; } // Лекция, Практика, Лабораторная

        [Display(Name = "Активно")]
        public bool IsActive { get; set; } = true;

        // Вычисляемые свойства
        [NotMapped]
        [Display(Name = "Длительность (мин)")]
        public int Duration => (int)(EndTime - StartTime).TotalMinutes;

        [NotMapped]
        [Display(Name = "Время занятия")]
        public string TimeSlot => $"{StartTime:hh\\:mm} - {EndTime:hh\\:mm}";

        // Навигационные свойства
        public virtual Subject? Subject { get; set; }
        public virtual Group? Group { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace TeacherWorkplace.Models
{
    public enum AttendanceStatus
    {
        [Display(Name = "Присутствовал")]
        Present,
        [Display(Name = "Отсутствовал")]
        Absent,
        [Display(Name = "Опоздал")]
        Late,
        [Display(Name = "Уважительная причина")]
        Excused
    }

    public class Attendance
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Студент")]
        public int StudentId { get; set; }

        [Required]
        [Display(Name = "Дисциплина")]
        public int SubjectId { get; set; }

        [Required]
        [Display(Name = "Дата занятия")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; } = DateTime.Today;

        [Required]
        [Display(Name = "Статус")]
        public AttendanceStatus Status { get; set; } = AttendanceStatus.Present;

        [Display(Name = "Примечания")]
        [StringLength(500)]
        public string? Notes { get; set; }

        [Display(Name = "Время регистрации")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Навигационные свойства
        public virtual Student? Student { get; set; }
        public virtual Subject? Subject { get; set; }
    }
}

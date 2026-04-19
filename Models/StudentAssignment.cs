using System.ComponentModel.DataAnnotations;

namespace TeacherWorkplace.Models
{
    public enum AssignmentStatus
    {
        [Display(Name = "Не начато")]
        NotStarted,
        [Display(Name = "В работе")]
        InProgress,
        [Display(Name = "Сдано")]
        Submitted,
        [Display(Name = "Оценено")]
        Graded,
        [Display(Name = "Возвращено на доработку")]
        Returned
    }

    public class StudentAssignment
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Задание")]
        public int AssignmentId { get; set; }

        [Required]
        [Display(Name = "Студент")]
        public int StudentId { get; set; }

        [Display(Name = "Дата сдачи")]
        [DataType(DataType.DateTime)]
        public DateTime? SubmittedDate { get; set; }

        [Display(Name = "Балл")]
        [Range(0, 100)]
        public int? Score { get; set; }

        [Required]
        [Display(Name = "Статус")]
        public AssignmentStatus Status { get; set; } = AssignmentStatus.NotStarted;

        [Display(Name = "Комментарий студента")]
        public string? StudentComment { get; set; }

        [Display(Name = "Комментарий преподавателя")]
        public string? TeacherComment { get; set; }

        [Display(Name = "Файл работы")]
        public string? FilePath { get; set; }

        // Навигационные свойства
        public virtual Assignment? Assignment { get; set; }
        public virtual Student? Student { get; set; }
    }
}

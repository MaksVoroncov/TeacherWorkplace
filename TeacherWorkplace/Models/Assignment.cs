using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TeacherWorkplace.Models
{
    public class Assignment
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Дисциплина")]
        public int SubjectId { get; set; }

        [Required(ErrorMessage = "Название задания обязательно")]
        [StringLength(200)]
        [Display(Name = "Название задания")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Описание обязательно")]
        [Display(Name = "Описание")]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Дедлайн")]
        [DataType(DataType.DateTime)]
        public DateTime Deadline { get; set; }

        [Required]
        [Display(Name = "Максимальный балл")]
        [Range(1, 100, ErrorMessage = "Максимальный балл должен быть от 1 до 100")]
        public int MaxScore { get; set; } = 100;

        [Display(Name = "Дата создания")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Display(Name = "Активно")]
        public bool IsActive { get; set; } = true;

        // Вычисляемые свойства
        [NotMapped]
        [Display(Name = "Просрочено")]
        public bool IsOverdue => DateTime.Now > Deadline;

        [NotMapped]
        [Display(Name = "Сдано работ")]
        public int SubmittedCount => StudentAssignments?.Count(sa => sa.Status == AssignmentStatus.Submitted || sa.Status == AssignmentStatus.Graded) ?? 0;

        // Навигационные свойства
        public virtual Subject? Subject { get; set; }
        public virtual ICollection<StudentAssignment> StudentAssignments { get; set; } = new List<StudentAssignment>();
    }
}

using System.ComponentModel.DataAnnotations;

namespace TeacherWorkplace.Models
{
    public enum GradeType
    {
        [Display(Name = "Текущая")]
        Current,
        [Display(Name = "Контрольная работа")]
        Test,
        [Display(Name = "Лабораторная работа")]
        Lab,
        [Display(Name = "Практическая работа")]
        Practice,
        [Display(Name = "Зачет")]
        Credit,
        [Display(Name = "Экзамен")]
        Exam,
        [Display(Name = "Итоговая")]
        Final
    }

    public class Grade
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Студент")]
        public int StudentId { get; set; }

        [Required]
        [Display(Name = "Дисциплина")]
        public int SubjectId { get; set; }

        [Required]
        [Display(Name = "Дата")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; } = DateTime.Today;

        [Required]
        [Display(Name = "Оценка")]
        [Range(1, 5, ErrorMessage = "Оценка должна быть от 1 до 5")]
        public int Score { get; set; }

        [Required]
        [Display(Name = "Тип оценки")]
        public GradeType Type { get; set; } = GradeType.Current;

        [Display(Name = "Описание")]
        [StringLength(200)]
        public string? Description { get; set; }

        [Display(Name = "Вес оценки")]
        [Range(1, 10)]
        public int Weight { get; set; } = 1;

        [Display(Name = "Время выставления")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Навигационные свойства
        public virtual Student? Student { get; set; }
        public virtual Subject? Subject { get; set; }
    }
}

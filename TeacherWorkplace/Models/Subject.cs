using System.ComponentModel.DataAnnotations;

namespace TeacherWorkplace.Models
{
    public class Subject
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Название дисциплины обязательно")]
        [StringLength(200)]
        [Display(Name = "Название дисциплины")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Количество часов")]
        [Range(1, 500, ErrorMessage = "Количество часов должно быть от 1 до 500")]
        public int TotalHours { get; set; }

        [Required]
        [Display(Name = "Семестр")]
        [Range(1, 8, ErrorMessage = "Семестр должен быть от 1 до 8")]
        public int Semester { get; set; }

        [Display(Name = "Описание")]
        public string? Description { get; set; }

        [Display(Name = "Код дисциплины")]
        [StringLength(20)]
        public string? Code { get; set; }

        [Display(Name = "Активна")]
        public bool IsActive { get; set; } = true;

        // Навигационные свойства
        public virtual ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
        public virtual ICollection<Grade> Grades { get; set; } = new List<Grade>();
        public virtual ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();
        public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
    }
}

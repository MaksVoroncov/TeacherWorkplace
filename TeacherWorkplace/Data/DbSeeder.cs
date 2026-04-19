using TeacherWorkplace.Models;
using Microsoft.EntityFrameworkCore;

namespace TeacherWorkplace.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(ApplicationDbContext ctx)
        {
            // Already seeded — skip
            if (await ctx.Users.AnyAsync()) return;

            Console.WriteLine("Seeding database...");

            // ── User ──────────────────────────────────────────────────────────
            ctx.Users.Add(new User {
                Username="teacher", Password="teacher123",
                FullName="Михеев Кирилл Сергеевич",
                Position="Преподаватель информатики",
                Email="mikheev@college.ru", Role="Teacher",
                IsActive=true, CreatedAt=new DateTime(2025,9,1)
            });
            await ctx.SaveChangesAsync();

            // ── Groups ────────────────────────────────────────────────────────
            var groups = new[] {
                new Group{Name="ИСС-221",Course=3,Specialty="Информационные системы и программирование",EnrollmentYear=2022},
                new Group{Name="ИСС-231",Course=2,Specialty="Информационные системы и программирование",EnrollmentYear=2023},
                new Group{Name="ИСС-241",Course=1,Specialty="Информационные системы и программирование",EnrollmentYear=2024},
                new Group{Name="ПИ-221", Course=3,Specialty="Программная инженерия",EnrollmentYear=2022},
                new Group{Name="ПИ-231", Course=2,Specialty="Программная инженерия",EnrollmentYear=2023},
                new Group{Name="ВЕБ-241",Course=1,Specialty="Разработка и эксплуатация веб-приложений",EnrollmentYear=2024},
            };
            ctx.Groups.AddRange(groups);
            await ctx.SaveChangesAsync();

            // ── Subjects ──────────────────────────────────────────────────────
            ctx.Subjects.AddRange(
                new Subject{Name="Основы программирования",TotalHours=72,Semester=5,Code="ОП.01",IsActive=true,Description="Алгоритмы, ООП, C# 12"},
                new Subject{Name="Базы данных",TotalHours=54,Semester=5,Code="ПМ.02",IsActive=true,Description="SQL, нормализация, EF Core 8"},
                new Subject{Name="Разработка веб-приложений",TotalHours=90,Semester=6,Code="ПМ.03",IsActive=true,Description="ASP.NET Core MVC, Bootstrap 5"},
                new Subject{Name="Информационная безопасность",TotalHours=36,Semester=5,Code="ОП.05",IsActive=true,Description="Угрозы ИБ, криптография"},
                new Subject{Name="Алгоритмы и структуры данных",TotalHours=54,Semester=3,Code="ОП.02",IsActive=true,Description="Сортировки, деревья, графы"}
            );
            await ctx.SaveChangesAsync();

            Console.WriteLine("Base seed complete. Full data will load in background...");
        }
    }
}

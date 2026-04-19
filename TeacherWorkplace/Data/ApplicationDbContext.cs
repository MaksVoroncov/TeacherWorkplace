using Microsoft.EntityFrameworkCore;
using TeacherWorkplace.Models;

namespace TeacherWorkplace.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Student>           Students           { get; set; }
        public DbSet<Group>             Groups             { get; set; }
        public DbSet<Subject>           Subjects           { get; set; }
        public DbSet<Attendance>        Attendances        { get; set; }
        public DbSet<Grade>             Grades             { get; set; }
        public DbSet<Assignment>        Assignments        { get; set; }
        public DbSet<StudentAssignment> StudentAssignments { get; set; }
        public DbSet<Schedule>          Schedules          { get; set; }
        public DbSet<User>              Users              { get; set; }
        public DbSet<LogEntry>          LogEntries         { get; set; }

        protected override void OnModelCreating(ModelBuilder m)
        {
            base.OnModelCreating(m);

            m.Entity<User>().HasIndex(u=>u.Username).IsUnique();
            m.Entity<Student>().HasOne(s=>s.Group).WithMany(g=>g.Students)
                .HasForeignKey(s=>s.GroupId).OnDelete(DeleteBehavior.Restrict);
            m.Entity<Attendance>().HasOne(a=>a.Student).WithMany(s=>s.Attendances)
                .HasForeignKey(a=>a.StudentId).OnDelete(DeleteBehavior.Cascade);
            m.Entity<Attendance>().HasOne(a=>a.Subject).WithMany(s=>s.Attendances)
                .HasForeignKey(a=>a.SubjectId).OnDelete(DeleteBehavior.Restrict);
            m.Entity<Grade>().HasOne(g=>g.Student).WithMany(s=>s.Grades)
                .HasForeignKey(g=>g.StudentId).OnDelete(DeleteBehavior.Cascade);
            m.Entity<Grade>().HasOne(g=>g.Subject).WithMany(s=>s.Grades)
                .HasForeignKey(g=>g.SubjectId).OnDelete(DeleteBehavior.Restrict);
            m.Entity<Assignment>().HasOne(a=>a.Subject).WithMany(s=>s.Assignments)
                .HasForeignKey(a=>a.SubjectId).OnDelete(DeleteBehavior.Restrict);
            m.Entity<StudentAssignment>().HasOne(sa=>sa.Assignment).WithMany(a=>a.StudentAssignments)
                .HasForeignKey(sa=>sa.AssignmentId).OnDelete(DeleteBehavior.Cascade);
            m.Entity<StudentAssignment>().HasOne(sa=>sa.Student).WithMany(s=>s.StudentAssignments)
                .HasForeignKey(sa=>sa.StudentId).OnDelete(DeleteBehavior.Cascade);
            m.Entity<Schedule>().HasOne(s=>s.Subject).WithMany(sub=>sub.Schedules)
                .HasForeignKey(s=>s.SubjectId).OnDelete(DeleteBehavior.Restrict);
            m.Entity<Schedule>().HasOne(s=>s.Group).WithMany(g=>g.Schedules)
                .HasForeignKey(s=>s.GroupId).OnDelete(DeleteBehavior.Restrict);
            m.Entity<LogEntry>().HasOne(l=>l.User).WithMany(u=>u.LogEntries)
                .HasForeignKey(l=>l.UserId).OnDelete(DeleteBehavior.SetNull);
        }
    }
}

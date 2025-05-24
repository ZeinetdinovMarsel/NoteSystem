using Microsoft.EntityFrameworkCore;
using NoteSystem.DataAccess.Entities;


namespace NoteSystem.DataAccess;
public class NoteSystemDbContext(DbContextOptions<NoteSystemDbContext> options) : DbContext(options)
{
    public DbSet<UserEntity> Users { get; set; }
    public DbSet<CategoryEntity> Categories { get; set; }
    public DbSet<NoteEntity> Notes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(NoteSystemDbContext).Assembly);
    }
}

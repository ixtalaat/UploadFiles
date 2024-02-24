using Microsoft.EntityFrameworkCore;
using Upload.Api.Models;

namespace Upload.Api.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<UploadedFile> Files { get; set; }
}

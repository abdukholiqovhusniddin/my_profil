using AboutMe.Models;
using Microsoft.EntityFrameworkCore;

namespace AboutMe.Data
{
    public class MyDb(DbContextOptions<MyDb> options) : DbContext(options)
    {
        public DbSet<Counts> Counts { get; set; }
    }
}

using Microsoft.EntityFrameworkCore;
using Victor.models;

namespace Victor
{
    public class AppDataContext : DbContext
    {
        public DbSet<Consumo> Consumos { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=Matheus_Victor.db");
        }
    }
}
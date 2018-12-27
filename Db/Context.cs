using System.Data.Entity;

namespace Db
{
    public class Dbc : DbContext
    {
        public DbSet<Product> Products { get; set; }
        
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //Configure default schema
            modelBuilder.HasDefaultSchema("...");
        }
    }
}
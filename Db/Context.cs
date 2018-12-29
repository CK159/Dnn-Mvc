using System.Data.Entity;

namespace Db
{
    public class Dbc : DbContext
    {
        public DbSet<Product> Products { get; set; }

        public Dbc() : base("name=SiteSqlServer")
        {
        }
        
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //Configure default schema
            modelBuilder.HasDefaultSchema("...");
        }
    }
}
using System.Data.Entity;

namespace Db
{
    public class Dbc : DbContext
    {
        public DbSet<Product> Products { get; set; }
    }
}
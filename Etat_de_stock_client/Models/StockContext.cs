using Microsoft.EntityFrameworkCore;

namespace Etat_de_stock_client.Models
{
    public class StockContext : DbContext
    {
        public virtual DbSet<Magasin> magasin { get; set; }
        public virtual DbSet<Article> article { get; set; }
        public virtual DbSet<MagasinArticle> magasinarticle { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionBuilder)
        {
            optionBuilder.UseNpgsql(@"Host=localhost;Port=5432;Database=stock;Username=miaro;Password=miaro");
        }
    }
}
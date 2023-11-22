using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Etat_de_stock_client.Models
{
    public class Magasin
    {

        public Magasin[] FindById(string id_magasin)
        {
            Magasin[] magasins = null;
            using (StockContext ctx = new StockContext())
            {
                magasins = ctx.magasin.
                                Where(m => m.idMagasin.Equals(id_magasin)).
                                Take(1).
                                ToArray();
            }
            return magasins;
        }

        public Magasin[] FindAll()
        {
            Magasin[] magasins = null;
            using (StockContext ctx = new StockContext())
            {
                magasins = ctx.magasin.ToArray();
            }
            return magasins;
        }

        public Magasin(string idMagasin)
        {
            this.idMagasin = idMagasin;
        }

        public Magasin() { }
        [Key]
        [Column("id_magasin")]
        public string idMagasin { get; set; }
    }
}
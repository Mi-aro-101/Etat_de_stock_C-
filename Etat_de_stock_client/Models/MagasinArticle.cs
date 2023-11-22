using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Etat_de_stock_client.Models
{
    /// <summary>
    /// As The doctrine relation ManyToMany does not exist due to my low version of dotnet framework
    /// This class represent it (A table containing the data about article contained in a magasin [do not take on account the quantity])
    /// </summary>
    public class MagasinArticle
    {

        /// <summary>
        /// Find Articles present in a specified magasin
        /// </summary>
        /// <param name="id_magasin"></param>
        /// <returns></returns>
        public MagasinArticle[] FindArticleInMagasin(string id_magasin)
        {
            MagasinArticle[] magasinarticles = null;
            using (StockContext ctx = new StockContext())
            {
                magasinarticles = ctx.magasinarticle.
                                    Include(ma => ma.article).
                                    Include(ma => ma.magasin).
                                    Where(ma => ma.magasin.idMagasin.Equals(id_magasin)).
                                    ToArray();
            }
            return magasinarticles;
        }

        /// <summary>
        /// Find all magasinarticle
        /// </summary>
        /// <returns></returns>
        public MagasinArticle[] FindAll()
        {
            MagasinArticle[] magasinarticles = null;
            using (StockContext ctx = new StockContext())
            {
                magasinarticles = ctx.magasinarticle.
                                    Include(ma => ma.article).
                                    Include(ma => ma.magasin).
                                    ToArray();
            }
            return magasinarticles;
        }

        public MagasinArticle() { }

        [Key]
        public int id_magasin_article { get; set; }

        [ForeignKey("id_article")]
        public virtual Article article { get; set; }

        [ForeignKey("id_magasin")]
        public virtual Magasin magasin { get; set; }
    }
}
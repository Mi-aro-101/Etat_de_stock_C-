using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Etat_de_stock_client.Models
{
    public class Article
    {
        public Article() { }

        public Article FindById(string idArticle)
        {
            Article[] article = null;
            using (StockContext ctx = new StockContext())
            {
                article = ctx.article.
                                Where(a => a.idArticle.Equals(idArticle)).
                                Take(1).
                                ToArray();
            }
            return article[0];
        }

        public Article[] FindAll()
        {
            Article[] articles = null;
            using (StockContext ctx = new StockContext())
            {
                articles = ctx.article.
                                ToArray();
            }
            return articles;
        } 

        public Article(string idArticle, string desArticle, int sortieType)
        {
            this.idArticle = idArticle;
            this.desArticle = desArticle;
            this.sortieType = sortieType;
        }

        [Key]
        [Column("id_article")]
        public string idArticle { get; set; }
        [Column("des_article")]
        public string desArticle { get; set; }
        [Column("sortietype")]
        public int sortieType { get; set; }
    }
}
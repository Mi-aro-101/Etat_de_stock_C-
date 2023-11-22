using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Etat_de_stock_client.Models
{
    public class EtatDeStock
    {

        public async Task<Stock[]> GetStock(DateTime date1, DateTime date2, Magasin[] magasins)
        {
            List<Stock> stocks = new List<Stock>();
            foreach (Magasin magasin in magasins)
            {
                MagasinArticle[] articles = new MagasinArticle().FindArticleInMagasin(magasin.idMagasin);
                foreach (MagasinArticle article in articles)
                {
                    Stock stock = new Stock(article.article, magasin);
                    await stock.GetStockInterval(date1, date2);
                    stocks.Add(stock);
                }
            }

            return stocks.ToArray();
        }

        public DateTime dateInitiale { get; set; }
        public DateTime dateFinale { get; set; }
        public virtual Stock[] stocks { get; set; }
    }
}
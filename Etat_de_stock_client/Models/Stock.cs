using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Etat_de_stock_client.Models
{
    public class Stock
    {

        /// <summary>
        /// Calculate stock between two date
        /// </summary>
        /// <param name="date1"></param>
        /// <param name="date2"></param>
        public async Task GetStockInterval(DateTime date1, DateTime date2)
        {
            WebServiceClient Ws = new WebServiceClient("http://localhost:8080/");
            Entree firstEntree = await new Entree(Ws).GetFirstEntree(article.idArticle, magasin.idMagasin);

            //Get qte initiale [Depending on first date entree and the first date from the interval]
            this.QuantiteInitiale = await this.CalculQteIntervale(firstEntree.dateEntree, date1);
            this.QuantiteReste = await this.CalculQteReste(date1, date2);
            double montant = await this.CalculMontant(date2);
            this.Montant = montant;
            this.PrixUnitaire = this.Montant / this.QuantiteReste;
        }

        /// <summary>
        /// Calculate quantite on a date x
        /// Entree - Sortie between date1 and date2
        /// </summary>
        /// <param name="date1"></param>
        /// <param name="date2"></param>
        /// <returns></returns>
        public async Task<double> CalculQteIntervale(DateTime date1, DateTime date2)
        {
            //Instanciate a WebServiceClientObject
            WebServiceClient WsC = new WebServiceClient("http://localhost:8080/");
            // Provide WsClient in entree and sortie
            Entree callerObjEntree = new Entree(WsC);
            Sortie callerObjSortie = new Sortie(WsC);

            // Get entree et sorties that belongs to an interval of date
            Entree[] AllEntree = await callerObjEntree.GetEntreeInterval(date1, date2, article.idArticle, magasin.idMagasin);
            Sortie[] AllSortie = await callerObjSortie.GetSortieInterval(date1, date2, article.idArticle, magasin.idMagasin);

            //Sum quantite of entree and sortie
            double sumEntree = Utils.SumQte(AllEntree);
            double sumSortie = Utils.SumQte(AllSortie);

            //Return the difference between Entree Interval and Sortie Interval
            return sumEntree - sumSortie;
        }

        /// <summary>
        /// Function that calculate the quantite restante
        /// Get all entree - Get all sortie (between date1 and date2) + Quantite initiale  (Entreee - Sortie between date initiale and date1)
        /// </summary>
        /// <param name="date1"></param>
        /// <param name="date2"></param>
        /// <returns></returns>
        public async Task<double> CalculQteReste(DateTime date1, DateTime date2)
        {
            double qteMouvementEntre = await this.CalculQteIntervale(date1, date2);

            return this.QuantiteInitiale + qteMouvementEntre;
        }

        public Stock() { }

        public Stock(Article article, Magasin magasin)
        {
            this.article = article;
            this.magasin = magasin;
        }
        
        /// <summary>
        /// Voi les quantites restants, faire leur somme en les multipliants par leur prix
        /// </summary>
        /// <param name="date2"></param>
        /// <returns></returns>
        public async Task<double> CalculMontant(DateTime date2)
        {
            double result = 0;
            //Instanciate a WebServiceClientObject
            WebServiceClient WsC = new WebServiceClient("http://localhost:8080/");
            EtatEntree etatentree = new EtatEntree(WsC);
            EtatEntree[] etats = await etatentree.GetEtatEntreeBefore(date2, this.article.idArticle, this.magasin.idMagasin);

            foreach(EtatEntree ea in etats)
            {
                result += (ea.quantiteReste * ea.entree.prix);

            }

            return result;
        }

        public virtual Magasin magasin { get; set; }
        public virtual Article article { get; set; }
        public double QuantiteInitiale { get; set; }
        public double QuantiteReste { get; set; }
        public double Montant { get; set; }
        public double PrixUnitaire { get; set; }
    }
}
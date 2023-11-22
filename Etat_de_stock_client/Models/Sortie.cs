using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Etat_de_stock_client.Models
{
    public class Sortie : Mouvement
    {
        /// <summary>
        /// Send this insert to the database using WebService
        /// </summary>
        /// <returns></returns>
        public async Task<string> Flush()
        {
            string url = "http://localhost:8080/";
            WebServiceClient WsClient = new WebServiceClient(url);
            string endpoint = "api/sortie/insert";

            var json = JsonConvert.SerializeObject(this, new IsoDateTimeConverter() { DateTimeFormat = "yyyy-MM-dd" });

            HttpContent content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            string result = await WsClient.Flush(endpoint, content, json);

            return result;
        }

        public Sortie NewSortie(Article article, Magasin magasin, double quantite, DateTime dateSortie)
        {
            Sortie sortie = new Sortie();
            string pk = sortie.ConstructPrimaryKey("OUT");
            sortie.idSortie = pk;
            sortie.article = article;
            sortie.magasin = magasin;
            sortie.quantite = quantite;
            sortie.dateSortie = dateSortie;

            return sortie;
        }

        /// <summary>
        /// Inserting a sortie and proceed to Fifo system, takes the etatentree in ascending order by it's date
        /// And then diminish the first EtatEntree and recursively take the next if the current is DRY UP
        /// </summary>
        /// <param name="article"></param>
        /// <param name="magasin"></param>
        /// <param name="quantite"></param>
        /// <param name="dateSortie"></param>
        /// <returns>String message that the update and insertion are successful or not</returns>
        public async Task<string> ProceedFifoSystem(Article article, Magasin magasin, double quantite, DateTime dateSortie)
        {
            Sortie toInsert = NewSortie(article, magasin, quantite, dateSortie);

            EtatEntree[] etatentrees = await new EtatEntree(new WebServiceClient("http://localhost:8080/")).
                                        GetEtatEntreeBefore(dateSortie, article.idArticle, magasin.idMagasin);
            Int16 i = 0;
            i = Utils.Execute(article, magasin, quantite, dateSortie, etatentrees, i, toInsert);
            EtatEntree[] etats = new EtatEntree[i+1];
            Array.Copy(etatentrees, 0, etats, 0, i+1);

            SortieEtatEntree sortieEtatEntree = new SortieEtatEntree(toInsert, etats);

            string result = await sortieEtatEntree.Flush();

            return result;

        }


        /// <summary>
        /// Insert a sortie and proceeding to Lifo system, 
        /// That means taking the last entry first and recursively take the next last if the present is dried up
        /// </summary>
        /// <param name="article"></param>
        /// <param name="magasin"></param>
        /// <param name="quantite"></param>
        /// <param name="dateSortie"></param>
        /// <returns>String message that the update and insertion are successful or not</returns>
        public async Task<string> ProceedLifoSystem(Article article, Magasin magasin, double quantite, DateTime dateSortie)
        {

            Sortie toInsert = NewSortie(article, magasin, quantite, dateSortie);

            EtatEntree[] etatentreesFifo = await new EtatEntree(new WebServiceClient("http://localhost:8080/")).
                                        GetEtatEntreeBefore(dateSortie, article.idArticle, magasin.idMagasin);

            // Order the etat entree by date to descending as it takes the last date to first out
            EtatEntree[] etatentrees = Utils.ToDesc(etatentreesFifo);

            Int16 i = 0;
            i = Utils.Execute(article, magasin, quantite, dateSortie, etatentrees, i, toInsert);
            EtatEntree[] etats = new EtatEntree[i + 1];
            Array.Copy(etatentrees, 0, etats, 0, i + 1);

            SortieEtatEntree sortieEtatEntree = new SortieEtatEntree(toInsert, etats);

            string result = await sortieEtatEntree.Flush();

            return result;
        }

        /// <summary>
        /// Inserer une sortie
        /// </summary>
        /// <param name="article"></param>
        /// <param name="magasin"></param>
        /// <param name="quantite"></param>
        /// <param name="dateSortie"></param>
        public async Task<string> InsertSortie(Article article, Magasin magasin, double quantite, DateTime dateSortie)
        {
            string result = "";
            try
            {
                this.ControlQte(article, magasin, quantite, dateSortie);

                if(article.sortieType == 1) { result = await this.ProceedFifoSystem(article, magasin, quantite, dateSortie); }
                else if(article.sortieType == -1) { result = await this.ProceedLifoSystem(article, magasin, quantite, dateSortie); }

            }catch(Exception e)
            {
                throw e;
            }

            return result;
        }

        /// <summary>
        /// Check out if there is enough qte in stock before proceeding in insert the sortie
        /// </summary>
        /// <param name="article"></param>
        /// <param name="magasin"></param>
        /// <param name="quantite"></param>
        /// <param name="dateSortie"></param>
        /// <returns>True if there is enough stock to make this sortie</returns>
        /// <throws> A message Exception </throws>
        public async Task<Boolean> ControlQte(Article article, Magasin magasin, double quantite, DateTime dateSortie)
        {
            Boolean result = true;
            // Calculate quantite restant of this article in this magasin to see if there is enough left
            // Instanciate Stock object as this contains the function that can calculate quantite restante of an article in a magasin
            Stock stock = new Stock(article, magasin);

            // Get the first entree to have a reference and calculate the quantite on this date sortie
            Entree firstEntree = await new Entree(new WebServiceClient("http://localhost:8080/")).GetFirstEntree(article.idArticle, magasin.idMagasin);

            double qteRestante = await stock.CalculQteIntervale(firstEntree.dateEntree, dateSortie);

            if (qteRestante - quantite < 0)
            {
                throw new Exception("Quantite insuffisant en stock, vous ne pouvez pas effectuer cette transaction");
            }

            return result;
        }


        public Sortie(WebServiceClient Ws)
        {
            this.WsClient = Ws;
        }

        public Sortie()
        {
        }

        /// <summary>
        /// Get all sortie of a specified article and magasin between two dates
        /// </summary>
        /// <param name="date1"></param>
        /// <param name="date2"></param>
        /// <param name="idArticle"></param>
        /// <param name="idMagasin"></param>
        /// <returns></returns>
        public async Task<Sortie[]> GetSortieInterval(DateTime date1, DateTime date2, string idArticle, string idMagasin)
        {
            // Instanciate Webservice caller class
            string endpoint = "api/sortie/interval";

            var parameter = new Dictionary<string, string>
            {
                {"date1", date1.ToString("yyyy-MM-dd") },
                {"date2", date2.ToString("yyyy-MM-dd") },
                {"idArticle", idArticle },
                {"idMagasin", idMagasin }
            };

            string response =await this.WsClient.CallWebService(endpoint, parameter);

            return JsonConvert.DeserializeObject<Sortie[]>(response);
        }

        public string idSortie { get; set; }
        public DateTime dateSortie { get; set; }
        [System.Text.Json.Serialization.JsonIgnore]
        public WebServiceClient WsClient { get; set; }
    }
}
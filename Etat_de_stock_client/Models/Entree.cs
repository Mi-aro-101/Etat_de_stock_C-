using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Etat_de_stock_client.Models
{
    public class Entree : Mouvement
    {

        public Entree(WebServiceClient Ws)
        {
            this.WsClient = Ws;
        }

        public Entree() { }

        public async Task<Entree> GetFirstEntree(string idArticle, string idMagasin)
        {
            string endpoint = "api/entree/first";

            Dictionary<string, string> parameter = new Dictionary<string, string>
            {
                {"idArticle", idArticle },
                {"idMagasin", idMagasin }
            };

            string response = await this.WsClient.CallWebService(endpoint, parameter);
            return JsonConvert.DeserializeObject<Entree>(response);
        }

        public async Task<Entree[]> GetEntreeInterval(DateTime date1, DateTime date2, string idArticle, string idMagasin)
        {
            // Instanciate Webservice caller class
            string endpoint = "api/entree/interval";

            Dictionary<string, string> parameter = new Dictionary<string, string>
            {
                {"date1", date1.ToString("yyyy-MM-dd") },
                {"date2", date2.ToString("yyyy-MM-dd") },
                {"idArticle", idArticle },
                {"idMagasin", idMagasin }
            };

            string response = await this.WsClient.CallWebService(endpoint, parameter);
            Console.WriteLine(response);

            return JsonConvert.DeserializeObject<Entree[]>(response);
        }

        public async Task<string> GetEntreeIntervaleee(DateTime date1, DateTime date2, string idArticle, string idMagasin)
        {
            // Instanciate Webservice caller class
            string endpoint = "api/entree/interval";

            Dictionary<string, string> parameter = new Dictionary<string, string>
            {
                {"date1", date1.ToString("yyyy-MM-dd") },
                {"date2", date2.ToString("yyyy-MM-dd") },
                {"idArticle", idArticle },
                {"idMagasin", idMagasin }
            };

            string response = await this.WsClient.CallWebService(endpoint, parameter);
            Console.WriteLine(response);

            return response;
        }

        public string idEntree { get; set; }
        public double prix { get; set; }
        public DateTime dateEntree { get; set; }
        [System.Text.Json.Serialization.JsonIgnore]
        public WebServiceClient WsClient { get; set; }
    }
}
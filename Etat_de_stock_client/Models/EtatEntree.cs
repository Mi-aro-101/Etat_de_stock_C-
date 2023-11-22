using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Etat_de_stock_client.Models
{
    public class EtatEntree
    {

        public static async Task<string> FlushAll(EtatEntree[] etats, string jsonvalue)
        {
            WebServiceClient WsCLient = new WebServiceClient("http://localhost:8080/");
            EtatEntree etatentrees = new EtatEntree(WsCLient);
            string endpoint = "api/etatentree/updates";

            HttpContent content = new StringContent(jsonvalue, System.Text.Encoding.UTF8, "application/json");

            string result = await WsCLient.Flush(endpoint, content, jsonvalue);
            return result;
        }

        public async Task<EtatEntree[]> GetEtatEntreeBefore(DateTime date2, string idArticle, string idMagasin)
        {
            string endpoint = "api/etatentree/actuel";

            Dictionary<string, string> parameter = new Dictionary<string, string>
            {
                {"date2", date2.ToString("yyyy-MM-dd") },
                {"idArticle", idArticle },
                {"idMagasin", idMagasin }
            };

            string response = await this.WsClient.CallWebService(endpoint, parameter);

            return JsonConvert.DeserializeObject<EtatEntree[]>(response);
        }


        public EtatEntree(WebServiceClient Ws)
        {
            this.WsClient = Ws;
        }

        public int idEtatEntree { get; set; }
        public virtual Entree entree { get; set; }
        public virtual Sortie sortie { get; set; }
        public double quantiteReste { get; set; }
        public DateTime dateEtat { get; set; }
        [System.Text.Json.Serialization.JsonIgnore]
        public WebServiceClient WsClient { get; set; }

    }
}
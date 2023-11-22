using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Etat_de_stock_client.Models
{
    public class SortieEtatEntree
    {

        /// <summary>
        /// flush this Sortie EtatEntree (a class containing the sortie and The EtatEntree[] affected by it) 
        /// send it to a web service where it will be inserted
        /// </summary>
        /// <returns>string value json result of the operation (error or not)</returns>
        public async Task<string> Flush()
        {
            string endpoint = "api/etatentree/updates";
            WebServiceClient WsClient = new WebServiceClient("http://localhost:8080/");

            // Options for JsonSerialize to not take null values and format all the dates as "yyyy-mm-dd"
            JsonSerializerSettings options = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                Converters = { new IsoDateTimeConverter() { DateTimeFormat = "yyyy-MM-dd" } }
            };

            string json = JsonConvert.SerializeObject(this, options);

            HttpContent content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            string result = await WsClient.Flush(endpoint, content, json);

            return result;

        }

        public SortieEtatEntree(Sortie sortie, EtatEntree[] etatEntree)
        {
            this.sortie = sortie;
            this.etatEntree = etatEntree;
        }

        public SortieEtatEntree() { }

        public virtual Sortie sortie { get; set; }
        public EtatEntree[] etatEntree { get; set; }

    }
}
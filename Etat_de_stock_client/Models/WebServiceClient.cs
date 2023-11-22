using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Etat_de_stock_client.Models
{
    public class WebServiceClient
    {
        public HttpClient _httpClient { get; set; }
        public string _url { get; set; }

        public WebServiceClient(string _url)
        {
            this._httpClient = new HttpClient();
            this._url = _url;
        }

        /// <summary>
        /// Call a web service (Spring boot) to retrieve data having special queries
        /// </summary>
        /// <param name="endpoint"></param>
        /// <param name="getparam"></param>
        /// <returns></returns>
        public async Task<string> CallWebService(string endpoint, Dictionary<string, string> getparam)
        {
            string url = $"{_url}{endpoint}";

            if(getparam != null && getparam.Count > 0)
            {
                url = this.queryParam(url, getparam);
            }

            try
            {
                HttpResponseMessage response = await this._httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    return result;
                }
                else
                {
                    return $"{response.StatusCode}";
                }
            }catch(Exception e)
            {
                throw e;
            }
        }

        private string queryParam(string url, Dictionary<string, string> getparam)
        {
            string queryString = string.Empty;
            foreach(var kvp in getparam)
            {
                queryString+= $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value)}&";
            }
            queryString = queryString.TrimEnd('&');

            return url+=$"?{queryString}";
        }

        public async Task<string> Flush(string endpoint, HttpContent content, string json)
        {
            string url = $"{_url}{endpoint}";

            try
            {
                HttpResponseMessage response = await this._httpClient.PostAsync(url, content);
                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    return result;
                }
                else
                {
                    //return response.Content.ReadAsStringAsync().Result;
                    return json;
                }
            }
            catch (Exception e)
            {
                return $"{e.Message}";
            }
        }
    }
}
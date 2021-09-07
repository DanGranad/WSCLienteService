using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace WSGCliente.DataAccess
{
    public class HttpClientService : HttpClient
    {
        // singleton instance
        private static readonly HttpClientService instance = new HttpClientService();

        static HttpClientService() { }

        private HttpClientService() : base()
        {
            Timeout = TimeSpan.FromSeconds(60);
            MaxResponseContentBufferSize = 999999;
        }

        /// <summary>
        /// Returns the singleton instance of HttpClient
        /// </summary>
        public static HttpClientService Instance
        {
            get
            {
                return instance;
            }
        }

        public async Task<T> GetItem<T>(string Url)
        {
            try
            {
                var uri = new Uri(Url);

                var response = await GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var Item = JsonConvert.DeserializeObject<T>(content);
                    return Item;
                }
                throw new Exception(response.ReasonPhrase);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<T>> GetListItems<T>(string Url)
        {
            var uri = new Uri(Url);
            //if (VariablesGlobales.Token != "")
            //{
            //    DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", VariablesGlobales.Token);
            //}

            DefaultRequestHeaders.ExpectContinue = false;
            var response = await GetAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var Items = JsonConvert.DeserializeObject<List<T>>(content);
                return Items;
            }
            throw new Exception(response.ReasonPhrase);
        }
    }
}

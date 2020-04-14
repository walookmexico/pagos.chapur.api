using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace PagosGranChapur.Services.Helpers
{
    public static class RequestService
    {
        /// <summary>
        /// Método que permite realizar peticiones GET
        /// </summary>
        /// <typeparam name="TU"></typeparam>
        /// <param name="method"></param>
        /// <param name="endPoint"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static async Task<TU> Get<TU>(string method, string endPoint, string parameters)
        {
            try
            {
                using (var client = new HttpClient { MaxResponseContentBufferSize = 2147483647, Timeout = TimeSpan.FromSeconds(60) })
                {
                    client.DefaultRequestHeaders.Clear();

                    var response = await client.GetAsync($"{endPoint}/{method}/{parameters}");
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        return (TU)Convert.ChangeType(content, typeof(TU));
                    }
                    else
                    {
                        if (response.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable)
                            throw new HttpRequestException($"Problemas al conectarse con el servidor de servicios: {(int)response.StatusCode} - {response.StatusCode}");
                        else
                            throw new Exception(response.ReasonPhrase);
                    }
                }
                
            }
            catch (System.Net.Http.HttpRequestException e)
            {
                throw new System.Net.Http.HttpRequestException(e.Message);
            }
            catch (Exception e)
            {
                return default(TU);
            }
        }

        /// <summary>
        /// Método que permite realizar peticiones POST 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TU"></typeparam>
        /// <param name="method"></param>
        /// <param name="endPoint"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static async Task<TU> Post<T, TU>(string method, string endPoint, T obj)
        {
            try
            {
                using (var client = new HttpClient { MaxResponseContentBufferSize = 2147483647, Timeout = TimeSpan.FromSeconds(60) })
                {
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    var json = JsonConvert.SerializeObject(obj);
                    var postContent = new StringContent(json, Encoding.UTF8, "application/json");
                    var response = await client.PostAsync($"{endPoint}/{method}", postContent);
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        return JsonConvert.DeserializeObject<TU>(content);
                    }
                    else {

                        if (response.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable)
                            throw new HttpRequestException($"Problemas al conectarse con el servidor de servicios: {(int)response.StatusCode} - {response.StatusCode}"   );
                        else
                            return default(TU);
                    }
                    
                }
            }
            catch (System.Net.Http.HttpRequestException e)
            {
                throw new System.Net.Http.HttpRequestException(e.Message);
            }
            catch (Exception e)
            {                

                return default(TU);
            }
        }
    }
}

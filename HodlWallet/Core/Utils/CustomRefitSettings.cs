using HodlWallet.Core.Interfaces;
using Refit;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace HodlWallet.Core.Utils
{
    public class CustomRefitSettings
    {
        static string endPointUrl2;
        static HttpClient httpClient;

        static void CreateCustomHttpClient()
        {
#if DEBUG
            HttpClientHandler httpHandler = new HttpClientHandler 
            { 
                ServerCertificateCustomValidationCallback = (message, cert, chain, sslErrors) => true 
            };
#else
            HttpClientHandler httpHandler = new HttpClientHandler ();
#endif
            httpClient = new HttpClient(httpHandler) { BaseAddress = new Uri(endPointUrl2) };

        }

        public static IPrecioHttpService RestClient(string endPointUrl = Constants.PRECIO_HOST_URL)
        {
            endPointUrl2 = endPointUrl;
            CreateCustomHttpClient();

            var reffitSetting = new RefitSettings(new NewtonsoftJsonContentSerializer());
            IPrecioHttpService PrecioHttpService = RestService.For<IPrecioHttpService>(httpClient, reffitSetting);
            return PrecioHttpService;
        }

    }
}

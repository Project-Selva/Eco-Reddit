using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Eco_Reddit.Models;
using Newtonsoft.Json.Linq;

namespace Eco_Reddit.Helpers
{
    internal class LoginHelper
    {
        private readonly HttpClient client = new HttpClient();
        private readonly string clientId = "-bL9o_t7kgNNmA";
        private readonly string clientSecret = "SESshAirmwAuAvBFHbq_JUkAMmk";

        public LoginHelper(string clientId, string clientSecret)
        {
            this.clientId = clientId;
            this.clientSecret = clientSecret;
        }

        public Task<AuthViewModel> Login_Refresh(string refreshToken)
        {
            //Use the code to retrieve access Token and refresh token
            var nvc = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("grant_type", "refresh_token"),
                new KeyValuePair<string, string>("refresh_token", refreshToken)
            };

            var req = new HttpRequestMessage(HttpMethod.Post, Constants.Constants.redditApiBaseUrl + "access_token")
                {Content = new FormUrlEncodedContent(nvc)};
            req.Headers.Authorization = new AuthenticationHeaderValue("Basic",
                Convert.ToBase64String(Encoding.ASCII.GetBytes(clientId + ":" + clientSecret)));
            return GetResult<AuthViewModel>(req);
        }

        public Task<AuthViewModel> Login_Stage2(Uri callbackUri)
        {
            var regex = new Regex(Regex.Escape("#"));
            var updatedUriStr = regex.Replace(callbackUri.AbsoluteUri, "?", 1);
            var updatedUri = new Uri(updatedUriStr);
            var code = HttpUtility.ParseQueryString(updatedUri.Query).Get("code");

            //Use the code to retrieve access Token and refresh token
            var nvc = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("code", code),
                new KeyValuePair<string, string>("grant_type", "authorization_code"),
                new KeyValuePair<string, string>("redirect_uri", "http://127.0.0.1:3000/reddit_callback")
            };

            var req = new HttpRequestMessage(HttpMethod.Post, Constants.Constants.redditApiBaseUrl + "access_token")
                {Content = new FormUrlEncodedContent(nvc)};
            req.Headers.Authorization = new AuthenticationHeaderValue("Basic",
                Convert.ToBase64String(Encoding.ASCII.GetBytes(clientId + ":" + clientSecret)));
            return GetResult<AuthViewModel>(req);
        }

        /// <summary>
        ///     Private method for retrieving result and converting to .NET Type
        /// </summary>
        /// <typeparam name="Response">TResponse</typeparam>
        /// <param name="msg">HttpRequestMessage</param>
        /// <returns></returns>
        private async Task<Response> GetResult<Response>(HttpRequestMessage msg)
        {
            using (var response = await client.SendAsync(msg))
            {
                using (var content = response.Content)
                {
                    var responseContent = await content.ReadAsStringAsync();
                    if (!response.IsSuccessStatusCode)
                        throw new Exception(responseContent);

                    if (typeof(IConvertible).IsAssignableFrom(typeof(Response)))
                        return (Response) Convert.ChangeType(responseContent, typeof(Response));
                    return JToken.Parse(responseContent).ToObject<Response>();
                }
            }
        }
    }
}

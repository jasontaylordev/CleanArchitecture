using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace WpfUI.Api
{
    public class Api : IApi
    {
        private HttpClient _client;

        public Api()
        {
            InitializeClient();
        }

        public HttpClient Client { get { return _client; } }

        private void InitializeClient()
        {
            string uri = "https://localhost:5001/";//ConfigurationManager.AppSettings["api"];

            _client = new HttpClient();
            _client.BaseAddress = new Uri(uri);
        }
    }
}

using System;
using System.Net.Http;

namespace org.ochin.interoperability.OCHINInterfaceUtilities.Utilities
{
    public class HTTPConnection : IDisposable
    {
        public string UrlBase { get; private set; }

        private HttpClient Client = null;

        public HTTPConnection(string urlBase)
        {
            UrlBase = urlBase;

            InitClient();
        }

        private void InitClient()
        {
            if (Client == null)
            {
                Client = new HttpClient()
                {
                    BaseAddress = new Uri(UrlBase)
                };
            }
        }

        public bool Get(string request, out string response)
        {
            string fullRequest = Client.BaseAddress.PathAndQuery + request;

            var result = Client.GetAsync(fullRequest).Result;
            response = result.Content.ReadAsStringAsync().Result;

            return result.IsSuccessStatusCode;
        }

        public void Dispose()
        {
            Client?.Dispose();
        }

        private string GetAllExceptionMessages(Exception ex)
        {
            string msg = string.Empty;
            do
            {
                msg += ex.Message + Environment.NewLine;
                ex = ex.InnerException;
            } while (ex != null);

            return msg;
        }
    }
}

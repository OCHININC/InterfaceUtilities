using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace org.ochin.interoperability.OCHINInterfaceUtilities.Mirth
{
    public class MirthConnection : IDisposable
    {
        public string BaseUrl { get; private set; }

        private HttpClient Client = null;

        public MirthConnection(string mirthUrlBase)
        {
            BaseUrl = mirthUrlBase;
            InitHttpClient();
        }

        private void InitHttpClient()
        {
            if (Client == null)
            {
                var handler = new WebRequestHandler();
                handler.ServerCertificateValidationCallback += CertValidationCallback;

                Client = new HttpClient(handler, true);

                Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));

                Client.BaseAddress = new Uri(BaseUrl);
            }
        }

        private bool CertValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        public bool Login(string username, string password, out string statusMsg)
        {
            List<KeyValuePair<string, string>> credentials = new List<KeyValuePair<string, string>>(2)
            {
                new KeyValuePair<string, string>("username", username),
                new KeyValuePair<string, string>("password", password)
            };

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "users/_login")
            {
                Content = new FormUrlEncodedContent(credentials)
            };

            bool ret = false;
            statusMsg = string.Empty;
            try
            {
                HttpResponseMessage result = Client.SendAsync(request).Result;
                string resultContent = result.Content.ReadAsStringAsync().Result;
                XmlDocument resultDoc = new XmlDocument();
                resultDoc.LoadXml(resultContent);

                string resultStatus = resultDoc.SelectSingleNode("//status").InnerText;
                string resultMessage = resultDoc.SelectSingleNode("//message").InnerText;

                statusMsg = resultStatus + ": " + resultMessage;

                ret = result.IsSuccessStatusCode;
            }
            catch(Exception ex)
            {
                statusMsg = GetAllExceptionMessages(ex);
            }

            return ret;
        }

        public bool Logout(out string statusMsg)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "users/_logout");

            bool ret = false;
            statusMsg = string.Empty;
            try
            {
                HttpResponseMessage result = Client.SendAsync(request).Result;

                ret = result.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                statusMsg = GetAllExceptionMessages(ex);
            }

            return ret;
        }

        public bool GetChannelsStatuses(bool includeUndeployed, out string statuses)
        {
            return HttpGet("channels/statuses?includeUndeployed=" + includeUndeployed, out statuses);
        }

        public bool GetChannels(bool pollingOnly, out string channels)
        {
            return HttpGet("channels?pollingOnly=" + pollingOnly, out channels);
        }

        public bool StopChannels(List<string> channelIds, bool returnErrors, out string statusMsg)
        {
            List<KeyValuePair<string, string>> c = new List<KeyValuePair<string, string>>();
            foreach (string channelId in channelIds)
            {
                c.Add(new KeyValuePair<string, string>("channelId", channelId));
            }

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "channels/_stop?returnErrors=" + returnErrors)
            {
                Content = new FormUrlEncodedContent(c)
            };

            bool ret = false;
            statusMsg = string.Empty;
            try
            {
                HttpResponseMessage result = Client.SendAsync(request).Result;
                //string resultContent = result.Content.ReadAsStringAsync().Result;
                //XmlDocument resultDoc = new XmlDocument();
                //resultDoc.LoadXml(resultContent);

                ret = result.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                statusMsg = GetAllExceptionMessages(ex);
            }

            return ret;
        }

        public bool StartChannels(List<string> channelIds, bool returnErrors, out string statusMsg)
        {
            List<KeyValuePair<string, string>> c = new List<KeyValuePair<string, string>>();
            foreach (string channelId in channelIds)
            {
                c.Add(new KeyValuePair<string, string>("channelId", channelId));
            }

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "channels/_start?returnErrors=" + returnErrors)
            {
                Content = new FormUrlEncodedContent(c)
            };

            bool ret = false;
            statusMsg = string.Empty;
            try
            {
                HttpResponseMessage result = Client.SendAsync(request).Result;
                //string resultContent = result.Content.ReadAsStringAsync().Result;
                //XmlDocument resultDoc = new XmlDocument();
                //resultDoc.LoadXml(resultContent);

                ret = result.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                statusMsg = GetAllExceptionMessages(ex);
            }

            return ret;
        }

        private bool HttpGet(string requestPath, out string resultString)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, requestPath);
            HttpResponseMessage result = Client.SendAsync(request).Result;

            resultString = result.Content.ReadAsStringAsync().Result;

            return result.IsSuccessStatusCode;
        }

        public void Dispose()
        {
            if (Client != null)
            {
                Client.Dispose();
            }
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

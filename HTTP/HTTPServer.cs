using System.Text;

namespace org.ochin.interoperability.OCHINInterfaceUtilities.Utilities
{
    public class HTTPServer
    {
        public HTTPConnection HttpConnection { get; private set; }

        public HTTPServer(string urlBase)
        {
            HttpConnection = new HTTPConnection(urlBase);
        }

        ~HTTPServer()
        {
            HttpConnection?.Dispose();
        }

        public bool Get(string request, out string response)
        {
            return HttpConnection.Get(request, out response);
        }

        public override string ToString()
        {
            StringBuilder server = new StringBuilder();
            server.AppendLine("URL Base: " + HttpConnection.UrlBase);

            return server.ToString();
        }
    }
}

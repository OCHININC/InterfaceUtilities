namespace org.ochin.interoperability.OCHINInterfaceUtilities.Utilities
{
    public class HTTPVM
    {
        private readonly HTTPServer Server;

        public HTTPVM(string urlBase)
        {
            Server = new HTTPServer(urlBase);
        }

        public bool Get(string request, out string response)
        {
            return Server.Get(request, out response);
        }
    }
}

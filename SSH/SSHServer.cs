using System.Text;

namespace org.ochin.interoperability.OCHINInterfaceUtilities.Utilities
{
    public class SSHServer
    {
        public SSHConnection SSHConnection { get; private set; }

        public SSHServer(string username, string password, string remoteHost, int port)
        {
            SSHConnection = new SSHConnection(username, password, remoteHost, port);
        }

        ~SSHServer()
        {
            SSHConnection?.Dispose();
        }

        public bool Connect(out string statusMsg)
        {
            return SSHConnection.Connect(out statusMsg);
        }

        public bool Disconnect(out string statusMsg)
        {
            return SSHConnection.Disconnect(out statusMsg);
        }

        public bool RunCommand(string cmd, bool sudo, string[] stdInputs, out string result, out string cmdExecuted)
        {
            return SSHConnection.RunCommand(cmd, sudo, stdInputs, out result, out cmdExecuted);
        }

        public override string ToString()
        {
            StringBuilder server = new StringBuilder();
            server.AppendLine("Server: " + SSHConnection.RemoteHost);

            return server.ToString();
        }
    }
}

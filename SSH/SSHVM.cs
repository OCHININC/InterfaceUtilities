namespace org.ochin.interoperability.OCHINInterfaceUtilities.Utilities
{
    public class SSHVM
    {
        protected readonly SSHServer Server;

        public SSHVM(string username, string password, string remoteHost, int port)
        {
            Server = new SSHServer(username, password, remoteHost, port);
        }

        public bool Connect(out string statusMsg)
        {
            return Server.Connect(out statusMsg);
        }

        public bool Disconnect(out string statusMsg)
        {
            return Server.Disconnect(out statusMsg);
        }

        public bool RunCommand(string cmd, bool sudo, out string result, out string cmdExecuted)
        {
            return RunCommand(cmd, sudo, new string[] { }, out result, out cmdExecuted);
        }

        public bool RunCommand(string cmd, bool sudo, string[] stdInputs, out string result, out string cmdExecuted)
        {
            return Server.RunCommand(cmd, sudo, stdInputs, out result, out cmdExecuted);
        }
    }
}

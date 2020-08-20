using org.ochin.interoperability.OCHINInterfaceUtilities.Utilities;

namespace org.ochin.interoperability.OCHINInterfaceUtilities.Pentra
{
    public class PentraServer : SSHServer
    {
        public PentraServer(string username, string password, string remoteHost, int port) :
            base(username, password, remoteHost, port)
        {
        }

        public bool StartGateway(out string result)
        {
            string cmd = "/usr/local/ochin/InterfaceComm/start.sh /usr/local/ochin/InterfaceComm/configs/pentra/pentra_peer_gateway.cfg";
            return RunCommand(cmd, true, out result);
        }

        public bool StopGateway(out string result)
        {
            string cmd = "/usr/local/ochin/InterfaceComm/stop.sh /usr/local/ochin/InterfaceComm/configs/pentra/pentra_peer_gateway.cfg";
            return RunCommand(cmd, true, out result);
        }

        public bool TailGatewayLogs(int lines, out string result)
        {
            string cmd = $"tail -{lines} /usr/local/ochin/InterfaceComm/logs/pentra_peer_gateway_prod.log";
            return RunCommand(cmd, true, out result);
        }
    }
}

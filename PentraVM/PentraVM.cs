using org.ochin.interoperability.OCHINInterfaceUtilities.Utilities;
using System;

namespace org.ochin.interoperability.OCHINInterfaceUtilities.Pentra
{
    public class PentraVM : SSHVM
    {
        public PentraVM(string username, string password, string remoteHost, int port) :
            base(username, password, remoteHost, port)
        {
        }

        public bool StartGateway(out string result, out string cmdExecuted)
        {
            string cmdStart = "/usr/local/ochin/InterfaceComm/start-Xincgc.sh /usr/local/ochin/InterfaceComm/configs/pentra/pentra_peer_gateway.cfg";
            bool ret = RunCommand(cmdStart, true, out result, out cmdExecuted);

            return ret;
        }

        public bool StopGateway(out string result, out string cmdExecuted)
        {
            string cmd = "/usr/local/ochin/InterfaceComm/stop.sh /usr/local/ochin/InterfaceComm/configs/pentra/pentra_peer_gateway.cfg";
            return RunCommand(cmd, true, new string[] { "N" }, out result, out cmdExecuted);
        }

        public bool TailGatewayLogs(int lines, out string result, out string cmdExecuted)
        {
            string cmd = $"tail -{lines} /usr/local/ochin/InterfaceComm/logs/pentra_peer_gateway_prod.log";
            return RunCommand(cmd, true, out result, out cmdExecuted);
        }
    }
}

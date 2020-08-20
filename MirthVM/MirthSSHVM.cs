using org.ochin.interoperability.OCHINInterfaceUtilities.Utilities;
using System;

namespace org.ochin.interoperability.OCHINInterfaceUtilities.Mirth
{
    public class MirthSSHVM : SSHVM
    {
        public MirthSSHVM(string username, string password, string remoteHost, int port) :
            base(username, password, remoteHost, port)
        {
        }

        public bool SearchPrdLogs(string text, string files, bool ignoreCase, bool regEx, out string result, out string cmdExecuted)
        {
            string options =
                "-n " + 
                (ignoreCase ? "-i " : string.Empty) +
                (regEx ? "-e " : string.Empty)
                ;

            string cmd = $"zgrep {options}\"{text}\" /usr/local/ochin/mirth/logs/prd/archive/{files}|less";

            return RunCommand(cmd, false, out result, out cmdExecuted);
        }
    }
}

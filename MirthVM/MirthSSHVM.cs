using org.ochin.interoperability.OCHINInterfaceUtilities.Utilities;
using System;

namespace org.ochin.interoperability.OCHINInterfaceUtilities.Mirth
{
    public class MirthSSHVM : SSHVM
    {
        private const string LOG_FILES_BASE_PATH = "/usr/local/ochin/mirth/logs/prd/archive";

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

            string cmd = $"zgrep {options}\"{text}\" {LOG_FILES_BASE_PATH}/{files}|less";

            return RunCommand(cmd, false, out result, out cmdExecuted);
        }

        public bool ListFiles(string files, bool ignoreCase, bool regEx, out string result, out string cmdExecuted)
        {
            string options = string.Empty;
            if (ignoreCase)
                options = "i";

            if (regEx)
                options += "E";

            if (!string.IsNullOrEmpty(options))
                options = "-" + options;

            string cmd = $"ls {LOG_FILES_BASE_PATH}/ | grep {options} \"{files}\"";

            return RunCommand(cmd, false, out result, out cmdExecuted);
        }

        public bool ListUniqueFiles(out string result, out string cmdExecuted)
        {
            string cmd = $"ls -1f {LOG_FILES_BASE_PATH}/ | rev | cut -c18- | rev | uniq";

            return RunCommand(cmd, false, out result, out cmdExecuted);
        }
    }
}

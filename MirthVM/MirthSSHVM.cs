using org.ochin.interoperability.OCHINInterfaceUtilities.Utilities;
using System;

namespace org.ochin.interoperability.OCHINInterfaceUtilities.Mirth
{
    public class MirthSSHVM : SSHVM
    {
        private const string LOG_FILES_BASE_PATH = "/usr/local/ochin/mirth/logs";

        public MirthSSHVM(string username, string password, string remoteHost, int port) :
            base(username, password, remoteHost, port)
        {
        }

        public bool SearchPrdLogs(string env, string loc, string text, string files, bool ignoreCase, bool regEx, out string result, out string cmdExecuted)
        {
            string options =
                "-n " + 
                (ignoreCase ? "-i " : string.Empty) +
                (regEx ? "-e " : string.Empty)
                ;

            string cmd = $"zgrep {options}\"{text}\" {LOG_FILES_BASE_PATH}/{env}/{loc}/{files}|less";

            return RunCommand(cmd, false, out result, out cmdExecuted);
        }

        public bool ListFiles(string env, string loc, string files, bool ignoreCase, bool regEx, out string result, out string cmdExecuted)
        {
            string options = string.Empty;
            if (ignoreCase)
                options = "i";

            if (regEx)
                options += "E";

            if (!string.IsNullOrEmpty(options))
                options = "-" + options;

            string cmd = $"ls {LOG_FILES_BASE_PATH}/{env}/{loc}/ | grep {options} \"{files}\"";

            return RunCommand(cmd, false, out result, out cmdExecuted);
        }

        public bool ListUniqueFiles(string env, string loc, out string result, out string cmdExecuted)
        {
            string cmd = $"ls -1f {LOG_FILES_BASE_PATH}/{env}/{loc}/ | rev | cut -c18- | rev | uniq";

            return RunCommand(cmd, false, out result, out cmdExecuted);
        }
    }
}

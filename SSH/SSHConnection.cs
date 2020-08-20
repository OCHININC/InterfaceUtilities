using Renci.SshNet;
using Renci.SshNet.Common;
using System;
using System.Collections.Generic;

namespace org.ochin.interoperability.OCHINInterfaceUtilities.Utilities
{
    public class SSHConnection : IDisposable
    {
        public string Username { get; private set; }
        public string Password { get; private set; }
        public string RemoteHost { get; private set; }
        public int Port { get; private set; }

        private SshClient Client = null;

        public SSHConnection(string username, string password, string remoteHost, int port)
        {
            Username = username;
            Password = password;
            RemoteHost = remoteHost;
            Port = port;
        }

        private void InitClient(string username, string password, bool autoAcceptHostKey)
        {
            if (Client == null)
            {
                Client = new SshClient(RemoteHost, Port, username, password);

                if (autoAcceptHostKey)
                {
                    Client.HostKeyReceived += delegate (object sender, HostKeyEventArgs e)
                    {
                        e.CanTrust = true;
                    };
                }
            }
        }

        public bool Connect(out string statusMsg, bool autoAcceptHostKey = true)
        {
            bool connected = false;
            statusMsg = string.Empty;

            try
            {
                InitClient(Username, Password, autoAcceptHostKey);

                if (Client != null)
                {
                    Client.Connect();
                    connected = true;
                }
            }
            catch (Exception ex)
            {
                statusMsg = GetAllExceptionMessages(ex);
            }

            return connected;
        }

        public bool Disconnect(out string statusMsg)
        {
            bool disconnected = false;
            statusMsg = string.Empty;

            try
            {
                Client?.Disconnect();
                disconnected = true;
            }
            catch (Exception ex)
            {
                statusMsg = GetAllExceptionMessages(ex);
            }

            return disconnected;
        }

        public bool RunCommand(string cmd, bool sudo, string[] stdInputs, out string result, out string cmdExecuted)
        {
            bool cmdRun = false;
            result = string.Empty;
            cmdExecuted = string.Empty;

            if (Client != null)
            {
                try
                {
                    string echoInput = string.Empty, echoInputMasked = string.Empty;
                    string inputs = string.Join("\\n", stdInputs);

                    if (sudo)
                    {
                        echoInput = Password;
                        echoInputMasked = "******";
                        cmd = "sudo -S " + cmd;
                    }

                    if (stdInputs.Length > 0)
                    {
                        if (!string.IsNullOrEmpty(echoInput))
                        {
                            echoInput += "\\n";
                            echoInputMasked += "\\n";
                        }

                        echoInput += inputs;
                        echoInputMasked += inputs;
                    }

                    string finalCmd = (!string.IsNullOrEmpty(echoInput) ? $"echo -e '{echoInput}' | " : string.Empty) + cmd;
                    cmdExecuted = (!string.IsNullOrEmpty(echoInputMasked) ? $"echo -e '{echoInputMasked}' | " : string.Empty) + cmd;

                    using (SshCommand cmdResult = Client.RunCommand(finalCmd))
                    {
                        cmdRun = (cmdResult.ExitStatus == 0);

                        result = cmdResult.Result;

                        if (!string.IsNullOrEmpty(cmdResult.Error))
                        {
                            result += Environment.NewLine + "Error: " + cmdResult.Error;
                        }
                    }
                }
                catch (Exception ex)
                {
                    result = GetAllExceptionMessages(ex);
                }
            }

            return cmdRun;
        }

        public void Dispose()
        {
            Client?.Dispose();
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

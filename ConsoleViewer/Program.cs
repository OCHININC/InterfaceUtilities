using org.ochin.interoperability.OCHINInterfaceUtilities.Mirth;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace org.ochin.interoperability.OCHINInterfaceUtilities
{
    class Program
    {
        static void Main(string[] args)
        {
            string env;
            do
            {
                Console.WriteLine("Select Mirth environment to connect to:");
                Console.WriteLine("(1) DEV");
                Console.WriteLine("(2) REL");
                Console.WriteLine("(3) PRD");
                Console.WriteLine("(4) STAGING");
                env = Console.ReadLine();
            } while (env != "1" && env != "2" && env != "3" && env != "4");

            MirthRestApiVM mirth = new MirthRestApiVM(env);

            Console.Write("Mirth username: ");
            string username = Console.ReadLine();

            Console.Write("Mirth password: ");
            string password = Console.ReadLine();

            bool loggedIn = mirth.Login(username, password, out string statusMsg);
            Console.WriteLine("Logged in: " + loggedIn + " - " + statusMsg);

            string option;
            do
            {
                Console.WriteLine();
                Console.WriteLine("Available options:");
                Console.WriteLine("(1) View deployed channels statuses");
                Console.WriteLine("(2) Stop all deployed channels");
                Console.WriteLine("(3) Start all deployed channels");

                Console.WriteLine("(0) Exit");
                option = Console.ReadLine();

                switch(option)
                {
                    case "1":
                        XmlDocument statuses = mirth.GetChannelStatuses(false);
                        Console.WriteLine(PrettyPrintXml(statuses));
                        break;
                    case "2":
                    case "3":
                        Console.WriteLine("This functionality is not yet supported");
                        break;
                    default:
                        break;
                }
            } while (option != "0");

            bool loggedOut = mirth.Logout(out _);
            Console.WriteLine("Logged out: " + loggedOut);

            Console.Write("Press any key to exit");
            _ = Console.ReadKey();
        }

        private static string PrettyPrintXml(XmlDocument doc)
        {
            string prettyXml = string.Empty;

            using (StringWriter sw = new StringWriter())
            {
                using (XmlTextWriter xw = new XmlTextWriter(sw))
                {
                    xw.Formatting = Formatting.Indented;
                    doc.WriteTo(xw);
                }

                prettyXml = sw.ToString();
            }

            return prettyXml;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace org.ochin.interoperability.OCHINInterfaceUtilities.Utilities
{
    public static class HTTPUtilities
    {
        public static void DownloadListAsCsv(List<string> list, string filename, HttpResponse response)
        {
            if (list.Count > 0)
            {
                StringBuilder sb = new StringBuilder(list.Count);
                foreach (string l in list)
                {
                    sb.AppendLine(l.Replace('^', ','));
                }

                Download(sb.ToString(), filename, response);
            }
        }

        public static void Download(string content, string filename, HttpResponse response)
        {
            response.Clear();
            response.ContentType = "application/octet-stream";
            response.AddHeader("Content-Disposition", $"attachment; filename={filename}");

            byte[] bytes = Encoding.Default.GetBytes(content);
            response.OutputStream.Write(bytes, 0, bytes.Length);

            response.End();
        }
    }
}

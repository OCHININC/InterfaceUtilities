using Newtonsoft.Json;
using org.ochin.interoperability.OCHINInterfaceUtilities.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace org.ochin.interoperability.OCHINInterfaceUtilities.Epic
{
    public class EpicInterConnectVM : HTTPVM
    {
        public EpicInterConnectVM(string urlBase) :
            base(urlBase)
        {
        }

        public bool GetDepList(string sa, string iit, out List<string> deps, out string response)
        {
            deps = new List<string>();

            string request = $"/GeneralUtils/getDepList?sa={sa}&iit={iit}";
            bool ret = Get(request, out response);

            if (ret)
            {
                var depList = new { deps = new List<string>() };
                var json = JsonConvert.DeserializeAnonymousType(response, depList);

                deps = json.deps;
            }

            return ret;
        }

        public bool GetLabAccts(out List<string> labAccts, out string response)
        {
            labAccts = new List<string>();

            string request = $"/GeneralUtils/getLabAccts";
            bool ret = Get(request, out response);

            if (ret)
            {
                var labAcctList = new { labAccts = new List<string>() };
                var json = JsonConvert.DeserializeAnonymousType(response, labAcctList);

                labAccts = json.labAccts;
            }

            return ret;
        }
    }
}

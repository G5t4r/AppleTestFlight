using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppleTestFlight.Core.Models
{
    public class BulkDeleteBetaTesterModel
    {
        public Data[] data { get; set; }
        public class Data
        {
            public string type { get; set; }
            public string id { get; set; }
        }
    }
}

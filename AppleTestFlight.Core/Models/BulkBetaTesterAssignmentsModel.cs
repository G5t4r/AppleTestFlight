using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppleTestFlight.Core.Models
{

    public class BulkBetaTesterAssignmentsModel
    {
        public Data data { get; set; }
    }

    public class Data
    {
        public string type { get; set; }
        public Attributes attributes { get; set; }
        public Relationships relationships { get; set; }
    }

    public class Attributes
    {
        public Betatester[] betaTesters { get; set; }
    }

    public class Betatester
    {
        public object id { get; set; }
        public string email { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public object assignmentResult { get; set; }
        public object[] errors { get; set; }
    }

    public class Relationships
    {
        public Betagroup betaGroup { get; set; }
    }

    public class Betagroup
    {
        public Data data { get; set; }
        public class Data
        {
            public string type { get; set; }
            public string id { get; set; }
        }
    }
}

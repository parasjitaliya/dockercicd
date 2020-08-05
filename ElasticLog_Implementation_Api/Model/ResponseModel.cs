using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElasticLog_Implementation_Api.Model
{
    public class ResponseModel
    {
        //[Nest.Keyword(IgnoreAbove =256)]
        public string Date { get; set; }
        public string Request { get; set; }
        public string Response { get; set; }
        public string ApiName { get; set; }
    }
    public class ErrorModel
    {
        public string Date { get; set; }
        public string Message { get; set; }
        public string StrackTrace { get; set; }
        public string Source { get; set; }
        public string TargetSite { get; set; }

    }
}

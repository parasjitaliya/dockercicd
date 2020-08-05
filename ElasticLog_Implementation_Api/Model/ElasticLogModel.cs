using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElasticLog_Implementation_Api.Model
{
    public class RequestResponseLogger
    {
        public string Date { get; set; }
        public string ProgramCode { get; set; }
        public string Request { get; set; }
        public string Response { get; set; }
        public string ApiName { get; set; }
    }

    public class ErrorLogger
    {
        public string Date { get; set; }
        public string ProgramCode { get; set; }
        public string Request { get; set; }
        public string ErrorMessage { get; set; }
        public string StackTrace { get; set; }
        public string Source { get; set; }
        public string TargetSite { get; set; }
        public string ApiName { get; set; }
    }
    public class ElasticSearchInput
    {
        public string ProgramCode { get; set; }
        public string ApiName { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
    }
}

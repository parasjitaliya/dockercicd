using ElasticLog_Implementation_Api.Model;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ElasticLog_Implementation_Api.DataAccess
{
    public class EmployeeDataAccess
    {
        public Employee GetEmployee(int employeeId)
        {
            Employee emp = new Employee();
            if(employeeId >0)
            {
                emp= new Employee()
                {
                    EmployeeId="12345",
                   FullName="bdec",
                   Department="devloper"
                };
            }
            else
            {
                emp = null;
            }
            return emp;
        }
        public IEnumerable<RequestResponseLogger> ElasticLogResponseSearch(ElasticSearchInput inputData)
        {
            if(inputData.StartDate.Contains("PM"))
            {
                string startDate = inputData.StartDate.Split(" ")[1];
                //int hour = startDate.Split(":")[0];
                
            }
            DateTime startdate = Convert.ToDateTime(inputData.StartDate);
            DateTime enddate = Convert.ToDateTime(inputData.EndDate);
            string responseStr = string.Empty;
            IEnumerable<RequestResponseLogger> logList = null;
            List<RequestResponseLogger> responseLogList = new List<RequestResponseLogger>();
            try
            {
                if(inputData!=null)
                {
                    
                    if(!String.IsNullOrEmpty(inputData.ProgramCode)|| !String.IsNullOrEmpty(inputData.ApiName)|| !String.IsNullOrEmpty(inputData.StartDate)|| !String.IsNullOrEmpty(inputData.EndDate))
                    {
                        string ProgramCode = inputData.ProgramCode.ToLower();
                        string ApiName = inputData.ApiName.ToLower();
                        
                        string ElasticURL = string.Format(AppConfig.ElasticUrl+AppConfig.ResponseElasticLogIndex,ProgramCode+"/_search");
                        string request = string.Format(ElasticURL, ProgramCode.Trim());
                        //responseLogList = SearchElasticData(request);
                        HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create(request.ToLower());
                        webReq.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                        using HttpWebResponse response = (HttpWebResponse)webReq.GetResponse();
                        using Stream stream = response.GetResponseStream();
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            responseStr = reader.ReadToEnd();
                            var jsonResult = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(responseStr);
                            var jsonResultList = jsonResult.hits.hits;
                            /*var loList = from l in jsonResultList
                                         where Convert.ToDateTime(l.date) > enddate && Convert.ToDateTime(l.date) < startdate
                                         select new { RequestResponseLogger = l.RequestResponseLogger };*/
                            /*var list = jsonResultList.Where(m => m.inputData.StartDate > DateTime.Today.AddMonths(-3))
                                                                       .OrderByDescending(m => m.billDate).Take(1).ToList();*/

                            foreach (var logItems in jsonResultList)
                            {
                                responseLogList.Add(new RequestResponseLogger()
                                {
                                    Date= logItems["_source"]["date"],
                                    ProgramCode= logItems["_source"]["programCode"],
                                    Request = logItems["_source"]["request"],
                                    Response = logItems["_source"]["response"],
                                    ApiName = logItems["_source"]["apiName"]
                                });
                            }
                            logList = from l in responseLogList
                                      where Convert.ToDateTime(l.Date) < enddate && Convert.ToDateTime(l.Date) > startdate && l.ProgramCode == ProgramCode && l.ApiName==ApiName
                                      select new RequestResponseLogger()
                                      {
                                          Date = l.Date,
                                          ProgramCode = l.ProgramCode,
                                          Request = l.Request,
                                          Response = l.Response,
                                          ApiName = l.ApiName
                                      };

                        }
                    }
                }
                return logList.ToList();
            }
            catch(Exception)
            {
                throw;
            }
        }
        public List<RequestResponseLogger> SearchItemDetails(ElasticSearchInput inputData)
        {
            List<RequestResponseLogger> response = new List<RequestResponseLogger>();
            try
            {
                string request = PrepareJsonRequestForsearch2(inputData);
                if(!String.IsNullOrEmpty(request))
                {
                    string apiUrl= string.Format(AppConfig.ElasticUrl + AppConfig.ResponseElasticLogIndex, inputData.ProgramCode.ToLower().Trim() + "/_search");
                    response = GetAPIResponseRabbit(request, apiUrl);
                }
                return response;
            }
            catch(Exception)
            {
                throw;
            }
        }
        private List<RequestResponseLogger> GetAPIResponseRabbit(string requestStr, string apiUrl)
        {
            string responseStr = string.Empty;
            //string requestStr = Regex.Replace(request, @"\t|\n|\r", "");
            List<RequestResponseLogger> responseList = new List<RequestResponseLogger>();
            try
            {
                HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create(apiUrl.ToLower());
                /*byte[] bytes = Encoding.ASCII.GetBytes(requestStr);
                webReq.ContentType = "application/json; encoding='utf-8'";
                webReq.ContentLength = bytes.Length;
                webReq.Method = "POST";
                Stream requestStream = webReq.GetRequestStream();
                requestStream.Write(bytes, 0, bytes.Length);
                requestStream.Close();*/
                using (HttpWebResponse response = (HttpWebResponse)webReq.GetResponse())
                {
                    if (response.StatusCode == HttpStatusCode.Created || response.StatusCode == HttpStatusCode.OK)
                    {
                        Stream responseStream = response.GetResponseStream();
                        responseStr = new StreamReader(responseStream).ReadToEnd();
                        var jsonResult = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(responseStr);
                        foreach(var logItems in jsonResult.hits.hits)
                        {
                            responseList.Add(new RequestResponseLogger()
                            {
                                Date = logItems["_source"]["date"],
                                ProgramCode = logItems["_source"]["programCode"],
                                Request = logItems["_source"]["request"],
                                Response = logItems["_source"]["response"],
                                ApiName = logItems["_source"]["apiName"]
                            });
                        }
                    }
                }
                return responseList;

            }
            catch(Exception)
            {
                throw;
            }
        }
        private string PrepareJsonRequestForsearch2(ElasticSearchInput inputData)
        {
            string Request = string.Empty;
            try
            {
                if (inputData != null)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("{");
                    sb.Append("\"query\": {");
                    sb.Append("\"bool\": {");
                    sb.Append("\"must\": [");

                    if (!string.IsNullOrEmpty(inputData.ProgramCode))
                    {
                        sb.Append("{");
                        sb.Append("\"match_phrase\": {");
                        sb.AppendFormat("\"programCode.keyword\": \"{0}\"", inputData.ProgramCode);
                        sb.Append("}");
                        sb.Append("}");

                    }
                    if (!string.IsNullOrEmpty(inputData.ApiName))
                    {
                        sb.Append("{");
                        sb.Append("\"match_phrase\": {");
                        sb.AppendFormat("\"apiName\": \"{0}\"", inputData.ApiName);
                        sb.Append("}");
                        sb.Append("}");

                    }
                    if (!string.IsNullOrEmpty(inputData.StartDate) && !string.IsNullOrEmpty(inputData.EndDate))
                    {
                        sb.Append("{");
                        sb.Append("\"range\": {");
                        sb.Append("\"date.keyword\": {");
                        sb.AppendFormat("\"gte\": \"{0},\"", inputData.StartDate);
                        sb.AppendFormat("\"lte\": \"{0},\"", inputData.EndDate);
                        sb.AppendFormat("\"format\": \"{0},\"", "dd-MM-yyyy");
                        sb.Append("}");
                        sb.Append("}");
                        sb.Append("}");
                    }
                    sb.Append("]");
                    sb.Append("}");
                    sb.Append("}");
                    sb.Append("}");

                    Request = sb.ToString();
                }
            }
            catch (Exception)
            {
                throw;
            }
            return Request;
        }
        /*public static List<RequestResponseLogger> SearchElasticData(string request)
        {
            string responseStr = string.Empty;
            List<RequestResponseLogger> responseList = new List<RequestResponseLogger>();
            try
            {
                HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create(request.ToLower());
                webReq.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                using HttpWebResponse response = (HttpWebResponse)webReq.GetResponse();
                using Stream stream = response.GetResponseStream();
                using (StreamReader reader = new StreamReader(stream))
                {
                    responseStr = reader.ReadToEnd();
                    var jsonResultList = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(responseStr).hits.hits;
                   var list = jsonResultList.Where(m => m.billDate > DateTime.Today.AddMonths(-3))
                                                              .OrderByDescending(m => m.billDate).Take(1).ToList();

                }
            }
        }*/
    }
}

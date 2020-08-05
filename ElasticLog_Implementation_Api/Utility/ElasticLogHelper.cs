using ElasticLog_Implementation_Api.Model;
using Microsoft.Extensions.Logging;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElasticLog_Implementation_Api.Utility
{
    public  class ElasticLogHelper
    {
       public static Uri EsNode = null;
       public static ConnectionSettings EsConfig = null;
       public static ElasticClient EsClient = null;
        public static ILogger<ElasticLogHelper> _logger;
        public ElasticLogHelper(ILogger<ElasticLogHelper> logger)
        {
            _logger = logger;
        }
        private static void ElasticLogProcess(ResponseModel body)
        {
            ResponseModel responseLog=null;
            string Date = string.Empty;
            string Request = string.Empty;
            string Response = string.Empty;
            string ApiName = string.Empty;
            try
            {
                if(body!=null)
                {
                    Date = Convert.ToString(body.Date);
                    Request = Convert.ToString(body.Request);
                    Response = Convert.ToString(body.Response);
                    ApiName = Convert.ToString(body.ApiName);
                    responseLog = new ResponseModel()
                    {
                        Date=Date.ToLower(),
                        Request=Request.ToLower(),
                        Response=Response.ToLower(),
                        ApiName=ApiName.ToLower()
                    };
                    if(responseLog!=null)
                    {
                        EsNode = new Uri(AppConfig.ElasticUrl);
                        if (EsNode != null)
                        {
                            EsConfig = new ConnectionSettings(EsNode);
                            if (EsConfig != null)
                            {
                                EsClient = new ElasticClient(EsConfig);
                                if (EsClient != null)
                                {
                                    var settings = new IndexSettings { NumberOfReplicas = 1, NumberOfShards = 2 };
                                    var indexConfig = new IndexState
                                    {
                                        Settings = settings
                                    };
                                    if (!EsClient.IndexExists("elastic_log_employee").Exists)
                                    {
                                       var result= EsClient.CreateIndex("elastic_log_employee", c => c.
                                        InitializeUsing(indexConfig)
                                        .Mappings(m => m.Map<ResponseModel>(mp => mp.AutoMap())));
                                    }
                                    var ElasticResult = EsClient.Index(responseLog, i => i
                                       .Index("elastic_log_employee")
                                       .Type(typeof(ResponseModel))
                                       .Refresh(Elasticsearch.Net.Refresh.True)
                                    );
                                    if (ElasticResult.Result == Result.Created)
                                    {
                                        _logger.LogInformation(string.Format("Date: [{0}]", Date.ToLower()));
                                        _logger.LogInformation(string.Format("Request: [{0}]", Request.ToLower()));
                                        _logger.LogInformation(string.Format("Response: [{0}]", Response.ToLower()));
                                        _logger.LogInformation(string.Format("ApiName: [{0}]", ApiName.ToLower()));
                                    }

                                }
                            }
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                //throw;
                Console.Write(ex.Message);
            }
        }
        public void LogProcessing(ResponseModel body)
        {
            _logger.LogInformation("LogProcessing Started...");
            try
            {
                ElasticLogProcess(body);
            }
            catch (Exception)
            {
                //_logger.LogError("LogProcessing error while getting data  Error Message: " + ex.Message + "");
                throw;
            }
            //_logger.LogInformation("ProcessElasticLog Ended...");
        }
        /*public static void InsertDocument(ResponseModel body)
        {

          var result=  EsClient.Index(body, i => i
                    .Index("Elastic_Log_Employee")
                    .Type("myEmployee")
                    .Id("123")
                    );
        }*/
        public void ErrorLogProcess(Exception ex)
        {
            DateTime date = DateTime.Now;
            try
            {
                ElasticErrorLogProcess(new ErrorModel()
                { 
                Date= date.ToString(),
                Message =ex.Message.ToString(),
                StrackTrace=ex.StackTrace.ToString(),
                Source=ex.Source.ToString(),
                TargetSite=ex.TargetSite.ToString()
                });
            }
            catch(Exception)
            {
                throw;
            }
        }
        public static void ElasticErrorLogProcess(ErrorModel body)
        {
            ErrorModel errorLog = null;
            string Date = string.Empty;
            string Message = string.Empty;
            string StrackTrace = string.Empty;
            string Source = string.Empty;
            string TargetSite = string.Empty;
            try
            {
                if(body!=null)
                {
                    Date = Convert.ToString(body.Date);
                    Message = Convert.ToString(body.Message);
                    StrackTrace = Convert.ToString(body.StrackTrace);
                    Source = Convert.ToString(body.Source);
                    TargetSite = Convert.ToString(body.TargetSite);
                    errorLog = new ErrorModel()
                    {
                        Date=Date.ToString(),
                        Message=Message.ToLower(),
                        StrackTrace = StrackTrace.ToLower(),
                        Source = Source.ToLower(),
                        TargetSite = TargetSite.ToLower()
                    };
                    if(errorLog!=null)
                    {
                        EsNode = new Uri(AppConfig.ElasticUrl);
                        if(EsNode!=null)
                        {
                            EsConfig = new ConnectionSettings(EsNode);
                            if(EsConfig!=null)
                            {
                                EsClient = new ElasticClient(EsConfig);
                                if(EsClient!=null)
                                {
                                    var settings = new IndexSettings { NumberOfReplicas = 1, NumberOfShards = 2 };
                                    var indexConfig = new IndexState
                                    {
                                        Settings = settings
                                    };
                                    if (!EsClient.IndexExists("errorlogemployee").Exists)
                                    {
                                        EsClient.CreateIndex("errorlogemployee", c => c.
                                        InitializeUsing(indexConfig)
                                        .Mappings(m => m.Map<ErrorModel>(mp => mp.AutoMap())));
                                    }
                                    var ElasticResult = EsClient.Index(errorLog, i => i
                                       .Index("errorlogemployee")
                                       .Type(typeof(ErrorModel))
                                       .Refresh(Elasticsearch.Net.Refresh.True)
                                    );
                                    if (ElasticResult.Result == Result.Created)
                                    {
                                        _logger.LogInformation(string.Format("Date: [{0}]", Date.ToString()));
                                        _logger.LogInformation(string.Format("Message: [{0}]", Message.ToLower()));
                                        _logger.LogInformation(string.Format("StrackTrace: [{0}]", StrackTrace.ToLower()));
                                        _logger.LogInformation(string.Format("Source: [{0}]", Source.ToLower()));
                                        _logger.LogInformation(string.Format("TargetSite: [{0}]", TargetSite.ToLower()));
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                //throw;
                Console.Write(ex.Message);
            }
        }
    }
}

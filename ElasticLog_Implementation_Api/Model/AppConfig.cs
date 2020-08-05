using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElasticLog_Implementation_Api.Model
{
    public static class AppConfig
    {
        static IConfiguration appConfig = new ConfigurationBuilder()
        .AddJsonFile("appSettings.json", true, true)
        .Build();
        public static string ElasticUrl { get { return appConfig["ElasticLogger:ElasticUrl"]; } }
        public static string ResponseElasticLogIndex { get { return appConfig["ElasticLogger:ResponseElasticLogIndex"]; } }
        public static string ErrorElasticLogIndex { get { return appConfig["ElasticLogger:ErrorElasticLogIndex"]; } }
    }
}

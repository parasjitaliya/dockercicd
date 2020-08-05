using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ElasticLog_Implementation_Api.Model;
using ElasticLog_Implementation_Api.Processor;
using ElasticLog_Implementation_Api.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ElasticLog_Implementation_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly EmployeeProcessor _processor;
        private readonly ElasticLogHelper _helper;
        public EmployeeController(EmployeeProcessor processor, ElasticLogHelper helper)
        {
            _processor = processor;
            _helper = helper;
        }

        class GenericLog<T> where T : class {
            public string createdAt = DateTime.Now.ToString();
            public T data = null;
            //public string msg;
        }
        /*var errmodel = new GenericLog<resquestmodel>();
        errmodel.data=new requestmodel() { 
         uderid=898
        }*/

        [HttpPost]
        [Route("GetEmployee")]
        public IActionResult GetEmployee(int employeeId)
        {
            try
            {
                DateTime date = DateTime.Now;
                if(employeeId>0)
                {
                    Employee response = _processor.GetEmployee(employeeId);
                    
                    _helper.LogProcessing(new ResponseModel()
                    {
                        Date = date.ToString(),
                        Request = employeeId.ToString(),
                        Response = JsonConvert.SerializeObject(response).ToString().ToLower(),
                        ApiName = "GetEmployee"
                     });
                    return this.Ok(_processor.GetEmployee(employeeId));
                }
                else
                {
                    throw new Exception();
                }


            }
            catch(Exception ex)
            {
                _helper.ErrorLogProcess(ex);

                return this.BadRequest(ex.Message);
            }
        }
        [HttpPost]
        [Route("RequestResponseLog")]
        public IActionResult RequestResponseLog(ElasticSearchInput inputData)
        {
            try
            {
                IEnumerable<RequestResponseLogger> response = _processor.RequestResponseLogger(inputData);
                return this.Ok(response);
            }
            catch(Exception ex)
            {
                return this.BadRequest(ex.Message);
            }
        }
        [HttpPost]
        [Route("SearchItemDetails")]
        public IActionResult SearchItemDetails(ElasticSearchInput inputData)
        {
            try
            {
                List<RequestResponseLogger> response = _processor.SearchItemDetails(inputData);
                var jsonResponse= JsonConvert.SerializeObject(response);
                return this.Ok(jsonResponse);
            }
            catch(Exception ex)
            {
                return this.BadRequest(ex.Message);
            }
        }
    }
}

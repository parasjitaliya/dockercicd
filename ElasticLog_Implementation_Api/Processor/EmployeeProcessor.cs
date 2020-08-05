using ElasticLog_Implementation_Api.DataAccess;
using ElasticLog_Implementation_Api.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElasticLog_Implementation_Api.Processor
{
    public class EmployeeProcessor
    {
        private readonly EmployeeDataAccess empDataAccess;
        public EmployeeProcessor(EmployeeDataAccess empDAtaAccess)
        {
            empDataAccess = empDAtaAccess;
        }
        public Employee GetEmployee(int employeeId)
        {
            return empDataAccess.GetEmployee(employeeId);
        }
        public IEnumerable<RequestResponseLogger> RequestResponseLogger(ElasticSearchInput inputData)
        {
            return empDataAccess.ElasticLogResponseSearch(inputData);
        }
        public List<RequestResponseLogger> SearchItemDetails(ElasticSearchInput inputData)
        {
            return empDataAccess.SearchItemDetails(inputData);
        }
    }
}

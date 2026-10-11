namespace FC.Domain.Sales.DAO
{
    using System;
    using FC.Core;
    using FC.Core.Data;
    using FC.Domain.Sales.Entity;
   
    public class EmploymentDAO : DAO<EmploymentEntity>
    {
        public void RunTerminationProcess(PersonEntity personEntity)
        {
            try
            {
                DbExcutor.ExcuteDataTable(
                     query: $@"TerminationProcess <# {personEntity.SysKey} #>, 
                            <# {personEntity.ReasonToTerminate} #>",
                     connection: CommonConstant.SalesConnection,
                     name: "CashbookLogDataTable");
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}

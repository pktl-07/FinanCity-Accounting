namespace FC.Domain.Accounts.DAO
{
    using System;
    using System.Collections.Generic;
    using FC.Core;
    using FC.Core.Data;
    using FC.Domain.Accounts.DTO;
    using FC.Domain.Accounts.Entity;
   
    public class AccountingPeriodDAO : DAO<AccountingPeriodEntity>
    {

        #region Methods

        public IEnumerable<AccountingPeriodDTO> GetAccountingPeriodDTO(string YC)
        {
            try
            {
                var query = $@"SELECT {With<AccountingPeriodEntity>(x => x.SysKey)} {nameof(AccountingPeriodDTO.PeriodSK)},
                  {With<AccountingPeriodEntity>(x => x.YC)} {nameof(AccountingPeriodDTO.YC)}, 
                  {With<AccountingPeriodEntity>(x => x.PC)} {nameof(AccountingPeriodDTO.PC)}, 
                  {With<AccountingPeriodEntity>(x => x.Date1)} {nameof(AccountingPeriodDTO.Date1)},
                  {With<AccountingPeriodEntity>(x => x.Date2)} {nameof(AccountingPeriodDTO.Date2)},
                  SUM (CASE {With<BatchModuleEntity>(x => x.ModuleCode)} WHEN  '{CommonConstant.Module.MODULE_GENERAL_LEDGER}' THEN {With<BatchModuleEntity>(x => x.EntryStatus)} ELSE NULL END) {nameof(AccountingPeriodDTO.GLC_Status)},
                  SUM (CASE {With<BatchModuleEntity>(x => x.ModuleCode)} WHEN '{CommonConstant.Module.MODULE_SALE}' THEN {With<BatchModuleEntity>(x => x.EntryStatus)} ELSE NULL END) {nameof(AccountingPeriodDTO.SOP_Status)}, 
                  SUM (CASE {With<BatchModuleEntity>(x => x.ModuleCode)} WHEN '{CommonConstant.Module.MODULE_PURCHASE}' THEN {With<BatchModuleEntity>(x => x.EntryStatus)} ELSE NULL END) {nameof(AccountingPeriodDTO.POP_Status)},
                  SUM (CASE {With<BatchModuleEntity>(x => x.ModuleCode)} WHEN '{CommonConstant.Module.MODULE_STOCK}' THEN {With<BatchModuleEntity>(x => x.EntryStatus)} ELSE NULL END) {nameof(AccountingPeriodDTO.STK_Status)},
                  SUM (CASE {With<BatchModuleEntity>(x => x.ModuleCode)} WHEN '{CommonConstant.Module.MODULE_CASHBOOK_RECEIPT}' THEN {With<BatchModuleEntity>(x => x.EntryStatus)} ELSE NULL END) {nameof(AccountingPeriodDTO.CBR_Status)},
                  SUM (CASE {With<BatchModuleEntity>(x => x.ModuleCode)} WHEN '{CommonConstant.Module.MODULE_CASHBOOK_PAYMENT}' THEN {With<BatchModuleEntity>(x => x.EntryStatus)} ELSE NULL END) {nameof(AccountingPeriodDTO.CBP_Status)}, 
                  SUM (CASE {With<BatchModuleEntity>(x => x.ModuleCode)} WHEN '{CommonConstant.Module.MODULE_EXTRA_ONE}' THEN {With<BatchModuleEntity>(x => x.EntryStatus)} ELSE NULL END) {nameof(AccountingPeriodDTO.EXT1_Status)}, 
                  SUM (CASE {With<BatchModuleEntity>(x => x.ModuleCode)} WHEN '{CommonConstant.Module.MODULE_EXTRA_TWO}' THEN {With<BatchModuleEntity>(x => x.EntryStatus)} ELSE NULL END) {nameof(AccountingPeriodDTO.EXT2_Status)}
                  FROM {Table<AccountingPeriodEntity>()}
                  INNER JOIN {Table<BatchModuleEntity>()} ON {With<AccountingPeriodEntity>(x => x.YC)} = {With<BatchModuleEntity>(x => x.YC)} 
                  AND {With<AccountingPeriodEntity>(x => x.PC)} = {With<BatchModuleEntity>(x => x.PC)}
                  WHERE 
                  {With<AccountingPeriodEntity>(x => x.YC)} ='{YC}' AND {With<AccountingPeriodEntity>(x => x.PC)} <> '00'
                  GROUP BY {With<AccountingPeriodEntity>(x => x.SysKey)},
                  {With<AccountingPeriodEntity>(x => x.YC)},
                  {With<AccountingPeriodEntity>(x => x.PC)},
                  {With<AccountingPeriodEntity>(x => x.Date1)},
                  {With<AccountingPeriodEntity>(x => x.Date2)} 
                  ORDER BY 
                  {With<AccountingPeriodEntity>(x => x.Date1)}";

                return DbExcutor<AccountingPeriodDTO>.ExcuteReader(query);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public string LastAccountingYearCode()
        {
            try
            {
                var query = $@"SELECT ISNULL(MAX({With<AccountingPeriodEntity>(x => x.YC)}),'00') 
                FROM {Table<AccountingPeriodEntity>()}";

                return (string)DbExcutor.ExcuteScalar(query, CommonConstant.AccountConnection);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public int? LastAccountingYear()
        {
            try
            {
                var query = $@"SELECT
                MAX({With<AccountingPeriodEntity>(x => x.Date1)})
                FROM {Table<AccountingPeriodEntity>()}";

                var r = DbExcutor.ExcuteScalar(query, CommonConstant.AccountConnection).ToString();
                var s = r == "" ? null : r.Substring(0, 4);

                if (s == null)
                    return null;
                else
                    return int.Parse(s);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public string CurrentAccountingYearCode()
        {
            var query = $@" SELECT Top 1 {With<AccountingPeriodEntity>(x => x.YC)} 
                    FROM {Table<AccountingPeriodEntity>()} 
                    WHERE {With<AccountingPeriodEntity>(x => x.IsDefaultYear)}=1";

            return (string)DbExcutor.ExcuteScalar(query, CommonConstant.AccountConnection);
        }

        public int? CurrentAccountingYear()
        {
            try
            {
                var query = $@" SELECT Top 1 {With<AccountingPeriodEntity>(x => x.Date1)} 
                    FROM {Table<AccountingPeriodEntity>()} 
                    WHERE {With<AccountingPeriodEntity>(x => x.IsDefaultYear)}=1";

                var r = DbExcutor.ExcuteScalar(query, CommonConstant.AccountConnection).ToString();
                var s = r == "" ? null : r.Substring(0, 4);

                if (s == null)
                    return null;
                else
                    return int.Parse(s);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        #endregion
    }
}

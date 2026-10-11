namespace FC.Domain.Report
{
    using System;
    using System.Collections.Generic;

    public class RegisterFormulaFieldFactory : IRegisterFormulaFieldFactory
    {
        private IDictionary<string, AbstractFormulaField> _factoryInfo;

        public RegisterFormulaFieldFactory(
            PayableStatusFor payableStatusFor,
            SalespersonFor salesPersonFor,
            DateRangeFor dateRangeFor,
            ReceivableStatusFor saleStatusFor,
            TNC_PropertyFormulaFieldFor tncPropertyFor,
            TNC_TransactionFormulaFieldFor tncTransactionFor,
            TopFor topFor,
            ClientFor clientFor,
            CashbookFor cashbookFor,
            VendorFor _vendorFor,
            SalespersonTypeFor salespersonTypeFor,
            CobrokerAccuralChangeDateFor _cobrokerAccuralChangeDateFor,
            TaxFor _taxFor,
            AccountCodeFor _accountCodeFor,
            YCStartFor _ycstartFor,
            YCEndFor _ycendFor,
            TransTypeFor _transTypeFor,
            RankTypeFor _rankTypeFor,
            RecordStatusFor _recordStatusFor,
            PaidDateFor _paidDateFor,
            JobFor _jobFor,
            GLRefFor _glRefFor,
            SecondRefFor _secondRefFor,
            T1For _t1For,
            FranchiseeFor _franchiseeFor)
        {
            _factoryInfo = new Dictionary<string, AbstractFormulaField>();

            _factoryInfo.Add($"{nameof(IAccountingDateFor.AccountingDate)}", dateRangeFor);
            _factoryInfo.Add($"{nameof(ISaleDateFor.SalesDate)}", dateRangeFor);
            _factoryInfo.Add($"{nameof(ISalespersonFor.SPSK)}", salesPersonFor);
            _factoryInfo.Add($"{nameof(ICommissionReceivedStatusFor.ReceivedStatus)}", saleStatusFor);
            _factoryInfo.Add($"{nameof(ITNC_PropertyTypeFor.PropertyType)}", tncPropertyFor);
            _factoryInfo.Add($"{nameof(ITNC_TransactionTypeFor.TransactionType)}", tncTransactionFor);
            _factoryInfo.Add($"{nameof(ICommissionPaidStatusFor.PaidStatus)}", payableStatusFor);
            _factoryInfo.Add($"{nameof(IMaxTopFor.Top)}", topFor);
            _factoryInfo.Add($"{nameof(IClientFor.ClientSK)}", clientFor);
            _factoryInfo.Add($"{nameof(ICashbookFor.CBSK)}", cashbookFor);
            _factoryInfo.Add($"{nameof(ISalespersonTypeFor.SalespersonType)}", salespersonTypeFor);
            _factoryInfo.Add($"{nameof(ICobrokerAccuralChangeDateFor.CobrokerAccuralChangeDate)}", _cobrokerAccuralChangeDateFor);
            _factoryInfo.Add($"{nameof(ITaxFor.TaxSK)}", _taxFor);
            _factoryInfo.Add($"{nameof(IAccountCodeFor.AccountSK)}", _accountCodeFor);
            _factoryInfo.Add($"{nameof(IYCStartFor.YCStart)}", _ycstartFor);
            _factoryInfo.Add($"{nameof(IYCEndFor.YCEnd)}", _ycendFor);
            _factoryInfo.Add($"{nameof(ITransTypeFor.TransType)}", _transTypeFor);
            _factoryInfo.Add($"{nameof(IJobSK.JobSK)}", _jobFor);
            _factoryInfo.Add($"{nameof(IVendorFor.VendorSK)}", _vendorFor);
            _factoryInfo.Add($"{nameof(IRecordStatusFor.RecordStatus)}", _recordStatusFor);
            _factoryInfo.Add($"{nameof(IRankFor.Rank)}", _rankTypeFor);
            _factoryInfo.Add($"{nameof(IPaidDate.PaidDate)}", _paidDateFor);
            _factoryInfo.Add($"{nameof(IGLRef.GLRef)}", _glRefFor);
            _factoryInfo.Add($"{nameof(ISecondRef.SecondRef)}", _secondRefFor);
            _factoryInfo.Add($"{nameof(IT1.T1)}", _t1For);
            _factoryInfo.Add($"{nameof(IFranchiseeFor.FranchiseeSK)}", _franchiseeFor);
        }

        public AbstractFormulaField GetFactory(string key)
        {
            return _factoryInfo[key];
        }

        public void RegisterdFactory(string key, AbstractFormulaField abstractFormula)
        {
            try
            {
                _factoryInfo.Add(key, abstractFormula);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}

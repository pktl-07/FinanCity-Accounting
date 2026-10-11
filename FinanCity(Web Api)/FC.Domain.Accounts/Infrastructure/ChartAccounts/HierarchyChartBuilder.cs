namespace FC.Domain.Accounts.Business.ChartAccounts
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FC.Core;
    using FC.Domain.Accounts.DTO;
  
    public class HierarchyChartBuilder
    {
        #region Fields

        IEnumerable<ChartOfAccountDTO> _charOfAccountsDTO;
        IEnumerable<BTableExtension> _bTableEntityList;

        #endregion

        #region Ctor

        public HierarchyChartBuilder() { }

        #endregion

        #region Implements

        public void Build(IEnumerable<ChartOfAccountDTO> coaDataTable,
            IEnumerable<BTableExtension> bTableEntityList)
        {
            _charOfAccountsDTO = coaDataTable;
            _bTableEntityList = bTableEntityList;

            ConstructHeaderIndex();
        }

        public void ConstructHeaderIndex()
        {
            foreach (var list in _charOfAccountsDTO.Where(x => x.AccountType == (int)AccountType.Header))
            {
                list.Index = GetHeaderIndex(list);
            }

            ConstructPostingIndex();
        }

        public void ConstructPostingIndex()
        {
            foreach (var list in _charOfAccountsDTO.Where(x => x.AccountType == (int)AccountType.Posting))
            {
                list.Index = _charOfAccountsDTO.Where(x => int.Parse(x.AccountSK) < int.Parse(list.AccountSK) && x.AccountType == (int)AccountType.Header).Last().Index + 1;
            }

            ConstructPostingAmount();
        }

        public void ConstructPostingAmount()
        {
            foreach (var chartOfAccount in _charOfAccountsDTO.Where(x => x.AccountType == (int)AccountType.Posting))
            {
                chartOfAccount.Debit = _bTableEntityList.Where(x => x.AccSK == chartOfAccount.AccountSK).Sum(x => x.Debit.ToRoundUp());
                chartOfAccount.Credit = _bTableEntityList.Where(x => x.AccSK == chartOfAccount.AccountSK).Sum(x => x.Credit.ToRoundUp());

                if (chartOfAccount.Debit == chartOfAccount.Credit)
                {
                    chartOfAccount.Debit = 0;
                    chartOfAccount.Credit = 0;
                }
                else if (chartOfAccount.Debit > chartOfAccount.Credit)
                {
                    chartOfAccount.Debit = chartOfAccount.Debit - chartOfAccount.Credit;
                    chartOfAccount.Credit = 0;
                }
                else if (chartOfAccount.Credit > chartOfAccount.Debit)
                {
                    chartOfAccount.Credit = chartOfAccount.Credit - chartOfAccount.Debit;
                    chartOfAccount.Debit = 0;
                }
            }

            ConstructHeaderAmount();
        }

        public void ConstructHeaderAmount()
        {
            foreach (var chartOfAccount in _charOfAccountsDTO.Where(x => x.AccountType == (int)AccountType.Header))
            {
                var maxRange = chartOfAccount.AccountSK.Substring(0, 1) + "9999";
                var limitedRange = _charOfAccountsDTO.Where(x => int.Parse(x.AccountSK) > int.Parse(chartOfAccount.AccountSK)
                    && int.Parse(x.AccountSK) < int.Parse(maxRange)).OrderBy(x => x.AccountSK);

                foreach (var limited in limitedRange)
                {
                    if (int.Parse(limited.AccountSK) > int.Parse(chartOfAccount.AccountSK) &&
                        limited.AccountType == (int)AccountType.Header &&
                        limited.Index <= chartOfAccount.Index)
                    {
                        break;
                    }

                    if (limited.AccountType != (int)AccountType.Header)
                    {
                        chartOfAccount.Debit += limited.Debit;
                        chartOfAccount.Credit += limited.Credit;
                    }
                }

                if (chartOfAccount.Credit == chartOfAccount.Debit)
                {
                    chartOfAccount.Credit = 0;
                    chartOfAccount.Debit = 0;
                }
                else if (chartOfAccount.Credit > chartOfAccount.Debit)
                {
                    chartOfAccount.Credit = chartOfAccount.Credit - chartOfAccount.Debit;
                    chartOfAccount.Debit = 0;
                }
                else if (chartOfAccount.Credit < chartOfAccount.Debit)
                {
                    chartOfAccount.Debit = chartOfAccount.Debit - chartOfAccount.Credit;
                    chartOfAccount.Credit = 0;
                }
            }

            ReOrderIndexing();
        }

        public void ReOrderIndexing()
        {
            _charOfAccountsDTO.OrderBy(x => x.AccountSK);
        }

        #endregion

        #region Utilities

        private int GetHeaderIndex(ChartOfAccountDTO chartOfAccount)
        {
            if (chartOfAccount.AccountSK.EndsWith("0000"))
                return 0;

            Func<string, ChartOfAccountDTO> FilterLookUp = (setup) =>
            {
                var charaterReplace = setup.Replace("0", "");
                var length = charaterReplace.Length;
                var lookupLevel = charaterReplace.Substring(0, length - 1);

                return _charOfAccountsDTO.Where(x => x.AccountSK.Replace("0", "") == lookupLevel).SingleOrDefault();
            };

            for (int i = 4; i > 1; i--)
            {
                var lookup = FilterLookUp(chartOfAccount.AccountSK.Substring(0, i));

                if (lookup != null)
                {
                    return lookup.Index + 1;
                }
            }

            throw new Exception("");
        }

        #endregion
    }
}

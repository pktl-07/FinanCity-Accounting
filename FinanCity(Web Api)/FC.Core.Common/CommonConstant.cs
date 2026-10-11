namespace FC.Core
{
    public class CommonConstant
    {
        public class Module
        {
            public const string MODULE_GENERAL_LEDGER = "GLC";
            public const string MODULE_CASHBOOK_PAYMENT = "CBP";
            public const string MODULE_CASHBOOK_RECEIPT = "CBR";
            public const string MODULE_SALE = "SOP";
            public const string MODULE_PURCHASE = "POP";
            public const string MODULE_STOCK = "STK";
            public const string MODULE_EXTRA_ONE = "EX1";
            public const string MODULE_EXTRA_TWO = "EX2";
        }

        public class AccountType
        {
            #region AccountType

            public const string BS_START = "10000"; // BALANCE SHEET START
            public const string BS_END = "39999"; // BALANCE SHEET END
            public const string INCOME_START = "40000"; // REVENUE START
            public const string INCOME_END = "49999"; // REVENUE END
            public const string COGS_START = "50000"; // COST OF SALES START
            public const string COGS_END = "59999";// COST OF SALES END
            public const string EX_START = "60000"; // EXPENSE START
            public const string EX_END = "69999"; // EXPENSE END
            public const string OTHER_INCOME_START = "80000";
            public const string OTHER_INCOME_END = "89999";
            public const string OTHER_EXPENSE_START = "90000";
            public const string OTHER_EXPENSE_END = "99999";

            #endregion
        }

        public class DefaultIndexing
        {
            public const string R1 = "TBA";
            public const int VoucherId = 0;
            public const string Prefix = "";
            public const string Postfix = "";
        }

        public const string AccountConnection = "FC.Acc";

        public const string SalesConnection = "FC.Biz";

        public const string AuthConnection = "FC.Auth";
    }
}

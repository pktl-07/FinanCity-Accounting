namespace FC.Core
{
    using FC.Core.Infrastructure.Attributes;

    public enum SalesViewType
    {
        [EnumDescription("ALL")]
        All = 0,

        [EnumDescription("Pending")]
        Pending = 1,

        [EnumDescription("Closed")]
        Close = 2,
    }

    public enum FileType
    {
        [EnumDescription("People"), FolderName("People")]
        People = 1,

        [EnumDescription("Bank Best Rate Profile"), FolderName("BestBankRateProfile")]
        BankBestRateProfile = 4,

        [EnumDescription("Bank Best Rate"), FolderName("BankBestRate")]
        BankBestRate = 5,

        [EnumDescription("Company"), FolderName("Company")]
        Company = 10,

        [EnumDescription("Chatting"), FolderName("Chatting")]
        Chatting = 15,
    }

    public enum CashBookType
    {
        [EnumDescription("CASH")]
        Cash = 1,

        [EnumDescription("BANK")]
        Bank = 2,
    }

    public enum SalepersonStatus
    {
        [EnumDescription("LIVE")]
        Live = 1,

        [EnumDescription("DUMMY")]
        Dummy = 5,

        [EnumDescription("PENDING")]
        Pending = 10,

        [EnumDescription("IPA")]
        IPA = 15,

        [EnumDescription("RESIGNED")]
        Resignd = 20,

        [EnumDescription("LAPSED")]
        Lapsed = 25,

        [EnumDescription("WITHDRAWN")]
        Widthdraw = 30,

        [EnumDescription("REJECTED")]
        Rejected = 35,

        [EnumDescription("TERMINATED")]
        Terminated = 40,
    }

    public enum TransType
    {
        LeadFollowUp = -1,

        [Table(Name = "JUN012")]
        [EnumDescription("Lead Remark")]
        [ModuleType(ModuleType.Sales)]
        Chatting = 1,

        [Table(Name = "Company")]
        [EnumDescription("COMPANY")]
        [ModuleType(ModuleType.Sales)]
        MyCompany = 5,

        [Table(Name = "Company")]
        [EnumDescription("FRANCHISEE")]
        [ModuleType(ModuleType.Sales)]
        Franchisee = 6,

        [Table("Lead")]
        [EnumDescription("LEAD MANAGEMENT")]
        [ModuleType(ModuleType.Sales)]
        Lead = 9,

        [Table("Person")]
        [EnumDescription("CLIENT MANAGEMENT")]
        [ModuleType(ModuleType.Sales)]
        Client = 10,

        [Table(Name = "Company")]
        [EnumDescription("REA")]
        [ModuleType(ModuleType.Sales)]
        RealEstateAgent = 15,

        [Table(Name = "Company")]
        [EnumDescription("PROPERTY DEVELOPER")]
        [ModuleType(ModuleType.Sales)]
        PropertyDeveloper = 16,

        [Table(Name = "Company")]
        [EnumDescription("LAW FIRM")]
        [ModuleType(ModuleType.Sales)]
        Lawyer = 17,

        [Table(Name = "Company")]
        [EnumDescription("BANK")]
        [ModuleType(ModuleType.Sales)]
        Bank = 18,

        [Table(Name = "Company")]
        [EnumDescription("OTHERS")]
        [ModuleType(ModuleType.Sales)]
        Others = 19,

        [Table(Name = "Person")]
        [EnumDescription("EXTERNAL SP")]
        [ModuleType(ModuleType.Sales)]
        [TaxDescription(Name = "Sales Commission – External Co-broke")]
        ExternalSalesPerson = 20, // EXTERNAL SALESPERSON

        //[Table("Company")]
        //[EnumDescription("REA")]
        //[ModuleType(ModuleType.Sales)]
        //RealEstateAgent = 15,

        //[Table("Company")]
        //[EnumDescription("PROPERTY DEVELOPER")]
        //[ModuleType(ModuleType.Sales)]
        //PropertyDeveloper = 16,

        //[Table("Company")]
        //[EnumDescription("LAW FIRM")]
        //[ModuleType(ModuleType.Sales)]
        //Lawyer = 17,

        //[Table("Company")]
        //[EnumDescription("BANK")]
        //[ModuleType(ModuleType.Sales)]
        //Bank = 18,

        //[Table("Company")]
        //[EnumDescription("OTHERS")]
        //[ModuleType(ModuleType.Sales)]
        //Others = 19,

        //[Table("Person")]
        //[EnumDescription("EXTERNAL VENDOR")]
        //[ModuleType(ModuleType.Sales)]
        //ExternalSalesPerson = 20,

        [Table("Person")]
        [EnumDescription("INTERNAL SP")]
        [ModuleType(ModuleType.Sales)]
        InternalSalesperson = 25,

        [Table("Person")]
        [EnumDescription("MAIN SP")]
        [ModuleType(ModuleType.Sales)]
        MainSalesperson = 30,

        [Table(Name = "CareerManagement")]
        [EnumDescription("CHANGE OF STATUS")]
        [ModuleType(ModuleType.Sales)]
        ChangeOfStatus = 31,

        [Table(Name = "CareerManagement")]
        [EnumDescription("BRANCH OUT")]
        [ModuleType(ModuleType.Sales)]
        BranchOut = 32,

        [Table("AMS100")]
        [EnumDescription("PROPERTY TRANSACTION")]
        [ModuleType(ModuleType.Sales)]
        PropertyTransaction = 35,

        [Table(Name = "AMS100")]
        [EnumDescription("REFERRAL SALES ORDER")]
        [ModuleType(ModuleType.Sales)]
        ReferralSalesOrder = 36, // Referral Sales Order

        [Table(Name = "AMS102")]
        [EnumDescription("SALES INVOICE")]
        [ModuleType(ModuleType.Sales)]
        [TaxDescription(Name = "SALES COMMISSION INVOICE")]
        Invoice = 45, // INVOICE

        [Table(Name = "AMS107")]
        [EnumDescription("CREDIT NOTE")]
        [ModuleType(ModuleType.Sales)]
        [TaxDescription(Name = "CREDIT NOTE")]
        CreditNote = 49, // COMMISSION ALLOCATION DISCOUNT

        [Table(Name = "AMS107")]
        [EnumDescription("DEBIT NOTE")]
        [ModuleType(ModuleType.Sales)]
        [TaxDescription(Name = "DEBIT NOTE")]
        DebitNote = 50,

        [Table("AMS107")]
        [EnumDescription("COMMISSION ALLOCATION")]
        [ModuleType(ModuleType.Sales)]
        [TaxDescription(Name = "COMMISSION ALLOCATED")]
        AllocationCommission = 60, // COMMISSION ALLOCATION

        [Table("AMS107")]
        [EnumDescription("MARKETING EXPENSE ALLOCATION")]
        [ModuleType(ModuleType.Sales)]
        AllocationMarketingExpense = 61, // COMMISSION ALLOCATION

        [Table(Name = "AMS103")]
        [EnumDescription("COMMISSION RECEIVABLE")]
        [ModuleType(ModuleType.Sales)]
        [TaxDescription(Name = "COMMISSION RECEIVED")]
        CommissionReceivable = 65, // COMMISSION RECEIVABLE

        [Table(Name = "AMS106")]
        [EnumDescription("COMMISSION PAYABLE")]
        [ModuleType(ModuleType.Sales)]
        [TaxDescription(Name = "COMMISSION PAID")]
        CommissionPayable = 70, // COMMISSION PAYABLE

        [EnumDescription("OVERRIDING BONUS PAYABLE")]
        [Table(Name = "AMS108")]
        [ModuleType(ModuleType.Sales)]
        [TaxDescription(Name = "OVERRIDING BONUS PAID")]
        OverridingBonusPayable = 75,

        [ModuleType(ModuleType.GL)]
        [EnumDescription("GENERAL LEDGER")]
        Journal = 200,

        [ModuleType(ModuleType.GL)]
        [EnumDescription("GENERAL LEDGER")]
        CashbookSetup = 201,
      
        [Table(Name = "CBM002")]
        [EnumDescription("CASHBOOK RECEIPT")]
        [ModuleType(ModuleType.CB)]
        [TaxDescription(Name = "CASHBOOK RECEIPT")]
        CashBookReceived = 205,

        [Table(Name = "CBM002")]
        [EnumDescription("CASHBOOK PAYMENT")]
        [ModuleType(ModuleType.CB)]
        CashBookPayment = 210,
       
        [Table(Name = "CBM004")]
        [EnumDescription("CASHBOOK TRANSFER")]
        [ModuleType(ModuleType.CB)]
        [TaxDescription(Name = "Cashbook Transfer")]
        CashBookTransfer = 220,

        [Table(Name = "Department")]
        [EnumDescription("Department")]
        [ModuleType(ModuleType.GL)]
        Department = 255,

        [Table(Name = "JOB")]
        [EnumDescription("JOB")]
        [ModuleType(ModuleType.GL)]
        Job = 225,
    }
    
    public enum ActiveViewType
    {
        [EnumDescription("All")]
        All = 0,

        [EnumDescription("YES")]
        Active = 10,

        [EnumDescription("NO")]
        InActive = 20,
    }

    public enum ModuleType
    {
        [EnumDescription("Default")]
        Default = 1,

        [EnumDescription("Setup")]
        Setup = 2,

        [EnumDescription("SiteOwner")]
        SiteOwner = 3,

        [EnumDescription("Sales")]
        Sales = 10,

        [EnumDescription("SPO")]
        SPO = 20,

        [EnumDescription("PMS")]
        PMS = 30,

        [EnumDescription("GL")]
        GL = 40,

        [EnumDescription("CB")]
        CB = 50,

        [EnumDescription("POS")]
        POS = 60,

        [EnumDescription("SOM")]
        SOM = 65,

        [EnumDescription("AdminReport")]
        AdminReport = 70,

        [EnumDescription("PMSReport")]
        PMSReport = 75,

        [EnumDescription("HRMS")]
        HRMS = 80
    }

    public enum IndexingFormat
    {
        [EnumDescription("Manual")]
        Manual = 15,

        [EnumDescription("Build In")]
        BuildIn = 10,

        [EnumDescription("yy:mm:####")]
        BaseOnCurrentDate = 20,
    }

    public enum TransactionType
    {
        [EnumDescription("SALES")]
        Sales = 5,

        [EnumDescription("LEASE")]
        Lease = 10,

        [EnumDescription("ASSIGNMENT")]
        Assignment = 15
    }

    public enum Gender
    {
        [EnumDescription("Male")]
        Male = 0,

        [EnumDescription("Female")]
        Female = 1
    }

    public enum MartialStatus
    {
        [EnumDescription("Single")]
        Single,

        [EnumDescription("Married")]
        Married
    }

    public enum AccountType
    {
        [EnumDescription("Posting")]
        Posting = 1,

        [EnumDescription("Header")]
        Header = 2,
    }

    public enum TaxType
    {
        [EnumDescription("Common Module Base")]
        CommonModuleBase = 1,

        [EnumDescription("Supply")]
        Supply = 2,

        [EnumDescription("Purchase")]
        Purchase = 3,
    }

    public enum TaxCategory
    {
        [EnumDescription("Standard")]
        Standard = 1,

        [EnumDescription("Zero")]
        Zero = 2,

        [EnumDescription("Exempted")]
        Exempted = 3,

        [EnumDescription("NA")]
        NA = 4,

        [EnumDescription("Not Clamiable")]
        NotClamiable = 5
    }

    public enum PaymentMode
    {
        [EnumDescription("CASH")]
        Cash = 15,

        [EnumDescription("CHEQUE")]
        CHEQUE = 1,

        [EnumDescription("NETS")]
        NETS = 5,

        [EnumDescription("TT")]
        TT = 10,
    }

    public enum MailingType
    {
        [EnumDescription("Mail As Vendor")]
        MailAsVendor = 5,

        [EnumDescription("Mail As Property")]
        MailAsProperty = 10,

        [EnumDescription("Mail As Others")]
        MailAsOthers = 15,
    }

    public enum RankType
    {
        [EnumDescription("Salesperson")]
        SP = 1,

        [EnumDescription("Team Manager")]
        TM = 5,

        [EnumDescription("Division Director")]
        DD = 10,
    }

    public enum PaymentType
    {
        [EnumDescription("Partial")]
        Partial = 1,

        [EnumDescription("Full")]
        Full = 2,
    }

    public enum LoggerType
    {
        [EnumDescription("Promotion Scheme Change")]
        PromotionSchemeChange = 1,

        [EnumDescription("OpeningAccSales Change")]
        OpeningAccSalesChange = 2,

        [EnumDescription("Tire Change")]
        TireChange = 3,

        [EnumDescription("Rank Change")]
        RankChange = 6,

        [EnumDescription("Department Chanage")]
        DepartmentChanage = 4,

        [EnumDescription("EmploymentStatus Change")]
        EmploymentStatusChange = 5,
    }

    public enum UserAccountType
    {
        AdminAccount = 1,
        SalespersonAccount = 2
    }

    public enum CommissonStructureType
    {
        [EnumDescription("Company Leads")]
        CompanyLeads = 1,

        [EnumDescription("Personal Leads")]
        PersonalLeads = 2,

        [EnumDescription("RES Leads")]
        RESLeads = 3,

        [EnumDescription("Tier")]
        Tier = 4,
    }

    public enum EntryStatus
    {
        [EnumDescription("OPEN")]
        Open = 1,

        [EnumDescription("CLOSE")]
        Close = 2,

        [EnumDescription("CLOSE")]
        UnChangeClose = 4,
    }

    public enum PeasExportOption
    {
        [EnumDescription("pdf")]
        Pdf,

        [EnumDescription("excel")]
        Excel,
    }

    public enum Size
    {
        Small,
        Medium,
    }
}

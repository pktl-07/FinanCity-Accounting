namespace FC.Core
{
    public enum RecordStatus
    {
        [EnumDescription("Delete Draft")]
        DeleteDraft = -5,

        [EnumDescription("Update Draft")]
        UpdateDraft = -4,

        [EnumDescription("Save Draft")]
        SaveDraft = -3,

        [EnumDescription("Reject")]
        Reject = -2,

        [EnumDescription("Submit")]
        Submit = -1,
           
        /// <summary>
        /// CustomizeUpdateBuilder and CustomizeInsertBuilder for logic use.
        /// Sometime it also needs to set the record status new.
        /// </summary>
        [EnumDescription("None")]
        None = 0,

        [EnumDescription("New")]
        New = 1,

        [EnumDescription("Save")]
        Save = 2,

        [EnumDescription("Update")]
        Update = 3,

        [EnumDescription("Delete")]
        Delete = 4,

        [EnumDescription("Archive")]
        Archive = 5,

        [EnumDescription("Void")]
        Void = 6,

        [EnumDescription("Post")]
        Post = 8,

        [EnumDescription("Reversal")]
        Reversal = 125,

        [EnumDescription("Reversed")]
        Reversed = 126,

        //[Description("Allocate")]
        //Allocate = 127,
    }

    public enum ActionType
    {
        New = 0,
        Save = 1,
        Update = 2,
        Delete = 3,
        Void = 4,
        Post = 8,
        Allocate = 128
    }

    public enum DbProvider
    {
        [EnumDescription("MSSQL")]
        MSSQL,

        [EnumDescription("MySQL")]
        MySQL,
    }
    
    public enum TimeFormat
    {
        [EnumDescription("None")]
        None,

        [EnumDescription("Date")]
        Date,

        [EnumDescription("Time")]
        Time
    }

    public enum LetterCasing
    {
        Normal = 0,
        Upper = 1,
        Lower = 2,
    }

    public enum StateManager
    {
        SessionState,
        Cookie,
    }

    public enum SoftwareLicense
    {
        FreeTrial = 1,
        License = 5,
    }

    public enum ApplicationType
    {
        [EnumDescription("Real Estate")]
        MSC_RealEstate = 1,

        [EnumDescription("Insurance")]
        MSC_Insurance = 2,
    }

    public enum CultureFormat
    {
        [EnumDescription("")]
        None,

        [EnumDescription("eCurrency:2:'$':false:true")]
        Currency,
        
        [EnumDescription("eDate:'dd/mm/yyyy'")]
        DMY,

        [EnumDescription("eDate:'yyyy/mm/dd'")]
        YMD,

        [EnumDescription("eDate:'mm/dd/yyyy'")]
        MDY,

        [EnumDescription("eBoolToHtml")]
        Boolean,
    }
}

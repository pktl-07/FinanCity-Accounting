namespace FC.Core.Data
{
    using FC.Core.Attributes;

    public enum OrderType
    {
        [EnumDescription("DESC ")]
        DESC,

        [EnumDescription("ASC ")]
        ASC,
    }

    public enum JoinType
    {
        [EnumDescription("INNER JOIN")]
        INNNER_JOIN,

        [EnumDescription("LEFT JOIN")]
        LEFT_JOIN,

        [EnumDescription("RIGHT JOIN")]
        RIGHT_JOIN,

        [EnumDescription("CROSS JOIN")]
        CROSS_JOIN
    }

    public enum QueryType
    {
        [EnumDescription(" SELECT")]
        Select,

        [EnumDescription("INSERT INTO ")]
        Insert,

        [EnumDescription("UPDATE ")]
        Update,

        [EnumDescription("DELETE FROM ")]
        Delete,
    }

    public enum AggregrateType
    {
        [EnumDescription("AVG")]
        Avg,

        [EnumDescription("COUNT")]
        Count,

        [EnumDescription("MIN")]
        Min,

        [EnumDescription("MAX")]
        Max,

        [EnumDescription("SUM")]
        Sum,
    }
}

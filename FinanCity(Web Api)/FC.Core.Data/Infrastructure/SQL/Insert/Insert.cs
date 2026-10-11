namespace FC.Core.Data.SQL
{
    using System.Linq.Expressions;
    using FC.Core.Data.Infrastructure;
    using FC.Core.Data.Infrastructure.Implementation;
    using FC.Core.Data.Extensions;

    public class Insert : QueryComponentBase 
    {
        public delegate void FetchQuery(string query);
        public event FetchQuery OnFetchQuery;

        public Insert(QueryType type = QueryType.Insert) : base(type) { }

        public override void WriteSelector<T>(Expression expr)
        {
            var fieldCollection = (new WriterExpressionVisitor<T>(expr) as IExpressionSelector).FieldCollection;

            fieldCollection.InsertValidate();
            Field += "(";

            fieldCollection.Each(field =>
            {
                Field += field.Split('=')[0].Trim().AddChar(',');
            });

            Field = Field.RemoveLastChar(',');
            Field += ") VALUES (";

            fieldCollection.Each(field =>
            {
                var split = field.Split('=');
                for (int i = 0; i < split.Length; i++)
                {
                    if (i == 1)
                    {
                        Field += split[1].Trim();
                    }
                    else if (i > 1)
                    {
                        Field += "=" + split[i];
                    }
                    if (i == split.Length - 1)
                    {
                        Field += "".AddChar(',');
                    }
                }
            });

            Field = Field.RemoveLastChar(',');
            Field += ")";
        }

        public override string ToString()
        {
            var query = QueryContext.ToString() + " " + Table + Field;

             if (OnFetchQuery != null)
                OnFetchQuery(query);

            return query;
        }
    }
}

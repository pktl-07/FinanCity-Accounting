namespace FC.Core
{
    public abstract class AbstractConverter<Model,Value>
        where Model : class
        where Value : class
    {
        public abstract Model Convert(Value value);

        public abstract Value ConvertBack(Model value);
    }
}

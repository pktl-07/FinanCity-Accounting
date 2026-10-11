namespace FC.Core
{
    /// <summary>
    /// Helper interface which may require to assist when convert to and from model vs data entity.
    /// </summary>
    public interface IModelConverter
    {
        /// <summary>
        /// To convert model
        /// </summary>
        /// <param name="data">entity which usually come from database object</param>
        /// <returns>Api model</returns>
        object Convert(object data);

        /// <summary>
        /// To convert to entity
        /// </summary>
        /// <param name="data">Api model</param>
        /// <returns>Data entity</returns>
        object ConvertBack(object data);
    }
}

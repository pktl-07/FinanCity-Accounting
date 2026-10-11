namespace FC.Core
{
    using FC.Core.Data;
    using System.Collections.Generic;

    public interface IMultiCurrencyConverter
    {
        /// <summary>
        /// To base currency.
        /// </summary>
        /// <param name="value">value</param>
        /// <param name="rate">rate</param>
        /// <param name="op">operator</param>
        /// <returns>Base currency</returns>
        decimal ToBaseCurrency(decimal value, decimal rate, Operator op);

        /// <summary>
        /// To multi-currency.
        /// </summary>
        /// <param name="value">value</param>
        /// <param name="rate">rate</param>
        /// <param name="op">operator</param>
        /// <returns>Currency</returns>
        decimal ToMultiCurrency(decimal value, decimal rate, Operator op);

        /// <summary>
        /// To base currency.
        /// </summary>
        /// <typeparam name="T">IEntity</typeparam>
        /// <param name="entity">source</param>
        /// <returns>Currency</returns>
        T ToMultiCurrency<T>(T entity) where T : IEntity;

        /// <summary>
        /// To multi-currency.
        /// </summary>
        /// <typeparam name="T">IEntity</typeparam>
        /// <param name="entities">source</param>
        /// <returns>Currency</returns>
        IEnumerable<T> ToMultiCurrency<T>(IEnumerable<T> entities) where T : IEntity;

        /// <summary>
        /// To multi-currency.
        /// </summary>
        /// <typeparam name="T">IEntity</typeparam>
        /// <param name="entity">source</param>
        /// <returns>Currency</returns>
        T ToBaseCurrency<T>(T entity) where T : IEntity;

        /// <summary>
        /// To base-currency.
        /// </summary>
        /// <typeparam name="T">IEntity</typeparam>
        /// <param name="entities">source</param>
        /// <returns>Currency</returns>
        IEnumerable<T> ToBaseCurrency<T>(IEnumerable<T> entities) where T : IEntity;

        /// <summary>
        /// To multi-currency.
        /// </summary>
        /// <typeparam name="T">IEntity</typeparam>
        /// <param name="entities">source</param>
        /// <returns>Currency</returns>
        IDataPagerResult<T> ToMultiCurrency<T>(IDataPagerResult<T> entity) where T : IEntity;

        /// <summary>
        /// To currency format.
        /// </summary>
        /// <typeparam name="T">IEntity</typeparam>
        /// <param name="entity">source</param>
        /// <returns>Currency</returns>
        T ToCurrency<T>(T entity) where T : IEntity;
    }
}

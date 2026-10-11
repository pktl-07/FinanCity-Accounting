namespace FC.Core.Infrastructure.Implements
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using FC.Core;
    using FC.Core.Data;
    using FC.Core.Infrastructure;
    
    public class MultiCurrencyConverter : IMultiCurrencyConverter
    {
        #region Methods

        public decimal ToBaseCurrency(decimal value, decimal rate, Operator op)
        {
            try
            {
                if (op == Operator.Multiple)
                    return (value * rate).ToRoundUp();

                return (value / rate).ToRoundUp();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public IEnumerable<T> ToBaseCurrency<T>(IEnumerable<T> entities) where T : IEntity
        {
            entities.Each(entity =>
            {
                entity = ToBaseCurrency(entity);
            });

            return entities;
        }

        public decimal ToMultiCurrency(decimal value, decimal rate, Operator op)
        {
            try
            {
                if (op == Operator.Multiple)
                    return (value / rate).ToRoundUp();

                return (value * rate).ToRoundUp();
            }
            catch (Exception ex)
            {

                throw ex; 
            }
        }

        public T ToMultiCurrency<T>(T entity) where T : IEntity
        {
            try
            {
                FetchAttributeData(entity, (prop, currency) =>
                {
                    object a = prop.GetValue(entity);
                    if (a != null)
                    {
                        var value = ToMultiCurrency((decimal)a,
                            currency.CURRATE,
                            currency.CURROP == "*" ? Operator.Multiple : Operator.Divider);

                        prop.SetValue(entity, value);
                    }
                });

                return entity;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public T ToBaseCurrency<T>(T entity) where T : IEntity
        {
            try
            {
                Guard.IsNotNull(entity, "entity");

                FetchAttributeData(entity, (prop, currency) =>
                {
                    object a = prop.GetValue(entity);
                    if (a != null)
                    {
                        var value = ToBaseCurrency((decimal)a,
                            currency.CURRATE,
                            currency.CURROP == "*" ? Operator.Multiple : Operator.Divider);

                        prop.SetValue(entity, value);
                    }
                });

                return entity;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public IEnumerable<T> ToMultiCurrency<T>(IEnumerable<T> entities) where T : IEntity
        {
            try
            {
                entities.Each(entity =>
                {
                   entity = ToMultiCurrency(entity);
                });

                return entities;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public IDataPagerResult<T> ToMultiCurrency<T>(IDataPagerResult<T> pager) where T : IEntity
        {
            try
            {
                var data = ToMultiCurrency(pager.Data).ToList();
                pager.Data = data;

                return pager;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public T ToCurrency<T>(T entity) where T : IEntity
        {
            entity.GetType().GetProperties().Each(prop =>
            {
                prop.GetCustomAttributes(typeof(MultiCurrencyAttribute), false)
                .Each(attr =>
                {
                    object a = prop.GetValue(entity);
                    if (a != null)
                    {
                        var value = (decimal)a;
                        prop.SetValue(entity, value.ToRoundUp());
                    }
                });
            });

            return entity;
        }

        #endregion

        #region Utilities

        private void FetchAttributeData<T>(T entity, Action<PropertyInfo, IMultiCurrency> action)
        {
            try
            {
                if (entity is IMultiCurrency)
                {
                    var currency = (entity as IMultiCurrency);
                    entity.GetType().GetProperties().Each(prop =>
                    {
                        prop.GetCustomAttributes(typeof(MultiCurrencyAttribute), false)
                        .Each(attr =>
                        {
                            if (action != null)
                            {
                                action(prop, currency);
                            }
                        });
                    });
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        #endregion
    }
}

public enum Operator
{
    Multiple,
    Divider,
}

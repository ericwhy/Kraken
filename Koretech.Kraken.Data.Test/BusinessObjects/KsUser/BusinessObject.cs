using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Koretech.Kraken.Data.Test.BusinessObjects.KsUser
{
    internal class BusinessObject<TEntity, TBusinessObject>
        where TEntity : class
        where TBusinessObject : BusinessObject<TEntity, TBusinessObject>, new()
    {
        public static List<TBusinessObject> NewInstance(List<TEntity> entities)
        {
            List<TBusinessObject> businessObjects = new();

            foreach (TEntity entity in entities)
            {
                TBusinessObject newBusinessObject = new()
                {
                    _entity = entity
                };
                businessObjects.Add(newBusinessObject);
            }

            return businessObjects;
        }

        protected TEntity? _entity;

        protected BusinessObject()
        {
            _entity = default;
        }
    }
}

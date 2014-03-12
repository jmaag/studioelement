using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SquareHook.Membership.Models
{
    public class ObjectIdBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var result = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            return result == null || String.IsNullOrEmpty(result.AttemptedValue) ? MongoDB.Bson.ObjectId.Empty : new MongoDB.Bson.ObjectId(result.AttemptedValue);
        }
    }
}
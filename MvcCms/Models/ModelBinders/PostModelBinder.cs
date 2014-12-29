
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace MvcCms.Models.ModelBinders
{
    public class PostModelBinder : DefaultModelBinder
    {
        protected override object GetPropertyValue(ControllerContext controllerContext, 
            ModelBindingContext bindingContext, 
            System.ComponentModel.PropertyDescriptor propertyDescriptor,
            IModelBinder propertyBinder)
        {
            if (propertyDescriptor.Name != "Tags")
            {
                return base.GetPropertyValue(controllerContext, bindingContext, propertyDescriptor, propertyBinder);
            }

            var tags = bindingContext.ValueProvider.GetValue("Tags").AttemptedValue;

            if (string.IsNullOrWhiteSpace(tags))
            {
                return new List<String>();
            }

            return tags.Split(new[] {','}).Select(t => t.Trim()).ToList();
        }
    }
}

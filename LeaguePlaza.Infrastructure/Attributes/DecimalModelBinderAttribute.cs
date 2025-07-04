using LeaguePlaza.Infrastructure.ModelBinders;
using Microsoft.AspNetCore.Mvc;

namespace LeaguePlaza.Infrastructure.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DecimalModelBinderAttribute : ModelBinderAttribute
    {
        public DecimalModelBinderAttribute()
        {
            BinderType = typeof(DecimalModelBinder);
        }
    }
}

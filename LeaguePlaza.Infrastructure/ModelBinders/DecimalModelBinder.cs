using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Globalization;

namespace LeaguePlaza.Infrastructure.ModelBinders
{
    public class DecimalModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

            if (valueProviderResult != ValueProviderResult.None)
            {
                var value = valueProviderResult.FirstValue;

                if (decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
                {
                    bindingContext.Result = ModelBindingResult.Success(result);
                }
                else
                {
                    bindingContext.ModelState.TryAddModelError(bindingContext.ModelName, "Something went wrong.");
                }
            }

            return Task.CompletedTask;
        }
    }
}

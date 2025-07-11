using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using System.Globalization;

using static LeaguePlaza.Common.Constants.ErrorConstants;

namespace LeaguePlaza.Infrastructure.ModelBinders
{
    public class DecimalModelBinder(ILogger<DecimalModelBinder> logger) : IModelBinder
    {
        private readonly ILogger<DecimalModelBinder> _logger = logger;

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
                    bindingContext.ModelState.TryAddModelError(bindingContext.ModelName, GenericErrorMessage);

                    _logger.LogError(FailedAt, nameof(BindModelAsync));
                    _logger.LogError(DecimalModelBinderErrorMessage, value);
                }
            }

            return Task.CompletedTask;
        }
    }
}

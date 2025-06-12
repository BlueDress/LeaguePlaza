using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace LeaguePlaza.Web.TagHelpers
{
    [HtmlTargetElement("a", Attributes = "asp-controller, asp-action")]
    public class ActiveNavigationLinkTagHelper : TagHelper
    {
        public const string ActiveClass = "active";

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; } = null!;
        public string AspController { get; set; } = null!;
        public string AspAction { get; set; } = null!;

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var currentController = ViewContext.RouteData.Values["controller"]?.ToString() ?? string.Empty;
            var currentAction = ViewContext.RouteData.Values["action"]?.ToString() ?? string.Empty;

            if (string.Equals(currentController, AspController, StringComparison.OrdinalIgnoreCase) && string.Equals(currentAction, AspAction, StringComparison.OrdinalIgnoreCase))
            {
                var existingClass = output.Attributes["class"]?.Value?.ToString();
                output.Attributes.SetAttribute("class", $"{existingClass} {ActiveClass}");
            }
        }
    }
}
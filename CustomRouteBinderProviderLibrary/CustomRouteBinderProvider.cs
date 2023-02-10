using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CustomRouteBinderProviderLibrary
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public class FromRouteUnsafeAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public class FromRouteRawAttribute : Attribute { }

    public class CustomRouteBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context.Metadata is not DefaultModelMetadata metadata) { return null; }
            var paramaterAttributes = metadata.Attributes.ParameterAttributes;

            if (metadata.Attributes.ParameterAttributes.Any(pa => pa.GetType() == typeof(FromRouteUnsafe)))
            {
                return new BinderTypeModelBinder(typeof(RouteUnsafeBinder));
            }

            if (metadata.Attributes.ParameterAttributes.Any(pa => pa.GetType() == typeof(FromRouteRaw)))
            {
                return new BinderTypeModelBinder(typeof(RouteRawBinder));
            }

            return null;
        }
    }

    public static class ModelBindingContextHelper
    {
        public static string GetRawRouteValue(this ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            var modelName = bindingContext.ModelName;

            // If the default route data couldn't find it, this either can not or should not
            if (!bindingContext.ActionContext.RouteData.Values.Keys.Contains(modelName))
            {
                throw new NotSupportedException();
            }

            // Wrap in curly braces
            var templateToMatch = $"{{{modelName}}}";

            var request = bindingContext.HttpContext.Request;
            var template = bindingContext.ActionContext.ActionDescriptor.AttributeRouteInfo.Template;

            // Get true raw
            var rawTarget = bindingContext.HttpContext.Features.Get<IHttpRequestFeature>().RawTarget;
            // Parse through Uri to strip query string
            var path = new Uri($"{request.Scheme}://{request.Host}{rawTarget}").AbsolutePath;

            // Go through route template and find which segment by index
            var index = template
                .Split('/', StringSplitOptions.RemoveEmptyEntries)
                .Select((segment, index) => new { segment, index })
                .SingleOrDefault(iter => iter.segment.Equals(templateToMatch, StringComparison.OrdinalIgnoreCase))
                ?.index;

            var segments = path.ToString().Split('/', StringSplitOptions.RemoveEmptyEntries);

            if (index.HasValue)
            {
                var rawUrlSegment = segments[index.Value];
                return rawUrlSegment;
            }

            throw new NotSupportedException();
        }
    }

    public class RouteRawBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var rawRouteValue = bindingContext.GetRawRouteValue();
            bindingContext.Result = ModelBindingResult.Success(rawRouteValue);

            return Task.CompletedTask;
        }
    }

    public class RouteUnsafeBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var rawRouteValue = bindingContext.GetRawRouteValue();
            var decoded = Uri.UnescapeDataString(rawRouteValue);
            bindingContext.Result = ModelBindingResult.Success(decoded);

            return Task.CompletedTask;
        }
    }
}

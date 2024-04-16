using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Payment.Api.Filter
{
    public class AddRequiredIdempotencyKeyAsHeader : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            operation.Parameters ??= [];

            if (context.ApiDescription.HttpMethod != HttpMethod.Post.ToString())
            {
                return;
            }

            operation.Parameters.Add(new OpenApiParameter
            {
                Name = "IdempotencyKey",
                In = ParameterLocation.Header,
                Required = true,
            });
        }
    }
}

using System;

namespace CleanShop.Api.Helpers.Errors;

public class ApiValidation : ApiResponse
{
    public ApiValidation() : base(400)
    {
    }
    public IEnumerable<string> Errors { get; set; } = Array.Empty<string>();


}

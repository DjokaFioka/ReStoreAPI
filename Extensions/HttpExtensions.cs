using ReStoreAPI.Entities;
using ReStoreAPI.RequestHelpers;
using System.Text.Json;

namespace ReStoreAPI.Extensions
{
    public static class HttpExtensions
    {
        public static void AddPaginationHeader(this HttpResponse response, MetaData metaData)
        {
            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

            response.Headers.Append("Pagination", JsonSerializer.Serialize(metaData, options)); //Append instead of Add
            response.Headers.Append("Access-Control-Expose-Headers", "Pagination"); 
        }
    }
}

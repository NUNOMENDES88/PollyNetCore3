using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Polly.Api.Extensions
{
    public static class JsonExtensions
    {
        public static string SerializeObj<T>(this T requestObject, bool ignoreCamelCase = false)
        {
            if (requestObject == null)
            {
                return string.Empty;
            }

            if (requestObject is string)
            {
                return requestObject as string;
            }

            return ignoreCamelCase
             ? JsonSerializer.Serialize(requestObject)
             : JsonSerializer.Serialize(requestObject,
             new JsonSerializerOptions
             {
                 PropertyNamingPolicy = JsonNamingPolicy.CamelCase
             });
        }

        public static T DeserializeObj<T>(this string responseString, bool ignoreCamelCase = false)
        {
            if (responseString == null)
            {
                return default;
            }

            if (typeof(T) == typeof(string))
            {
                return (T)(object)responseString;
            }

            return ignoreCamelCase
            ? JsonSerializer.Deserialize<T>(responseString)
            : JsonSerializer.Deserialize<T>(responseString,
            new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
        }
    }
}

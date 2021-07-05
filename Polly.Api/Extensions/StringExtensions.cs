using System.Net.Http;
using System.Text;

namespace Polly.Api.Extensions
{
    public static class StringExtensions
    {
        public static StringContent ToStringContent(this string input)
        {
            return new StringContent(input, Encoding.UTF8, "application/json");
        }
    }
}

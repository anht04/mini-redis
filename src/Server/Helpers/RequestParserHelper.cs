using Common;

namespace MiniRedis.Helpers
{
    public class RequestParserHelper
    {
        public static List<string> Parse(string request)
        {
            string[] args = [.. request.Split(RedisConstants.CRLF, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)];

            return Parse(args);
        }

        private static List<string> Parse(string[] args)
        {
            if (args.Length < 3)
            {
                return [];
            }

            var result = new List<string>();
            for (int i = 2; i < args.Length; i++)
            {
                result.Add(args[i]);
            }

            return result;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;

namespace Doc.Compression.DataUri
{
    public class MediaType
    {
        public MediaType(string type, string subType, Dictionary<string, string> parameters)
        {
            Type = type;
            SubType = subType;
            Parameters = parameters;
        }

        public string Type { get; }
        public string SubType { get; }
        public Dictionary<string, string> Parameters { get; }

        public static bool TryParse(string value, out MediaType mediaType)
        {
            mediaType = null;
            if (DataUriRegex.MediaType.IsMatch(value))
            {
                var splits = value.Split(';');

                // Get type and subtype
                var types = GetSplit(splits[0], '/');
                // get params
                var parameters = splits.Where(split => split.Contains("=")).ToDictionary(split => GetSplit(split, '=').Item1, split => GetSplit(split, '=').Item2);

                mediaType = new MediaType(types.Item1, types.Item2, parameters);
                return true;
            }

            return false;
        }

        public static MediaType Parse(string value)
        {
            var success = TryParse(value, out MediaType result);
            if(success)
            {
                return result;
            }
            throw new ArgumentException($"string {value} is not in the proper format of a media type");
        }

        private static Tuple<string, string> GetSplit(string value, char separator)
        {
            var split = value.Split(separator);
            return new Tuple<string, string>(split[0], split[1]);
        }

        public override string ToString()
        {
            string baseString = $"{Type}/{SubType}";
            if (Parameters?.Count > 0)
            {
                foreach(var pair in Parameters)
                {
                    baseString = $"{baseString};{pair.Key}={pair.Value}";
                }
            }

            return baseString;
        }
    }
}

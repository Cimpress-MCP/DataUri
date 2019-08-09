using System;
using System.Collections.Generic;
using System.Linq;

namespace Cimpress.DataUri
{
    public class MediaType
    {
        public MediaType(string type, Dictionary<string, string> parameters)
        {
            MimeType = type;
            Parameters = parameters;
        }

        public string MimeType { get; }
        public Dictionary<string, string> Parameters { get; }

        public static bool TryParse(string value, out MediaType mediaType)
        {
            mediaType = null;
            if (DataUriRegex.MediaType.IsMatch(value))
            {
                var splits = value.Split(';');

                // get params
                var parameters = splits.Where(split => split.Contains("=")).ToDictionary(split => GetSplit(split, '=').Item1, split => GetSplit(split, '=').Item2);

                mediaType = new MediaType(splits[0], parameters);
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
            string baseString = MimeType;
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

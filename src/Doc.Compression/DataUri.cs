using System;

namespace Doc.Compression
{
    public class DataUri
    {
        public MediaType MediaType { get; }
        public string Data { get; }
        public bool Base64 { get; }

        public DataUri(MediaType mediaType, string data, bool base64)
        {
            MediaType = mediaType;
            Data = data;
            Base64 = base64;
        }

        public static DataUri Parse(string dataUri)
        {
            bool valid = TryParse(dataUri, out DataUri data);
            if(valid)
            {
                return data;
            }
            throw new ArgumentException($"data uri {dataUri} is not a valid uri");
        }

        public static bool TryParse(string dataUri, out DataUri dataObj)
        {
            dataObj = null;

            if(DataUriRegex.DataUri.IsMatch(dataUri))
            {
                bool base64 = dataUri.Contains(";base64,");
                string data = dataUri.Split(',')[1];
                string mediaString = DataUriRegex.CaptureMediaType.Match(dataUri)?.Groups["mime"]?.Value;
                if (!string.IsNullOrEmpty(mediaString) && MediaType.TryParse(mediaString, out MediaType mediaType))
                {
                    dataObj = new DataUri(mediaType, data, base64);
                    return true;
                }
            }

            return false;
        }
    }
}

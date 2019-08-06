using System.Text.RegularExpressions;

namespace Doc.Compression.DataUri
{
    public static class DataUriRegex
    {
        /// <summary>
        /// Regex to test if a string is in the correct format of a data uri
        /// </summary>
        public static readonly Regex DataUri = new Regex(@"data:([a-zA-Z0-9-_\+]+\/[a-zA-Z0-9-_\+]+)+(;[a-zA-Z0-9-_\+]+=[a-zA-Z0-9-_\+]+)*(;base64)*,.*", RegexOptions.Compiled);

        /// <summary>
        /// Regex to fine the media type of a dataUri. Look in match.Groups["mime"].Value
        /// </summary>
        public static readonly Regex CaptureMediaType = new Regex(@"data:(?<mime>(([a-zA-Z0-9-_\+]+\/[a-zA-Z0-9-_\+]+)+(;[a-zA-Z0-9-_\+]+=[a-zA-Z0-9-_\+]+)*))+.*", RegexOptions.Compiled);

        /// <summary>
        /// Regex to tell if a string is a valid media type string
        /// </summary>
        public static readonly Regex MediaType = new Regex(@"([a-zA-Z0-9-_\+]+\/[a-zA-Z0-9-_\+]+)+(;[a-zA-Z0-9-_\+]+=[a-zA-Z0-9-_\+]+)*", RegexOptions.Compiled);
    }
}

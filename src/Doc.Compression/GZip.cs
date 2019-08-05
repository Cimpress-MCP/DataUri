using Newtonsoft.Json;
using System;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace Doc.Compression
{
    public static class GZip
    {
        /// <summary>
        /// Decodes the input payload and handles deflating / base64 decoding if nessecary.
        /// </summary>
        public static T Decode<T>(byte[] input)
        {
            if (TryDecoding(input, out T deflatedPayload))
            {
                return deflatedPayload;
            }
            throw new ArgumentException("Input is not a GZipped");
        }

        /// <summary>
        /// Encodes a payload into a deflated string.
        /// </summary>
        public static string Encode(object payload)
        {
            string json = JsonConvert.SerializeObject(payload);

            using (var inputStream = new MemoryStream(Encoding.UTF8.GetBytes(json)))
            using (var outputStream = new MemoryStream())
            using (var zipStream = new GZipStream(outputStream, CompressionMode.Compress))
            {
                inputStream.CopyTo(zipStream);
                zipStream.Close();

                return Convert.ToBase64String(outputStream.ToArray());
            }
        }

        // Tries to deflate an input byte array into a given payload
        public static bool TryDecoding<T>(byte[] input, out T output)
        {
            try
            {
                using (var inputStream = new MemoryStream(input))
                using (var zipStream = new GZipStream(inputStream, CompressionMode.Decompress))
                using (var outputStream = new MemoryStream())
                {
                    zipStream.CopyTo(outputStream);

                    string deflatedOutput = Encoding.UTF8.GetString(outputStream.ToArray());

                    if (string.IsNullOrEmpty(deflatedOutput))
                    {
                        output = default;
                        return false;
                    }

                    try
                    {
                        output = JsonConvert.DeserializeObject<T>(deflatedOutput);
                        return output != null;
                    }
                    catch
                    {
                        output = default;
                        return false;
                    }
                }
            }
            catch
            {
                output = default;
                return false;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StompHelper
{
    public class StompMessageSerializer
    {
        /// <summary>
        ///   Serializes the specified message.
        /// </summary>
        /// <param name = "message">The message.</param>
        /// <returns>A serialized version of the given <see cref="StompMessage"/></returns>
        public string Serialize(StompMessage message)
        {
            var buffer = new StringBuilder();

            buffer.Append(message.Command + "\n");

            if (message.Headers != null)
            {
                foreach (var header in message.Headers)
                {
                    buffer.Append(header.Key + ":" + header.Value + "\n");
                }
            }

            buffer.Append("\n");
            buffer.Append(message.Body);
            buffer.Append('\0');

            return buffer.ToString();
        }

        /// <summary>
        /// Deserializes the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>A <see cref="StompMessage"/> instance</returns>
        public StompMessage Deserialize(string message)
        {
            var reader = new StringReader(message);

            var command = reader.ReadLine();

            var headers = new Dictionary<string, string>();

            var header = reader.ReadLine();
            while (!string.IsNullOrEmpty(header))
            {
                var split = header.Split(':');
                if (split.Length == 2) headers[split[0].Trim()] = split[1].Trim();
                header = reader.ReadLine() ?? string.Empty;
            }

            var body = reader.ReadToEnd() ?? string.Empty;
            body = body.TrimEnd('\r', '\n', '\0');

            return new StompMessage(command, body, headers);
        }
    }
}

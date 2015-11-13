using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StompHelper
{
    public class StompMessage
    {
        private readonly Dictionary<string, string> _headers = new Dictionary<string, string>();

        /// <summary>
        ///   Initializes a new instance of the <see cref = "StompMessage" /> class.
        /// </summary>
        /// <param name = "command">The command.</param>
        public StompMessage(string command)
            : this(command, string.Empty)
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref = "StompMessage" /> class.
        /// </summary>
        /// <param name = "command">The command.</param>
        /// <param name = "body">The body.</param>
        public StompMessage(string command, string body)
            : this(command, body, new Dictionary<string, string>())
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref = "StompMessage" /> class.
        /// </summary>
        /// <param name = "command">The command.</param>
        /// <param name = "body">The body.</param>
        /// <param name = "headers">The headers.</param>
        internal StompMessage(string command, string body, Dictionary<string, string> headers)
        {
            Command = command;
            Body = body;
            _headers = headers;

            this["content-length"] = body.Length.ToString();
        }

        public Dictionary<string, string> Headers
        {
            get { return _headers; }
        }

        /// <summary>
        /// Gets the body.
        /// </summary>
        public string Body { get; private set; }

        /// <summary>
        /// Gets the command.
        /// </summary>
        public string Command { get; private set; }

        /// <summary>
        /// Gets or sets the specified header attribute.
        /// </summary>
        public string this[string header]
        {
            get { return _headers.ContainsKey(header) ? _headers[header] : string.Empty; }
            set { _headers[header] = value; }
        }
    }
}

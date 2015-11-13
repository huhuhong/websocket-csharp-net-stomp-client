using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using StompHelper;
//using System.Net.WebSockets;
using WebSocketSharp;

namespace SampleHardwareServiceWithStomp
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var ws = new WebSocket("ws://localhost:8080/spring-websocket-stomp-apollo/chat/websocket"))
            {
                ws.OnMessage += ws_OnMessage;
                ws.OnOpen += ws_OnOpen;
                ws.OnError += ws_OnError;
                ws.Connect();
                Thread.Sleep(1000);

                StompMessageSerializer serializer = new StompMessageSerializer();

                var connect = new StompMessage("CONNECT");
                connect["accept-version"] = "1.2";
                connect["host"] = "";
                ws.Send(serializer.Serialize(connect));

                var clientId = RandomString(5);
                Console.WriteLine("Client Id :" + clientId);
                Thread.Sleep(1000);

                var sub = new StompMessage("SUBSCRIBE");
                sub["id"] = "sub-0";
                sub["destination"] = "/topic/broadcast";
                ws.Send(serializer.Serialize(sub));

                var sub1 = new StompMessage("SUBSCRIBE");
                sub1["id"] = "sub-1";
                sub1["destination"] = "/queue/message-" + clientId;
                ws.Send(serializer.Serialize(sub1));

                Thread.Sleep(1000);
                var content = new Content(){ Subject ="Stomp client", Message = "Hello World!!"};
                var broad = new StompMessage("SEND", JsonConvert.SerializeObject(content));
                broad["content-type"] = "application/json";
                broad["destination"] = "/app/broadcast";
                ws.Send(serializer.Serialize(broad));

                Console.ReadKey(true);
            }
            
        }

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        static void ws_OnOpen(object sender, EventArgs e)
        {
            Console.WriteLine(DateTime.Now.ToString() + " ws_OnOpen says: " + e.ToString());
        }

        static void ws_OnMessage(object sender, MessageEventArgs e)
        {
            Console.WriteLine("-----------------------------");
            Console.WriteLine(DateTime.Now.ToString() + " ws_OnMessage says: " + e.Data);

        }

        static void ws_OnError(object sender, ErrorEventArgs e)
        {
            Console.WriteLine(DateTime.Now.ToString() + " ws_OnError says: " + e.Message);
        }

        public class Content
        {
            public string Subject { get; set; }
            public string Message { get; set; }
        }
    }
}

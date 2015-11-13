using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using StompHelper;
using WebSocketSharp;

namespace SampleHardwareServiceApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        WebSocket ws = new WebSocket("ws://localhost:8080/spring-websocket-stomp-apollo/chat/websocket");
        StompMessageSerializer serializer = new StompMessageSerializer();
        String clientId = string.Empty;

        private void button1_Click(object sender, EventArgs e)
        {
            ws.OnMessage += ws_OnMessage;
            ws.OnClose += ws_OnClose;
            ws.OnOpen += ws_OnOpen;
            ws.OnError += ws_OnError;
            ws.Connect();

            this.clientId = RandomString(5);
            this.label3.Text = this.clientId;
        }
        private void ConnectStomp()
        {
            var connect = new StompMessage(StompFrame.CONNECT);
            connect["accept-version"] = "1.2";
            connect["host"] = "";
            // first number Zero mean client not able to send Heartbeat, 
            //Second number mean Server will sending heartbeat to client instead
            connect["heart-beat"] = "0,10000";
            ws.Send(serializer.Serialize(connect));
        }

        private void SubscribeStomp()
        {
            var sub = new StompMessage(StompFrame.SUBSCRIBE);
            //unique Key per subscription
            sub["id"] = "sub-0"; 
            sub["destination"] = "/topic/broadcast";
            ws.Send(serializer.Serialize(sub));

            var sub1 = new StompMessage(StompFrame.SUBSCRIBE);
            //unique Key per subscription
            sub1["id"] = "sub-1";
            sub1["destination"] = "/queue/message-" + clientId;
            ws.Send(serializer.Serialize(sub1));
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var content = new Content() { Subject = "Stomp client", Message = "Hello World!!" };
            var broad = new StompMessage(StompFrame.SEND, JsonConvert.SerializeObject(content));
            broad["content-type"] = "application/json";
            broad["destination"] = "/app/broadcast";
            ws.Send(serializer.Serialize(broad));
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var content = new Content() { Subject = "Stomp client", Message = "Hello World!!" };
            var broad = new StompMessage(StompFrame.SEND, JsonConvert.SerializeObject(content));
            broad["content-type"] = "application/json";
            broad["destination"] = "/queue/message-" + txtTarget.Text;
            ws.Send(serializer.Serialize(broad));
        }

        void ws_OnOpen(object sender, EventArgs e)
        {
            UpdateListBox(" ws_OnOpen says: " + e.ToString());
            ConnectStomp();
        }

        void ws_OnMessage(object sender, MessageEventArgs e)
        {
            Console.WriteLine("-----------------------------");
            Console.WriteLine(" ws_OnMessage says: " + e.Data);
            StompMessage msg = serializer.Deserialize(e.Data);
            if (msg.Command == StompFrame.CONNECTED)
            {
                UpdateListBox(e.Data);
                SubscribeStomp();
            }
            else if (msg.Command == StompFrame.MESSAGE)
            {
                UpdateListBox(e.Data);
            }
        }

        void ws_OnClose(object sender, CloseEventArgs e)
        {
            UpdateListBox(" ws_OnClose says: " + e.ToString());
            ConnectStomp();
        }


        void UpdateListBox(string data)
        {
            if (this.listBox1.InvokeRequired)
            {
                this.listBox1.Invoke(new MethodInvoker(delegate { UpdateListBox(DateTime.Now.ToString()  + data); }));
            }
            else
                this.listBox1.Items.Add(data);
        }

        void ws_OnError(object sender, ErrorEventArgs e)
        {
            UpdateListBox(DateTime.Now.ToString() + " ws_OnError says: " + e.ToString());
        }

        public class Content
        {
            public string Subject { get; set; }
            public string Message { get; set; }
        }
        public string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}

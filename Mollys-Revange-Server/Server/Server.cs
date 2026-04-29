using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Runtime.Serialization.Formatters.Binary;
using Connection;

namespace Server
{
    public class Server
    {
        private TcpListener listener;
        private string serverIp;
        private int serverPort;
        private Dictionary<string, TcpClient> clients;
        private Dictionary<string, Thread> clientsThreads;
        private string[] items = { "bread" , "water_glass", "donat" };
        private List<Entity> entities;

        public Server(string ip, int port) {

            this.serverIp = ip;
            this.serverPort = port;
            this.clients = new Dictionary<string, TcpClient>();
            this.clientsThreads = new Dictionary<string, Thread>();
            this.entities = new List<Entity>();
        }

        public void AddFood(int x, int y, int fresh, string name) {

            Entity item = new Entity(x, y, fresh, name);
            entities.Add(item);
        }

        public void AddAmmo(int x, int y, string name) {

            Entity item = new Entity(x, y, name);
            entities.Add(item);
        }

        public void LoadItems() {

            AddFood(765, 255 - 30, 30, "bread");
            AddFood(85, 765, 30, "bread");
            AddFood(1900, 1005 - 50, 30, "donat");
            AddFood(150, 1300 - 50, 30, "water_glass");
            AddAmmo(450, 250, "ammo");
            for (int i = 0; i < 10; i++)
            {
                AddAmmo(680 + (i * 85), 1195, "ammo");
            }
        }

        public void SendItem(Entity item) {

            try
            {

                while (true)
                {
                    foreach (KeyValuePair<string, TcpClient> otherCLient in clients)
                    {
                        NetworkStream netStream = otherCLient.Value.GetStream();

                        EntityData fd = new EntityData(item.GetFresh(), item.GetXPos(), item.GetYPos(), 0, item.GetName());
                        byte[] bytes = ObjectToByteArray(fd);
                        netStream.Write(bytes, 0, bytes.Length);

                        Thread.Sleep(1000);
                    }
                    break;
                }
            }
            catch (Exception)
            {

            }
        }

        public void Start() {

            try
            {
                listener = new TcpListener(IPAddress.Any, serverPort);
                listener.Start();

                Console.WriteLine("Server started on ip '" + serverIp + "' and port '" + serverPort + ".");
                LoadItems();

                new Thread(WaitForConnections).Start();
                new Thread(Chat).Start();


            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }

        public void WaitForConnections() {

            while (true)
            {

                Console.WriteLine("Waiting for connections..");
                TcpClient client = listener.AcceptTcpClient();

                string clientIp = ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString();

                if (!clients.ContainsKey(clientIp))
                {
                    clients.Add(clientIp, client);
                    Console.WriteLine(clientIp + " has connected to the server.");


                    clientsThreads.Add(clientIp, new Thread(() => Recieve(clientIp, clients[clientIp])));
                    clientsThreads[clientIp].IsBackground = true;
                    clientsThreads[clientIp].Start();

                    Thread t = new Thread(() => Update(client));
                    t.Start();
                }
                else
                {
                    Console.WriteLine("You are already connected to the server!");
                }

                


                Thread.Sleep(10);
            }

        }

        public void Update(TcpClient client) {
            int i = 0;
            while (i < entities.Count) //Send all entities to the player when join server.
            {
                SendObjectTo(entities[i], client);
                SendObjectTo(entities[i], client);
                SendObjectTo(entities[i], client);
                SendObjectTo(entities[i], client);
                Thread.Sleep(1000);
                i++;
            }
        }

        public void Recieve(string clientIp, TcpClient client) {

            while (true)
            {
                try
                {
                    NetworkStream netStream = client.GetStream();
                    byte[] bytes = new byte[1024];
                    netStream.Read(bytes, 0, bytes.Length);

                    Object obj = ByteArrayToObject(bytes);

                    if (obj is PlayerData) {

                        PlayerData pd = obj as PlayerData;
                        pd.SetHealth(pd.GetHealth());
                        pd.SetRotation(pd.GetRotation());
                        pd.SetXPos(pd.GetXPos());
                        pd.SetYPos(pd.GetYPos());
                        pd.SetXSpeed(pd.GetXSpeed());
                        pd.SetYSpeed(pd.GetYSpeed());
                        pd.SetCanShoot(pd.GetCanShoot());
                        
                        //Console.WriteLine(pd.ToString());
                    }

                    SendAll(bytes, clientIp, client);
                }
                catch (Exception e)
                {
                    clients.Remove(clientIp);
                    clientsThreads.Remove(clientIp);
                    client.Close();
                    Console.WriteLine(clientIp + " has disconnected.");
                    break;
                }

                Thread.Sleep(10);
            }
        }

        public void Chat() {

            while (true)
            {
                string text = Console.ReadLine();
                string message = "";
                if (!text.Equals(""))
                {
                    if (text[0] == '/')
                    {
                        if (text.Equals("/clients"))
                            if (clients.Count == 1)
                                message = "<Server>: There is " + clients.Count + " client.";
                            else
                                message = "<Server>: There are " + clients.Count + " clients.";

                        Console.WriteLine("<Server>:" + message);

                        if (text.IndexOf("/add " + items[0]) != -1 || text.IndexOf("/add " + items[1])  != -1 || text.IndexOf("/add " + items[2]) != -1)
                        {
                            try
                            {
                                string item = "";
                                for (int i = 0; i < items.Length; i++)
                                {
                                    if (text.IndexOf(items[i]) != -1)
                                    {
                                        item = items[i];
                                        break;
                                    }
                                }

                                string x = "";
                                int xIndex = text.IndexOf('[') + 1;
                                int xLastIndex = text.IndexOf(',') - 1;
                                for (int i = xIndex; i <= xLastIndex; i++)
                                    x += text[i];

                                string y = "";
                                int yIndex = text.IndexOf(',') + 1;
                                int yLastIndex = text.IndexOf(']') - 1;
                                for (int i = yIndex; i <= yLastIndex; i++)
                                    y += text[i];

                                Console.WriteLine("x =" + x);
                                Console.WriteLine("y =" + y);

                                EntityData ed = new EntityData(25, float.Parse(x), float.Parse(y), 0, item);
                                Entity en = new Entity(ed.GetXPos(), ed.GetYPos(), ed.GetFresh(), ed.GetName());
                                entities.Add(en);
                                foreach(KeyValuePair<string, TcpClient> otherClient in clients)
                                {
                                    NetworkStream netStream = otherClient.Value.GetStream();
                                    byte[] bytes = ObjectToByteArray(ed);
                                    netStream.Write(bytes, 0, bytes.Length);
                                }
                            }
                            catch (Exception)
                            {
                                Console.WriteLine("There is not command like that.");
                            }
                        }
                       
                    }
                    
                }
            }
        }

        public void SendObjectTo(Object obj, TcpClient client) {

            try
            {
                if (obj is Entity)
                {
                    Entity e = obj as Entity;

                    EntityData newed = new EntityData(e.GetFresh(), e.GetXPos(), e.GetYPos(), 0, e.GetName());

                    NetworkStream netStream = client.GetStream();
                    byte[] bytes = ObjectToByteArray(newed);
                    netStream.Write(bytes, 0, bytes.Length);
                }
            }
            catch (Exception e)
            {

            }
        }

        public void SendAll(byte[] recievedBytes, string clientIp, TcpClient client) {

            foreach (KeyValuePair<string, TcpClient> otherClient in clients)
            {
                try
                {
                    string otherClientIp = ((IPEndPoint)otherClient.Value.Client.RemoteEndPoint).Address.ToString();

                    if (!otherClientIp.Equals(clientIp))
                    {
                        NetworkStream newStream = otherClient.Value.GetStream();
                        newStream.Write(recievedBytes, 0, recievedBytes.Length);
                    }
                }
                catch (Exception e)
                {
                    clients.Remove(clientIp);
                    clientsThreads.Remove(clientIp);
                    client.Close();
                    Console.WriteLine(clientIp + " has disconnected.");
                    break;
                }
            }
        }

        public void SendTo(TcpClient client, string message) {

            NetworkStream netStream = client.GetStream();
            byte[] bytes = Encoding.ASCII.GetBytes(message);
            netStream.Write(bytes, 0, bytes.Length);

        }

        public byte[] ObjectToByteArray(Object obj) {

            if (obj == null)
                return null;

            MemoryStream stream = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, obj);
            byte[] arr = stream.ToArray();
            stream.Close();
            return arr;
        }

        public object ByteArrayToObject(byte[] Buffer) {

            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream stream = new MemoryStream(Buffer);
            object obj = null;
            try
            {
                obj = formatter.Deserialize(stream);
            }
            catch
            {
                obj = null;
            }
            stream.Close();
            return obj;
        }
    }
}

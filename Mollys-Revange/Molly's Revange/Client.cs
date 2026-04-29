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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Molly_s_Revange
{
    public class Client
    {
        private TcpClient server;
        private string ip;
        private int port;

        public Client(string ip, int port) {

            this.ip = ip;
            this.port = port;
            server = new TcpClient(ip, port);
        }

        public void DisconnectFromServer(Dictionary<string, Player> players) {

            server.Close();
            players.Clear();
            Game1.IsMainMenu = true;
        }

        public void RecievePlayerData(Dictionary<string, Player> players, List<Entity> items ,ContentManager content) {

            while (true)
            {
                try
                {
                    NetworkStream netStream = server.GetStream();
                    byte[] bytes = new byte[1024];
                    netStream.Read(bytes, 0, bytes.Length);
                    Object obj = ByteArrayToObject(bytes);
                    if(obj is PlayerData)
                    {
                        PlayerData pd = obj as PlayerData;
                        string ip = pd.GetIp();

                        if (players.ContainsKey(ip))
                        {
                            lock (players)
                            {
                                players[ip].SetPosition(new Vector2(pd.GetXPos(), pd.GetYPos()));
                                players[ip].SetSpeed(new Vector2(pd.GetXSpeed(), pd.GetYSpeed()));
                                players[ip].SetHealth(pd.GetHealth());
                                players[ip].Rotation = pd.GetRotation();
                                players[ip].SetIp(pd.GetIp());
                                players[ip].SetCanShoot(pd.GetCanShoot());
                                players[ip].SetDirection(new Vector2(pd.GetXDirection(), pd.GetYDirection()));

                                players[ip].SetRectangle(new Rectangle((int)pd.GetXPos(), (int)pd.GetYPos(), players[ip].GetRectangle().Width, players[ip].GetRectangle().Height));

                                if (!players[ip].GetCanShoot())
                                {
                                    players[ip].Shoot();
                                    //players[ip].AddRocket(players[ip].GetPosition(), players[ip].GetDirection(), players[ip].Rotation);
                                }

                            }
                        }
                        else
                        {
                            Player newPlayer = new Player(new Vector2(pd.GetXPos(), pd.GetYPos()), pd.GetHealth());
                            newPlayer.SetSpeed(new Vector2(pd.GetXSpeed(), pd.GetYSpeed()));
                            newPlayer.Rotation = pd.GetRotation();
                            newPlayer.Content = content;
                            newPlayer.LoadContent();
                            newPlayer.SetRectangle(new Rectangle((int)pd.GetXPos(), (int)pd.GetYPos(), newPlayer.GetRectangle().Width, newPlayer.GetRectangle().Height));
                            players.Add(ip, newPlayer);
                        }
                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);

                    DisconnectFromServer(players);
                    break;
                }

                Thread.Sleep(10);
            }
        }

        public void RecieveEntityData(Dictionary<string, Player> players, List<Entity> items, ContentManager content) {

            while (true)
            {
                try
                {
                    NetworkStream netStream = server.GetStream();
                    byte[] bytes = new byte[1024];
                    netStream.Read(bytes, 0, bytes.Length);
                    Object obj = ByteArrayToObject(bytes);

                   
                    if (obj is EntityData)
                    {
                        EntityData ed = obj as EntityData;

                        if(ed.GetName().Equals("bread") || ed.GetName().Equals("donat") || ed.GetName().Equals("water_glass")){
                            Food food = new Food(new Vector2(ed.GetXPos(), ed.GetYPos()), ed.GetFresh(), ed.GetName());
                            food.Content = content;
                            food.LoadContent();
                            items.Add(food);
                        }
                        else if (ed.GetName().Equals("ammo")){
                            Ammo ammo = new Ammo(new Vector2(ed.GetXPos(), ed.GetYPos()), ed.GetFresh(), ed.GetName());
                            ammo.Content = content;
                            ammo.LoadContent();
                            items.Add(ammo);
                        }
                        
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);

                    DisconnectFromServer(players);
                    break;
                }

                Thread.Sleep(10);
            }
        }

        public void SendToServer(Player player, Dictionary<string, Player> players) {

            while (true)
            {
                try
                {
                    NetworkStream netStream = server.GetStream();

                    PlayerData pd = new PlayerData(player.GetHealth(), player.GetPosition().X, player.GetPosition().Y, player.Rotation, player.GetSpeed().X, player.GetSpeed().Y,
                        player.GetIp(), player.GetCanShoot(), player.GetDirection().X, player.GetDirection().Y);

                    byte[] bytes = ObjectToByteArray(pd);
                    netStream.Write(bytes, 0, bytes.Length);

                }
                catch (Exception e)
                {
                    DisconnectFromServer(players);
                    break;
                }

                Thread.Sleep(10);
            }
        }

        public byte[] ObjectToByteArray(Object obj) {
            if (obj == null)
                return null;

            MemoryStream fs = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(fs, obj);
            byte[] rval = fs.ToArray();
            fs.Close();
            return rval;
        }

        public object ByteArrayToObject(byte[] Buffer) {
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream stream = new MemoryStream(Buffer);
            object rval = null;
            try
            {
                rval = formatter.Deserialize(stream);
            }
            catch
            {
                rval = null;
            }
            stream.Close();
            return rval;
        }

    }
}

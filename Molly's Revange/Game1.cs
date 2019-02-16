using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Media;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Molly_s_Revange
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    { 
        public enum Menu {Page};
        public enum Page { Coop, Multiplayer, Options, Exit};

        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private MainMenu mainMenu;
        private Map map;
        private Player player;
        private Camera camera;
        private Random rnd = new Random();
        private SpriteFont font;
        private Song backgroundMusic;
        private BackgroundEntity background, background2;//moly;
        private List<BackgroundEntity> backgroundObjects;
        private List<Meteor> meteors;
        private List<Entity> items;
        private BlackHole blackHole1, blackHole2;
        private int time;
        public static bool IsMainMenu = true;
        public static string gameMode = "co-op";

        private SoundEffect meteorHitSound, gunPickSound;

        private Thread blocksCollisionThread, rocketsCollisionBlocksThread;


        private Client client;
        private Dictionary<string, Player> players;


        public Game1() {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            graphics.IsFullScreen = false;
        }

        protected override void Initialize() {

            base.Initialize();
            IsMouseVisible = true;
        }

        protected override void LoadContent() {

            spriteBatch = new SpriteBatch(GraphicsDevice);

            mainMenu = new MainMenu(graphics, Content);
            mainMenu.LoadContent();

            meteors = new List<Meteor>();
            meteorHitSound = Content.Load<SoundEffect>("music/sounds/meteor_hit");
            gunPickSound = Content.Load<SoundEffect>("music/sounds/gun_pick");
            backgroundMusic = Content.Load<Song>("music/background_music");
            MediaPlayer.Play(backgroundMusic);
            MediaPlayer.Volume = 0.40f;
            MediaPlayer.IsRepeating = true;

            background = new BackgroundEntity(new Vector2(0, 0), Content.Load<Texture2D>("images/stars/stars_night"));
            background2 = new BackgroundEntity(new Vector2(0, 1600), Content.Load<Texture2D>("images/stars/stars_night"));
            //moly = new BackgroundEntity(new Rectangle(500, 700, 100, 150), Content.Load<Texture2D>("images/moly"));
            font = Content.Load<SpriteFont>("fonts/Font");
            camera = new Camera(GraphicsDevice.Viewport);
            map = new Map(Content);

            player = new Player(new Vector2(200, 11 * 50), 100);
            player.Content = Content;
            player.LoadContent();

            backgroundObjects = new List<BackgroundEntity>();
            backgroundObjects.Add(new BackgroundEntity(new Vector2(1300, 400), Content.Load<Texture2D>("images/stars/giant_star")));
            backgroundObjects.Add(new BackgroundEntity(new Vector2(1300, 1400), Content.Load<Texture2D>("images/stars/earth")));

            items = new List<Entity>();

            blackHole1 = new BlackHole(new Vector2(200, 1300));
            blackHole1.Content = Content;
            blackHole1.LoadContent();

            blackHole2 = new BlackHole(new Vector2(1800, 1700));
            blackHole2.Content = Content;
            blackHole2.LoadContent();

            blocksCollisionThread = new Thread(() => CheckBlocksCollision(map.GetBlocks()));
            blocksCollisionThread.IsBackground = true;
            blocksCollisionThread.Start();

            rocketsCollisionBlocksThread = new Thread(() => CheckRocketsBlocksCollision(map.GetBlocks()));
            rocketsCollisionBlocksThread.IsBackground = true;
            rocketsCollisionBlocksThread.Start();

            players = new Dictionary<string, Player>();


        }

        public void LoadDefaultItems() {
            AddAmmo(450, 250, "ammo");
            for (int i = 0; i < 10; i++)
            {
                AddAmmo(680 + (i * 85), 1195, "ammo");
            }
            AddFood(765, 255 - 30, "bread");
            AddFood(85, 765, "bread");
            AddFood(1900, 1005 - 50, "donat");
            AddFood(150, 1300 - 50, "water_glass");
        }

        public void ConnectToServer() {
            try
            {
                client = new Client("192.168.1.17", 4466);

                Thread sendToServerThread = new Thread(() => client.SendToServer(player, players));
                sendToServerThread.IsBackground = true;
                sendToServerThread.Start();

                Thread recieveFromServerThread = new Thread(() => client.RecieveEntityData(players, items, Content));
                recieveFromServerThread.IsBackground = true;
                recieveFromServerThread.Start();

                Thread recieveFromServerThread2 = new Thread(() => client.RecievePlayerData(players, items, Content));
                recieveFromServerThread2.IsBackground = true;
                recieveFromServerThread2.Start();

                Thread playersRocketCollisionBlocksThread = new Thread(() => CheckPlayersRocketsBlocksCollision(map.GetBlocks()));
                playersRocketCollisionBlocksThread.IsBackground = true;
                playersRocketCollisionBlocksThread.Start();

                Thread playerCollisionPlayersThread = new Thread(CheckPlayerPlayersCollision);
                playerCollisionPlayersThread.IsBackground = true;
                playerCollisionPlayersThread.Start();

                gameMode = "multiplayer";
                IsMainMenu = false;

            }
            catch (Exception e)
            {
                IsMainMenu = true;
            }
        }

        public static string GetLocalIPAddress() {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("Local IP Address Not Found!");
        }

        public void CheckBlocksCollision(List<Block> blocks) {

            while (true)
            {
                if (!player.IsCollisionBlocks(blocks))
                    player.IsCanMove = true;
                else
                {
                    player.IsCanMove = false;
                    player.SetSpeed(new Vector2(0, 0));
                }
            }
        }

        public void CheckPlayerPlayersCollision() {

            foreach(KeyValuePair<string,Player> otherPlayer in players) {

                if (!player.IsCollisionPlayer(otherPlayer.Value))
                {
                    player.IsCanMove = true;
                }
                else
                {
                    player.IsCanMove = false;
                    player.SetSpeed(new Vector2(0, 0));
                }
            }
        }

        public void CheckRocketsBlocksCollision(List<Block> blocks) {

            while (true)
            {
                for (int j = 0; j < map.GetBlocks().Count; j++)
                {
                    for (int i = 0; i < player.GetRockets().Count; i++)
                    {
                        if (player.GetRockets()[i] != null)
                        {
                            if (player.GetRockets()[i].IsCollisionBlock(map.GetBlocks()[j]))
                            {
                                player.GetRockets().RemoveAt(i);
                            }
                        }
                    }
                }
            }
            
        }

        public void CheckPlayersRocketsBlocksCollision(List<Block> blocks) {

            while (true)
            {
                for (int j = 0; j < map.GetBlocks().Count; j++)
                {
                    if (players.Count > 0)
                    {
                        foreach (KeyValuePair<string, Player> p in players)
                        {
                            for (int i = 0; i < p.Value.GetRockets().Count; i++)
                            {
                                if (p.Value.GetRockets()[i] != null)
                                {
                                    if (p.Value.GetRockets()[i].IsCollisionBlock(map.GetBlocks()[j]))
                                    {
                                        p.Value.GetRockets().RemoveAt(i);
                                    }
                                }
                            }
                        }
                    }
                }

                Thread.Sleep(100);
            }

        }

        protected override void UnloadContent() {
            // TODO: Unload any non ContentManager content here
        }


        public void AddMeteor(int x, int y) {

            Meteor meteor = new Meteor(new Vector2(x, y), rnd.Next(3, 11));
            meteor.Content = Content;
            meteor.LoadContent();
            meteors.Add(meteor);
        }

        public void AddFood(int x, int y, string type) {

            Food food = new Food(new Vector2(x, y), rnd.Next(10, 22), type);
            food.Content = Content;
            food.LoadContent();
            items.Add(food);
        }

        public void AddAmmo(int x, int y, string type) {

            Ammo ammo = new Ammo(new Vector2(x, y), rnd.Next(10, 22), type);
            ammo.Content = Content;
            ammo.LoadContent();
            items.Add(ammo);
        }

        public void GenerateMeteors() {

            if (time < 200)
                time++;
            else
            {
                time = 0;

                int num = rnd.Next(1, 5);
                for(int i = 1; i <= num; i++)
                    AddMeteor(rnd.Next(0, 2000), -1 * rnd.Next(0, 300));
            }

        }

        protected override void Update(GameTime gameTime) {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();


            if (Keyboard.GetState().IsKeyDown(Keys.LeftAlt) && Keyboard.GetState().IsKeyDown(Keys.Enter))
                graphics.ToggleFullScreen();

            if (!IsMainMenu)
            {
                player.Update(gameTime);


                //moly.Update(gameTime);

                foreach (Block b in map.GetBlocks())
                {
                    camera.Update(player.GetPosition(), map.Width, map.Height);
                    b.Update(gameTime);
                }

                foreach (Rocket r in player.GetRockets())
                {
                    r.Update(gameTime);
                }


                foreach (KeyValuePair<string, Player> p in players)
                {
                    foreach (Rocket r in p.Value.GetRockets())
                    {
                        r.Update(gameTime);
                    }
                }

                //Items loop:
                for (int i = 0; i < items.Count; i++)
                {
                    if (player.IsCollisionItem(items[i]))
                    {
                        if (items[i] is Food)
                        {
                            if (player.GetHealth() < 100)
                            {
                                player.SetHealth(player.GetHealth() + 30);

                                if (player.GetHealth() > 100)
                                    player.SetHealth(100);

                                items.RemoveAt(i);
                            }
                        }
                        else if (items[i] is Ammo)
                        {
                            if (player.GetNumRockets() < player.GetMaxRockets())
                            {
                                player.SetNumRockets(player.GetNumRockets() + 10);
                                gunPickSound.Play();

                                if (player.GetNumRockets() > player.GetMaxRockets())
                                    player.SetNumRockets(player.GetMaxRockets());

                                items.RemoveAt(i);
                            }
                        }
                    }
                }

                if (gameMode.Equals("co-op"))
                {
                    GenerateMeteors();

                    //Meteors loop:
                    for (int i = 0; i < meteors.Count; i++)
                    {
                        if (meteors[i].IsCollisionPlayer(player))
                        {
                            if (player.GetHealth() > 0)
                            {
                                meteorHitSound.Play();
                                int hp = player.GetHealth();
                                int dmg = meteors[i].GetDamage();
                                player.SetHealth(hp - dmg);
                                meteors.RemoveAt(i);

                            }
                        }
                        else
                        {
                            if (!meteors[i].IsOutsideMap(graphics))
                            {
                                meteors[i].Update(gameTime);
                            }
                            else
                            {
                                if (meteors[i].IsOutsideMap(graphics))
                                    meteors.RemoveAt(i);
                            }
                        }
                    }
                }


                blackHole1.Update(gameTime);
                if (blackHole1.IsCollisionEntity(player))
                {
                    blackHole1.IsCanTeleport = true;
                    blackHole1.Teleport(player, blackHole2.GetPosition());
                }
                else
                    blackHole1.Reset();

                blackHole2.Update(gameTime);
                if (blackHole2.IsCollisionEntity(player))
                {
                    blackHole2.IsCanTeleport = true;
                    blackHole2.Teleport(player, blackHole1.GetPosition());
                }
                else
                    blackHole2.Reset();

            }
            else
            {
                mainMenu.Update(gameTime, this);
            }
            base.Update(gameTime);
        }
 

        protected override void Draw(GameTime gameTime) {
            

            if (!IsMainMenu)
            {
                GraphicsDevice.Clear(Color.Black);

                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, camera.Transform);
                background.Draw(spriteBatch);

                background2.Draw(spriteBatch);

                foreach (BackgroundEntity e in backgroundObjects)
                    e.Draw(spriteBatch);

                map.Draw(spriteBatch);

                foreach (Rocket r in player.GetRockets())
                    r.Draw(spriteBatch);

                foreach (Meteor m in meteors)
                    m.Draw(spriteBatch);

                foreach (Entity e in items)
                    e.Draw(spriteBatch);

                blackHole1.Draw(spriteBatch);
                blackHole2.Draw(spriteBatch);

                player.Draw(spriteBatch);

                foreach (KeyValuePair<string, Player> p in players)
                {
                    p.Value.Draw(spriteBatch);
                }

                foreach (KeyValuePair<string, Player> p in players)
                {
                    foreach (Rocket r in p.Value.GetRockets())
                    {
                        r.Draw(spriteBatch);
                    }
                }

                //moly.Draw(spriteBatch);

                spriteBatch.End();

                spriteBatch.Begin();


                spriteBatch.DrawString(font, " Health: " + player.GetHealth() + "\n Rockets: " + player.GetNumRockets() + "\n (x=" + player.GetPosition().X + ", y=" + player.GetPosition().Y + ")", new Vector2(0, 0), Color.White);
                spriteBatch.DrawString(font, " All rights reserved to Idan Bachar.", new Vector2(0, graphics.PreferredBackBufferHeight - 35), Color.White);

                if(gameMode.Equals("multiplayer"))
                    spriteBatch.DrawString(font, "Waiting for other players. (" + (players.Count + 1) + "/4)", new Vector2(graphics.PreferredBackBufferWidth / 2 - 150, 200), Color.White);
                else if(gameMode.Equals("co-op"))
                    spriteBatch.DrawString(font, "Co-op", new Vector2(graphics.PreferredBackBufferWidth / 2 - 50, 200), Color.White);

                spriteBatch.End();
            }
            else
            {
                GraphicsDevice.Clear(Color.Black);
                
                spriteBatch.Begin();
                
                background.Draw(spriteBatch);

                spriteBatch.DrawString(font, "Molly's Revange", new Vector2(graphics.PreferredBackBufferWidth / 2 - 100, 200), Color.White);
                spriteBatch.DrawString(font, "All rights reserved to Idan Bachar.", new Vector2(0, 20), Color.White);
                mainMenu.DrawButtons(spriteBatch);


                spriteBatch.End();
            }

            base.Draw(gameTime);
        }
    }

    public static class PerPixel
    {
        public static bool IntersectPixels(Entity spriteA, Entity spriteB) {
            // Calculate a matrix which transforms from A's local space into
            // world space and then into B's local space
            Matrix transformAToB = spriteA.Transform * Matrix.Invert(spriteB.Transform);

            // When a point moves in A's local space, it moves in B's local space with a
            // fixed direction and distance proportional to the movement in A.
            // This algorithm steps through A one pixel at a time along A's X and Y axes
            // Calculate the analogous steps in B:
            Vector2 stepX = Vector2.TransformNormal(Vector2.UnitX, transformAToB);
            Vector2 stepY = Vector2.TransformNormal(Vector2.UnitY, transformAToB);

            // Calculate the top left corner of A in B's local space
            // This variable will be reused to keep track of the start of each row
            Vector2 yPosInB = Vector2.Transform(Vector2.Zero, transformAToB);

            // For each row of pixels in A
            for (int yA = 0; yA < spriteA.GetRectangle().Height; yA++)
            {
                // Start at the beginning of the row
                Vector2 posInB = yPosInB;

                // For each pixel in this row
                for (int xA = 0; xA < spriteA.GetRectangle().Width; xA++)
                {
                    // Round to the nearest pixel
                    int xB = (int)Math.Round(posInB.X);
                    int yB = (int)Math.Round(posInB.Y);

                    // If the pixel lies within the bounds of B
                    if (0 <= xB && xB < spriteB.GetRectangle().Width &&
                        0 <= yB && yB < spriteB.GetRectangle().Height)
                    {
                        // Get the colors of the overlapping pixels
                        Color colorA = spriteA.TextureData[xA + yA * spriteA.GetRectangle().Width];
                        Color colorB = spriteB.TextureData[xB + yB * spriteB.GetRectangle().Width];

                        // If both pixels are not completely transparent,
                        if (colorA.A != 0 && colorB.A != 0)
                        {
                            // then an intersection has been found
                            return true;
                        }
                    }

                    // Move to the next pixel in the row
                    posInB += stepX;
                }

                // Move to the next row
                yPosInB += stepY;
            }

            // No intersection found
            return false;
        }
    }
}

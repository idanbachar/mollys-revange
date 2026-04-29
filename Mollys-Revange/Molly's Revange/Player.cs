using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

namespace Molly_s_Revange
{
    public class Player: Entity
    {
        private Vector2 speed;
        private Vector2 direction;
        private List<Rocket> rockets;
        private bool canShoot;
        private bool isCanMove;
        private int health;
        private int maxHealth;
        private int numRockets;
        private int maxRockets;
        private string ip;

        private GamePadState oldState;

        private SoundEffect lazerSound;

        public bool IsCanMove { get { return this.isCanMove; } set { this.isCanMove = value; } }

        public Player(Vector2 pos, int health): base(pos) {

            position = new Vector2(pos.X, pos.Y);
            rotation = 0f;
            rockets = new List<Rocket>();
            canShoot = true;
            isCanMove = true;
            numRockets = 25;
            maxRockets = 25;
            maxHealth = 100;
            this.health = health;
            this.ip = Game1.GetLocalIPAddress();
        }

        public override void LoadContent() {

            texture = content.Load<Texture2D>("images/ships/space_ship");
            textureData = new Color[texture.Width * texture.Height];
            texture.GetData(textureData);
            rectangle = new Rectangle((int)position.X, (int)position.Y, texture.Width, texture.Height);

            oldState = GamePad.GetState(PlayerIndex.One);

            lazerSound = Content.Load<SoundEffect>("music/sounds/lazer");
        }
  
        public void MoveForward() {

            position += speed;
        }

        public override void Update(GameTime gameTime) {

            rectangle = new Rectangle((int)position.X, (int)position.Y, texture.Width, texture.Height);
            direction = new Vector2((float)Math.Cos(rotation) * 5f, (float)Math.Sin(rotation) * 5f);

            KeyDown(gameTime);
            KeyUp(gameTime);
            Controller(gameTime);

            if (isCanMove)
            {
                MoveForward();
            }
 
        }

        public int GetHealth() {
            return health;
        }

        public void SetHealth(int newHealth) {
            health = newHealth;
        }

        public bool IsCollisionBlocks(List<Block> blocks) {

            foreach (Block b in blocks)
            {
                if (PerPixel.IntersectPixels(this, b))
                    return true;
            }

            return false;
        }

        public bool IsCollisionMeteors(List<Meteor> meteors) {

            foreach(Meteor m in meteors)
            {
                if (PerPixel.IntersectPixels(this, m))
                    return true;
            }

            return false;
        }

        public bool IsCollisionBlackHole(BlackHole blackHole) {

            if (PerPixel.IntersectPixels(this, blackHole))
                return true;

            return false;
        }

        public void KeyDown(GameTime gameTime) 
        {

            if (Keyboard.GetState().IsKeyDown(Keys.W))
            {
                speed = direction;

            }
            else if (speed != Vector2.Zero)
            {
                Vector2 i = speed;
                speed = i -= 0.01f * i;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.D))
                rotation += 0.03f;

            if (Keyboard.GetState().IsKeyDown(Keys.A))
                rotation -= 0.03f;

            if (Keyboard.GetState().IsKeyDown(Keys.R))
                SetNumRockets(25);

            if (Keyboard.GetState().IsKeyDown(Keys.Space) && canShoot)
            {
                if (numRockets > 0)
                {
                    canShoot = false;
                    Shoot();
                }
            }
        }

        public void KeyUp(GameTime gameTime) {

            if (Keyboard.GetState().IsKeyUp(Keys.Space))
            {
                canShoot = true;
            }

        }

        public void Controller(GameTime gameTime) {

            if (GamePad.GetState(PlayerIndex.One).IsConnected)
            {
                if (GamePad.GetState(PlayerIndex.One).Triggers.Right > 0)
                    speed = direction;
                else if (speed != Vector2.Zero)
                {
                    Vector2 i = speed;
                    speed = i -= 0.01f * i;
                }
                rotation += (float)GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.X * 0.03f;



                GamePadState state = GamePad.GetState(PlayerIndex.One);


                if (state.IsButtonDown(Buttons.A) && !oldState.IsButtonDown(Buttons.A))
                {
                    if (numRockets > 0)
                    {
                        canShoot = false;
                        Shoot();
                    }
                }

                if (GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.Y))
                {
                    SetNumRockets(GetMaxRockets());
                }

                oldState = state;
 
            }
        }

        public void Shoot() {

            AddRocket(position, direction, rotation);
        }

        public void AddRocket(Vector2 position, Vector2 direction, float rotation) {

            Rocket rocket = new Rocket(position, direction, rotation);
            rocket.Content = content;
            rocket.LoadContent();
            rockets.Add(rocket);
            lazerSound.Play();
            numRockets--;
        }

        public bool IsCollisionItem(Entity item) {

            if (PerPixel.IntersectPixels(this, item))
                return true;

            return false;
        }

        public bool IsCollisionPlayer(Player player) {

            if (PerPixel.IntersectPixels(this, player))
                return true;

            return false;
        }

        public override void Draw(SpriteBatch spriteBatch) {

            spriteBatch.Draw(texture, rectangle, null, Color.White, rotation, origin, SpriteEffects.None, 1.0f);
        }

        public List<Rocket> GetRockets() {
            return rockets;
        }

        public Vector2 GetSpeed() {
            return speed;
        }

        public void SetSpeed(Vector2 newSpeed) {
            speed = newSpeed;
        }

        public int GetNumRockets() {
            return numRockets;
        }

        public void SetNumRockets(int newNum) {
            numRockets = newNum;
        }

        public int GetMaxRockets() {
            return maxRockets;
        }

        public void SetMaxRockets(int newMax) {
            maxRockets = newMax;
        }

        public int GetMaxHealth() {
            return maxHealth;
        }

        public void SetMaxHealth(int newHealth) {
            maxHealth = newHealth;
        }

        public string GetIp() {
            return ip;
        }

        public void SetIp(string newIp) {
            ip = newIp;
        }

        public bool GetCanShoot() {
            return canShoot;
        }

        public void SetCanShoot(bool value) {
            canShoot = value;
        }

        public Vector2 GetDirection() {
            return direction;
        }

        public void SetDirection(Vector2 newDirection) {
            direction = newDirection;
        }
    }
}

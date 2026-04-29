using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace Molly_s_Revange
{
    public class BlackHole : Entity
    {
        private bool isCanTeleport;
        private int timer;
        private Animation animation;

        public BlackHole(Vector2 pos) : base(pos) {

            position = pos;
            isCanTeleport = false;
            animation = new Animation(3, 5,"black_hole/black_hole_");
        }

        public bool IsCanTeleport { get { return this.isCanTeleport; } set { this.isCanTeleport = value; } }

        public override void LoadContent() {

            texture = content.Load<Texture2D>("images/black_hole/black_hole_0");
            textureData = new Color[texture.Width * texture.Height];
            texture.GetData(textureData);
            rectangle = new Rectangle((int)position.X, (int)position.Y, texture.Width, texture.Height);
            animation.Content = content;
            animation.LoadContent();
        }

        public void Teleport(Entity target, Vector2 destination) {

            if (timer < 75)
                timer++;
            else
            {
                target.SetPosition(destination);
                Reset();
            }
        }

        public void Tp(Entity target, Vector2 destination) {

            target.SetPosition(destination);
        }

        public override void Update(GameTime gameTime) {

            rectangle = new Rectangle((int)position.X, (int)position.Y, texture.Width, texture.Height);

            animation.Start();

            //position += direction * 3f;
        }

        public void Reset() {

            timer = 0;
            isCanTeleport = false;
        }

        public bool IsCollisionEntity(Entity somthing) {

            if (PerPixel.IntersectPixels(this, somthing))
                return true;

            return false;
        }

        public override void Draw(SpriteBatch spriteBatch) {

            spriteBatch.Draw(animation.Textures[animation.Frame], rectangle, null, Color.White, rotation, origin, SpriteEffects.None, 1);
        }
    }
}

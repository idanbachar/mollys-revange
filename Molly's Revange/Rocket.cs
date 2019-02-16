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
    public class Rocket: Entity
    {
        private Vector2 direction;

        public Rocket(Vector2 pos, Vector2 direction , float rotation): base(pos) {

            this.position = pos;
            this.rotation = rotation;
            this.direction = direction;
        }

        public override void LoadContent() {

            texture = content.Load<Texture2D>("images/rocket");
            textureData = new Color[texture.Width * texture.Height];
            texture.GetData(textureData);
            rectangle = new Rectangle((int)position.X, (int)position.Y, texture.Width, texture.Height);
        }

        public override void Update(GameTime gameTime) {

            rectangle = new Rectangle((int)position.X, (int)position.Y, texture.Width, texture.Height);
            position += direction * 3f;
        }

        public bool IsCollisionBlock(Block block) {

            if (PerPixel.IntersectPixels(this, block))
                return true;

            return false;
        }

        public override void Draw(SpriteBatch spriteBatch) {

            spriteBatch.Draw(texture, rectangle, null, Color.White, rotation, origin, SpriteEffects.None, 1);
        }
    }
}

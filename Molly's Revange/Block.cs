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
    public class Block: Entity
    {
        private string type;
        private string path;

        public Block(Vector2 pos, string type): base(pos) {

            rotation = 0f;
            this.type = type;
        }

        public override void LoadContent() {

            switch (type)
            {
                case "colide":
                    path = "block";
                    break;
                case "deadly":
                    path = "dead_block";
                    break;
            }

            texture = content.Load<Texture2D>("images/blocks/" + path);
            textureData = new Color[texture.Width * texture.Height];
            texture.GetData(textureData);
            rectangle = new Rectangle((int)position.X, (int)position.Y, texture.Width, texture.Height);
        }

        public override void Update(GameTime gameTime) {

            rectangle = new Rectangle((int)position.X, (int)position.Y, texture.Width, texture.Height);
        }

        public override void Draw(SpriteBatch spriteBatch) {

            spriteBatch.Draw(texture, rectangle, null,Color.White, rotation, origin, SpriteEffects.None, 1);
        }

    }
}

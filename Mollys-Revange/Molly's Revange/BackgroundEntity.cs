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
    public class BackgroundEntity: Entity
    {
        public BackgroundEntity(Vector2 pos, Texture2D texture): base(pos) {
 
            position = new Vector2(pos.X, pos.Y);
            this.texture = texture;
            textureData = new Color[texture.Width * texture.Height];
            texture.GetData(textureData);
            rectangle = new Rectangle((int)position.X, (int)position.Y, texture.Width, texture.Height);
        }

        public override void LoadContent() { }


        public override void Update(GameTime gameTime) {

            rectangle = new Rectangle((int)position.X , (int)position.Y, texture.Width, texture.Height);
            position = new Vector2(position.X, position.Y);

        }

        public override void Draw(SpriteBatch spriteBatch) {

            spriteBatch.Draw(texture, rectangle, Color.White);
        }

    }
}

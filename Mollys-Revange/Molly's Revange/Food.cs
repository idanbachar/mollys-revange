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
    public class Food : Entity
    { 
        private int fresh;
        private string name;

        public Food(Vector2 pos, int fresh, string name) : base(pos) {

            this.fresh = fresh;
            this.name = name;
        }

        public override void LoadContent() {

            texture = content.Load<Texture2D>("images/food/" + name);
            textureData = new Color[texture.Width * texture.Height];
            texture.GetData(textureData);
            rectangle = new Rectangle((int)position.X, (int)position.Y, texture.Width, texture.Height);
        }

        public override void Update(GameTime gameTime) {

            rectangle = new Rectangle((int)position.X, (int)position.Y, texture.Width, texture.Height);




        }

        public override void Draw(SpriteBatch spriteBatch) {

            spriteBatch.Draw(texture, rectangle, Color.White);
        }

        public int GetFresh() {
            return fresh;
        }

        public void SetFresh(int newFresh) {
            this.fresh = newFresh;
        }

        public string GetName() {
            return name;
        }

        public void SetName(string newName) {
            this.name = newName;
        }
    }
}

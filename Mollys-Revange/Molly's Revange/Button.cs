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
    public class Button
    {
        private Texture2D texture;
        private Vector2 position;
        private Rectangle rectangle;
        private ContentManager content;
        private string name;
        private string folderPath;

        public Texture2D Texture { get { return texture; } set { texture = value; } }
        public ContentManager Content { get { return this.content; } set { content = value; }  }
        public string Name { get { return this.name; } }

        public Button(Rectangle rec, string name, string folderPath) {

            position = new Vector2(rec.X, rec.Y);
            rectangle = new Rectangle(rec.X, rec.Y, rec.Width, rec.Height);
            this.name = name;
            this.folderPath = folderPath;
        }

        public void LoadContent() {

            texture = content.Load<Texture2D>("images/buttons/" + folderPath + "/" + name);
        }

        public void Update(GameTime gameTime) {

            rectangle = new Rectangle((int)position.X, (int)position.Y, rectangle.Width, rectangle.Height);


        }

        public bool IsOver() {

            Rectangle mouseRectangle = new Rectangle(Mouse.GetState().Position.X, Mouse.GetState().Position.Y, 7, 15);

            if (mouseRectangle.Intersects(rectangle))
                return true;

            return false;
        }

        public void Draw(SpriteBatch spriteBatch) {

            if(!IsOver())
                spriteBatch.Draw(texture, rectangle, Color.White);
            else
                spriteBatch.Draw(texture, rectangle, Color.DarkGray);
        }

        public Rectangle GetRectangle() {
            return rectangle;
        }

        public void SetRectangle(Rectangle newRec) {
            rectangle = newRec;
        }

        public Vector2 GetPosition() {
            return position;
        }
    }
}

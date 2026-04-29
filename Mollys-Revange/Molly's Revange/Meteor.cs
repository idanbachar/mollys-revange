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
    public class Meteor: Entity
    {
        private int side;
        private Random rnd;
        private int damage;
        private Vector2 direction;
 
        public Meteor(Vector2 pos, int dmg): base(pos) {

            rnd = new Random();
            position = new Vector2(pos.X, pos.Y);
            side = rnd.Next(0, 2);
            damage = dmg;
            direction = new Vector2(rnd.Next(2, 10), rnd.Next(2, 7));

            switch (side)
            {
                case 0:
                    side = -1;
                    break;
                case 1:
                    side = 1;
                    break;
            }


        }

        public override void LoadContent() {

            texture = content.Load<Texture2D>("images/stars/meteor");
            textureData = new Color[texture.Width * texture.Height];
            texture.GetData(textureData);
            rectangle = new Rectangle((int)position.X, (int)position.Y, texture.Width, texture.Height);

        }

        public override void Update(GameTime gameTime) {

            rectangle = new Rectangle((int)position.X, (int)position.Y, texture.Width, texture.Height);
            position += new Vector2(side * direction.X, side * direction.Y) * 0.5f;

        }

        public override void Draw(SpriteBatch spriteBatch) {

            spriteBatch.Draw(texture, rectangle, Color.White);
        }

        public bool IsOutsideMap(GraphicsDeviceManager graphics) {

            if (rectangle.Right >= 2200  
                || rectangle.Left <= -100  || rectangle.Top >= 2500)
                    return true;

            return false;
        }

        public bool IsCollisionPlayer(Player player) {

            if (PerPixel.IntersectPixels(this, player))
                return true;

            return false;       
        }

        public int GetDamage() {
            return damage;
        }

        public void SetDamage(int newDamage) {
            damage = newDamage;
        }
    }
}

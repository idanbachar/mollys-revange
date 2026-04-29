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
    public abstract class Entity
    {
        protected Texture2D texture;
        protected Rectangle rectangle;
        protected Vector2 position;
        protected Vector2 origin;
        protected ContentManager content;
        protected Color[] textureData;
        protected float rotation;

        public float Rotation { get { return this.rotation; } set { this.rotation = value; } }
        public Color[] TextureData { get { return this.textureData; } }
        public Texture2D Texture { get { return this.texture; } set { this.texture = value; } }
        public ContentManager Content { get { return this.content; } set { this.content = value; } }

        public Matrix Transform {
            get {
                return Matrix.CreateTranslation(new Vector3(-this.origin, 0.0f)) *
                                        Matrix.CreateRotationZ(this.Rotation) *
                                        Matrix.CreateTranslation(new Vector3(this.position, 0.0f));
            }
        }

        public Entity(Vector2 pos) {

            position = new Vector2(pos.X, pos.Y);
            origin = Vector2.Zero;

        }

        public abstract void LoadContent();
        public abstract void Update(GameTime gameTime);
        public abstract void Draw(SpriteBatch spriteBatch);

        public Rectangle GetRectangle() {
            return rectangle;
        }

        public void SetRectangle(Rectangle newRec) {
            rectangle = newRec;
        }

        public Vector2 GetPosition() {
            return position;
        }

        public void SetPosition(Vector2 newPos) {
            position = new Vector2(newPos.X, newPos.Y);
        }
    }
}

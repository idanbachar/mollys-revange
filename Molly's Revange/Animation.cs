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
    public class Animation
    {
        private Texture2D[] textures;
        private Rectangle rectangle;
        private Color[] textureData;
        private ContentManager content;
        private string path;
        private int frame;
        private int timer;
        private int speed;

        public ContentManager Content { get { return this.content; } set { this.content = value; } }
        public int Frame { get { return this.frame; } set { this.frame = value; } }
        public Texture2D [] Textures { get { return this.textures; } }

        public Animation(int size, int speed, string path) {

            textures = new Texture2D[size];
            this.path = path;
            this.frame = 0;
            this.speed = speed;
        }

        public void LoadContent() {

            for (int i = 0; i < textures.Length; i++)
                textures[i] = content.Load<Texture2D>("images/" + path + i);
        }

        public void Start() {

            if (timer < speed)
                timer++;
            else
            {
                timer = 0;
                if (frame < textures.Length - 1)
                    frame++;
                else
                {
                    frame = 0;
                }
            }
        }
    }
}

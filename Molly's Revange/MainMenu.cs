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
    public class MainMenu
    {
        private GraphicsDeviceManager graphics;
        private ContentManager content;
        private Button[] buttons;
        private MouseState oldState;

        public MainMenu(GraphicsDeviceManager graphics, ContentManager content){

            this.graphics = graphics;
            this.content = content;
            buttons = new Button[4];
        }

        public void LoadContent() {

            buttons[0] = new Button(new Rectangle(graphics.PreferredBackBufferWidth / 2 - 125, graphics.PreferredBackBufferHeight / 2 - 40, 250, 80), "co-op", "main menu");
            buttons[0].Content = content;
            buttons[0].LoadContent();

            buttons[1] = new Button(new Rectangle(graphics.PreferredBackBufferWidth / 2 - 125, graphics.PreferredBackBufferHeight / 2 -40 + 100, 250, 80), "Multiplayer", "main menu");
            buttons[1].Content = content;
            buttons[1].LoadContent();

            buttons[2] = new Button(new Rectangle(graphics.PreferredBackBufferWidth / 2 - 125, graphics.PreferredBackBufferHeight / 2 - 40 + 200, 250, 80), "Options", "main menu");
            buttons[2].Content = content;
            buttons[2].LoadContent();

            buttons[3] = new Button(new Rectangle(graphics.PreferredBackBufferWidth / 2 - 125, graphics.PreferredBackBufferHeight / 2 - 40 + 300, 250, 80), "Exit", "main menu");
            buttons[3].Content = content;
            buttons[3].LoadContent();
        }

        public void Update(GameTime gameTime, Game1 game1) {

            MouseState newState = Mouse.GetState();


            if (newState.LeftButton == ButtonState.Pressed && oldState.LeftButton == ButtonState.Released)
            {
                //Offine CO-OP:
                if (buttons[0].IsOver())
                {
                    Game1.IsMainMenu = false;
                    Game1.gameMode = "co-op";
                    game1.LoadDefaultItems();
                }
                //Multiplayer:
                if (buttons[1].IsOver())
                {
                    game1.ConnectToServer();
                }
                if (buttons[3].IsOver())
                {
                    game1.Exit();
                }
            }

            oldState = newState;

        }

        public void DrawButtons(SpriteBatch spriteBatch) {

            foreach (Button b in buttons)
                b.Draw(spriteBatch);
        }

        public Button [] GetButtons() {
            return buttons;
        }
 
    }
}

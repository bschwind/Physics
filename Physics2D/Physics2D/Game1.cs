using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Physics2D.GUI;
using GraphicsToolkit;
using GraphicsToolkit.GUI;
using GraphicsToolkit.Input;

namespace Physics2D
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        GUIManager guiManager;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            this.Window.AllowUserResizing = true;

            IsMouseVisible = true;
            IsFixedTimeStep = true;

            guiManager = new GUIManager(this, graphics);
            guiManager.AddPanel(new DemoPanel(Vector2.Zero, Vector2.One));
            Components.Add(guiManager);
            Components.Add(new InputHandler(this));

            graphics.PreferredBackBufferWidth = Config.ScreenWidth;
            graphics.PreferredBackBufferHeight = Config.ScreenHeight;
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void UnloadContent()
        {

        }

        protected override void Update(GameTime gameTime)
        {
            if (InputHandler.IsNewKeyPress(Keys.Escape))
            {
                this.Exit();
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            base.Draw(gameTime);
        }
    }
}

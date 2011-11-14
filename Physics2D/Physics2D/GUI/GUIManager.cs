using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Physics2D.Graphics;

namespace Physics2D.GUI
{
    public class GUIManager : DrawableGameComponent
    {
        public Color BackgroundColor = Color.Red;
        private List<Panel> panels;
        private List<Texture2D> panelTextures;
        private SpriteBatch spriteBatch;
        private bool hasLaoded = false;

        public GUIManager(Game g)
            : base(g)
        {
            panels = new List<Panel>();
            panelTextures = new List<Texture2D>();
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            Panel.Device = Game.GraphicsDevice;

            spriteBatch = new SpriteBatch(Game.GraphicsDevice);

            foreach (Panel p in panels)
            {
                p.LoadContent(Game.Content);
            }

            hasLaoded = true;
        }

        public void AddPanel(Panel p)
        {
            panels.Add(p);
            panelTextures.Add(null);
            if (hasLaoded)
            {
                p.LoadContent(Game.Content);
            }
        }

        public void RemovePanel(Panel p)
        {
            panels.Remove(p);
        }

        public void RefreshPanels()
        {
            foreach (Panel p in panels)
            {
                p.Refresh();
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            foreach (Panel p in panels)
            {
                p.Update(gameTime);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            //Render the panels to textures
            foreach (Panel p in panels)
            {
                p.BeginDraw();
                p.Draw(gameTime);
                p.EndDraw();
            }
            
            //Clear the screen
            Game.GraphicsDevice.Clear(BackgroundColor);

            //Draw our panel textures
            spriteBatch.Begin();
            foreach (Panel p in panels)
            {
                spriteBatch.Draw(p.PanelTexture, p.ScreenRect, Color.White);      
            }
            spriteBatch.End();
        }
    }
}

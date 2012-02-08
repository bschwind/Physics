using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Physics2D.Physics;
using Physics2D.Physics.Bodies;
using Physics2D.Graphics;
using GraphicsToolkit.GUI;
using GraphicsToolkit.Graphics;
using GraphicsToolkit.Input;

namespace Physics2D.GUI
{
    public class DemoPanel : Panel
    {
        PrimitiveBatch primBatch;
        Camera2D cam;
        Vector2 mouseForceStart;
        bool mouseHeldDown = false;

        PhysicsEngine engine;

        public DemoPanel(Vector2 upLeft, Vector2 botRight)
            : base(upLeft, botRight)
        {
            cam = new Camera2D(new Vector2(5, 5), this);
        }

        public override void LoadContent(Microsoft.Xna.Framework.Content.ContentManager content)
        {
            base.LoadContent(content);

            primBatch = new PrimitiveBatch(Panel.Device);

            setupEngine();
        }

        private void setupEngine()
        {
            engine = new PhysicsEngine();
            int bodyCount = 8; 
            for (int i = 0; i < bodyCount; i++)
            {
                engine.AddRigidBody(new CircleBody(new Vector2(i+1, 8), new Vector2(0, 0), 10f, 0.2f));
            }

            for (int i = 0; i < bodyCount; i++)
            {
                engine.AddRigidBody(new CircleBody(new Vector2(i+1.1f, 0), new Vector2(0,0), 10f, 0.2f));
            }

            engine.AddRigidBody(new PlaneBody(Vector2.UnitY, Vector2.Zero));
            engine.AddRigidBody(new PlaneBody(Vector2.UnitX, Vector2.Zero));
            engine.AddRigidBody(new PlaneBody(-Vector2.UnitY, new Vector2(0, 10)));
            engine.AddRigidBody(new PlaneBody(-Vector2.UnitX, new Vector2(10, 0)));
        }

        protected override void OnRefresh()
        {
            base.OnRefresh();
            cam.Resize();
        }

        public override void Update(GameTime g)
        {
            base.Update(g);

            cam.Update(g);

            if (InputHandler.MouseState.LeftButton == ButtonState.Pressed && InputHandler.LastMouseState.LeftButton == ButtonState.Released)
            {
                mouseHeldDown = true;
                mouseForceStart = cam.GetWorldMousePos();
                
            }
            else if (InputHandler.MouseState.LeftButton != ButtonState.Pressed)
            {
                mouseHeldDown = false;
            }

            if (InputHandler.MouseState.RightButton == ButtonState.Pressed)
            {
                engine.AddRigidBody(new CircleBody(cam.GetWorldMousePos(), new Vector2(0, 0), 10f, 0.2f));
            }

            Vector2 force = Vector2.Zero;
            Vector2 mouseWorldPos = cam.GetWorldMousePos();
            if (mouseHeldDown)
            {
                force = mouseWorldPos - mouseForceStart;
                engine.SetGravity(force);
            }

            engine.Update(g);
        }

        public override void Draw(GameTime g)
        {
            base.Draw(g);
            Panel.Device.Clear(Color.Black);

            Vector2 worldMousePos = cam.GetWorldMousePos();

            primBatch.Begin(PrimitiveType.LineList, cam);
            primBatch.Draw2DGrid(10, 10, Color.DarkSlateBlue);
            foreach (RigidBody2D rb in engine.GetBodies())
            {
                CircleBody c = rb as CircleBody;
                if (c != null)
                {
                    primBatch.DrawCircle(c.Pos, c.Radius, 12, Color.Orange);
                }
            }
            if (mouseHeldDown)
            {
                primBatch.DrawLine(new Vector3(mouseForceStart, 0), new Vector3(worldMousePos, 0), Color.Red);
            }
            primBatch.End();
        }
    }
}
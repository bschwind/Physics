using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Physics2D.Physics;
using Physics2D.Physics.Bodies;
using Physics2D.Physics.Geometry;
using Physics2D.Graphics;
using Physics2D.Physics.Constraints;
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
            int bodyCount = 1; 
            for (int i = 0; i < bodyCount; i++)
            {
                engine.AddRigidBody(new CircleBody(new Vector2(i+2, 8), new Vector2(0, 0), 0f, 0.2f));
            }

            for (int i = 0; i < bodyCount; i++)
            {
                RigidBody2D rb = new CircleBody(new Vector2(i + 1.1f, 0), new Vector2(0, 0), 10f, 0.2f);
                engine.AddRigidBody(rb);
                engine.AddConstraint(new DistanceConstraint(rb, engine.GetBodies()[0], 4));
            }

            LineBody b = new LineBody(Vector2.UnitY, Vector2.Zero);
            engine.AddRigidBody(b);
            engine.AddRigidBody(new LineBody(Vector2.UnitX, Vector2.Zero));
            engine.AddRigidBody(new LineBody(-Vector2.UnitY, new Vector2(0, 10)));
            engine.AddRigidBody(new LineBody(-Vector2.UnitX, new Vector2(20, 0)));

            //engine.AddConstraint(new DistanceConstraint(b, engine.GetBodies()[0], 4));
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
                RigidBody2D rb = new CircleBody(cam.GetWorldMousePos(), new Vector2(0, 0), 10f, 0.2f);
                engine.AddRigidBody(rb);
                engine.AddConstraint(new DistanceConstraint(rb, engine.GetBodies()[engine.GetBodies().Count-2], 1));
            }

            Vector2 force = Vector2.Zero;
            Vector2 mouseWorldPos = cam.GetWorldMousePos();
            if (mouseHeldDown)
            {
                force = mouseWorldPos - mouseForceStart;
                foreach (RigidBody2D rb in engine.GetBodies())
                {
                    rb.AddForce(force);
                }
            }
            engine.Update(g);
        }

        public override void Draw(GameTime g)
        {
            base.Draw(g);
            Panel.Device.Clear(Color.Black);

            Vector2 worldMousePos = cam.GetWorldMousePos();

            primBatch.Begin(PrimitiveType.LineList, cam);
            primBatch.Draw2DGrid(20, 10, Color.DarkSlateBlue);
            foreach (RigidBody2D rb in engine.GetBodies())
            {
                CircleBody c = rb as CircleBody;
                if (c != null)
                {
                    primBatch.DrawRotatedCircle(c.Pos, c.Radius, 12, c.Rot, Color.Orange);
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
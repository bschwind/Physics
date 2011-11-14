using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Physics2D.Graphics;
using Physics2D.Physics;
using Physics2D.Input;

namespace Physics2D.GUI
{
    public class DemoPanel : Panel
    {
        PrimitiveBatch2D primBatch;
        SpriteBatch spriteBatch;
        Camera2D cam;
        Vector2 mouseForceStart;
        bool mouseHeldDown = false;

        PhysicsEngine engine;
        CircleObject obj;
        LineSegment floor;

        public DemoPanel(Vector2 upLeft, Vector2 botRight)
            : base(upLeft, botRight)
        {
            cam = new Camera2D(new Vector2(5, 5), this);
            cam.SetZoom(10);
        }

        public override void LoadContent(Microsoft.Xna.Framework.Content.ContentManager content)
        {
            base.LoadContent(content);

            primBatch = new PrimitiveBatch2D(Panel.Device);
            spriteBatch = new SpriteBatch(Panel.Device);

            setupEngine();
        }

        private void setupEngine()
        {
            engine = new PhysicsEngine();
            obj = new CircleObject(1, 3, new Vector2(5, 5));
            engine.AddCircle(obj);

            floor = new LineSegment(Vector2.Zero, new Vector2(10, 0));
            engine.AddSegment(floor);
        }

        protected override void OnRefresh()
        {
            base.OnRefresh();
            cam.Resize(width, height);
        }

        public override void Update(GameTime g)
        {
            base.Update(g);

            cam.Update(g);

            if (InputHandler.IsNewLeftMousePress())
            {
                mouseHeldDown = true;
                mouseForceStart = cam.GetWorldMousePos();
            }
            else if (InputHandler.MouseState.LeftButton != ButtonState.Pressed)
            {
                mouseHeldDown = false;
            }

            Vector2 force = Vector2.Zero;
            Vector2 mouseWorldPos = cam.GetWorldMousePos();
            if (mouseHeldDown)
            {
                force = mouseWorldPos - mouseForceStart;
            }

            obj.AddForce(force, mouseWorldPos-obj.Pos);

            engine.Update(g);
        }

        public override void Draw(GameTime g)
        {
            base.Draw(g);
            Panel.Device.Clear(Color.Black);

            Vector2 worldMousePos = cam.GetWorldMousePos();

            primBatch.Begin(PrimitiveType.LineList, cam);
            primBatch.DrawGrid(10, 10, Color.DarkSlateBlue);
            foreach (CircleObject c in engine.GetCircles())
            {
                primBatch.DrawRotatedCircle(new Circle(new Vector3(c.Pos, 0), c.Radius), c.Rot, Color.Orange, Color.Brown);
            }
            if (mouseHeldDown)
            {
                primBatch.DrawLine(new Vector3(mouseForceStart, 0), new Vector3(worldMousePos, 0), Color.Red);
            }
            primBatch.End();

            
            //node.Pos = worldMousePos;

            //Vector2 drawPos = cam.GetScreenPos(new Vector3(node.Pos, 0));
            //spriteBatch.Draw(tex, cam.WorldRectToScreen(node.GetWorldRect()), Color.White);
        }
    }
}
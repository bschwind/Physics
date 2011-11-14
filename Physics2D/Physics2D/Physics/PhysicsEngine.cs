using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Physics2D.Graphics;

namespace Physics2D.Physics
{
    public class PhysicsEngine
    {
        private List<CircleObject> circles;
        private List<LineSegment> segments;
        private Vector2 gravity = new Vector2(0, -1f);

        public PhysicsEngine()
        {
            circles = new List<CircleObject>();
            segments = new List<LineSegment>();
        }

        public List<CircleObject> GetCircles()
        {
            return circles;
        }

        public List<LineSegment> GetSegments()
        {
            return segments;
        }

        public void AddCircle(CircleObject c)
        {
            circles.Add(c);
        }

        public void AddSegment(LineSegment s)
        {
            segments.Add(s);
        }

        public void Update(GameTime g)
        {
            float dt = (float)g.ElapsedGameTime.TotalSeconds;

            foreach (CircleObject c in circles)
            {
                //Get the force
                Vector2 force = c.Force;
                float torque = c.Torque;
                c.ClearForces();
                //F = ma
                //a = F / m
                Vector2 acc = force / c.Mass;
                //Account for gravity
                acc += gravity;
                float angAcc = torque / c.Inertia;

                c.Vel += (acc * dt);
                Vector2 newPos = c.Pos + (c.Vel * dt);

                //Check the capsule from oldPos to newPos and see if we hit a wall
                bool hit = false;
                float closestDistSqrd = float.MaxValue;
                Vector2 hitPos = Vector2.Zero;
                Vector2 normal = Vector2.Zero;
                Vector2 newCirclePos = Vector2.Zero;
                Vector2 tempNewPos = Vector2.Zero;
                Vector2 tempHitPos = Vector2.Zero;
                foreach (LineSegment seg in segments)
                {
                    bool collided = Intersection.CollideCapsuleLineSegment(c.Pos, newPos, c.Radius, seg, out tempHitPos, out tempNewPos);
                    Console.WriteLine(collided);
                    if (collided)
                    {
                        //If this is the closest intersection so far, keep it
                        if ((tempHitPos - c.Pos).LengthSquared() < closestDistSqrd)
                        {
                            hitPos = tempHitPos;
                            normal = c.Pos - hitPos;
                            newCirclePos = tempNewPos;
                            closestDistSqrd = (tempHitPos - c.Pos).LengthSquared();
                        }
                        hit = true;
                    }
                }

                if (hit)
                {
                    c.Pos += (c.Vel * dt);
                    c.AddForce(Vector2.UnitX*50, hitPos - newCirclePos);
                }
                else
                {
                    c.Pos += (c.Vel * dt);

                    c.RotVel += (angAcc * dt);
                    c.Rot += (c.RotVel * dt);
                }
            }
        }
    }
}

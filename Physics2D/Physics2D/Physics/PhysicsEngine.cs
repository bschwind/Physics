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

                //c.Vel += (acc * dt);
                Vector2 tempVel = c.Vel + (acc * dt);
                Vector2 newPos = c.Pos + (tempVel * dt);

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
                    if (collided)
                    {
                        //Discard hit points that we are touching but are behind us
                        if (Vector2.Dot(tempHitPos - c.Pos, newPos - c.Pos) < 0)
                        {
                            continue;
                        }
                        //If this is the closest intersection so far, keep it
                        if ((tempHitPos - c.Pos).LengthSquared() < closestDistSqrd)
                        {
                            hitPos = tempHitPos;
                            normal = Vector2.Normalize(c.Pos - hitPos); //This is suspect
                            newCirclePos = tempNewPos + (normal*0.001f);
                            closestDistSqrd = (tempHitPos - c.Pos).LengthSquared();
                        }
                        hit = true;
                    }
                }

                //We have a contact point. Resolve the collision
                if (hit)
                {
                    //Get the time of the collision
                    float newDt = (newCirclePos - c.Pos).Length() / (newPos - c.Pos).Length();
                    newDt *= dt;
                    //Check the velocity relative to the normal
                    //First get the point on the circle relative to the center
                    Vector2 relativeHitPoint = hitPos - c.Pos;
                    Vector2 pointVel = (c.Vel + (acc * newDt));// +(new Vector2(-relativeHitPoint.Y, relativeHitPoint.X) * (c.RotVel + (angAcc * newDt)));
                    float relVel = Vector2.Dot(pointVel, normal);

                    if (relVel > 0)
                    {
                        //The body is leaving the collision point. Let it go on its way
                        newDt = dt - newDt;

                        c.Pos = newCirclePos;
                        c.Pos += (pointVel * newDt);
                    }
                    else if ((float)Math.Abs(relVel) <= float.Epsilon)
                    {
                        //The point is in contact
                    }
                    else
                    {
                        //The points are hitting each other, resolve the collision
                        float segmentCOR = 1f;
                        float cor = c.COR * segmentCOR;
                        float numerator = -(1 + cor) * Vector2.Dot(pointVel, normal);
                        float denom = Vector2.Dot(normal, (1f / c.Mass) * normal);
                        float j = numerator / denom;

                        Vector2 newVel = pointVel + (j / c.Mass) * normal;
                        c.Vel = newVel;
                        newDt = dt - newDt;

                        c.Pos = newCirclePos;
                        c.Pos += (c.Vel * newDt);
                        //c.RotVel += (angAcc * newDt);
                        //c.Rot += (c.RotVel * newDt);
                    }
                }
                else //Continue as usual
                {
                    c.Vel = tempVel;
                    c.Pos += (c.Vel * dt);

                    //c.RotVel += (angAcc * dt);
                    //c.Rot += (c.RotVel * dt);
                }
            }
        }
    }
}

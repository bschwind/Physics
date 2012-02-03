using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Physics2D.Graphics;

namespace Physics2D.Physics
{
    public class PhysicsEngineOld
    {
        private List<CircleObject> circles;
        private List<LineSegment> segments;
        private Vector2 gravity = new Vector2(0, -1f);

        public PhysicsEngineOld()
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
                Vector2 predictedVel = c.Vel + (acc * dt);
                Vector2 predictedPos = c.Pos + (predictedVel * dt);

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
                    bool collided = Intersection.CollideCapsuleLineSegment(c.Pos, predictedPos, c.Radius, seg, out tempHitPos, out tempNewPos);
                    if (collided)
                    {
                        //Discard hit points that we are touching but are behind us
                        if (Vector2.Dot(tempHitPos - c.Pos, predictedPos - c.Pos) < 0)
                        {
                            continue;
                        }
                        //If this is the closest intersection so far, keep it
                        if ((tempHitPos - c.Pos).LengthSquared() < closestDistSqrd)
                        {
                            hitPos = tempHitPos;
                            normal = c.Pos - hitPos; //This is suspect
                            newCirclePos = tempNewPos +(Vector2.Normalize(normal) * 0.0001f);
                            closestDistSqrd = (tempHitPos - c.Pos).LengthSquared();
                        }
                        hit = true;
                    }
                }

                //We have a contact point. Resolve the collision
                if (hit)
                {
                    //Get the time of the collision
                    float newDt = (newCirclePos - c.Pos).Length() / (predictedPos - c.Pos).Length();
                    newDt *= dt;
                    //Check the velocity relative to the normal
                    //First get the point on the circle relative to the center
                    Vector2 relativeHitPoint = hitPos - c.Pos;
                    Vector2 pointCollisionVel = (c.Vel + (acc * newDt));// +(new Vector2(-relativeHitPoint.Y, relativeHitPoint.X) * (c.RotVel + (angAcc * newDt)));
                    float normalRelVel = Vector2.Dot(pointCollisionVel, normal);
                    Console.WriteLine(normalRelVel);

                    if (normalRelVel > 0)
                    {
                        //The body is leaving the collision point. Let it go on its way
                        newDt = dt - newDt;

                        c.Pos = newCirclePos;
                        c.Pos += (pointCollisionVel * newDt);
                    }
                    else if ((float)Math.Abs(normalRelVel) <= float.Epsilon)
                    {
                        //The point is in contact
                    }
                    else
                    {
                        //The points are hitting each other, resolve the collision
                        float segmentCOR = 1f;
                        float cor = c.COR * segmentCOR;
                        float numerator = -(1f + cor) * Vector2.Dot(pointCollisionVel, normal);
                        float denom = Vector2.Dot(normal, (1f / c.Mass) * normal);
                        float j = numerator / denom;

                        Vector2 newVel = pointCollisionVel + (j / c.Mass) * normal;
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
                    c.Vel = predictedVel;
                    c.Pos = predictedPos;

                    //c.RotVel += (angAcc * dt);
                    //c.Rot += (c.RotVel * dt);
                }
            }
        }
    }
}

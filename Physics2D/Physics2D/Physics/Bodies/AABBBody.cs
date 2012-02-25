using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Physics2D.Physics.Bodies
{
    public class AABBBody : RigidBody2D
    {
        private Vector2 halfExtents;

        public Vector2 HalfExtents
        {
            get
            {
                return halfExtents;
            }
        }

        public AABBBody(Vector2 pos, Vector2 vel, float mass, Vector2 halfExtents)
            : base(pos, vel, 0f, mass, 0f)
        {
            this.halfExtents = halfExtents;
        }

        public override void GenerateMotionAABB(float dt)
        {
            throw new NotImplementedException();
        }

        public override Contact GenerateContact(RigidBody2D rb, float dt)
        {
            if (rb as CircleBody != null)
            {
                CircleBody c = rb as CircleBody;
                Vector2 closestPt = Intersection.ClosestPointAABBPt(c.Pos, this);
                Vector2 normal = c.Pos - closestPt;
                float normLen = normal.Length();
                normal /= normLen;


                return new Contact(normal, normLen - c.Radius, this, c, closestPt, c.Pos - (normal * c.Radius));
            }
            else if (rb as LineBody != null)
            {
                
            }
            else
            {
                throw new NotImplementedException();
            }

            return new Contact();
        }
    }
}

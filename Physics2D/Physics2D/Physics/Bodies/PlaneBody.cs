using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Physics2D.Physics.Bodies
{
    public class PlaneBody : RigidBody2D
    {
        private Vector2 normal;
        private Vector2 p;

        public Vector2 Normal
        {
            get
            {
                return normal;
            }
        }

        public Vector2 P
        {
            get
            {
                return p;
            }
        }

        public PlaneBody(Vector2 normal, Vector2 p)
            : base(p, Vector2.Zero, 0)
        {
            this.normal = Vector2.Normalize(normal);
            this.p = p;
        }

        public float DistanceToPoint(Vector2 q)
        {
            Vector2 d = q - p;
            float dist = Vector2.Dot(d, normal);
            return dist;
        }

        public Vector2 ProjectPointOntoPlane(Vector2 q)
        {
            Vector2 d = q - p;
            float dist = Vector2.Dot(d, normal);
            return q - Vector2.Normalize(d) * dist;
        }

        public override Contact GenerateContact(RigidBody2D rb, float dt)
        {
            if (rb as CircleBody != null)
            {
                CircleBody c = rb as CircleBody;
                float dist = DistanceToPoint(c.Pos) - c.Radius;

                Vector2 pointOnPlane = c.Pos - (normal * (dist+c.Radius));
                Vector2 pointOnBall = c.Pos - (normal * c.Radius);

                return new Contact(normal, dist, this, rb, pointOnPlane, pointOnBall);
            }
            else if (rb as PlaneBody != null)
            {
                return new Contact();
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Physics2D.Physics.Bodies
{
    public class CircleBody : RigidBody2D
    {
        private float radius;

        public float Radius
        {
            get
            {
                return radius;
            }
        }

        public CircleBody(Vector2 pos, Vector2 vel, float mass, float radius)
            : base(pos, vel, mass)
        {
            this.radius = radius;
        }

        public override Contact GenerateContact(RigidBody2D rb, float dt)
        {
            if (rb as CircleBody != null)
            {
                CircleBody c = rb as CircleBody;
                Vector2 normal = rb.Pos - this.Pos;
                float normLen = normal.Length();
                float dist = normLen - (this.radius + c.radius);
                normal /= normLen;
                Vector2 pa = this.Pos + normal * this.radius;
                Vector2 pb = rb.Pos - normal * c.radius;

                return new Contact(normal, dist, this, rb, pa, pb);
            }
            else if (rb as PlaneBody != null)
            {
                PlaneBody pb = rb as PlaneBody;
                return pb.GenerateContact(this, dt);
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}

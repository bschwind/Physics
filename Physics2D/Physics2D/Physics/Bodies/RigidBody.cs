using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Physics2D.Physics.Bodies
{
    public abstract class RigidBody2D
    {
        private Vector2 pos;
        private Vector2 vel;
        private float invMass;

        public Vector2 Pos
        {
            get
            {
                return pos;
            }
        }

        public Vector2 Vel
        {
            get
            {
                return vel;
            }

            set
            {
                vel = value;
            }
        }

        public float InvMass
        {
            get
            {
                return invMass;
            }
        }

        public RigidBody2D(Vector2 pos, Vector2 vel, float mass)
        {
            if (mass < 0)
            {
                throw new Exception("Mass can not be less than 0");
            }

            //This checks to see if the body has "infinite" mass
            if (mass == 0)
            {
                invMass = 0;
            }
            else
            {
                invMass = 1f / mass;
            }

            this.pos = pos;
            this.vel = vel;
        }

        public abstract Contact GenerateContact(RigidBody2D rb, float dt);

        public void Integrate(float dt)
        {
            pos += vel * dt;
        }
    }
}

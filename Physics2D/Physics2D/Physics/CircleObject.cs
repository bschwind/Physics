using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Physics2D.Physics
{
    public class CircleObject
    {
        private float mass;
        private float radius;
        private float cor;

        private float rot;
        private float rotVel;
        private float torque;
        private float inertia;
        private Vector2 pos;
        private Vector2 vel;
        private Vector2 force;

        public Vector2 Pos
        {
            get
            {
                return pos;
            }
            set
            {
                pos = value;
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

        public float Rot
        {
            get
            {
                return rot;
            }
            set
            {
                rot = value;
            }
        }

        public float RotVel
        {
            get
            {
                return rotVel;
            }
            set
            {
                rotVel = value;
            }
        }

        public float Mass
        {
            get
            {
                return mass;
            }
        }

        public float Inertia
        {
            get
            {
                return inertia;
            }
        }

        public float Radius
        {
            get
            {
                return radius;
            }
        }

        public float COR
        {
            get
            {
                return cor;
            }
        }

        public Vector2 Force
        {
            get
            {
                return force;
            }
        }

        public float Torque
        {
            get
            {
                return torque;
            }
        }

        public CircleObject(float mass, float radius, float cor, Vector2 pos)
        {
            this.mass = mass;
            this.radius = radius;
            this.cor = cor;
            this.pos = pos;
            this.inertia = (mass * radius * radius) / 2;
        }

        public void SetPos(Vector2 pos)
        {
            this.pos = pos;
        }

        //Apply a force f at point p. p is in local coordinates
        public void AddForce(Vector2 f, Vector2 p)
        {
            force += f;

            Vector2 fPerp = new Vector2(f.Y, -f.X);
            torque += Vector2.Dot(fPerp, p);
        }

        public void ClearForces()
        {
            force = Vector2.Zero;
            torque = 0f;
        }
    }
}

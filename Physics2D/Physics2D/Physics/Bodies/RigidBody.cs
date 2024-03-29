﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Physics2D.Physics.Geometry;

namespace Physics2D.Physics.Bodies
{
    public abstract class RigidBody2D
    {
        //Linear properties
        private Vector2 pos;
        private Vector2 vel;
        private float invMass;
        private Vector2 force;

        //Rotational properties
        private float rot;
        private float rotVel;
        private float invInertia;
        private float torque;

        //Intersection properties
        protected AABB2D motionBounds;

        public Vector2 Pos
        {
            get
            {
                return pos;
            }
        }

        public float Rot
        {
            get
            {
                return rot;
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

        public float InvMass
        {
            get
            {
                return invMass;
            }
        }

        public float InvInertia
        {
            get
            {
                return invInertia;
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

        public AABB2D MotionBounds
        {
            get
            {
                return motionBounds;
            }
        }

        public RigidBody2D(Vector2 pos, Vector2 vel, float rotVel, float mass, float inertia)
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

            if (inertia == 0)
            {
                invInertia = 0;
            }
            else
            {
                invInertia = 1f / inertia;
            }

            this.pos = pos;
            this.vel = vel;
            this.rotVel = rotVel;

            GenerateMotionAABB(0f);
        }

        public void AddForce(Vector2 f)
        {
            force += f;
        }

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

        public abstract Contact GenerateContact(RigidBody2D rb, float dt);
        public abstract void GenerateMotionAABB(float dt);

        public void Integrate(float dt)
        {
            pos += vel * dt;
            rot += rotVel * dt;
        }

        public Vector2 GetVelocityOfPoint(Vector2 p)
        {
            return this.vel + (rotVel * GameMath.Perp(p - this.pos));
        }
    }
}

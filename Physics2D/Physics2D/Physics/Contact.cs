﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Physics2D.Physics.Bodies;

namespace Physics2D.Physics
{
    public struct Contact
    {
        public Vector2 Normal;
        public Vector2 pointA, pointB;
        public float Dist;
        public float Impulse;
        public RigidBody2D A, B;

        public Contact(Vector2 normal, float d, RigidBody2D a, RigidBody2D b, Vector2 pointA, Vector2 pointB)
        {
            Normal = normal;
            Dist = d;
            A = a;
            B = b;
            this.pointA = pointA;
            this.pointB = pointB;

            //I = (1+e)*N*(Vr • N) / (1/Ma + 1/Mb)
            //Impulse = (Vector2.Dot(b.Vel - a.Vel, normal) / (a.InvMass + b.InvMass));
            Impulse = 0;
        }

        public void ApplyImpulses(Vector2 imp)
        {
            A.Vel = A.Vel + (imp * A.InvMass);
            B.Vel = B.Vel - (imp * B.InvMass);
        }
    }
}

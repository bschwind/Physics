using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Physics2D.Physics.Bodies;

namespace Physics2D.Physics.Constraints
{
    public abstract class Constraint
    {
        protected RigidBody2D BodyA;
        protected RigidBody2D BodyB;

        public Constraint(RigidBody2D bodyA, RigidBody2D bodyB)
        {
            BodyA = bodyA;
            BodyB = bodyB;
        }

        public void ApplyImpulse(Vector2 impulse)
        {
            BodyA.Vel += impulse * BodyA.InvMass;
            BodyB.Vel -= impulse * BodyB.InvMass;
        }

        public abstract void Solve(float dt);
    }
}

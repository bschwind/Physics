using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Physics2D.Physics.Bodies;

namespace Physics2D.Physics
{
    public class PhysicsEngine
    {
        private Vector2 gravity = new Vector2(0, -0.1f);
        private List<RigidBody2D> bodies;
        List<Contact> contacts = new List<Contact>();

        public PhysicsEngine()
        {
            bodies = new List<RigidBody2D>();
        }

        public void AddRigidBody(RigidBody2D rb)
        {
            bodies.Add(rb);
        }

        public List<RigidBody2D> GetBodies()
        {
            return bodies;
        }

        public void SetGravity(Vector2 g)
        {
            gravity = g;
        }

        public void Update(GameTime g)
        {
            float dt = Math.Max((float)g.ElapsedGameTime.TotalSeconds, 1f / 60);

            //Apply gravity to each body
            //Also apply external forces here, such as player input
            foreach (RigidBody2D rb in bodies)
            {
                //Only apply to moving objects
                if (rb.InvMass > 0)
                {
                    rb.Vel = rb.Vel + gravity;
                }
            }

            //Detect and resolve contacts
            contacts.Clear();
            //Use a triangular loop to find contacts and prevent double contacts
            for (int i = 0; i < bodies.Count-1; i++)
            {
                RigidBody2D a = bodies[i];
                for (int j = i + 1; j < bodies.Count; j++)
                {
                    if (bodies[i].InvMass != 0 || bodies[j].InvMass != 0)
                    {
                        RigidBody2D b = bodies[j];
                        //Add a speculative contact
                        contacts.Add(a.GenerateContact(b, dt));
                    }
                }
            }

            Solver.Solve(contacts, 1, dt);

            //Integrate
            foreach (RigidBody2D rb in bodies)
            {
                rb.Integrate(dt);
            }
        }
    }
}

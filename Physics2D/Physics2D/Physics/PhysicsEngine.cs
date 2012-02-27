using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Physics2D.Physics.Bodies;
using Physics2D.Physics.Geometry;
using Physics2D.Physics.Partitions;

namespace Physics2D.Physics
{
    public class PhysicsEngine
    {
        private Vector2 gravity = new Vector2(0, -0.1f);
        private List<RigidBody2D> bodies;
        private List<RigidBody2D> lines;
        private List<Contact> contacts = new List<Contact>();
        private Partition partition;
        private AABB2D merged = new AABB2D();

        public PhysicsEngine()
        {
            bodies = new List<RigidBody2D>();
            lines = new List<RigidBody2D>();

            //By default, use a grid partition
            partition = new GridPartition(Vector2.Zero, new Vector2(20, 10), 40, 40);
        }

        public PhysicsEngine(Partition p)
        {
            partition = p;
        }

        public void AddRigidBody(RigidBody2D rb)
        {
            if (rb as LineBody != null)
            {
                lines.Add(rb);
            }
            else
            {
                bodies.Add(rb);
            }
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
            //merged = bodies[0].MotionBounds;
            //Apply gravity to each body
            //Also apply external forces here, such as player input
            foreach (RigidBody2D rb in bodies)
            {
                //Only apply to moving objects
                if (rb.InvMass > 0)
                {
                    //Add in gravity, as well as any forces applied to our object
                    rb.Vel = rb.Vel + gravity + (rb.Force * rb.InvMass);
                }
                if (rb.InvInertia > 0)
                {
                    rb.RotVel = rb.RotVel + (rb.Torque * rb.InvInertia);
                }

                rb.ClearForces();
                rb.GenerateMotionAABB(dt);
                //merged = AABB2D.CreateMerged(merged, rb.MotionBounds);
            }

            //merged.Inflate(1f);

            //(partition as GridPartition).UpdateMinMax(merged.GetMin(), merged.GetMax());

            //Detect and resolve contacts
            contacts.Clear();

            //Use a triangular loop to find contacts and prevent double contacts
            /*for (int i = 0; i < bodies.Count-1; i++)
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
            }*/

            partition.GenerateContacts(ref bodies, ref contacts, dt);

            for (int i = 0; i < bodies.Count; i++)
            {
                for (int j = 0; j < lines.Count; j++)
                {
                    contacts.Add(bodies[i].GenerateContact(lines[j], dt));
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

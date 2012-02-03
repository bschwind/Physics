using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Physics2D.Physics.Bodies;

namespace Physics2D.Physics
{
    public class Solver
    {
        public static void Solve(List<Contact> contacts, int iterations, float dt)
        {
            int numSolved = 0;
            bool SpecSequential = false;
            for (int j = 0; j < iterations; j++)
            {
                for (int i = 0; i < contacts.Count; i++)
                {
                    Contact con = contacts[i];
                    Vector2 n = con.Normal;

                    float relNv = Vector2.Dot(con.B.Vel - con.A.Vel, n);

                    //Do either speculative or speculative sequential
                    if (!SpecSequential)
                    {
                        float remove = relNv + con.Dist / dt;

                        if (remove < 0)
                        {
                            float mag = remove / (con.A.InvMass + con.B.InvMass);
                            Vector2 imp = con.Normal * mag;
                            con.ApplyImpulses(imp);
                            numSolved++;
                        }
                    }
                    else
                    {

                    }
                }
            }
        }
    }
}

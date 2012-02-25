using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Physics2D.Physics;
using Physics2D.Physics.Bodies;

namespace Physics2D.Physics.Partitions
{
    public abstract class Partition
    {
        public abstract void GenerateContacts(ref List<RigidBody2D> bodies, ref List<Contact> contacts);
    }
}

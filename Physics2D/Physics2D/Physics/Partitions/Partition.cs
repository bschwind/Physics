using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Physics2D.Physics;
using Physics2D.Physics.Bodies;

namespace Physics2D.Physics.Partitions
{
    //An abstract class for an object which takes a list of bodies and generates a list of contacts
    //For example, a grid, quadtree, octree, sweep-and-prune, bounding volume hierarchy, etc.
    public abstract class Partition
    {
        public abstract void GenerateContacts(ref List<RigidBody2D> bodies, ref List<Contact> contacts, float dt);
    }
}

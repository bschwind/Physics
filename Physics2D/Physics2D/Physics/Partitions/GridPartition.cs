using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Physics2D.Physics.Partitions
{
    public class GridPartition : Partition
    {
        public override void GenerateContacts(ref List<Bodies.RigidBody2D> bodies, ref List<Contact> contacts)
        {
            throw new NotImplementedException();
        }
    }
}

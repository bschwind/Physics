using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Physics2D.Physics
{
    public struct LineSegment
    {
        public Vector2 Start, End;

        public LineSegment(Vector2 start, Vector2 end)
        {
            Start = start;
            End = end;
        }
    }
}

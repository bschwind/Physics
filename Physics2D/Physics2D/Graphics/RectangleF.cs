using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Physics2D.Graphics
{
    public struct RectangleF
    {
        public Vector2 UpperLeft, BottomRight;

        public RectangleF(Vector2 upperLeft, Vector2 bottomRight)
        {
            UpperLeft = upperLeft;
            BottomRight = bottomRight;
        }
    }
}

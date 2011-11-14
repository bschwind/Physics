using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Physics2D.Input;

namespace Physics2D.Graphics
{
    //Used for keeping track of the view information
    public class Camera
    {
        protected Matrix view, projection;
        public Camera()
        {

        }

        public Matrix View
        {
            get
            {
                return view;
            }
        }

        public Matrix Projection
        {
            get
            {
                return projection;
            }
        }

        public virtual void Update(GameTime g)
        {
            
        }
    }
}

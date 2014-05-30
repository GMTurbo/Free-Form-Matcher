using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NsNodes
{
    public interface IRotatable
    {
        /// <summary>
        /// Give it point to rotate about (x,y,z) and an angle (in degrees)
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="angle"></param>
        void Rotate(double x, double y, double z, double angle);
    }
}

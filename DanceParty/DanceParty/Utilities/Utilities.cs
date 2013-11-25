using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace DanceParty.Utilities
{
    public static class Utilities
    {
        private static Random _random = new Random();
        public static float GetRandomFloat()
        {
            return (float)_random.NextDouble();
        }

        // Copy a matrix to avoid the garbage collecting of array copy.
        public static void CopyMatrix(ref Matrix src, ref Matrix dest)
        {
            dest.M11 = src.M11;
            dest.M12 = src.M12;
            dest.M13 = src.M13;
            dest.M14 = src.M14;

            dest.M21 = src.M21;
            dest.M22 = src.M22;
            dest.M23 = src.M23;
            dest.M24 = src.M24;

            dest.M31 = src.M31;
            dest.M32 = src.M32;
            dest.M33 = src.M33;
            dest.M34 = src.M34;

            dest.M41 = src.M41;
            dest.M42 = src.M42;
            dest.M43 = src.M43;
            dest.M44 = src.M44;
        }
    }
}

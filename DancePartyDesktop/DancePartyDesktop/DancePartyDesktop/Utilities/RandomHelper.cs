using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DanceParty.Utilities
{
    public static class RandomHelper
    {
        private static Random _random = new Random();
        public static float GetRandomFloat()
        {
            return (float)_random.NextDouble();
        }

        public static int GetRandomInt(int min, int max)
        {
            int ret = _random.Next(min, max);
            return ret;
        }
    }
}

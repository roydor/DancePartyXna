using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using DanceParty.Actors.DancerBehaviors;

using Utilities = DanceParty.Utilities.Utilities;

namespace DanceParty.Actors
{
    public class DancerEmitter
    {
        private Random _random = new Random();

        public Dancer EmitDancer()
        {
            float angle = ((float)_random.NextDouble()) * MathHelper.TwoPi;
            Dancer newDancer = DancerFactory.Instance.GetRandomDancer();

            newDancer.Position.X = 1300 * (float)Math.Sin(angle);
            newDancer.Position.Z = 1300 * (float)Math.Cos(angle);

            newDancer.SetDancerBehavior(new EnterFloorDancerBehavior(newDancer));

            return newDancer;
        }
    }
}

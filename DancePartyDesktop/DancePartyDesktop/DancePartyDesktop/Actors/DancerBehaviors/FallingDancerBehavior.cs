using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace DanceParty.Actors.DancerBehaviors
{
    public class FallingDancerBehavior : IDancerBehavior
    {
        public FallingDancerBehavior(Dancer dancer)
        {
            dancer.SetAnimation("Falling");
            dancer.EnqueueAnimation("Hurting");
        }

        public void Update(GameTime gameTime)
        {
        }

        public bool IsHostile()
        {
            return false;
        }
    }
}

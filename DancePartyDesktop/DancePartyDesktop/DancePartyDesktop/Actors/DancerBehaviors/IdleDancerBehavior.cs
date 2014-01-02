using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace DanceParty.Actors.DancerBehaviors
{
    public class IdleDancerBehavior : IDancerBehavior
    {
        public IdleDancerBehavior(Dancer dancer)
        {
            dancer.SetAnimation("Dancing");
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

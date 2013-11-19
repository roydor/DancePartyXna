using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace DanceParty.Actors.DancerBehaviors
{
    public class LeadDancerBehavior : IDancerBehavior
    {
        public static float LeadDancerWalkSpeed = 200f;
        private Dancer _dancer;

        public LeadDancerBehavior(Dancer dancer)
        {
            _dancer = dancer;
            _dancer.SetAnimation("Leader");
        }

        public void Update(GameTime gameTime)
        {
            _dancer.Position = _dancer.Position + _dancer.Forward * LeadDancerWalkSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }
    }
}

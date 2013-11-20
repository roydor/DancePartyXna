using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace DanceParty.Actors.DancerBehaviors
{
    public class JoinLineDancerBehavior : IDancerBehavior
    {
        public static float JoiningLineWalkSpeed = LeadDancerBehavior.LeadDancerWalkSpeed * 1.5f;

        private Dancer _following;
        private Dancer _me;

        public JoinLineDancerBehavior(Dancer me, Dancer following)
        {
            _me = me;
            _following = following;
            _me.SetAnimation("Walking");
        }

        public void Update(GameTime gameTime)
        {
            Vector3 displacement = _following.Position - _me.Position;
            _me.Forward = Vector3.Normalize(displacement);

            float stepLength = (float)gameTime.ElapsedGameTime.TotalSeconds * JoiningLineWalkSpeed;

            // Square roots are expensive.
            // If we would pass, or arrive at our destination this step, grab a new waypoint.
            if (stepLength * stepLength >= displacement.LengthSquared())
            {
                _me.SetDancerBehavior(new FollowingDancerBehavior(_me, _following));
                return;
            }

            _me.Position += _me.Forward * stepLength;
        }
    }
}

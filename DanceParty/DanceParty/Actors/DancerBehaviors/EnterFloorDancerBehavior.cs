using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace DanceParty.Actors.DancerBehaviors
{
    public class EnterFloorDancerBehavior : IDancerBehavior
    {
        private Dancer _me;
        private Vector3 _destination;
        public static float WalkSpeed = 150f;

        public EnterFloorDancerBehavior(Dancer dancer)
        {
            _me = dancer;

            float angle = Utilities.Utilities.GetRandomFloat() * MathHelper.TwoPi;
            // TODO: Make this a global constant?
            float radius = Utilities.Utilities.GetRandomFloat() * 1200f;

            _destination = new Vector3();
            _destination.X = radius * (float)Math.Sin(angle);
            _destination.Y = 0;
            _destination.Z = radius * (float)Math.Cos(angle);

            _me.Forward = Vector3.Normalize(_destination - _me.Position);
            _me.SetAnimation("Walking");
        }

        public void Update(GameTime gameTime)
        {
            Vector3 displacement = _destination - _me.Position;

            float stepLength = (float)gameTime.ElapsedGameTime.TotalSeconds * WalkSpeed;

            // Square roots are expensive.
            // If we would pass, or arrive at our destination this step, grab a new waypoint.
            if (stepLength * stepLength >= displacement.LengthSquared())
            {
                _me.SetDancerBehavior(new IdleDancerBehavior(_me));
                return;
            }

            _me.Position += _me.Forward * stepLength;
        }

        public bool IsHostile()
        {
            return false;
        }
    }
}

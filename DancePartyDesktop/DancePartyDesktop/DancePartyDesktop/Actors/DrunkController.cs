using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace DanceParty.Actors
{
    public class DrunkController
    {
        private float _currentDrunkFactor;
        private int _desiredFactor;

        public int GetDrunkFactor()
        {
            return (int) Math.Round(_currentDrunkFactor);
        }

        public void AddDrink(int drinkValue)
        {
            _desiredFactor += drinkValue;
        }

        private Dancer _leadDancer;
        private float _drunkTime;

        public DrunkController(Dancer leadDancer)
        {
            _leadDancer = leadDancer;
        }

        public void Update(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (_desiredFactor < 0)
                _desiredFactor = 0;

            _currentDrunkFactor = MathHelper.Lerp(_currentDrunkFactor, (float)_desiredFactor, dt / 2);

            _drunkTime += dt;

            if (_drunkTime > MathHelper.TwoPi)
            {
                _desiredFactor -= 6;
                _drunkTime -= MathHelper.TwoPi;
            }

            _leadDancer.Forward = Vector3.Transform(
                _leadDancer.Forward,
                Matrix.CreateRotationY(
                    (float)( (_currentDrunkFactor / 500) * Math.Sin(_drunkTime) * gameTime.ElapsedGameTime.TotalSeconds)));
        }
    }
}

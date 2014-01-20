using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

using DanceParty.Cameras;

namespace DanceParty.Actors
{
    public class DrunkController
    {
        private float _currentDrunkFactor;
        private int _desiredFactor;
        private SoundEffectInstance _sound;
        private PerspectiveCamera _camera;

        private float _secondTimer = 0;

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

        public DrunkController(Dancer leadDancer, PerspectiveCamera camera)
        {
            _sound = SoundManager.Instance.GetPopSoundEffect();
            _camera = camera;
            _leadDancer = leadDancer;
        }

        public void Update(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (_desiredFactor < 0)
                _desiredFactor = 0;

            // Lerp our way to our goal.
            int prevFactor = GetDrunkFactor();
            _currentDrunkFactor = MathHelper.Lerp(_currentDrunkFactor, (float)_desiredFactor, dt / 2);

            float drunkRatio = _currentDrunkFactor / 255;

            // Play a sound if it increased.
            if (prevFactor < GetDrunkFactor())
            {
                //_sound.Stop(true);
                _sound.Pitch = 2 * MathHelper.Clamp(_currentDrunkFactor / (_desiredFactor), 0, 1f) - 1;
                _sound.Play();
            }

            _drunkTime += dt;
            _secondTimer += dt;

            if (_secondTimer > 1)
            {
                _desiredFactor--;
                _secondTimer = 0;
            }

            // Mess with the camera if you're drunk
            _camera.Up = Vector3.Transform(Vector3.Up, Matrix.CreateRotationZ((float)(-0.3f * drunkRatio * Math.Sin(0.8 * _drunkTime))));
            _camera.FieldOfViewScale = 1 + (float)(0.1 * drunkRatio * Math.Sin(0.5 * _drunkTime));
            _camera.AspectScale = 1 + (float)(0.1 * drunkRatio * Math.Cos(2 * _drunkTime));

            _leadDancer.Forward = Vector3.Transform(
                _leadDancer.Forward,
                Matrix.CreateRotationY(
                    (float)( (_currentDrunkFactor / 500) * Math.Sin(_drunkTime) * gameTime.ElapsedGameTime.TotalSeconds)));
        }
    }
}

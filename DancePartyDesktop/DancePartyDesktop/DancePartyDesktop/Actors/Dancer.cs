using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SkinnedModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using DanceParty.Utilities;
using DanceParty.Cameras;
using DanceParty.Actors.DancerBehaviors;

namespace DanceParty.Actors
{
    public class Dancer
    {
        /// <summary>
        /// We know that the models from blender are in a different orientation and scaled 
        /// very small (1 unit = 1 meter, Z is up)
        /// </summary>
        public static Matrix ModelAdjustment =
            Matrix.CreateScale(100) *
            Matrix.CreateRotationZ(-MathHelper.Pi) *
            Matrix.CreateRotationX(-MathHelper.PiOver2);

        /// <summary>
        /// The animation controller for this dancer.
        /// </summary>
        public AnimationPlayer _animationPlayer;

        /// <summary>
        /// The position of this dancer.
        /// </summary>
        public Vector3 Position;

        /// <summary>
        /// The direction the dancer is facing.
        /// </summary>
        public Vector3 Forward;

        /// <summary>
        /// The direction which is 'up' for this dancer.
        /// </summary>
        public Vector3 Up;

        /// <summary>
        /// The world matrix for this dancer.
        /// </summary>
        private Matrix _worldMatrix;
        public Matrix WorldMatrix
        {
            get
            {
                return _worldMatrix;
            }
        }

        /// <summary>
        /// The behavior that this dancer is currently executing.
        /// </summary>
        private IDancerBehavior _dancerBehavior;

        private AnimatedModelInstance _modelInstance;

        public Dancer(AnimatedModelInstance modelInstance)
        {
            _modelInstance = modelInstance;
            _animationPlayer = new AnimationPlayer(AnimationManager.Instance.GetAnimationData());

            Position = Vector3.Zero;
            Forward = Vector3.UnitZ;
            Up = Vector3.UnitY;

            _worldMatrix = Matrix.CreateWorld(Position, Forward, Up);
            _dancerBehavior = new IdleDancerBehavior(this);
            _modelInstance.SkinTransforms = _animationPlayer.GetSkinTransforms();
        }


        public void Update(GameTime gameTime)
        {
            _dancerBehavior.Update(gameTime);

            // Update the world transform of this dancer.
            Matrix.CreateWorld(ref Position, ref Forward, ref Up, out _worldMatrix);

            // Update the animation of this dancer.
            _animationPlayer.Update(gameTime.ElapsedGameTime, true, ModelAdjustment * _worldMatrix);
            _modelInstance.SkinTransforms = _animationPlayer.GetSkinTransforms();
        }

        public void SetAnimation(string animationName)
        {
            _animationPlayer.StartClip(animationName);
        }

        public void EnqueueAnimation(string animationName)
        {
            _animationPlayer.EnqueueAnimation(animationName);
        }

        public void SetDancerBehavior(IDancerBehavior behavior)
        {
            _dancerBehavior = behavior;
        }

        public bool CollidesWith(Dancer dancer)
        {
            float dist2 = Vector3.DistanceSquared(Position, dancer.Position);

            if (dancer.IsHostile())
                return dist2 < 8000;

            return dist2 < 10000;
        }

        public bool CollidesWith(Drink drink)
        {
            return Vector3.DistanceSquared(Position, drink.Position) < 10000;
        }

        public bool IsHostile()
        {
            return _dancerBehavior.IsHostile();
        }
    }
}

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

        /// <summary>
        /// All the accessories this dancer is wearing.
        /// </summary>
        private List<Accessory> _accessories;

        private AnimatedModelInstance _modelInstance;

        public Dancer(AnimatedModelInstance modelInstance)
        {
            _modelInstance = modelInstance;
            _animationPlayer = new AnimationPlayer((SkinningData)_modelInstance.OriginalModel.Tag);

            _accessories = new List<Accessory>();

            Position = Vector3.Zero;
            Forward = Vector3.UnitZ;
            Up = Vector3.UnitY;

            _worldMatrix = Matrix.CreateWorld(Position, Forward, Up);
            _dancerBehavior = new IdleDancerBehavior(this);
            _modelInstance.SkinTransforms = _animationPlayer.GetSkinTransforms();
        }

        public void AddAccessory(Accessory accessory)
        {
            _accessories.Add(accessory);
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

        /// <summary>
        /// Draws this dancer to the screen using the passed in camera.
        /// </summary>
        public void Draw(PerspectiveCamera camera)
        {
        }
        //    // Get the transformed positions of all the bones.
        //    Matrix[] bones = _animationPlayer.GetSkinTransforms();
        //    Matrix[] worldBones = _animationPlayer.GetWorldTransforms();

        //    // Draw the model. A model can have multiple meshes, so loop.
        //    foreach (ModelMesh mesh in _model.Meshes)
        //    {
        //        foreach (SkinnedEffect effect in mesh.Effects)
        //        {
        //            effect.EnableDefaultLighting();

        //            effect.SetBoneTransforms(bones);
        //            effect.World = _worldMatrix;

        //            effect.View = camera.ViewMatrix;
        //            effect.Projection = camera.ProjectionMatrix;

        //            effect.Texture = _skin;
        //        }

        //        // Draw the mesh, using the effects set above.
        //        mesh.Draw();
        //    }

        //    /*
        //    foreach (Accessory accessory in _accessories)
        //    {
        //        // Accessory may have no model, so nothing to draw.
        //        // (Hair?)
        //        if (accessory.Model == null)
        //            continue;

        //        // Which bone is it attached to?
        //        Matrix boneTransform = _animationPlayer.GetWorldTransformForBone(accessory.AttachedToBone);

        //        // Draw the accessory
        //        foreach (ModelMesh mesh in accessory.Model.Meshes)
        //        {
        //            foreach (BasicEffect effect in mesh.Effects)
        //            {
        //                effect.EnableDefaultLighting();

        //                // Attach it to the correct bone's world transform.
        //                effect.World = boneTransform * _worldMatrix;

        //                effect.View = camera.ViewMatrix;
        //                effect.Projection = camera.ProjectionMatrix;

        //                effect.TextureEnabled = true;
        //                effect.Texture = accessory.Skin;
        //            }

        //            // Draw the mesh, using the effects set above.
        //            mesh.Draw();
        //        }
            

        //    }*/
        //}

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
            return Vector3.DistanceSquared(Position, dancer.Position) < 10000;
        }

        public bool IsHostile()
        {
            return _dancerBehavior.IsHostile();
        }
    }
}

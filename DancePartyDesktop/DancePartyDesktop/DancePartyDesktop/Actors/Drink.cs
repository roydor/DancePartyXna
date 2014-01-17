using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using DanceParty.Cameras;
namespace DanceParty.Actors
{
    public class Drink
    {
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

        private Matrix _rotation;

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

        private Model _model;

        public Drink(Model drinkModel)
        {
            _model = drinkModel;
            Position = Vector3.Zero;
            Up = Vector3.Up;
            Forward = Vector3.Forward;
            _rotation = new Matrix();
        }

        public void Update(GameTime gameTime)
        {
            Matrix.CreateRotationY((float)(gameTime.ElapsedGameTime.TotalSeconds), out _rotation);
            Forward = Vector3.Transform(Forward, _rotation);
            Matrix.CreateWorld(ref Position, ref Forward, ref Up, out _worldMatrix);
        }

        public void Draw(PerspectiveCamera camera)
        {
            foreach (ModelMesh mesh in _model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = Dancer.ModelAdjustment * _worldMatrix;
                    effect.Projection = camera.ProjectionMatrix;
                    effect.View = camera.ViewMatrix;
                    effect.EnableDefaultLighting();
                }

                mesh.Draw();
                DancePartyGame.DrawsPerFrame++;
            }
        }
    }
}


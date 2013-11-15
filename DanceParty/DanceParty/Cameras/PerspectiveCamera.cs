using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DanceParty.Cameras
{
    public class PerspectiveCamera
    {
        internal const float PerspectiveFieldOfView = MathHelper.PiOver4; //45 degrees
        internal const float NearPlane = 0.1f;
        internal const float FarPlane = 5000.0f;

        /// <summary>
        /// The position of this camera.
        /// </summary>
        public Vector3 Position;

        /// <summary>
        /// The point at which this camera is looking
        /// </summary>
        public Vector3 LookAt;

        /// <summary>
        /// Pitch Yaw Roll
        /// </summary>
        public Vector3 Rotation;
        public Vector3 Up;
        public Vector3 Forward;

        private Matrix _projectionMatrix;
        public Matrix ProjectionMatrix 
        { 
            get 
            { 
                return _projectionMatrix; 
            } 
        }

        private Matrix _viewMatrix;
        public Matrix ViewMatrix 
        { 
            get 
            { 
                return _viewMatrix;
            } 
        }

        private Matrix _rotationMatrix;
        public Matrix RotationMatrix 
        { 
            get 
            { 
                return _rotationMatrix; 
            } 
        }

        public GraphicsDevice GraphicsDevice;

        /// <summary>
        /// Creates a new PerspectiveCamera with 45 degree field of view.
        /// </summary>
        /// <param name="device"></param>
        public PerspectiveCamera(GraphicsDevice device)
        {
            Up = Vector3.Up;
            Forward = Vector3.Forward;
            GraphicsDevice = device;

            Position = Vector3.Zero;
            LookAt = Position + Forward;

            _projectionMatrix = 
                Matrix.CreatePerspectiveFieldOfView(
                    PerspectiveFieldOfView,
                    GraphicsDevice.Viewport.AspectRatio,
                    NearPlane,
                    FarPlane);  

            // _projectionMatrix = Matrix.CreateOrthographic(30.0f, 30.0f, NearPlane, FarPlane);

            _viewMatrix = Matrix.CreateLookAt(Position, LookAt, Up);
            _rotationMatrix = Matrix.Identity;
        }

        /// <summary>
        /// Updates the ViewMatrix of this camera.
        /// </summary>
        public void Update()
        {
            Matrix.CreateLookAt(ref Position, ref LookAt, ref Up, out _viewMatrix);
            Matrix.CreateRotationX(Rotation.X, out _rotationMatrix);

            // pitch = rotation about X
            // yaw = rotation about y
            // roll = rotation about z
            Matrix.CreateFromYawPitchRoll(Rotation.Y, Rotation.X, Rotation.Z, out _rotationMatrix);

            Matrix.CreatePerspectiveFieldOfView(
                PerspectiveFieldOfView,
                GraphicsDevice.Viewport.AspectRatio,
                NearPlane,
                FarPlane,
                out _projectionMatrix);
        }
    }
}

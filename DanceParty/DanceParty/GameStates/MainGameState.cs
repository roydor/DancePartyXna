using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;

using DanceParty.Actors;
using DanceParty.Actors.DancerBehaviors;
using DanceParty.Utilities;
using DanceParty.Utilities.Accelerometer;
using DanceParty.Cameras.CameraControllerBehaviors;
using DanceParty.GameStates;
using SkinnedModel;
using DanceParty.Cameras;

namespace DanceParty.GameStates
{
    public class MainGameState : IGameState
    {
        #region Dependencies
        private GraphicsDevice _graphicsDevice;
        private SpriteBatch _spriteBatch;
        private ContentManager _contentManager;
        #endregion

        private CongaLine _congaLine;
        private DancerEmitter _dancerEmitter;
        private CameraController _cameraController;
        private IAccelerometerWrapper _accelerometer;
        private List<Dancer> _dancers;
        private bool _isLoaded = false;

        private static MainGameState _instance;
        public static MainGameState Instance
        {
            get { return _instance ?? 
                (_instance = new MainGameState(
                    DancePartyGame.Instance.GraphicsDevice,
                    DancePartyGame.Instance.SpriteBatch,
                    DancePartyGame.Instance.Content)); }
        }

        private MainGameState(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, ContentManager contentManager)
        {
            _graphicsDevice = graphicsDevice;
            _spriteBatch = spriteBatch;
            _contentManager = contentManager;

            _dancers = new List<Dancer>();
            _dancerEmitter = new DancerEmitter();
            _accelerometer = AccelerometerFactory.GetAccelerometer();
        }

        /// <summary>
        /// Loads all the content for this State.
        /// You should show a loading screen here, it may take a while.
        /// </summary>
        public void LoadContent()
        {
            BatchedModelManager.Instance.LoadContent(_contentManager, _graphicsDevice);

            Dancer leader = DancerFactory.Instance.GetRandomDancer();
            leader.SetDancerBehavior(new LeadDancerBehavior(leader));
            _congaLine = new CongaLine(leader);

            PerspectiveCamera camera = new PerspectiveCamera(_graphicsDevice);

            _cameraController = new CameraController(camera);
            _cameraController.SetCameraBehavior(new BehindViewBehavior(camera, _congaLine.LeadDancer));

            _accelerometer.Start();
            GC.Collect();
            _isLoaded = true;
        }

        public void Update(GameTime gameTime)
        {
            if (!_isLoaded)
            {
                GameStateManager.Instance.EnqueueGameState(LoadingStateFactory.GetLoadingState(LoadContent));
                return;
            }

            _congaLine.LeadDancer.Forward =
                Vector3.Transform(
                    _congaLine.LeadDancer.Forward,
                    Matrix.CreateRotationY(
                        2 * _accelerometer.CurrentReading.Y *
                        (float)gameTime.ElapsedGameTime.TotalSeconds));

            // Update dancers on the floor.
            foreach (Dancer dancer in _dancers)
                dancer.Update(gameTime);

            // Update the conga line.
            _congaLine.Update(gameTime);

            // Does the line collide with anyone
            for (int i = 0; i < _dancers.Count; i++)
            {
                if (_dancers[i].CollidesWith(_congaLine.LeadDancer))
                {
                    _congaLine.AppendDancer(_dancers[i]);
                    _dancers.RemoveAt(i--);
                }
            }

            // Add a new dancer if there is less than 15
            if (_dancers.Count < 15)
                _dancers.Add(_dancerEmitter.EmitDancer());

            if (_congaLine.CollidesWithSelf())
                GameOver();

            // Out of bounds?
            if (_congaLine.LeadDancer.Position.Length() > 1500f)
                GameOver();

            _cameraController.Update(gameTime);


            // Process touch events
            TouchCollection touchCollection = TouchPanel.GetState();
            foreach (TouchLocation tl in touchCollection)
            {
                if (tl.State == TouchLocationState.Pressed && _congaLine.HasStopped)
                    GameStateManager.Instance.PopGameState();
            }
        }

        // Stop the game.
        public void GameOver()
        {
            _congaLine.Stop();
        }

        // Reset the game.
        public void Reset()
        {
            if (!_isLoaded)
                return;

            BatchedModelManager.Instance.ClearInstances();
            _congaLine = new CongaLine(DancerFactory.Instance.GetRandomDancer());
            _congaLine.LeadDancer.SetDancerBehavior(new LeadDancerBehavior(_congaLine.LeadDancer));
            _cameraController.SetCameraBehavior(new BehindViewBehavior(_cameraController.Camera, _congaLine.LeadDancer));
            _dancers.Clear();
        }

        public void Draw(GameTime gameTime)
        {
            if (!_isLoaded)
                return;

            SamplerState ss = new SamplerState();
            ss.AddressU = TextureAddressMode.Clamp;
            ss.AddressV = TextureAddressMode.Clamp;

            _graphicsDevice.Clear(Color.DarkSlateBlue);
            _graphicsDevice.BlendState = BlendState.Opaque;
            _graphicsDevice.DepthStencilState = DepthStencilState.Default;
            _graphicsDevice.SamplerStates[0] = ss;

            BatchedModelManager.Instance.DrawInstances(_cameraController.Camera);

            // Dont draw the dance floor this way later...
            // It should be the model for the room
            DrawDanceFloor(_cameraController.Camera);
        }

        private void DrawDanceFloor(PerspectiveCamera camera)
        {
            VertexPositionColor[] userPrimitives;
            BasicEffect basicEffect;

            //Create the vertices for traingle
            userPrimitives = new VertexPositionColor[31];

            for (int i = 0; i < 31; i++)
            {
                float angle = MathHelper.TwoPi * (i / 30.0f);
                userPrimitives[i] = new VertexPositionColor();
                userPrimitives[i].Position = new Vector3(1500 * (float)Math.Sin(angle), 1, 1500 * (float)Math.Cos(angle));
                userPrimitives[i].Color = Color.Red;
            }

            //Create new basic effect and properties
            basicEffect = new BasicEffect(_graphicsDevice);
            basicEffect.World = Matrix.Identity;
            basicEffect.View = camera.ViewMatrix;
            basicEffect.Projection = camera.ProjectionMatrix;
            basicEffect.VertexColorEnabled = true;

            //Start using the BasicEffect
            basicEffect.CurrentTechnique.Passes[0].Apply();
            //Draw the primitives
            _graphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineStrip, userPrimitives, 0, 30);
        }

        public void Dispose()
        {
            _accelerometer.Stop();
        }
    }
}

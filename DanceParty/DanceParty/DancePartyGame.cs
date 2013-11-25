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

using SkinnedModel;
using DanceParty.Cameras;

using DanceParty.Actors;
using DanceParty.Actors.DancerBehaviors;
using DanceParty.Utilities;
using DanceParty.Utilities.Accelerometer;
using DanceParty.Cameras.CameraControllerBehaviors;

namespace DanceParty
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class DancePartyGame : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        private CongaLine _congaLine;
        private DancerEmitter _dancerEmitter;

        private List<Dancer> _dancers;
        private FPSTracker _fpsTracker;
        private CameraController _cameraController;
        private IAccelerometerWrapper _accelerometer;

        public static int DrawsPerFrame = 0;

        private SpriteFont _segoeUI;

        public DancePartyGame()
        {
            graphics = new GraphicsDeviceManager(this);

            Content.RootDirectory = "Content";

            // Frame rate is 30 fps by default for Windows Phone.
#if WINDOWS_PHONE
            TargetElapsedTime = TimeSpan.FromTicks(333333);
#endif

            // Extend battery life under lock.
            InactiveSleepTime = TimeSpan.FromSeconds(1);

            _dancers = new List<Dancer>();
            _dancerEmitter = new DancerEmitter();
            _fpsTracker = new FPSTracker();
            _accelerometer = AccelerometerFactory.GetAccelerometer();
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            _segoeUI = Content.Load<SpriteFont>("Fonts\\SegoeUI");

            DancerFactory.Instance.LoadContent(Content, GraphicsDevice);

            Dancer leader = DancerFactory.Instance.GetRandomDancer();
            leader.SetDancerBehavior(new LeadDancerBehavior(leader));
            _congaLine = new CongaLine(leader);

            PerspectiveCamera camera = new PerspectiveCamera(GraphicsDevice);

            _cameraController = new CameraController(camera);
            _cameraController.SetCameraBehavior(new BehindViewBehavior(camera, _congaLine.LeadDancer));

            _accelerometer.Start();
            GC.Collect();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            _accelerometer.Stop();

            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            _congaLine.LeadDancer.Forward = 
                Vector3.Transform(
                    _congaLine.LeadDancer.Forward, 
                    Matrix.CreateRotationY(
                        2 * _accelerometer.CurrentReading.Y * 
                        (float)gameTime.ElapsedGameTime.TotalSeconds));

            foreach (Dancer dancer in _dancers)
                dancer.Update(gameTime);
             
            _congaLine.Update(gameTime);

            for (int i = 0; i < _dancers.Count; i++)
            {
                if (_dancers[i].CollidesWith(_congaLine.LeadDancer))
                {
                    _congaLine.AppendDancer(_dancers[i]);
                    _dancers.RemoveAt(i--);
                }
            }

            if (_dancers.Count < 15)
                _dancers.Add(_dancerEmitter.EmitDancer());


            if (_congaLine.CollidesWithSelf())
                GameOver();

            if (_congaLine.LeadDancer.Position.Length() > 1500f)
                GameOver();

            _cameraController.Update(gameTime);

            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back ==
                ButtonState.Pressed)
                this.Exit();
            
            base.Update(gameTime);
        }

        public void GameOver()
        {
            _congaLine.Stop();
        }

        public void Reset()
        {
        }

        protected override void Draw(GameTime gameTime)
        {
            SamplerState ss = new SamplerState();
            ss.AddressU = TextureAddressMode.Clamp;
            ss.AddressV = TextureAddressMode.Clamp;

            graphics.GraphicsDevice.Clear(Color.DarkCyan);
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.SamplerStates[0] = ss;

            DancerFactory.Instance.DrawInstances(_cameraController.Camera);

            // Dont draw the dance floor this way later...
            // It should be the model for the room
            DrawDanceFloor(_cameraController.Camera);

            spriteBatch.Begin();

            spriteBatch.DrawString(
                _segoeUI, 
                string.Format("FPS: {0}\r\nCollected Dancers: {1}\r\nAccelerometer: {2}\r\nDraws Per Frame: {3}", 
                    _fpsTracker.CurrentFPS, 
                    _congaLine.Count, 
                    _accelerometer.CurrentReading,
                    DrawsPerFrame), 
                Vector2.One, 
                Color.Purple);

            DrawsPerFrame = 0;
            spriteBatch.End();

            _fpsTracker.Update(gameTime);

            base.Draw(gameTime);
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
            basicEffect = new BasicEffect(GraphicsDevice);
            basicEffect.World = Matrix.Identity;
            basicEffect.View = camera.ViewMatrix;
            basicEffect.Projection = camera.ProjectionMatrix;
            basicEffect.VertexColorEnabled = true;

            //Start using the BasicEffect
            basicEffect.CurrentTechnique.Passes[0].Apply();
            //Draw the primitives
            GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineStrip, userPrimitives, 0, 30);

        }
    }
}

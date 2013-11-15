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
using DanceParty.Utilities;

using Microsoft.Devices.Sensors;

namespace DanceParty
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class DancePartyGame : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        private PerspectiveCamera _camera;
        private List<Dancer> _dancers;
        private FPSTracker _fpsTracker;
        private Accelerometer _accelerometer;

        public DancePartyGame()
        {
            graphics = new GraphicsDeviceManager(this);
 //           graphics.PreferredBackBufferWidth = 420;
//            graphics.PreferredBackBufferHeight = 240;

            Content.RootDirectory = "Content";

            // Frame rate is 30 fps by default for Windows Phone.
            TargetElapsedTime = TimeSpan.FromTicks(333333);

            // Extend battery life under lock.
            InactiveSleepTime = TimeSpan.FromSeconds(1);

            _dancers = new List<Dancer>();
            _fpsTracker = new FPSTracker();
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

        SpriteFont _segoeUI;

        Random r = new Random();

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            Model manModel = Content.Load<Model>("Models\\human_lowx");
            _segoeUI = Content.Load<SpriteFont>("Fonts\\SegoeUI");
            Texture2D manSkin = Content.Load<Texture2D>("Textures\\male0");

            for (int i = 0; i < numModels; i++)
            {
                for (int j = 0; j < numModels; j++)
                {
                    Dancer newDancer = new Dancer(manModel, manSkin);
                    newDancer.Position = new Vector3((i - numModels / 2) * 100, 0, (j - numModels / 2) * 100);

                    // Randomly update them so they're not in sync.
                    newDancer.Update(new GameTime(new TimeSpan(), new TimeSpan(0, 0, 0, r.Next(2), r.Next(2000))));

                    _dancers.Add(newDancer);
                }
            }

            _camera = new PerspectiveCamera(GraphicsDevice);
            _accelerometer = new Accelerometer();
            _accelerometer.Start();
            GC.Collect();
        }

        public int numModels = 16;

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
            foreach (Dancer dancer in _dancers)
                dancer.Update(gameTime);

            modelRotation += (float)gameTime.ElapsedGameTime.TotalSeconds * _accelerometer.CurrentValue.Acceleration.Y;
            _camera.Position.X = (float)(1000.0f * Math.Sin(modelRotation));
            _camera.Position.Y = 750f;
            _camera.Position.Z = (float)(1000.0f * Math.Cos(modelRotation));
            _camera.Update();

            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back ==
                ButtonState.Pressed)
                this.Exit();
            
            base.Update(gameTime);
        }

        float modelRotation = 0.0f;

        protected override void Draw(GameTime gameTime)
        {
            SamplerState ss = new SamplerState();
            ss.AddressU = TextureAddressMode.Clamp;
            ss.AddressV = TextureAddressMode.Clamp;

            graphics.GraphicsDevice.Clear(Color.CornflowerBlue);
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.SamplerStates[0] = ss;

            foreach (Dancer dancer in _dancers)
                dancer.Draw(_camera);

            spriteBatch.Begin();

            spriteBatch.DrawString(
                _segoeUI, 
                string.Format("FPS: {0}\r\nDancers: {1}\r\nAccelerometer: {2}", 
                    _fpsTracker.CurrentFPS, 
                    _dancers.Count, 
                    _accelerometer.CurrentValue.Acceleration), 
                Vector2.One, 
                Color.Purple);

            spriteBatch.End();

            _fpsTracker.Update(gameTime);

            base.Draw(gameTime);
        }

    }
}

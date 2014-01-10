using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using SkinnedModel;
using DanceParty.Cameras;

using DanceParty.Actors;
using DanceParty.Actors.DancerBehaviors;
using DanceParty.Utilities;
using DanceParty.Utilities.Accelerometer;
using DanceParty.Cameras.CameraControllerBehaviors;
using DanceParty.GameStates;
using DanceParty.Utilities.Threading;

namespace DanceParty
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class DancePartyGame : Microsoft.Xna.Framework.Game
    {
        private static DancePartyGame _instance;
        public static DancePartyGame Instance
        {
            get
            {
                return _instance;
            }
        }

        private GameStateManager _gameStateManager;

        public GraphicsDeviceManager GraphicsDeviceManager;
        public SpriteBatch SpriteBatch;

        private FPSTracker _fpsTracker;

        public static int DrawsPerFrame = 0;

        public DancePartyGame()
        {
            _instance = this;
            this.IsMouseVisible = true;
            GraphicsDeviceManager = new GraphicsDeviceManager(this);
            GraphicsDeviceManager.IsFullScreen = true;
//            GraphicsDeviceManager.PreferredBackBufferWidth = 800;
  //          GraphicsDeviceManager.PreferredBackBufferWidth = 480;
            _gameStateManager = GameStateManager.Instance;
            _fpsTracker = new FPSTracker();

            Content.RootDirectory = "Content";

            // Frame rate is 30 fps by default for Windows Phone.
#if WINDOWS_PHONE
            TargetElapsedTime = TimeSpan.FromTicks(333333);
#endif

            // Extend battery life under lock.
            InactiveSleepTime = TimeSpan.FromSeconds(1);
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            FontManager.Instance.LoadContent();

            _gameStateManager.EnqueueGameState(MainMenuState.Instance);
            _gameStateManager.EnqueueGameState(LoadingStateFactory.GetLoadingState(LoadAudio));
        }

        public void LoadAudio(LoadStateReporter loadStateReporter)
        {
            loadStateReporter.CurrentStatus = "Loading Audio";
            SoundManager.Instance.LoadAudio();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }
        
        protected override void Update(GameTime gameTime)
        {
            PointerInputManager.Instance.Update();

            _gameStateManager.ProcessStateActions();
            _gameStateManager.GetCurrentState().Update(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            DrawsPerFrame = 0;
           _gameStateManager.GetCurrentState().Draw(gameTime);
           _fpsTracker.Update(gameTime);

           SpriteBatch.Begin();
           SpriteBatch.DrawString(FontManager.Instance.BangersSmall, string.Format("FPS: {0}\r\nDraws: {1}", _fpsTracker.CurrentFPS, DrawsPerFrame), Vector2.Zero, Color.White);
           SpriteBatch.End();
            base.Draw(gameTime);
        }
    }
}

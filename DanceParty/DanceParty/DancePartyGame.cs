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
using DanceParty.GameStates;

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

        GraphicsDeviceManager graphics;
        public SpriteBatch SpriteBatch;

        private FPSTracker _fpsTracker;

        public static int DrawsPerFrame = 0;

        private SpriteFont _segoeUI;

        public DancePartyGame()
        {
            _instance = this;
            graphics = new GraphicsDeviceManager(this);
            _gameStateManager = GameStateManager.Instance;

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
            _gameStateManager.EnqueueGameState(MainGameState.Instance);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            _segoeUI = Content.Load<SpriteFont>("Fonts\\SegoeUI");
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
            _gameStateManager.ProcessStateActions();
            _gameStateManager.GetCurrentState().Update(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            _gameStateManager.GetCurrentState().Draw(gameTime);
            base.Draw(gameTime);
        }
    }
}

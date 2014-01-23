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

using DanceParty.Actors;
using DanceParty.Actors.DancerBehaviors;
using DanceParty.Utilities;
using DanceParty.Utilities.Accelerometer;
using DanceParty.Cameras.CameraControllerBehaviors;
using DanceParty.GameStates;
using SkinnedModel;
using DanceParty.Cameras;
using DanceParty.Utilities.Threading;

namespace DanceParty.GameStates
{
    public class MainGameState : IGameState
    {
        #region Dependencies
        private GraphicsDevice _graphicsDevice;
        private SpriteBatch _spriteBatch;
        private ContentManager _contentManager;
        #endregion
        public static float DanceFloorBoundary = 1350f;
        public static int MaxDancersOnFloor = 40;

        private CongaLine _congaLine;
        private DancerEmitter _dancerEmitter;
        private DrinkEmitter _drinkEmitter;
        
        private CameraController _cameraController;
        private IAccelerometerWrapper _accelerometer;
        private List<Dancer> _dancers;

        RenderTarget2D currentFrame;
        RenderTarget2D drunkFrame;
        
        private bool _isLoaded = false;
        private bool _beginGame = false;
        private bool _isGameOver = false;

        private Song _music;
        private SoundEffectInstance _loseSound;
        private SoundEffectInstance _winSound;

        private MainGameHud _mainHud;
        private GameOverMessage _gameOverHud;

        private Model _danceFloor;
        private Drink _drink;

        private ScoreComponent _score;

        // Radians per second.
        private const float KeyboardRotationSpeed = 1.0f;
        private const float AccelerometerRotationSpeed = 2.0f;

        public MainGameState(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, ContentManager contentManager)
        {
            _graphicsDevice = graphicsDevice;
            _spriteBatch = spriteBatch;
            _contentManager = contentManager;

            _dancers = new List<Dancer>();
            _dancerEmitter = new DancerEmitter();
            _drinkEmitter = new DrinkEmitter();

            _accelerometer = AccelerometerFactory.GetAccelerometer();
        }

        /// <summary>
        /// Loads all the content for this State.
        /// You should show a loading screen here, it may take a while.
        /// </summary>
        public void LoadContent(LoadStateReporter loadStateReporter)
        {
            _isLoaded = false;
            loadStateReporter.CurrentStatus = "Loading Models";
            BatchedModelManager.Instance.LoadContent(_contentManager, _graphicsDevice);
            BatchedModelManager.Instance.ClearInstances();

            loadStateReporter.CurrentStatus = "Loading Animation";
            AnimationManager.Instance.LoadContent();

            loadStateReporter.CurrentStatus = "Creating CongaLine";

            PerspectiveCamera camera = new PerspectiveCamera(_graphicsDevice);

            Dancer leader = DancerFactory.Instance.GetRandomDancer();
            leader.SetDancerBehavior(new LeadDancerBehavior(leader));
            _congaLine = new CongaLine(leader, camera);

            _cameraController = new CameraController(camera);
            _cameraController.SetCameraBehavior(new BehindViewBehavior(camera, _congaLine.LeadDancer));

            _accelerometer.Start();

            loadStateReporter.CurrentStatus = "Requesting DJ to play a song";
            _music = SoundManager.Instance.GetRandomSong();
            _loseSound = SoundManager.Instance.GetRecordScratchInstance();
            _winSound = SoundManager.Instance.GetApplauseSoundEffect();

            _mainHud = new MainGameHud(_graphicsDevice, _spriteBatch, FontManager.Instance.BangersMedium);

            loadStateReporter.CurrentStatus = "Loading DanceRoom";
            _danceFloor = _contentManager.Load<Model>("Models\\DanceRoom");
            _drinkEmitter.LoadContent();

            //Instance our first Render Target to the same size as the back buffer.
            currentFrame = new RenderTarget2D(_graphicsDevice,
                _graphicsDevice.PresentationParameters.BackBufferWidth ,
                _graphicsDevice.PresentationParameters.BackBufferHeight, 
                false, 
                _graphicsDevice.DisplayMode.Format, 
                DepthFormat.Depth24);

            drunkFrame = new RenderTarget2D(_graphicsDevice,
                currentFrame.Width / 4,
                currentFrame.Height / 4,
                false,
                _graphicsDevice.DisplayMode.Format,
                DepthFormat.None);

            _score = new ScoreComponent();

            GC.Collect();
            _isLoaded = true;
            _beginGame = true;
        }

        /// <summary>
        /// Called once on the first frame when the game is loaded.
        /// </summary>
        public void StartGame()
        {
            _beginGame = false;
           // Reset();
            MediaPlayer.Stop();
            MediaPlayer.Volume = 0.6f;
            MediaPlayer.IsRepeating = false;
            MediaPlayer.Play(_music);
        }

        public void Update(GameTime gameTime)
        {
            if (!_isLoaded)
            {
                GameStateManager.Instance.EnqueueGameState(LoadingStateFactory.GetLoadingState(LoadContent));
                return;
            }

            if (_beginGame)
                StartGame();

            _score.Update(gameTime);

            // Update dancers on the floor.
            foreach (Dancer dancer in _dancers)
                dancer.Update(gameTime);

            // Update the conga line.
            _congaLine.Update(gameTime);

            if (!_isGameOver)
            {
                _congaLine.LeadDancer.Forward =
                Vector3.Transform(
                    _congaLine.LeadDancer.Forward,
                    Matrix.CreateRotationY(
                        GetInputRotationAngle() * (float)gameTime.ElapsedGameTime.TotalSeconds));

                CheckForAddDancer();
                CheckForLoss();
                CheckForAddNewDancer();
                _mainHud.Update(_music.Duration - MediaPlayer.PlayPosition, _score.Score, _congaLine.DrunkFactor);
            }
            else
            {
                _gameOverHud.Update(gameTime);
                ProcessGameOver();
            }

            UpdateDrinks(gameTime);

            _cameraController.Update(gameTime);
        }

        public void UpdateDrinks(GameTime gameTime)
        {
            if (_drink != null && _congaLine.LeadDancer.CollidesWith(_drink))
            {
                _drink = null;
                _congaLine.AddDrink(50);
                _score.AddPoints(50);
            }

            if (_drink == null)
                _drink = _drinkEmitter.EmitDrink();

            _drink.Update(gameTime);
        }

        public void CheckForAddNewDancer()
        {
            // Add a new dancer if there is less than 15
            if (_dancers.Count < MaxDancersOnFloor)
                _dancers.Add(_dancerEmitter.EmitDancer());
        }

        public void CheckForLoss()
        {
            if (_congaLine.DrunkFactor >= 255)
            {
                _gameOverHud = new GameOverMessage(_graphicsDevice, "Game Over", "You drank too much!");
                GameOver(false);
            }

            if (_congaLine.CollidesWithSelf())
            {
                _gameOverHud = new GameOverMessage(_graphicsDevice, "Game Over", "You bumped into your conga line!");
                GameOver(false);
            }

            // Out of bounds?
            if (_congaLine.LeadDancer.Position.Length() > DanceFloorBoundary)
            {
                _gameOverHud = new GameOverMessage(_graphicsDevice, "Game Over", "You left the dance floor!");
                GameOver(false);
            }

            TimeSpan timeRemaining = _music.Duration - MediaPlayer.PlayPosition;

            if (timeRemaining.Seconds == 0 && timeRemaining.Minutes == 0)
            {
                _gameOverHud = new GameOverMessage(_graphicsDevice, "You did it!", "You're the champion!");
                GameOver(true);
            }
        }

        public void CheckForAddDancer()
        {
            // Does the line collide with anyone
            for (int i = 0; i < _dancers.Count; i++)
            {
                if (_congaLine.LeadDancer.CollidesWith(_dancers[i]))
                {
                    _congaLine.AppendDancer(_dancers[i]);
                    _dancers.RemoveAt(i--);
                    _score.AddPoints((int)Math.Round(100 * (1 + _congaLine.DrunkRatio)));
                }
            }
        }

        public void ProcessGameOver()
        {
            // Let you continue if the message says so, or the full line has fallen.
            if ((_gameOverHud.ShowTapMessage ||
                (_congaLine.LastFallenDancer == _congaLine.TailDancer)) &&
                PointerInputManager.Instance.GetClickedPosition() != null)
                GameStateManager.Instance.PopGameState();
        }

        // Stop the game.
        public void GameOver(bool win)
        {
            _congaLine.AddDrink(-_congaLine.DrunkFactor);

            if (win)
                _winSound.Play();
            else
                _loseSound.Play();

            _score.FinalizeScore();
            MediaPlayer.Stop();
            _congaLine.Stop();
            _cameraController.SetCameraBehavior(new GameOverCameraBehavior(_cameraController.Camera, _congaLine));
            _isGameOver = true;
        }

        /// <summary>
        /// Accelerometer < Keyboard
        /// </summary>
        /// <returns></returns>
        public float GetInputRotationAngle()
        {
            float rotationAngle = 0;
            if (_accelerometer.IsSupported)
                rotationAngle = AccelerometerRotationSpeed * _accelerometer.CurrentReading.Y;

            Vector2? click = PointerInputManager.Instance.GetClickedPosition();

            if (click.HasValue)
                rotationAngle = -KeyboardRotationSpeed * (2 * click.Value.X - _graphicsDevice.Viewport.Width) / _graphicsDevice.Viewport.Width;

            KeyboardState keyboardState = Keyboard.GetState();
            if (keyboardState.IsKeyDown(Keys.A) || keyboardState.IsKeyDown(Keys.Left))
                rotationAngle = KeyboardRotationSpeed;

            if (keyboardState.IsKeyDown(Keys.D) || keyboardState.IsKeyDown(Keys.Right))
                rotationAngle = -KeyboardRotationSpeed;

            return rotationAngle;
        }

        public void Draw(GameTime gameTime)
        {
            if (!_isLoaded)
                return;

            //Set the Graphics Device to render to our Render Target #1
            //instead of the back buffer.
            _graphicsDevice.SetRenderTarget(currentFrame);

            SamplerState ss = new SamplerState();
            ss.AddressU = TextureAddressMode.Clamp;
            ss.AddressV = TextureAddressMode.Clamp;

            _graphicsDevice.Clear(Color.Black);
            _graphicsDevice.BlendState = BlendState.AlphaBlend;
            _graphicsDevice.DepthStencilState = DepthStencilState.Default;
            _graphicsDevice.SamplerStates[0] = ss;

            //Draw Models and Dance Room.
            BatchedModelManager.Instance.DrawInstances(_cameraController.Camera);
            DrawDanceFloor(_cameraController.Camera);
            _drink.Draw(_cameraController.Camera);

            _spriteBatch.Begin();
            _spriteBatch.Draw(drunkFrame, 
                _graphicsDevice.Viewport.Bounds,
                new Color(_congaLine.DrunkFactor, _congaLine.DrunkFactor, _congaLine.DrunkFactor, _congaLine.DrunkFactor));
            _spriteBatch.End();

            //Re-Render our first Render Target onto a 2nd one at half the size.
            _graphicsDevice.SetRenderTarget(drunkFrame);
            _spriteBatch.Begin();
            _spriteBatch.Draw(currentFrame, _graphicsDevice.Viewport.Bounds, Color.White);
            _spriteBatch.End();

            //Reset the Graphics Device to render to the back buffer.
            _graphicsDevice.SetRenderTarget(null);

            //Lastly, render our 4th Render Target which contains all of our previous
            //rendered targets scaled down.  We will draw all of them now scaled up
            //to full resolution which will cause the blur.
            _spriteBatch.Begin();
            _spriteBatch.Draw(currentFrame, _graphicsDevice.Viewport.Bounds, Color.White);

            if (!_isGameOver)
                _mainHud.Draw(_music.Duration - MediaPlayer.PlayPosition, _score.Score, _congaLine.DrunkFactor);
            else
                _gameOverHud.Draw(_spriteBatch);

            _spriteBatch.End();
        }

        private void DrawDanceFloor(PerspectiveCamera camera)
        {
            foreach (ModelMesh mesh in _danceFloor.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = Dancer.ModelAdjustment;
                    effect.Projection = camera.ProjectionMatrix;
                    effect.View = camera.ViewMatrix;
                    effect.EnableDefaultLighting();
                }

                mesh.Draw();
            }
        }

        public void Dispose()
        {
            _accelerometer.Stop();
        }
    }
}

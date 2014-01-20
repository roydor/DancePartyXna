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
    public class GameOverMessage
    {
        private string _message;
        private Vector2 _messageDimensions;
        private Vector2 _messagePosition;
        private Vector2 _messageDestination;

        private string _description;
        private Vector2 _descriptionDimensions;
        private Vector2 _descriptionPosition;
        private Vector2 _descriptionDestination;

        private string _tapMessage;
        private Vector2 _tapMessageDimensions;
        private Vector2 _tapMessagePosition;
        private Vector2 _tapMessageDestination;

        private float _velocity;
        private float _totalTime;

        public bool ShowTapMessage;
        
        private GraphicsDevice _graphicsDevice;

        public GameOverMessage(GraphicsDevice graphicsDevice, string message, string description)
        {
            _graphicsDevice = graphicsDevice;
            _message = message;
            _description = description;

            _messageDimensions = FontManager.Instance.BangersLarge.MeasureString(_message);
            _descriptionDimensions = FontManager.Instance.BangersSmall.MeasureString(_description);

            _messagePosition.Y = -_messageDimensions.Y;
            _descriptionPosition.Y = -_descriptionDestination.Y;

            //Set the message to finish 3/5 down the screen.
            _messagePosition.X = (_graphicsDevice.Viewport.Width - _messageDimensions.X) / 2;
            _messageDestination.X = (_graphicsDevice.Viewport.Width - _messageDimensions.X) / 2;
            _messageDestination.Y = 3 * (_graphicsDevice.Viewport.Height / 5) - (_messageDimensions.Y / 2);

            // Set the description to fall all the way to the bottom.
            _descriptionPosition.X = (_graphicsDevice.Viewport.Width - _descriptionDimensions.X) / 2;
            _descriptionDestination.X = (_graphicsDevice.Viewport.Width - _descriptionDimensions.X) / 2;
            _descriptionDestination.Y = _messageDestination.Y + _messageDimensions.Y;

            _tapMessage = "(... Tap to continue ...)";
            _tapMessageDimensions = FontManager.Instance.BangersSmall.MeasureString(_tapMessage);
            _tapMessagePosition.X = (_graphicsDevice.Viewport.Width - _tapMessageDimensions.X) / 2;
            _tapMessageDestination.X =  _tapMessagePosition.X;
            _tapMessageDestination.Y = _descriptionDestination.Y + _descriptionDimensions.Y;
        }

        public void Update(GameTime gameTime)
        {
            float dt = (float)(gameTime.ElapsedGameTime.TotalSeconds);
            _velocity += 10;
            _totalTime += dt;

            float dist = _velocity * dt;

            _messagePosition.Y += dist;
            _descriptionPosition.Y += dist;
            _tapMessagePosition.Y += dist;

            if (_messagePosition.Y > _messageDestination.Y)
            {
                _messagePosition.Y = _messageDestination.Y;
                _descriptionPosition.Y = _descriptionDestination.Y;
                _tapMessagePosition.Y = _tapMessageDestination.Y;

                _velocity *= -0.5f;
            }

            if (_totalTime > 5)
                ShowTapMessage = true;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(FontManager.Instance.BangersLarge, _message, _messagePosition, Color.Yellow);
            spriteBatch.DrawString(FontManager.Instance.BangersSmall, _description, _descriptionPosition, Color.Yellow);

            if (ShowTapMessage)
                spriteBatch.DrawString(FontManager.Instance.BangersSmall, _tapMessage, _tapMessagePosition, Color.Yellow);
        }
    }

    public class MainGameHUD
    {
        private Vector2 _dancersCollectedPosition;
        private Vector2 _timeRemainingPosition;
        private Vector2 _drunkFactorPosition;

        private GraphicsDevice _graphicsDevice;
        private SpriteBatch _spriteBatch;
        private SpriteFont _font;

        public MainGameHUD(GraphicsDevice device, SpriteBatch spriteBatch, SpriteFont spriteFont)
        {
            _graphicsDevice = device;
            _spriteBatch = spriteBatch;
            _font = spriteFont;
        }

        private string GetHumanReadableTime(TimeSpan timeSpan)
        {
            int minutes = timeSpan.Minutes;
            int seconds = timeSpan.Seconds;

            if (seconds < 10)
                return minutes + ":0" + seconds;
            else
                return minutes + ":" + seconds;
        }

        // Dancers Collected centered at 1/4th the screen
        // Time remaining centered at 3/4th the screen
        public void Update(TimeSpan timeRemaining, int dancersCollected, int drunkFactor)
        {
            Vector2 dancersCollectedMeasurement = _font.MeasureString(dancersCollected.ToString());
            Vector2 remainingTimeMeasurement = _font.MeasureString(GetHumanReadableTime(timeRemaining));
            Vector2 drunkFactorMeasurement = _font.MeasureString(drunkFactor.ToString());

            _dancersCollectedPosition.X = (_graphicsDevice.Viewport.Width / 4) - (dancersCollectedMeasurement.X / 2);
            _dancersCollectedPosition.Y = 0;

            _timeRemainingPosition.X = (3 *_graphicsDevice.Viewport.Width / 4) - (remainingTimeMeasurement.X / 2);
            _timeRemainingPosition.Y = 0;

            _dancersCollectedPosition.X = (_graphicsDevice.Viewport.Width / 4) - (dancersCollectedMeasurement.X / 2);
            _dancersCollectedPosition.Y = 0;

            _drunkFactorPosition.X = (2 * _graphicsDevice.Viewport.Width / 4) - (remainingTimeMeasurement.X / 2);
        }

        public void Draw(TimeSpan timeRemaining, int dancersCollected, int drunkFactor)
        {
            _spriteBatch.DrawString(_font, dancersCollected.ToString(), _dancersCollectedPosition, Color.Yellow);
            _spriteBatch.DrawString(_font, GetHumanReadableTime(timeRemaining), _timeRemainingPosition, Color.Yellow);
            _spriteBatch.DrawString(_font, drunkFactor.ToString(), _drunkFactorPosition, Color.Yellow);
        }
    }

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
        private SoundEffectInstance _gameOverSound;

        private MainGameHUD _mainHud;
        private GameOverMessage _gameOverHud;

        private Model _danceFloor;
        
        private Drink _drink;

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
            _gameOverSound = SoundManager.Instance.GetRecordScratchInstance();

            _mainHud = new MainGameHUD(_graphicsDevice, _spriteBatch, FontManager.Instance.BangersMedium);

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
                _mainHud.Update(_music.Duration - MediaPlayer.PlayPosition, _congaLine.Count, _congaLine.DrunkFactor);
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
            if (_congaLine.DrunkFactor > 255)
            {
                _gameOverHud = new GameOverMessage(_graphicsDevice, "Game Over", "You drank too much!");
                GameOver();
            }

            if (_congaLine.CollidesWithSelf())
            {
                _gameOverHud = new GameOverMessage(_graphicsDevice, "Game Over", "You bumped into your conga line!");
                GameOver();
            }

            // Out of bounds?
            if (_congaLine.LeadDancer.Position.Length() > DanceFloorBoundary)
            {
                _gameOverHud = new GameOverMessage(_graphicsDevice, "Game Over", "You left the dance floor!");
                GameOver();
            }

            TimeSpan timeRemaining = _music.Duration - MediaPlayer.PlayPosition;

            if (timeRemaining.Seconds == 0 && timeRemaining.Minutes == 0)
            {
                _gameOverHud = new GameOverMessage(_graphicsDevice, "You did it!", "You're the champion!");
                GameOver();
            }
        }

        public void CheckForAddDancer()
        {
            // Does the line collide with anyone
            for (int i = 0; i < _dancers.Count; i++)
            {
                if (_dancers[i].CollidesWith(_congaLine.LeadDancer))
                {
                    _congaLine.AppendDancer(_dancers[i]);
                    _dancers.RemoveAt(i--);
                }
            }
        }

        public void ProcessGameOver()
        {
            // Let you continue if the message says so, or the full line has fallen.
            if ((_gameOverHud.ShowTapMessage) ||
                (_congaLine.LastFallenDancer == _congaLine.TailDancer)) &&
                PointerInputManager.Instance.GetClickedPosition() != null)
                GameStateManager.Instance.PopGameState();
        }

        // Stop the game.
        public void GameOver()
        {
            _congaLine.AddDrink(-_congaLine.DrunkFactor);
            _gameOverSound.Play();
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
            _graphicsDevice.Clear(Color.Black);

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
                _mainHud.Draw(_music.Duration - MediaPlayer.PlayPosition, _congaLine.Count, _congaLine.DrunkFactor);
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
                DancePartyGame.DrawsPerFrame++;
            }
        }

        public void Dispose()
        {
            _accelerometer.Stop();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;

namespace DanceParty
{
    public class PointerInputManager
    {
        private Vector2? _clickedLocation;
        private bool _mouseDownLastFrame;
        private bool _supportsTouch;

        private PointerInputManager()
        {
            _clickedLocation = Vector2.Zero;
            _supportsTouch = TouchPanel.GetCapabilities().IsConnected;
        }

        private static PointerInputManager _instance;
        public static PointerInputManager Instance
        {
            get
            {
                return _instance ?? (_instance = new PointerInputManager());
            }
        }

        /// <summary>
        ///  Give priority to mouse.
        /// </summary>
        public void Update()
        {
            _clickedLocation = null;
            if (_supportsTouch)
            {
                foreach (TouchLocation touchLocation in TouchPanel.GetState())
                {
                    if (touchLocation.State == TouchLocationState.Pressed)
                    {
                        _clickedLocation = touchLocation.Position;
                    }
                }
            }

            MouseState mouseState = Mouse.GetState();
            if (mouseState.LeftButton == ButtonState.Pressed && !_mouseDownLastFrame)
            {
                _clickedLocation = new Vector2(mouseState.X, mouseState.Y);
            }

            _mouseDownLastFrame = mouseState.LeftButton == ButtonState.Pressed;
        }

        public Vector2? GetClickedPosition()
        {
            return _clickedLocation;
        }

    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using DanceParty.Utilities.TouchWrapper;

namespace DanceParty
{
    public class PointerInputManager
    {
        private Vector2? _clickedLocation;
        private bool _mouseDownLastFrame;
        private bool _supportsTouch;
        private ITouchWrapper _touchWrapper;

        private PointerInputManager()
        {
            _clickedLocation = Vector2.Zero;
            _touchWrapper = TouchWrapperFactory.GetTouchWrapper();
            _supportsTouch = _touchWrapper.IsSupported();
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
            if (_touchWrapper.IsSupported())
                _clickedLocation = _touchWrapper.GetTouchPoint();

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
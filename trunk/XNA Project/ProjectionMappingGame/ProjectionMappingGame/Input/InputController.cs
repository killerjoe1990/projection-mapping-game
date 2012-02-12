using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace InterfaceElements
{
    /// <summary>
    /// Every class that does any sort of input handling
    /// needs to implement this interface.
    /// </summary>
    public interface InputController
    {
        bool HandleInput(int player);
    }


    /// <summary>
    /// Arguments sent when the mouse registers an event.
    /// Simply wraps the X and Y coordinates of the mouse when the even
    /// was fired.
    /// </summary>
    public class MouseEventArgs
    {
        public MouseEventArgs(int x, int y, int player)
        {
            X = x;
            Y = y;
            Player = player;
        }
        public int X
        {
            get;
            private set;
        }
        public int Y
        {
            get;
            private set;
        }
        public int Player
        {
            get;
            private set;
        }
    }

    /// <summary>
    /// The types of events the mouse can fire
    /// </summary>
    public enum MouseEventType
    {
        LeftClick,
        RightClick,
        LeftDrag,
        RightDrag,
        Move
    }

    /// <summary>
    /// A specialized class to handle all mouse input. UIElements can
    /// register for mouse events from this class.
    /// </summary>
    public class MouseInput : InputController
    {
        public delegate void MouseEvent(object sender, MouseEventArgs args);
        protected event MouseEvent m_LeftClick;
        protected event MouseEvent m_RightClick;
        protected event MouseEvent m_LeftDrag;
        protected event MouseEvent m_RightDrag;
        protected event MouseEvent m_Move;

        protected MouseState m_LastState;

        public MouseInput()
        {
            m_LastState = Mouse.GetState();
        }

        /// <summary>
        /// Allows a UIElement to register for a specific type of mouse event.
        /// When the event is fired, the provided delegate will be called.
        /// </summary>
        /// <param name="type">The type of event to register for</param>
        /// <param name="handler">The delegate to be called on event</param>
        public void RegisterMouseEvent(MouseEventType type, MouseEvent handler)
        {
            switch (type)
            {
                case MouseEventType.LeftClick:
                    m_LeftClick += handler;
                    break;
                case MouseEventType.LeftDrag:
                    m_LeftDrag += handler;
                    break;
                case MouseEventType.RightClick:
                    m_RightClick += handler;
                    break;
                case MouseEventType.RightDrag:
                    m_RightDrag += handler;
                    break;
                case MouseEventType.Move:
                    m_Move += handler;
                    break;
            }
        }

        /// <summary>
        /// Called every frame to check for event parameters. Will fire all appropriate events and update mouse states.
        /// </summary>
        /// <param name="player">The index of the player taking the mouse action.</param>
        /// <returns>Whether or not a mouse event was handled</returns>
        public bool HandleInput(int player)
        {
            bool handled = false;

            MouseState currentState = Mouse.GetState();

            MouseEventArgs args = new MouseEventArgs(currentState.X, currentState.Y, player);

            if (m_LastState.LeftButton == ButtonState.Pressed)
            {
                if (currentState.LeftButton == ButtonState.Pressed)
                {
                    if (m_LeftDrag != null)
                    {
                        m_LeftDrag(this, args);
                        handled = true;
                    }
                }
                else
                {
                    if (m_LeftClick != null)
                    {
                        m_LeftClick(this, args);
                        handled = true;
                    }
                }
            }

            if (m_LastState.RightButton == ButtonState.Pressed)
            {
                if (currentState.RightButton == ButtonState.Pressed)
                {
                    if (m_RightDrag != null)
                    {
                        m_RightDrag(this, args);
                        handled = true;
                    }
                }
                else
                {
                    if (m_RightClick != null)
                    {
                        m_RightClick(this, args);
                        handled = true;
                    }
                }
            }


            if (m_LastState.Y != currentState.Y || m_LastState.X != currentState.X)
            {
                if (m_Move != null)
                {
                    m_Move(this, args);
                    handled = true;
                }
            }

            m_LastState = currentState;

            return handled;
        }

    }

    public class KeyboardInput : InputController
    {

        public delegate void KeyboardEvent(object sender, Keys[] keys);

        KeyboardEvent m_KeyPressed;
        KeyboardEvent m_KeyDown;
        KeyboardState m_LastState;

        public KeyboardInput()
        {
            m_LastState = Keyboard.GetState();
        }

        public void RegisterKeyPress(KeyboardEvent handler)
        {
            m_KeyPressed += handler;
        }

        public void RegisterKeyDown(KeyboardEvent handler)
        {
            m_KeyDown += handler;
        }

        public bool HandleInput(int player)
        {
            KeyboardState keystate = Keyboard.GetState();

            bool handled = false;
            Keys[] keys = keystate.GetPressedKeys();
            Keys[] lastKeys = m_LastState.GetPressedKeys();
            Keys[] releasedKeys = lastKeys.Except(keys).ToArray();

            if (keys.Length != 0)
            {
                if (m_KeyDown != null)
                {
                    m_KeyDown(this, keys);
                }
            }

            if (releasedKeys.Length != 0)
            {
                if (m_KeyPressed != null)
                {
                    m_KeyPressed(this, releasedKeys);
                }
            }

            m_LastState = keystate;

            return handled;
        }
    }
}

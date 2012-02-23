using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace ProjectionMappingGame.GUI
{
    /// <summary>
    /// Every class that does any sort of input handling
    /// needs to implement this interface.
    /// </summary>
    public interface InputController
    {
        bool HandleInput(PlayerIndex player);
    }


    /// <summary>
    /// Arguments sent when the mouse registers an event.
    /// Simply wraps the X and Y coordinates of the mouse when the even
    /// was fired.
    /// </summary>
    public class MouseEventArgs
    {
        public MouseEventArgs(int x, int y, PlayerIndex player)
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
        public PlayerIndex Player
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
        protected Vector2 m_Offset;

        protected MouseState m_LastState;

        public MouseInput()
        {
           m_Offset = Vector2.Zero;
            m_LastState = Mouse.GetState();
        }

        public MouseInput(Vector2 offset)
        {
           m_Offset = offset;
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
        public bool HandleInput(PlayerIndex player)
        {
            bool handled = false;

            MouseState currentState = Mouse.GetState();

            MouseEventArgs args = new MouseEventArgs(currentState.X + (int)m_Offset.X, currentState.Y + (int)m_Offset.Y, player);

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

        KeyboardEvent m_KeyUp;
        KeyboardEvent m_KeyDown;
        KeyboardEvent m_KeyHeld;
        KeyboardState m_LastState;

        public KeyboardInput()
        {
            m_LastState = Keyboard.GetState();
        }

        public void RegisterKeyPress(KeyboardEvent handler)
        {
            m_KeyUp += handler;
        }

        public void RegisterKeyDown(KeyboardEvent handler)
        {
            m_KeyDown += handler;
        }

        public void RegisterKeyHeld(KeyboardEvent handler)
        {
            m_KeyHeld += handler;
        }

        public bool HandleInput(PlayerIndex player)
        {
            KeyboardState keystate = Keyboard.GetState();

            bool handled = false;
            Keys[] keys = keystate.GetPressedKeys();
            Keys[] lastKeys = m_LastState.GetPressedKeys();
            List<Keys> heldKeys = new List<Keys>();
            List<Keys> pressedKeys = new List<Keys>();
            Keys[] releasedKeys = lastKeys.Except(keys).ToArray();

             foreach (Keys k in keys)
            {
                if (m_LastState.IsKeyDown(k))
                {
                    heldKeys.Add(k);
                }
                else
                {
                    pressedKeys.Add(k);
                }
            }

            if (heldKeys.Count > 0 && m_KeyHeld != null)
            {
                m_KeyHeld(this, heldKeys.ToArray());
            }

            if (pressedKeys.Count > 0 && m_KeyDown != null)
            {
                m_KeyDown(this, pressedKeys.ToArray());
            }

            if (releasedKeys.Length != 0)
            {
                if (m_KeyUp != null)
                {
                    m_KeyUp(this, releasedKeys);
                }
            }

            m_LastState = keystate;

            return handled;
        }
    }

    public enum GamepadEventType
    {
        BUTTON_UP,
        BUTTON_HOLD,
        BUTTON_DOWN
    }

    public class GamepadInput : InputController
    {
        const float AXIS_BUFFER = 0.05f;

        public enum Buttons
        {
            A,
            B,
            X,
            Y,
            RS,
            LS,
            RB,
            LB,
            START,
            SELECT
        }
        public enum Axis
        {
            LS_X,
            LS_Y,
            RS_X,
            RS_Y,
            D_Y,
            D_X,
            LT,
            RT
        }

        public delegate void GamepadButtonEvent(object sender, Buttons buttons);
        public delegate void GamepadAxisEvent(object sender, Axis axis, float degree);

        GamepadButtonEvent[] m_ButtonDown;
        GamepadButtonEvent[] m_ButtonUp;
        GamepadButtonEvent[] m_ButtonHold;

        GamepadAxisEvent[] m_AxisChange;

        GamePadState[] m_LastState;

        public GamepadInput()
        {
            m_LastState = new GamePadState[GameConstants.MAX_PLAYERS];
            m_LastState[(int)PlayerIndex.One] = GamePad.GetState(PlayerIndex.One);
            m_LastState[(int)PlayerIndex.Two] = GamePad.GetState(PlayerIndex.Two);
            m_LastState[(int)PlayerIndex.Three] = GamePad.GetState(PlayerIndex.Three);
            m_LastState[(int)PlayerIndex.Four] = GamePad.GetState(PlayerIndex.Four);

            m_ButtonDown = new GamepadButtonEvent[GameConstants.MAX_PLAYERS];
            m_ButtonHold = new GamepadButtonEvent[GameConstants.MAX_PLAYERS];
            m_ButtonUp = new GamepadButtonEvent[GameConstants.MAX_PLAYERS];
            m_AxisChange = new GamepadAxisEvent[GameConstants.MAX_PLAYERS];
        }

        public void RegisterButtonEvent(GamepadEventType type, GamepadButtonEvent handler, PlayerIndex player)
        {
            switch (type)
            {
                case GamepadEventType.BUTTON_DOWN:
                    m_ButtonDown[(int)player] += handler;
                    break;
                case GamepadEventType.BUTTON_UP:
                    m_ButtonDown[(int)player] += handler;
                    break;
                case GamepadEventType.BUTTON_HOLD:
                    m_ButtonHold[(int)player] += handler;
                    break;
            }
        }

        public void RegisterAxisEvent(GamepadAxisEvent handler, PlayerIndex player)
        {
            m_AxisChange[(int)player] += handler;
        }


        #region InputController Members

        public bool HandleInput(PlayerIndex player)
        {
            bool handled = false;

            GamePadState padState = GamePad.GetState(player);

            if (padState.IsConnected)
            {

                #region Check A
                if (padState.Buttons.A == ButtonState.Pressed)
                {
                    if (m_LastState[(int)player].Buttons.A == ButtonState.Pressed)
                    {
                        if (m_ButtonHold[(int)player] != null)
                        {
                            m_ButtonHold[(int)player](this, Buttons.A);
                            handled = true;
                        }
                    }
                    else
                    {
                        if (m_ButtonDown[(int)player] != null)
                        {
                            m_ButtonDown[(int)player](this, Buttons.A);
                            handled = true;
                        }
                    }
                }
                else
                {
                    if (m_LastState[(int)player].Buttons.A == ButtonState.Pressed)
                    {
                        if (m_ButtonUp[(int)player] != null)
                        {
                            m_ButtonUp[(int)player](this, Buttons.A);
                            handled = true;
                        }
                    }
                }
                #endregion

                #region Check B
                if (padState.Buttons.B == ButtonState.Pressed)
                {
                    if (m_LastState[(int)player].Buttons.B == ButtonState.Pressed)
                    {
                        if (m_ButtonHold[(int)player] != null)
                        {
                            m_ButtonHold[(int)player](this, Buttons.B);
                            handled = true;
                        }
                    }
                    else
                    {
                        if (m_ButtonDown[(int)player] != null)
                        {
                            m_ButtonDown[(int)player](this, Buttons.B);
                            handled = true;
                        }
                    }
                }
                else
                {
                    if (m_LastState[(int)player].Buttons.B == ButtonState.Pressed)
                    {
                        if (m_ButtonUp[(int)player] != null)
                        {
                            m_ButtonUp[(int)player](this, Buttons.B);
                            handled = true;
                        }
                    }
                }
                #endregion

                #region Check X
                if (padState.Buttons.X == ButtonState.Pressed)
                {
                    if (m_LastState[(int)player].Buttons.X == ButtonState.Pressed)
                    {
                        if (m_ButtonHold[(int)player] != null)
                        {
                            m_ButtonHold[(int)player](this, Buttons.X);
                            handled = true;
                        }
                    }
                    else
                    {
                        if (m_ButtonDown[(int)player] != null)
                        {
                            m_ButtonDown[(int)player](this, Buttons.X);
                            handled = true;
                        }
                    }
                }
                else
                {
                    if (m_LastState[(int)player].Buttons.X == ButtonState.Pressed)
                    {
                        if (m_ButtonUp[(int)player] != null)
                        {
                            m_ButtonUp[(int)player](this, Buttons.X);
                            handled = true;
                        }
                    }
                }
                #endregion

                #region Check Y
                if (padState.Buttons.Y == ButtonState.Pressed)
                {
                    if (m_LastState[(int)player].Buttons.Y == ButtonState.Pressed)
                    {
                        if (m_ButtonHold[(int)player] != null)
                        {
                            m_ButtonHold[(int)player](this, Buttons.Y);
                            handled = true;
                        }
                    }
                    else
                    {
                        if (m_ButtonDown[(int)player] != null)
                        {
                            m_ButtonDown[(int)player](this, Buttons.Y);
                            handled = true;
                        }
                    }
                }
                else
                {
                    if (m_LastState[(int)player].Buttons.Y == ButtonState.Pressed)
                    {
                        if (m_ButtonUp[(int)player] != null)
                        {
                            m_ButtonUp[(int)player](this, Buttons.Y);
                            handled = true;
                        }
                    }
                }
                #endregion

                #region Check RB
                if (padState.Buttons.RightShoulder == ButtonState.Pressed)
                {
                    if (m_LastState[(int)player].Buttons.RightShoulder == ButtonState.Pressed)
                    {
                        if (m_ButtonHold[(int)player] != null)
                        {
                            m_ButtonHold[(int)player](this, Buttons.RB);
                            handled = true;
                        }
                    }
                    else
                    {
                        if (m_ButtonDown[(int)player] != null)
                        {
                            m_ButtonDown[(int)player](this, Buttons.RB);
                            handled = true;
                        }
                    }
                }
                else
                {
                    if (m_LastState[(int)player].Buttons.RightShoulder == ButtonState.Pressed)
                    {
                        if (m_ButtonUp[(int)player] != null)
                        {
                            m_ButtonUp[(int)player](this, Buttons.RB);
                            handled = true;
                        }
                    }
                }
                #endregion

                #region Check LB
                if (padState.Buttons.LeftShoulder == ButtonState.Pressed)
                {
                    if (m_LastState[(int)player].Buttons.LeftShoulder == ButtonState.Pressed)
                    {
                        if (m_ButtonHold[(int)player] != null)
                        {
                            m_ButtonHold[(int)player](this, Buttons.LB);
                            handled = true;
                        }
                    }
                    else
                    {
                        if (m_ButtonDown[(int)player] != null)
                        {
                            m_ButtonDown[(int)player](this, Buttons.LB);
                            handled = true;
                        }
                    }
                }
                else
                {
                    if (m_LastState[(int)player].Buttons.LeftShoulder == ButtonState.Pressed)
                    {
                        if (m_ButtonUp[(int)player] != null)
                        {
                            m_ButtonUp[(int)player](this, Buttons.LB);
                            handled = true;
                        }
                    }
                }
                #endregion

                #region Check START
                if (padState.Buttons.Start == ButtonState.Pressed)
                {
                    if (m_LastState[(int)player].Buttons.Start == ButtonState.Pressed)
                    {
                        if (m_ButtonHold[(int)player] != null)
                        {
                            m_ButtonHold[(int)player](this, Buttons.START);
                            handled = true;
                        }
                    }
                    else
                    {
                        if (m_ButtonDown[(int)player] != null)
                        {
                            m_ButtonDown[(int)player](this, Buttons.START);
                            handled = true;
                        }
                    }
                }
                else
                {
                    if (m_LastState[(int)player].Buttons.Start == ButtonState.Pressed)
                    {
                        if (m_ButtonUp[(int)player] != null)
                        {
                            m_ButtonUp[(int)player](this, Buttons.START);
                            handled = true;
                        }
                    }
                }
                #endregion

                #region Check SELECT
                if (padState.Buttons.Back == ButtonState.Pressed)
                {
                    if (m_LastState[(int)player].Buttons.Back == ButtonState.Pressed)
                    {
                        if (m_ButtonHold[(int)player] != null)
                        {
                            m_ButtonHold[(int)player](this, Buttons.SELECT);
                            handled = true;
                        }
                    }
                    else
                    {
                        if (m_ButtonDown[(int)player] != null)
                        {
                            m_ButtonDown[(int)player](this, Buttons.SELECT);
                            handled = true;
                        }
                    }
                }
                else
                {
                    if (m_LastState[(int)player].Buttons.Back == ButtonState.Pressed)
                    {
                        if (m_ButtonUp[(int)player] != null)
                        {
                            m_ButtonUp[(int)player](this, Buttons.SELECT);
                            handled = true;
                        }
                    }
                }
                #endregion

                #region Check RS
                if (padState.Buttons.RightStick == ButtonState.Pressed)
                {
                    if (m_LastState[(int)player].Buttons.RightStick == ButtonState.Pressed)
                    {
                        if (m_ButtonHold[(int)player] != null)
                        {
                            m_ButtonHold[(int)player](this, Buttons.RS);
                            handled = true;
                        }
                    }
                    else
                    {
                        if (m_ButtonDown[(int)player] != null)
                        {
                            m_ButtonDown[(int)player](this, Buttons.RS);
                            handled = true;
                        }
                    }
                }
                else
                {
                    if (m_LastState[(int)player].Buttons.RightStick == ButtonState.Pressed)
                    {
                        if (m_ButtonUp[(int)player] != null)
                        {
                            m_ButtonUp[(int)player](this, Buttons.RS);
                            handled = true;
                        }
                    }
                }
                #endregion

                #region Check LS
                if (padState.Buttons.LeftStick == ButtonState.Pressed)
                {
                    if (m_LastState[(int)player].Buttons.LeftStick == ButtonState.Pressed)
                    {
                        if (m_ButtonHold[(int)player] != null)
                        {
                            m_ButtonHold[(int)player](this, Buttons.LS);
                            handled = true;
                        }
                    }
                    else
                    {
                        if (m_ButtonDown[(int)player] != null)
                        {
                            m_ButtonDown[(int)player](this, Buttons.LS);
                            handled = true;
                        }
                    }
                }
                else
                {
                    if (m_LastState[(int)player].Buttons.LeftStick == ButtonState.Pressed)
                    {
                        if (m_ButtonUp[(int)player] != null)
                        {
                            m_ButtonUp[(int)player](this, Buttons.LS);
                            handled = true;
                        }
                    }
                }
                #endregion



                #region Check LS Axis

                if ((padState.ThumbSticks.Left.X >= AXIS_BUFFER || padState.ThumbSticks.Left.X <= -AXIS_BUFFER) && m_AxisChange[(int)player] != null)
                {
                    m_AxisChange[(int)player](this, Axis.LS_X, padState.ThumbSticks.Left.X);
                }
                if ((padState.ThumbSticks.Left.Y >= AXIS_BUFFER || padState.ThumbSticks.Left.Y <= -AXIS_BUFFER) && m_AxisChange[(int)player] != null)
                {
                    m_AxisChange[(int)player](this, Axis.LS_Y, padState.ThumbSticks.Left.Y);
                }

                #endregion

                #region Check RS Axis

                if ((padState.ThumbSticks.Right.X >= AXIS_BUFFER || padState.ThumbSticks.Right.X <= -AXIS_BUFFER) && m_AxisChange[(int)player] != null)
                {
                    m_AxisChange[(int)player](this, Axis.RS_X, padState.ThumbSticks.Right.X);
                }
                if ((padState.ThumbSticks.Right.Y >= AXIS_BUFFER || padState.ThumbSticks.Right.Y <= -AXIS_BUFFER) && m_AxisChange[(int)player] != null)
                {
                    m_AxisChange[(int)player](this, Axis.RS_Y, padState.ThumbSticks.Right.Y); ;
                }

                #endregion

                #region Check Trigger Axis
                if (padState.Triggers.Left >= AXIS_BUFFER && m_AxisChange[(int)player] != null)
                {
                    m_AxisChange[(int)player](this, Axis.LT, padState.Triggers.Left); ;
                }
                if (padState.Triggers.Right >= AXIS_BUFFER && m_AxisChange[(int)player] != null)
                {
                    m_AxisChange[(int)player](this, Axis.RT, padState.Triggers.Right); ;
                }
                #endregion
                m_LastState[(int)player] = padState;

            }
            return handled;
        }

        #endregion
    }
}

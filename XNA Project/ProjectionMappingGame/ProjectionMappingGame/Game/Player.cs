using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace ProjectionMappingGame.Game
{
    public class Player : MoveableObject
    {

        public enum Animations
        {
            IDLE = 0,
            RUN = 1,
            JUMP = 2
        }

        Animation[] m_Animations;
        Vector2 m_Impulse;
        PlayerIndex m_Player;

        float m_Move;
        bool m_OnGround;

        public Player(Texture2D image, Rectangle bounds, GUI.KeyboardInput keyboard, PlayerIndex player) 
            : base(bounds, Vector2.Zero, image)
        {
            keyboard.RegisterKeyHeld(onKeyDown);
            keyboard.RegisterKeyHeld(onKeyHeld);

            m_Player = player;

            m_Move = 0;

            m_Impulse = Vector2.Zero;

            m_Animations = new Animation[Enum.GetValues(typeof(Animations)).Length];

            m_OnGround = false;
        }

        public Player(Texture2D image, Rectangle bounds, GUI.GamepadInput gamepad, PlayerIndex player)
            : base(bounds, Vector2.Zero, image)
        {

            gamepad.RegisterButtonEvent(GUI.GamepadEventType.BUTTON_DOWN, onButtonDown, player);
            gamepad.RegisterAxisEvent(onAxisChange, player);

            m_Player = player;

            m_Move = 0;

            m_Impulse = Vector2.Zero;

            m_Animations = new Animation[Enum.GetValues(typeof(Animations)).Length];

            m_OnGround = false;
        }

        public Player(Texture2D image, Rectangle bounds, GUI.GamepadInput gamepad, GUI.KeyboardInput keyboard, PlayerIndex player)
            : base(bounds, Vector2.Zero, image)
        {
            keyboard.RegisterKeyDown(onKeyDown);
            keyboard.RegisterKeyHeld(onKeyHeld);

            gamepad.RegisterButtonEvent(GUI.GamepadEventType.BUTTON_DOWN, onButtonDown, player);
            gamepad.RegisterAxisEvent(onAxisChange, player);

            m_Player = player;

            m_Move = 0;

            m_Impulse = Vector2.Zero;

            m_Animations = new Animation[Enum.GetValues(typeof(Animations)).Length];

            m_OnGround = false;
        }

        public void AddAnimation(Animations type, Animation anim)
        {
            m_Animations[(int)type] = anim;
        }

        private void onKeyDown(object sender, Keys[] keys)
        {
            if(keys.Contains(Keys.Space) && m_OnGround)
            {
                m_Impulse += Vector2.UnitY * GameConstants.JUMP_IMPULSE;

                if (m_Animations[(int)Animations.JUMP] != null)
                {
                    m_CurrentAnimation = m_Animations[(int)Animations.JUMP];
                }
            }
        }

        private void onKeyHeld(object sender, Keys[] keys)
        {
            if (keys.Contains(Keys.A))
            {
                m_Move = -1;

                if (m_Animations[(int)Animations.RUN] != null && m_OnGround)
                {
                    m_CurrentAnimation = m_Animations[(int)Animations.RUN];
                }
            }
            if (keys.Contains(Keys.D))
            {
                m_Move = 1;

                if (m_Animations[(int)Animations.RUN] != null && m_OnGround)
                {
                    m_CurrentAnimation = m_Animations[(int)Animations.RUN];
                }
            }
        }

        private void onButtonDown(object sender, GUI.GamepadInput.Buttons button)
        {
            if (button.Equals(GUI.GamepadInput.Buttons.A) && m_OnGround)
            {
                m_Impulse += Vector2.UnitY * GameConstants.JUMP_IMPULSE;

                if (m_Animations[(int)Animations.JUMP] != null)
                {
                    m_CurrentAnimation = m_Animations[(int)Animations.JUMP];
                }
            }
        }

        private void onAxisChange(object sender, GUI.GamepadInput.Axis axis, float degree)
        {
            if (axis.Equals(GUI.GamepadInput.Axis.LS_X))
            {
                if (degree < 0)
                {
                    m_Move = -1 * degree;

                    if (m_Animations[(int)Animations.RUN] != null && m_OnGround)
                    {
                        m_CurrentAnimation = m_Animations[(int)Animations.RUN];
                    }
                }
                else
                {
                    m_Move = degree;

                    if (m_Animations[(int)Animations.RUN] != null && m_OnGround)
                    {
                        m_CurrentAnimation = m_Animations[(int)Animations.RUN];
                    }
                }
            }
        }

        public new void Update(float deltaTime)
        {
            m_Velocity += m_Impulse * deltaTime;
            m_Impulse = Vector2.Zero;

            m_Velocity.X = m_Move * GameConstants.MOVE_SPEED * deltaTime;
            if (!m_OnGround)
            {
                m_Velocity.Y += GameConstants.GRAVITY * deltaTime;
            }

            m_Move = 0;

            base.Update(deltaTime);
        }

        public void CheckCollisions(List<Platform> platforms, float deltaTime)
        {
            m_OnGround = false;
            foreach (Platform p in platforms)
            {
                /*Vector2 nextVelocity;

                nextVelocity = m_Velocity + m_Impulse * deltaTime;
                nextVelocity.X += m_Move * GameConstants.MOVE_SPEED * deltaTime;
                nextVelocity.Y += GameConstants.GRAVITY * deltaTime;
                Vector2 nextPosition = m_Position + nextVelocity * deltaTime;
                Rectangle platBounds = p.Bounds;

                switch (p.Type)
                {
                    case PlatformTypes.Impassable:
                        if (CheckTop(nextPosition, platBounds))
                        {
                            m_Velocity.Y = 0;
                            m_Impulse.Y = 0;
                        }
                        if (CheckBot(nextPosition, platBounds))
                        {
                            m_Velocity.Y = p.Velocity.Y;
                            m_OnGround = true;
                        }
                        if (CheckLeft(nextPosition, platBounds))
                        {
                            m_Velocity.X = 0;
                            m_Move = (m_Move < 0) ? 0 : m_Move;
                        }
                        if (CheckRight(nextPosition, platBounds))
                        {
                            m_Velocity.X = 0;
                            m_Move = (m_Move > 0) ? 0 : m_Move;
                        }
                        break;
                    case PlatformTypes.Platform:
                        if (CheckBot(nextPosition, platBounds))
                        {
                            m_Velocity.Y = p.Velocity.Y;
                            m_OnGround = true;
                        }
                        break;
                    case PlatformTypes.Passable:
                        break;
                }*/

                foreach (Tile t in p.Tiles)
                {
                    Vector2 nextVelocity;

                    nextVelocity = m_Velocity + m_Impulse * deltaTime;
                    nextVelocity.X += m_Move * GameConstants.MOVE_SPEED * deltaTime;
                    nextVelocity.Y += GameConstants.GRAVITY * deltaTime;
                    Vector2 nextPosition = m_Position + nextVelocity * deltaTime;
                    Rectangle platBounds = t.Bounds;

                    switch (t.Type)
                    {
                        case PlatformTypes.Impassable:
                            if (CheckTop(nextPosition, platBounds))
                            {
                                m_Velocity.Y = 0;
                                m_Impulse.Y = 0;
                            }
                            if (CheckBot(nextPosition, platBounds))
                            {
                                m_Velocity.Y = t.Velocity.Y;
                                m_OnGround = true;
                            }
                            if (CheckLeft(nextPosition, platBounds))
                            {
                                m_Velocity.X = 0;
                                m_Move = (m_Move < 0) ? 0 : m_Move;
                            }
                            if (CheckRight(nextPosition, platBounds))
                            {
                                m_Velocity.X = 0;
                                m_Move = (m_Move > 0) ? 0 : m_Move;
                            }
                            break;
                        case PlatformTypes.Platform:
                            if (CheckBot(nextPosition, platBounds))
                            {
                                m_Velocity.Y = t.Velocity.Y;
                                m_OnGround = true;
                            }
                            break;
                        case PlatformTypes.Passable:
                            break;
                    }
                }
                 
            }
        }

        public void CheckCollision(Player player, float deltaTime)
        {

        }

        private bool CheckTop(Vector2 newPos, Rectangle rec)
        {
            if (newPos.Y <= rec.Y + rec.Height
                && m_Bounds.Y > rec.Y + rec.Height 
                && ((rec.X > m_Position.X
                && rec.X < m_Position.X + m_Bounds.Width)
                || (rec.X + rec.Width > m_Position.X
                && rec.X + rec.Width < m_Position.X + m_Bounds.Width)))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool CheckBot(Vector2 newPos, Rectangle rec)
        {
            if (newPos.Y + m_Bounds.Height >= rec.Y
                && m_Position.Y + m_Bounds.Height < rec.Y
                && ((rec.X > m_Position.X
                && rec.X < newPos.X + m_Bounds.Width)
                || (rec.X + rec.Width > m_Position.X
                && rec.X + rec.Width < m_Position.X + m_Bounds.Width)))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool CheckLeft(Vector2 newPos, Rectangle rec)
        {
            if (newPos.X <= rec.X + rec.Width
                && m_Position.X > rec.X + rec.Width
                && ((rec.Y > m_Position.Y
                && rec.Y < m_Position.Y + m_Bounds.Height)
                || (rec.Y + rec.Height > m_Position.Y
                && rec.Y + rec.Height < m_Position.Y + m_Bounds.Height)))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool CheckRight(Vector2 newPos, Rectangle rec)
        {
            if (newPos.X + m_Bounds.Width >= rec.X
                && m_Position.X + m_Bounds.Width < rec.X
                && ((rec.Y > m_Position.Y
                && rec.Y < m_Position.Y + m_Bounds.Height)
                || (rec.Y + rec.Height > m_Position.Y
                && rec.Y + rec.Height < m_Position.Y + m_Bounds.Height)))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}

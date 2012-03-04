using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

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
        Vector2 m_CollisionImpulse;
        PlayerIndex m_Player;
        PlayerMenu m_PlayerHud;

        float m_SnapY;
        Vector2 m_HudOrigin;
        float m_Move;
        bool m_OnGround;
        bool m_IsAlive;
        private bool m_DrawHud;

        public Player(Texture2D image, Rectangle bounds, GUI.KeyboardInput keyboard, PlayerIndex player) 
            : base(bounds, Vector2.Zero, image)
        {
            keyboard.RegisterKeyHeld(onKeyHeld);
            keyboard.RegisterKeyHeld(onKeyDown);

            m_Player = player;

            m_Move = 0;

            m_DrawHud = false;
            m_IsAlive = true;

            if (player == PlayerIndex.One)
            {
                m_HudOrigin = new Vector2(0, 0);
            }
            else if (player == PlayerIndex.Two)
            {
                m_HudOrigin = new Vector2(1080, 0);
            }
            else if (player == PlayerIndex.Three)
            {
                m_HudOrigin = new Vector2(0, 660);
            }
            else if (player == PlayerIndex.Four)
            {
                m_HudOrigin = new Vector2(1080, 660);
            }
            m_PlayerHud = new PlayerMenu(m_Player);

            m_Impulse = Vector2.Zero;
            m_CollisionImpulse = Vector2.Zero;

            m_Animations = new Animation[Enum.GetValues(typeof(Animations)).Length];
            m_Animations[(int)Animations.IDLE] = m_CurrentAnimation;

            m_OnGround = false;

            m_SnapY = 0;
        }

        public Player(Texture2D image, Rectangle bounds, GUI.GamepadInput gamepad, PlayerIndex player)
            : base(bounds, Vector2.Zero, image)
        {
            gamepad.RegisterAxisEvent(onAxisChange, player);
            gamepad.RegisterButtonEvent(GUI.GamepadEventType.BUTTON_DOWN, onButtonHold, player);
            

            m_Player = player;

            m_Move = 0;

            m_DrawHud = false;
            m_IsAlive = true;

            if (player == PlayerIndex.One)
            {
                m_HudOrigin = new Vector2(0, 0);
            }
            else if (player == PlayerIndex.Two)
            {
                m_HudOrigin = new Vector2(1080, 0);
            }
            else if (player == PlayerIndex.Three)
            {
                m_HudOrigin = new Vector2(0, 660);
            }
            else if (player == PlayerIndex.Four)
            {
                m_HudOrigin = new Vector2(1080, 660);
            }
            m_PlayerHud = new PlayerMenu(m_Player);

            m_Impulse = Vector2.Zero;

            m_Animations = new Animation[Enum.GetValues(typeof(Animations)).Length];
            m_Animations[(int)Animations.IDLE] = m_CurrentAnimation;

            m_OnGround = false;

            m_SnapY = 0;
        }

        public Player(Texture2D image, Rectangle bounds, GUI.GamepadInput gamepad, GUI.KeyboardInput keyboard, PlayerIndex player)
            : base(bounds, Vector2.Zero, image)
        {
            keyboard.RegisterKeyDown(onKeyDown);
            keyboard.RegisterKeyHeld(onKeyHeld);

            gamepad.RegisterAxisEvent(onAxisChange, player);
            gamepad.RegisterButtonEvent(GUI.GamepadEventType.BUTTON_HOLD, onButtonHold, player);
            
            m_Player = player;

            m_Move = 0;

            m_Impulse = Vector2.Zero;

            m_DrawHud = false;
            m_IsAlive = true;

            if (player == PlayerIndex.One)
            {
                m_HudOrigin = new Vector2(0, 0);
            }
            else if (player == PlayerIndex.Two)
            {
                m_HudOrigin = new Vector2(1080, 0);
            }
            else if (player == PlayerIndex.Three)
            {
                m_HudOrigin = new Vector2(0, 660);
            }
            else if (player == PlayerIndex.Four)
            {
                m_HudOrigin = new Vector2(1080, 660);
            }
            m_PlayerHud = new PlayerMenu(m_Player);

            m_Animations = new Animation[Enum.GetValues(typeof(Animations)).Length];
            m_Animations[(int)Animations.IDLE] = m_CurrentAnimation;

            m_OnGround = false;
        }

        public Vector2 GetNextPosition(float deltaTime)
        {
            Vector2 pos = m_Position;
            Vector2 vel = m_Velocity;
            vel += m_Impulse * deltaTime;

            if (!m_OnGround)
            {
                vel.Y += GameConstants.GRAVITY * deltaTime;
            }

            vel.X = m_Move * GameConstants.MOVE_SPEED * deltaTime;

            pos += vel * deltaTime;

            if (m_OnGround)
            {
                pos.Y = m_SnapY;
            }

            return pos;

        }

        public void AddAnimation(Animations type, Animation anim)
        {
            m_Animations[(int)type] = anim;
        }

        public void LoadHudContent(SpriteFont font, Texture2D playerColor)
        {
            m_PlayerHud.LoadContent(font,playerColor);
        }

        private void onKeyDown(object sender, Keys[] keys)
        {
            if(keys.Contains(Keys.Space))// && m_OnGround)
            {
                if(m_OnGround)
                {
                     m_Impulse += Vector2.UnitY * GameConstants.JUMP_IMPULSE;

                    if (m_Animations[(int)Animations.JUMP] != null)
                    {
                        //m_CurrentAnimation.Reset();
                        m_CurrentAnimation = m_Animations[(int)Animations.JUMP];
                        m_CurrentAnimation.Reset();
                    }
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
                    //m_CurrentAnimation.Reset();
                    m_CurrentAnimation = m_Animations[(int)Animations.RUN];
                }
            }
            if (keys.Contains(Keys.D))
            {
                m_Move = 1;

                if (m_Animations[(int)Animations.RUN] != null && m_OnGround)
                {
                    //m_CurrentAnimation.Reset();
                    m_CurrentAnimation = m_Animations[(int)Animations.RUN];
                }
            }
        }

        private void onButtonHold(object sender, GUI.GamepadInput.Buttons button)
        {
            if (button.Equals(GUI.GamepadInput.Buttons.A) && m_OnGround)
            {
                m_Impulse += Vector2.UnitY * GameConstants.JUMP_IMPULSE;

                if (m_Animations[(int)Animations.JUMP] != null)
                {
                    m_CurrentAnimation = m_Animations[(int)Animations.JUMP];
                    m_CurrentAnimation.Reset();
                }
            }
        }

        private void onAxisChange(object sender, GUI.GamepadInput.Axis axis, float degree)
        {
            if (axis.Equals(GUI.GamepadInput.Axis.LS_X))
            {
                m_Move = degree;

                if (m_Animations[(int)Animations.RUN] != null && m_OnGround)
                {
                    m_CurrentAnimation = m_Animations[(int)Animations.RUN];
                }
            }
        }

        public new void Update(float deltaTime)
        {
            m_Velocity += m_Impulse * deltaTime;
            m_Velocity += m_CollisionImpulse * deltaTime;
            m_Impulse = Vector2.Zero;
            m_CollisionImpulse = Vector2.Zero;

            m_Velocity.X = m_Move * GameConstants.MOVE_SPEED * deltaTime;
            if (!m_OnGround)
            {
                m_Velocity.Y += GameConstants.GRAVITY * deltaTime;
            }

            if (m_Velocity.X < 0.00001f && m_Velocity.X > -0.00001f && m_OnGround)
            {
                //m_CurrentAnimation.Reset();
                m_CurrentAnimation = m_Animations[(int)Animations.IDLE];
            }

            m_Move = 0;

            base.Update(deltaTime);

            if (m_OnGround)
            {
                m_Position.Y = m_SnapY;
            }
        }

        public void CheckCollisions(List<Platform> platforms, float deltaTime)
        {
            m_OnGround = false;
            foreach (Platform p in platforms)
            {
                foreach (Tile t in p.Tiles)
                {
                    Vector2 nextVelocity;

                    nextVelocity = m_Velocity + m_Impulse * deltaTime;
                    nextVelocity.X = m_Move * GameConstants.MOVE_SPEED * deltaTime;
                    nextVelocity.Y += GameConstants.GRAVITY * deltaTime;
                    Vector2 nextPosition = m_Position + nextVelocity * deltaTime;
                    Vector2 platPos = t.Position;
                    Rectangle platBounds = t.Bounds;

                    switch (t.Type)
                    {
                        case PlatformTypes.Impassable:
                            if (CheckTop(nextPosition, platPos, platPos, platBounds))
                            {
                                m_Velocity.Y = 0;
                                m_Impulse.Y = 0;
                            }
                            if (CheckBot(nextPosition, platPos, platPos, platBounds))
                            {
                                m_Velocity.Y = t.Velocity.Y;
                                m_SnapY = t.Position.Y - m_Bounds.Height - 0.00001f;
                                m_OnGround = true;
                            }
                            if (CheckLeft(nextPosition, platPos, platBounds))
                            {
                                m_Velocity.X = 0;
                                m_Move = (m_Move < 0) ? 0 : m_Move;
                            }
                            if (CheckRight(nextPosition, platPos, platBounds))
                            {
                                m_Velocity.X = 0;
                                m_Move = (m_Move > 0) ? 0 : m_Move;
                            }
                            break;
                        case PlatformTypes.Platform:
                            if (CheckBot(nextPosition, platPos, platPos, platBounds))
                            {
                                m_Velocity.Y = t.Velocity.Y;
                                m_SnapY = t.Position.Y - m_Bounds.Height - 0.00001f;
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
            Vector2 otherPos = player.GetNextPosition(deltaTime);
            Rectangle otherBound = player.Bounds;

            Vector2 nextVelocity;

            nextVelocity = m_Velocity + m_Impulse * deltaTime;
            nextVelocity.X = m_Move * GameConstants.MOVE_SPEED * deltaTime;
            if (!m_OnGround)
            {
                nextVelocity.Y += GameConstants.GRAVITY * deltaTime;
            }
            Vector2 nextPosition = m_Position + nextVelocity * deltaTime;

            if (CheckTop(nextPosition, otherPos, player.GetNextPosition(deltaTime), otherBound))
            {
                m_CollisionImpulse += Vector2.UnitY * GameConstants.BOUNCE_IMPULSE_DOWN;
                m_Velocity.Y = 0;
                Console.WriteLine("HIT TOP " + m_Player.ToString());
            }
            if (CheckBot(nextPosition, otherPos, player.GetNextPosition(deltaTime), otherBound))
            {
                m_CollisionImpulse += Vector2.UnitY * GameConstants.BOUNCE_IMPULSE_UP;
                m_Velocity.Y = 0;
                Console.WriteLine("HIT BOT " + m_Player.ToString());
            }
        }
        public void setCharSelectionHud(bool flag)
        {
            m_DrawHud = flag;
        }
        public bool getDrawHudBool()
        {
            return m_DrawHud;
        }
        private bool CheckTop(Vector2 newPos1, Vector2 pos2, Vector2 newPos2, Rectangle rec)
        {
            if (newPos1.Y <= newPos2.Y + rec.Height
                && m_Position.Y > pos2.Y + rec.Height 
                && ((newPos2.X > newPos1.X
                && newPos2.X < newPos1.X + m_Bounds.Width)
                || (newPos2.X + rec.Width > newPos1.X
                && newPos2.X + rec.Width < newPos1.X + m_Bounds.Width)))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool CheckBot(Vector2 newPos1, Vector2 pos2, Vector2 newPos2, Rectangle rec)
        {
            if (newPos1.Y + m_Bounds.Height >= newPos2.Y
                && m_Position.Y + m_Bounds.Height < pos2.Y
                && ((pos2.X > newPos1.X
                && pos2.X < newPos1.X + m_Bounds.Width)
                || (pos2.X + rec.Width > newPos1.X
                && pos2.X + rec.Width < newPos1.X + m_Bounds.Width)))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool CheckLeft(Vector2 newPos, Vector2 pos, Rectangle rec)
        {
            if (newPos.X <= pos.X + rec.Width
                && m_Position.X > pos.X + rec.Width
                && ((pos.Y > m_Position.Y
                && pos.Y < m_Position.Y + m_Bounds.Height)
                || (pos.Y + rec.Height > m_Position.Y
                && pos.Y + rec.Height < m_Position.Y + m_Bounds.Height)))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        

        private bool CheckRight(Vector2 newPos, Vector2 pos, Rectangle rec)
        {
            if (newPos.X + m_Bounds.Width >= pos.X
                && m_Position.X + m_Bounds.Width < pos.X
                && ((pos.Y > m_Position.Y
                && pos.Y < m_Position.Y + m_Bounds.Height)
                || (pos.Y + rec.Height > m_Position.Y
                && pos.Y + rec.Height < m_Position.Y + m_Bounds.Height)))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public new void Draw(SpriteBatch batch)
        {
            SpriteEffects effect = SpriteEffects.None;

            if (m_Velocity.X > 0)
            {
                effect = SpriteEffects.FlipHorizontally;
            }

            m_CurrentAnimation.Draw(batch, m_Bounds, effect);

            if (this.m_DrawHud == true)
            {
                m_PlayerHud.DrawWithCharSelection(batch, m_HudOrigin);
            }
            else
            {
                m_PlayerHud.DrawWithNoCharSelection(batch, m_HudOrigin);
            }
        }
       
    }
     
}

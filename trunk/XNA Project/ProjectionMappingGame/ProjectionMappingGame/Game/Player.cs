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

        public enum States
        {
            DEAD,
            SPAWNING,
            PLAYING,
            PORTING
        }

        public enum Animations
        {
            IDLE,
            RUN,
            JUMP
        }

        Animation[] m_Animations;
        Vector2 m_Impulse;
        Vector2 m_CollisionImpulse;
        PlayerIndex m_Player;
        PlayerMenu m_PlayerHud;

        StateMachine.GamePlayState m_Parent;

        int m_ColorIndex;
        Color m_PlayerColor;

        Point m_WindowSize;

        float m_SnapY;
        float m_Move;
        bool m_OnGround;

        float m_PortalTimer;
        float m_PortAgainTimer;

        public Player(StateMachine.GamePlayState parent, Texture2D image, Rectangle bounds, GUI.KeyboardInput keyboard, PlayerIndex player) 
            : base(bounds, Vector2.Zero, image)
        {
            m_Parent = parent;

            keyboard.RegisterKeyHeld(onKeyHeld);
            keyboard.RegisterKeyHeld(onKeyDown);

            m_Player = player;

            m_Move = 0;

            m_PlayerHud = new PlayerMenu(player);

            m_Impulse = Vector2.Zero;
            m_CollisionImpulse = Vector2.Zero;

            m_Animations = new Animation[Enum.GetValues(typeof(Animations)).Length];
            m_Animations[(int)Animations.IDLE] = m_CurrentAnimation;

            m_OnGround = false;

            m_SnapY = 0;

            State = States.DEAD;

            m_PortalTimer = GameConstants.PORTAL_DELAY;
            m_PortAgainTimer = GameConstants.PORT_AGAIN_DELAY;

            m_ColorIndex = 0;
            m_PlayerColor = m_Parent.Colors[m_ColorIndex];
            SetColor(Color.White);
        }

        public Player(StateMachine.GamePlayState parent, Texture2D image, Rectangle bounds, GUI.GamepadInput gamepad, PlayerIndex player)
            : base(bounds, Vector2.Zero, image)
        {
            m_Parent = parent;

            gamepad.RegisterAxisEvent(onAxisChange, player);
            gamepad.RegisterButtonEvent(GUI.GamepadEventType.BUTTON_DOWN, onButtonHold, player);
            gamepad.RegisterButtonEvent(GUI.GamepadEventType.BUTTON_DOWN, onButtonDown, player);

            m_Player = player;

            m_Move = 0;


            m_PlayerHud = new PlayerMenu(player);

            m_Impulse = Vector2.Zero;

            m_Animations = new Animation[Enum.GetValues(typeof(Animations)).Length];
            m_Animations[(int)Animations.IDLE] = m_CurrentAnimation;

            m_OnGround = false;

            m_SnapY = 0;

            State = States.DEAD;

            m_PortalTimer = GameConstants.PORTAL_DELAY;
            m_PortAgainTimer = GameConstants.PORT_AGAIN_DELAY;

            m_ColorIndex = 0;
            m_PlayerColor = m_Parent.Colors[m_ColorIndex];
            SetColor(Color.White);
        }

        public Player(StateMachine.GamePlayState parent, Texture2D image, Rectangle bounds, GUI.GamepadInput gamepad, GUI.KeyboardInput keyboard, PlayerIndex player)
            : base(bounds, Vector2.Zero, image)
        {
            m_Parent = parent;

            keyboard.RegisterKeyDown(onKeyDown);
            keyboard.RegisterKeyHeld(onKeyHeld);

            gamepad.RegisterAxisEvent(onAxisChange, player);
            gamepad.RegisterButtonEvent(GUI.GamepadEventType.BUTTON_HOLD, onButtonHold, player);
            gamepad.RegisterButtonEvent(GUI.GamepadEventType.BUTTON_DOWN, onButtonDown, player);
            
            m_Player = player;

            m_Move = 0;

            m_Impulse = Vector2.Zero;

          
            m_PlayerHud = new PlayerMenu(player);

            m_Animations = new Animation[Enum.GetValues(typeof(Animations)).Length];
            m_Animations[(int)Animations.IDLE] = m_CurrentAnimation;

            m_OnGround = false;

            State = States.DEAD;

            m_PortalTimer = GameConstants.PORTAL_DELAY;
            m_PortAgainTimer = GameConstants.PORT_AGAIN_DELAY;

            m_ColorIndex = 0;
            m_PlayerColor = m_Parent.Colors[m_ColorIndex];
            SetColor(Color.White);
        }

        public States State
        {
            get;
            set;
        }

        public PlayerMenu HUD
        {
            get { return m_PlayerHud; } 
        }

        public PlayerIndex PlayerNumber
        {
            get
            {
                return m_Player;
            }
        }

        public void MovePlayer(Level lvl)
        {
            m_WindowSize = lvl.WindowSize;

            m_PlayerHud.WindowSize = lvl.WindowSize;
        }

        public void SetColor(Color c)
        {
            foreach (Animation a in m_Animations)
            {
                if (a != null)
                {
                    a.SetColor(c);
                }
            }
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

        public void LoadHudContent(SpriteFont font, GraphicsDevice device, Texture2D background)
        {
            m_PlayerHud.LoadContent(font,device,background);
        }

        private void onKeyDown(object sender, Keys[] keys)
        {
            switch(State)
            {
                case States.PLAYING:
                    if (keys.Contains(Keys.Space))// && m_OnGround)
                    {
                        if (m_OnGround)
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
                    break;
                case States.DEAD:
                    if (keys.Contains(Keys.Enter))
                    {
                        m_Position.Y = GameConstants.START_Y;
                        m_Position.X = (m_WindowSize.X / 2.0f) + (((int)m_Player - GameConstants.MAX_PLAYERS / 2)) * (GameConstants.PLAYER_DIM_X + GameConstants.START_SPACING);
                        m_Bounds.X = (int)m_Position.X;
                        m_Bounds.Y = (int)m_Position.Y;

                        State = States.SPAWNING;
                    }
                    break;
                case States.SPAWNING:
                    if (keys.Contains(Keys.Space))
                    {
                        m_ColorIndex %= m_Parent.Colors.Count;

                        List<Color> colors = m_Parent.Colors;
                        m_PlayerColor = colors[m_ColorIndex];
                        colors.RemoveAt(m_ColorIndex);
                        m_Parent.Colors = colors;

                        SetColor(m_PlayerColor);
                        State = States.PLAYING;
                    }
                    if (keys.Contains(Keys.A))
                    {
                        m_ColorIndex = (m_ColorIndex - 1) % m_Parent.Colors.Count;

                        if (m_ColorIndex < 0)
                        {
                            m_ColorIndex = GameConstants.GAME_COLORS.Length - 1;
                        }

                        m_Animations[(int)Animations.IDLE].SetColor(m_Parent.Colors[m_ColorIndex]);
                    }
                    if (keys.Contains(Keys.D))
                    {
                        m_ColorIndex = (m_ColorIndex + 1) % m_Parent.Colors.Count;
                        m_Animations[(int)Animations.IDLE].SetColor(m_Parent.Colors[m_ColorIndex]);
                    }
                    break;
            }
        }

        private void onKeyHeld(object sender, Keys[] keys)
        {
            if (State == States.PLAYING)
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
        }

        private void onButtonDown(object sender, GUI.GamepadInput.Buttons button)
        {
            switch (State)
            {
                case States.PLAYING:
                    if (button.Equals(GUI.GamepadInput.Buttons.A) && m_OnGround)
                    {
                        m_Impulse += Vector2.UnitY * GameConstants.JUMP_IMPULSE;

                        if (m_Animations[(int)Animations.JUMP] != null)
                        {
                            m_CurrentAnimation = m_Animations[(int)Animations.JUMP];
                            m_CurrentAnimation.Reset();
                        }
                    }
                    break;
                case States.DEAD:
                    if (button.Equals(GUI.GamepadInput.Buttons.START))
                    {
                        m_Position.Y = GameConstants.START_Y;
                        m_Position.X = (m_WindowSize.X / 2.0f) + (((int)m_Player - GameConstants.MAX_PLAYERS / 2)) * (GameConstants.PLAYER_DIM_X + GameConstants.START_SPACING);
                        m_Bounds.X = (int)m_Position.X;
                        m_Bounds.Y = (int)m_Position.Y;

                        State = States.SPAWNING;
                    }
                    break;
                case States.SPAWNING:
                    List<Color> colors = m_Parent.Colors;

                    if (button.Equals(GUI.GamepadInput.Buttons.A))
                    {
                        m_ColorIndex %= m_Parent.Colors.Count;

                        m_PlayerColor = colors[m_ColorIndex];
                        
                        colors.RemoveAt(m_ColorIndex);
                        m_Parent.Colors = colors;

                        SetColor(m_PlayerColor);
                        State = States.PLAYING;
                    }
                    if (button.Equals(GUI.GamepadInput.Buttons.LB))
                    {
                        m_ColorIndex = (m_ColorIndex - 1) % m_Parent.Colors.Count;

                        if (m_ColorIndex < 0)
                        {
                            m_ColorIndex = colors.Count - 1;
                        }

                        m_PlayerColor = colors[m_ColorIndex];

                        m_Animations[(int)Animations.IDLE].SetColor(m_Parent.Colors[m_ColorIndex]);
                    }
                    if (button.Equals(GUI.GamepadInput.Buttons.RB))
                    {
                        m_ColorIndex = (m_ColorIndex + 1) % m_Parent.Colors.Count;

                        m_PlayerColor = colors[m_ColorIndex];

                        m_Animations[(int)Animations.IDLE].SetColor(m_Parent.Colors[m_ColorIndex]);
                    }
                    break;
            }
        }

        private void onButtonHold(object sender, GUI.GamepadInput.Buttons button)
        {
            
        }

        private void onAxisChange(object sender, GUI.GamepadInput.Axis axis, float degree)
        {
            if (State == States.PLAYING)
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
        }

        public new void Update(float deltaTime)
        {
            switch(State)
            {
                case States.PLAYING:
                    if (m_PortAgainTimer > 0)
                    {
                        m_PortAgainTimer -= deltaTime;
                    }
                    
                    m_PlayerHud.Update(deltaTime);

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

                    m_Velocity.Y = (m_Velocity.Y > GameConstants.MAX_FALL_SPEED) ? GameConstants.MAX_FALL_SPEED : m_Velocity.Y;

                    base.Update(deltaTime);

                    if (m_OnGround)
                    {
                        m_Position.Y = m_SnapY;
                    }
                    break;

                case States.PORTING:
                    m_PortalTimer -= deltaTime;

                    if (m_PortalTimer <= 0)
                    {
                        m_PortalTimer = GameConstants.PORTAL_DELAY;
                        m_PortAgainTimer = GameConstants.PORT_AGAIN_DELAY;

                        State = States.PLAYING;
                    }
                    break;

                case States.SPAWNING:
                    m_PlayerColor = m_Parent.Colors[m_ColorIndex];
                    m_Animations[(int)Animations.IDLE].SetColor(m_PlayerColor);
                    break;
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

                    if (CheckTop(nextPosition, platPos, platPos, platBounds))
                    {
                        p.Collide(this, t, CollisionDirections.Top);
                    }
                    if (CheckBot(nextPosition, platPos, platPos, platBounds))
                    {
                        p.Collide(this, t, CollisionDirections.Bot);
                    }
                    if (CheckLeft(nextPosition, platPos, platBounds))
                    {
                        p.Collide(this, t, CollisionDirections.Left);        
                    }
                    if (CheckRight(nextPosition, platPos, platBounds))
                    {
                        p.Collide(this, t, CollisionDirections.Right);        
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

        public int CheckCollisions(List<Portal> portals, float deltaTime)
        {
            if (m_PortAgainTimer > 0)
            {
                return -1;
            }

            Vector2 nextVelocity = m_Velocity + m_Impulse * deltaTime;
            nextVelocity.X = m_Move * GameConstants.MOVE_SPEED * deltaTime;
            nextVelocity.Y += GameConstants.GRAVITY * deltaTime;
            Vector2 nextPosition = m_Position + nextVelocity * deltaTime;

            Rectangle rect = new Rectangle((int)nextPosition.X, (int)nextPosition.Y, m_Bounds.Width, m_Bounds.Height);

            foreach (Portal p in portals)
            {
                Rectangle portalRect = new Rectangle(p.Bounds.X + (int)(p.Bounds.Width / 3.0f), p.Bounds.Y + (int)(p.Bounds.Height / 3.0f), (int)(p.Bounds.Width / 3.0f), (int)(p.Bounds.Height / 3.0f));
                if(portalRect.Intersects(rect))
                {
                    return p.Destination;
                }
            }

            return -1;
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

        public void Kill()
        {
            List<Color> colors = m_Parent.Colors;
            colors.Add(m_PlayerColor);
            m_Parent.Colors = colors;

            m_PlayerHud.Reset();
            m_Position = Vector2.Zero;
            m_Velocity = Vector2.Zero;
            State = States.DEAD;
        }

        public new void Draw(SpriteBatch batch)
        {
            SpriteEffects effect = SpriteEffects.None;

            if (m_Velocity.X > 0)
            {
                effect = SpriteEffects.FlipHorizontally;
            }

            switch(State)
            {
                case States.PLAYING:
                    m_CurrentAnimation.Draw(batch, m_Bounds, effect);
                    break;
                case States.SPAWNING:
                    m_Animations[(int)Animations.IDLE].Draw(batch, m_Bounds, effect);
                    break;
                case States.PORTING:
                    break;
                case States.DEAD:
                    break;
            }
        }

        public void DrawHUD(SpriteBatch batch)
        {
            switch (State)
            {
                case States.PLAYING:
                    m_PlayerHud.DrawWithNoCharSelection(batch, m_PlayerColor);
                    break;
                case States.SPAWNING:
                    m_PlayerHud.DrawWithCharSelection(batch, m_PlayerColor);
                    break;
                case States.PORTING:
                    m_PlayerHud.DrawWithNoCharSelection(batch, m_PlayerColor);
                    break;
                case States.DEAD:
                    break;
            }
        }

        public void CollideTop()
        {
            m_Velocity.Y = 0;
            m_Impulse.Y = 0;
        }

        public void CollideBot(Tile t)
        {
            m_Velocity.Y = t.Velocity.Y;
            m_SnapY = t.Position.Y - m_Bounds.Height - 0.00001f;
            m_OnGround = true;
        }

        public void CollideLeft()
        {
            m_Velocity.X = 0;
            m_Move = (m_Move < 0) ? 0 : m_Move;
        }

        public void CollideRight()
        {
            m_Velocity.X = 0;
            m_Move = (m_Move > 0) ? 0 : m_Move;
        }
       
    }
     
}

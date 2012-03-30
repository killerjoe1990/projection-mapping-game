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
        public enum Bonus
        {
            NONE,
            RECOVERING,
            INVINCIBLE,
            STUNNED
        }

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
        int m_PlayerColor;

        Point m_WindowSize;

        float m_SnapY;
        float m_Move;
        bool m_OnGround;

        float m_PortalTimer;
        float m_PortAgainTimer;

        float m_SpeedMult;
        float m_SpeedTimer;

        Bonus m_Status;
        float m_StatusTimer;

        public Player(StateMachine.GamePlayState parent, Texture2D image, Rectangle bounds, GUI.KeyboardInput keyboard, PlayerIndex player) 
            : base(bounds, Vector2.Zero, image)
        {
            keyboard.RegisterKeyHeld(onKeyHeld);
            keyboard.RegisterKeyHeld(onKeyDown);

            Initialize(player, parent);
        }

        public Player(StateMachine.GamePlayState parent, Texture2D image, Rectangle bounds, GUI.GamepadInput gamepad, PlayerIndex player)
            : base(bounds, Vector2.Zero, image)
        {
            gamepad.RegisterAxisEvent(onAxisChange, player);
            gamepad.RegisterButtonEvent(GUI.GamepadEventType.BUTTON_DOWN, onButtonHold, player);
            gamepad.RegisterButtonEvent(GUI.GamepadEventType.BUTTON_DOWN, onButtonDown, player);

            Initialize(player, parent);
        }

        public Player(StateMachine.GamePlayState parent, Texture2D image, Rectangle bounds, GUI.GamepadInput gamepad, GUI.KeyboardInput keyboard, PlayerIndex player)
            : base(bounds, Vector2.Zero, image)
        {
            keyboard.RegisterKeyDown(onKeyDown);
            keyboard.RegisterKeyHeld(onKeyHeld);

            gamepad.RegisterAxisEvent(onAxisChange, player);
            gamepad.RegisterButtonEvent(GUI.GamepadEventType.BUTTON_HOLD, onButtonHold, player);
            gamepad.RegisterButtonEvent(GUI.GamepadEventType.BUTTON_DOWN, onButtonDown, player);

            Initialize(player, parent);
        }

        private void Initialize(PlayerIndex player, StateMachine.GamePlayState parent)
        {
            m_Parent = parent;

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
            m_PlayerColor = 0;
            SetColor(Color.White);

            m_SpeedMult = 1;
            m_SpeedTimer = 0;

            m_Status = Bonus.NONE;
            m_StatusTimer = 0;
        }

        public States State
        {
            get;
            set;
        }

        public Bonus Status
        {
            get
            {
                return m_Status;
            }
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

            //m_PlayerHud.WindowSize = lvl.WindowSize;
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
                        if (m_OnGround && m_Status != Bonus.STUNNED)
                        {
                            m_Impulse += Vector2.UnitY * GameConstants.JUMP_IMPULSE * m_SpeedMult;

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

                        List<int> colors = m_Parent.Colors;
                        m_PlayerColor = colors[m_ColorIndex];
                        colors.RemoveAt(m_ColorIndex);
                        m_Parent.Colors = colors;

                        SetColor(GameConstants.GAME_COLORS[m_PlayerColor]);
                        State = States.PLAYING;
                    }
                    if (keys.Contains(Keys.A))
                    {
                        m_ColorIndex = (m_ColorIndex - 1) % m_Parent.Colors.Count;

                        if (m_ColorIndex < 0)
                        {
                            m_ColorIndex = m_Parent.Colors.Count - 1;
                        }

                        m_Animations[(int)Animations.IDLE].SetColor(GameConstants.GAME_COLORS[m_Parent.Colors[m_ColorIndex]]);
                    }
                    if (keys.Contains(Keys.D))
                    {
                        m_ColorIndex = (m_ColorIndex + 1) % m_Parent.Colors.Count;
                        m_Animations[(int)Animations.IDLE].SetColor(GameConstants.GAME_COLORS[m_Parent.Colors[m_ColorIndex]]);
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
                        float mult = (float)Math.Pow((double)m_SpeedMult, 0.25);

                        m_Impulse += Vector2.UnitY * GameConstants.JUMP_IMPULSE * (mult);

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

                        for (int i = 0; i < m_Parent.Colors.Count; ++i)
                        {
                            if (m_Parent.Colors[i] == m_PlayerColor)
                            {
                                m_ColorIndex = i;
                                break;
                            }
                        }

                        State = States.SPAWNING;
                    }
                    break;
                case States.SPAWNING:
                    List<int> colors = m_Parent.Colors;

                    if (button.Equals(GUI.GamepadInput.Buttons.A))
                    {
                        m_ColorIndex %= m_Parent.Colors.Count;

                        m_PlayerColor = colors[m_ColorIndex];
                        colors.Remove(m_PlayerColor);
                        m_Parent.Colors = colors;

                        SetColor(GameConstants.GAME_COLORS[m_PlayerColor]);
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

                        m_Animations[(int)Animations.IDLE].SetColor(GameConstants.GAME_COLORS[colors[m_ColorIndex]]);
                    }
                    if (button.Equals(GUI.GamepadInput.Buttons.RB))
                    {
                        m_ColorIndex = (m_ColorIndex + 1) % m_Parent.Colors.Count;

                        m_PlayerColor = colors[m_ColorIndex];

                        m_Animations[(int)Animations.IDLE].SetColor(GameConstants.GAME_COLORS[colors[m_ColorIndex]]);
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

                    if (m_Status != Bonus.NONE)
                    {
                        if (m_StatusTimer > 0)
                        {
                            m_StatusTimer -= deltaTime;
                        }
                        else
                        {
                            if (m_Status == Bonus.RECOVERING)
                            {
                                m_Status = Bonus.NONE;
                            }
                            else
                            {
                                ChangeStatus(Bonus.RECOVERING, GameConstants.PORT_AGAIN_DELAY);
                            }
                        }
                    }

                    if (m_SpeedTimer > 0)
                    {
                        m_SpeedTimer -= deltaTime;
                    }
                    else
                    {
                        m_SpeedMult = 1;
                    }

                    m_PlayerHud.Update(deltaTime);

                    m_Velocity += m_Impulse * deltaTime;
                    m_Velocity += m_CollisionImpulse * deltaTime;
                    m_Impulse = Vector2.Zero;
                    m_CollisionImpulse = Vector2.Zero;

                    switch (m_Status)
                    {
                        case Bonus.STUNNED:
                            m_Move = 0;
                            break;
                    }

                    m_Velocity.X = m_Move * GameConstants.MOVE_SPEED * m_SpeedMult * deltaTime;
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
                        ChangeStatus(Bonus.RECOVERING, GameConstants.PORTAL_DELAY);
                        //m_PortalTimer = GameConstants.PORTAL_DELAY;
                        m_PortAgainTimer = GameConstants.PORT_AGAIN_DELAY;

                        State = States.PLAYING;
                    }
                    break;

                case States.SPAWNING:
                    m_ColorIndex %= m_Parent.Colors.Count;
                    m_PlayerColor = m_Parent.Colors[m_ColorIndex];
                    m_Animations[(int)Animations.IDLE].SetColor(GameConstants.GAME_COLORS[m_PlayerColor]);
                    break;
            }
        }

        public void CheckCollisions(List<Platform> objects, float deltaTime)
        {
            m_OnGround = false;
            foreach (Platform p in objects)
            {
                foreach (Tile o in p.Tiles)
                {
                    Vector2 nextVelocity;

                    nextVelocity = m_Velocity + m_Impulse * deltaTime;
                    nextVelocity.X = m_Move * GameConstants.MOVE_SPEED * deltaTime;
                    nextVelocity.Y += GameConstants.GRAVITY * deltaTime;
                    Vector2 nextPosition = m_Position + nextVelocity * deltaTime;
                    Vector2 platPos = o.Position;
                    Rectangle platBounds = o.Bounds;

                    if (CheckTop(nextPosition, platPos, platPos, platBounds))
                    {
                        if (p.Collide(this, CollisionDirections.Top))
                        {
                            Collide(o, CollisionDirections.Top);
                        }
                    }
                    if (CheckBot(nextPosition, platPos, platPos, platBounds))
                    {
                        if (p.Collide(this, CollisionDirections.Bot))
                        {
                            Collide(o, CollisionDirections.Bot);
                        }
                    }
                    if (CheckLeft(nextPosition, platPos, platBounds))
                    {
                        if (p.Collide(this, CollisionDirections.Left))
                        {
                            Collide(o, CollisionDirections.Left);
                        }
                    }
                    if (CheckRight(nextPosition, platPos, platBounds))
                    {
                        if (p.Collide(this, CollisionDirections.Right))
                        {
                            Collide(o, CollisionDirections.Right);
                        }
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

            switch (m_Status)
            {
                case Bonus.NONE:
                    if (player.Status != Bonus.INVINCIBLE)
                    {
                        if (CheckTop(nextPosition, otherPos, player.GetNextPosition(deltaTime), otherBound))
                        {
                            m_CollisionImpulse += Vector2.UnitY * GameConstants.BOUNCE_IMPULSE_DOWN;
                            m_Velocity.Y = 0;
                        }
                        if (CheckBot(nextPosition, otherPos, player.GetNextPosition(deltaTime), otherBound))
                        {
                            m_CollisionImpulse += Vector2.UnitY * GameConstants.BOUNCE_IMPULSE_UP;
                            m_Velocity.Y = 0;
                        }
                    }
                    break;
                case Bonus.INVINCIBLE:
                    if (CheckBot(nextPosition, otherPos, player.GetNextPosition(deltaTime), otherBound))
                    {
                        m_CollisionImpulse += Vector2.UnitY * GameConstants.BOUNCE_IMPULSE_UP;
                        m_Velocity.Y = 0;
                    }

                    Rectangle me = new Rectangle((int)nextPosition.X, (int)nextPosition.Y, m_Bounds.Width, m_Bounds.Height);
                    Rectangle them = new Rectangle((int)otherPos.X, (int)otherPos.Y, otherBound.Width, otherBound.Height);
                    if (me.Intersects(them) && player.Status != Bonus.STUNNED)
                    {
                        player.ChangeStatus(Bonus.STUNNED, GameConstants.STUN_TIME);
                    }
                    break;
            }
        }

        public int CheckCollisions(List<Portal> portals, float deltaTime)
        {
            if (m_Status == Bonus.RECOVERING)
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

        public void CheckCollisions(List<Collectable> objects, float deltaTime)
        {
            Vector2 nextVelocity = m_Velocity + m_Impulse * deltaTime;
            nextVelocity.X = m_Move * GameConstants.MOVE_SPEED * deltaTime;
            nextVelocity.Y += GameConstants.GRAVITY * deltaTime;
            Vector2 nextPosition = m_Position + nextVelocity * deltaTime;

            Rectangle rect = new Rectangle((int)nextPosition.X, (int)nextPosition.Y, m_Bounds.Width, m_Bounds.Height);

            foreach (Collectable obj in objects)
            {
                if (rect.Intersects(obj.Bounds))
                {
                    obj.Collide(this);
                }
            }
        }

        public void AddSpeedMultiplier(float mult, float time)
        {
            m_SpeedMult = mult;
            m_SpeedTimer = time;
        }

        public bool ChangeStatus(Bonus status, float timer)
        {
            if (m_Status != Bonus.RECOVERING)
            {
                m_Status = status;
                m_StatusTimer = timer;
                return true;
            }
            else
            {
                return false;
            }
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
            List<int> colors = m_Parent.Colors;
            colors.Add(m_PlayerColor);
            m_Parent.Colors = colors;

            m_PlayerHud.Reset();
            m_Position = Vector2.Zero;
            m_Velocity = Vector2.Zero;
            State = States.DEAD;
            m_Status = Bonus.NONE;

            m_SpeedMult = 1;
            m_SpeedTimer = 0;
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
                    
                    switch (m_Status)
                    {
                        case Bonus.INVINCIBLE:
                            int randIndex = GameConstants.RANDOM.Next(GameConstants.GAME_COLORS.Length);
                            m_CurrentAnimation.SetColor(GameConstants.GAME_COLORS[randIndex]);
                            break;
                        case Bonus.RECOVERING:
                            Color c = GameConstants.GAME_COLORS[m_PlayerColor];
                            //float randIntensity = (float)GameConstants.RANDOM.NextDouble() * 0.8f + 0.2f;
                            float randIntensity = ((float)Math.Sin(10 * (double)m_StatusTimer) + 1.0f) / 2.0f;
                            c.A = (byte)(randIntensity * 255);
                            m_CurrentAnimation.SetColor(c);
                            break;
                        case Bonus.STUNNED:
                            float intensity = ((float)Math.Sin(10 * (double)m_StatusTimer) + 1.0f) / 2.0f;
                            Color c1 = GameConstants.GAME_COLORS[m_PlayerColor];
                            c1 = Color.Multiply(c1, intensity);
                            c1.A = 255;
                            m_CurrentAnimation.SetColor(c1);
                            break;
                        case Bonus.NONE:
                            m_CurrentAnimation.SetColor(GameConstants.GAME_COLORS[m_PlayerColor]);
                            break;
                    }

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
                    m_PlayerHud.DrawWithNoCharSelection(batch, GameConstants.GAME_COLORS[m_PlayerColor]);
                    break;
                case States.SPAWNING:
                    m_PlayerHud.DrawWithCharSelection(batch, GameConstants.GAME_COLORS[m_PlayerColor]);
                    break;
                case States.PORTING:
                    m_PlayerHud.DrawWithNoCharSelection(batch, GameConstants.GAME_COLORS[m_PlayerColor]);
                    break;
                case States.DEAD:
                    break;
            }
        }

        public bool Collide(Tile obj, CollisionDirections dir)
        {
            Vector2 velocity = obj.Velocity;
            Vector2 position = obj.Position;

            switch (dir)
            {
                case CollisionDirections.Top:
                    m_Velocity.Y = 0;
                    m_Impulse.Y = 0;
                    break;
                case CollisionDirections.Bot:
                    m_Velocity.Y = velocity.Y;
                    m_SnapY = position.Y - m_Bounds.Height - 0.00001f;
                    m_OnGround = true;
                    break;
                case CollisionDirections.Left:
                    m_Velocity.X = 0;
                    m_Move = (m_Move < 0) ? 0 : m_Move;
                    break;
                case CollisionDirections.Right:
                    m_Velocity.X = 0;
                    m_Move = (m_Move > 0) ? 0 : m_Move;
                    break;
            }

            return true;
        }     
    }
     
}

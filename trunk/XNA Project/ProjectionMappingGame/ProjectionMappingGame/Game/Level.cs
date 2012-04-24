using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// XNA includes
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ProjectionMappingGame.Game
{
    public class Level
    {
        RenderTarget2D m_PlatformShadowTarget;
        RenderTarget2D m_PlayerShadowTarget;
        Vector2 m_ShadowOffset;
        Color SHADOW_COLOR = new Color(0, 0, 0, 0.6f);

        Matrix m_NormalRotation;
        Vector3 m_Normal;

        Theme m_NextTheme;
        Theme m_CurrentTheme;

        bool m_ChangeTheme;
        float m_ThemeTimerLast;
        float m_ThemeTimer;

        float m_TitleFade;
        Texture2D m_TitlePage;
        Rectangle m_TitleRect;
        bool m_ShowTitle;

        //Parent
        StateMachine.GamePlayState m_GameState;

        // Render target
        bool m_RenderTargetMode;
        Texture2D m_RenderTargetTexture;
        RenderTarget2D m_RenderTarget;

        int m_WindowWidth;
        int m_WindowHeight;

        int m_LevelNum;

        // Input
        GUI.KeyboardInput m_Keyboard;
        GUI.GamepadInput m_Gamepad;

        List<Game.Player> m_Players;
        List<Game.Portal> m_Portals;

        List<Game.Platform> m_Platforms;
        Game.PlatformSpawner m_PlatSpawn;

        List<Collectable> m_MiscObjects;

        public Level(StateMachine.GamePlayState state, int lvlNum, Game.PlatformSpawner spawner, StateMachine.ThemeTextures theme, GUI.KeyboardInput keyboard, GUI.GamepadInput gamepad, int width, int height, Vector3 normal)
        {
            SetNormal(normal);
            m_WindowHeight = height;
            m_WindowWidth = width;

            m_GameState = state;
            m_LevelNum = lvlNum;

            m_Players = new List<Game.Player>();

            m_Platforms = new List<Game.Platform>();

            m_PlatSpawn = spawner;

            m_MiscObjects = new List<Collectable>();

            m_Gamepad = gamepad;
            m_Keyboard = keyboard;

            m_Portals = new List<Portal>();

            // Initialize render target
            m_RenderTargetMode = false;
            m_RenderTarget = new RenderTarget2D(m_GameState.Graphics, m_WindowWidth, m_WindowHeight, true, m_GameState.Graphics.DisplayMode.Format, DepthFormat.Depth24);

            m_PlatformShadowTarget = new RenderTarget2D(m_GameState.Graphics, m_WindowWidth, m_WindowHeight, true, m_GameState.Graphics.DisplayMode.Format, DepthFormat.Depth24);
            m_PlayerShadowTarget = new RenderTarget2D(m_GameState.Graphics, m_WindowWidth, m_WindowHeight, true, m_GameState.Graphics.DisplayMode.Format, DepthFormat.Depth24);

            m_NextTheme = CreateTheme(theme);
            SwapTheme();
            m_ThemeTimer = m_ThemeTimerLast = 0;

            m_ShowTitle = false;
            m_TitleFade = 0;
            m_TitleRect = new Rectangle((int)(width * 0.25f), (int)(height * 0.25f), (int)(width * 0.5f), (int)(height * 0.5f));

            m_TitlePage = m_GameState.TitleScreen;
            // DEBUG
            //m_ShadowOffset = new Vector2(-10, -10);

            //m_NormalRotation = Matrix.Identity;
        }

        public void Update(float elapsedTime)
        {
            // DEBUG moving shadows
            //m_ShadowOffset.X += elapsedTime;
            //m_ShadowOffset.Y += elapsedTime;

            m_CurrentTheme.Update(elapsedTime);

            if (m_ChangeTheme)
            {
                m_ThemeTimerLast = m_ThemeTimer;
                m_ThemeTimer += elapsedTime;
            }

            if (m_ShowTitle)
            {
                m_TitleFade += (elapsedTime / GameConstants.TITLE_FADE);
            }
            else
            {
                m_TitleFade -= (elapsedTime / GameConstants.TITLE_FADE);
            }

            // Update any logic here
            List<Platform> newPlats = m_PlatSpawn.SpawnPlatforms(elapsedTime);

            // Add newly created platfoms to the list
            if (newPlats != null)
            {
                m_Platforms.AddRange(newPlats);
            }

            // Update all platforms FIRST
            for (int i = m_Platforms.Count - 1; i >= 0; --i)
            {
                m_Platforms[i].Update(elapsedTime);

                if (m_Platforms[i].Status == PlatformStatus.Dead || m_Platforms[i].Position.Y >= m_WindowHeight + GameConstants.TILE_DIM*2)
                {
                    m_Platforms.RemoveAt(i);
                }
            }

            // Update portals
            foreach (Portal portal in m_Portals)
            {
                portal.Update(elapsedTime);
            }

            // Update other objects 
            for (int i = m_MiscObjects.Count - 1; i >= 0; --i)
            {
                m_MiscObjects[i].Update(elapsedTime);

                if (!m_MiscObjects[i].Active)
                {
                    m_MiscObjects.RemoveAt(i);
                }
            }

            CheckBounds(m_MiscObjects);

            // Check all player/platform collisions and player/portal collisions
            for (int i = m_Players.Count - 1; i >= 0; --i)
            {
                Game.Player player = m_Players[i];

                if (player != null && player.State == Player.States.PLAYING)
                {
                    player.CheckCollisions(m_Platforms, elapsedTime);

                    player.CheckCollisions(m_MiscObjects, elapsedTime);

                    int portalDest = player.CheckCollisions(m_Portals, elapsedTime);

                    if (portalDest != -1)
                    {
                        m_Players.RemoveAt(i);
                        m_GameState.TransferPlayer(player, m_LevelNum, portalDest);
                    }
                }
            }

            // Check all player/player collisions
            foreach (Player player in m_Players)
            {
                if (player != null && player.State == Player.States.PLAYING)
                {
                    foreach (Player p in m_Players)
                    {
                        if (p != null && p.State == Player.States.PLAYING && p != player)
                        {
                            player.CheckCollision(p, elapsedTime);
                        }
                    }
                }
            }

            // Update each player
            foreach (Player player in m_Players)
            {
                if (player != null)
                {
                    player.Update(elapsedTime);

                    CheckBounds(player);
                }
            }
        }

        

        public void HandleInput()
        {
            foreach (Player p in m_Players)
            {
                m_Gamepad.HandleInput(p.PlayerNumber);
            }
        }

        public void Draw(SpriteBatch spriteBatch, GraphicsDevice device)
        {
            CreateShadows(spriteBatch, device);
            if (m_RenderTargetMode)
            {
                Color clear = new Color(0.0f, 0.0f, 0.0f, 0.0f);

                // Render the quads into the render target
                device.SetRenderTarget(m_RenderTarget);
                device.Clear(clear);

                RenderGame(spriteBatch);

                // Extract and store the contents of the render target in a texture
                device.SetRenderTarget(null);
                device.Clear(Color.CornflowerBlue);
                m_RenderTargetTexture = (Texture2D)m_RenderTarget;
            }
            else
            {
                RenderGame(spriteBatch);
            } 
        }

        private void CreateShadows(SpriteBatch spriteBatch, GraphicsDevice device)
        {
            device.SetRenderTarget(m_PlatformShadowTarget);
            device.Clear(Color.Transparent);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

            foreach (Platform platform in m_Platforms)
            {
                platform.Draw(spriteBatch, Color.White);
            }

            foreach (MoveableObject obj in m_MiscObjects)
            {
                obj.Draw(spriteBatch);
            }

            spriteBatch.End();

            device.SetRenderTarget(m_PlayerShadowTarget);
            device.Clear(Color.Transparent);

            spriteBatch.Begin();

            foreach (Player player in m_Players)
            {
                if (player != null)
                {
                    player.Draw(spriteBatch);
                }
            }

            spriteBatch.End();

            device.SetRenderTarget(null);
        }

        private void RenderGame(SpriteBatch spriteBatch)
        {
            Color themeFadeColor = Color.White;

            if (m_ChangeTheme)
            {
                float intensity = ((m_ThemeTimer * m_ThemeTimer) / (GameConstants.BACKGROUND_FADE * GameConstants.BACKGROUND_FADE));

                themeFadeColor.R = (byte)(255 * intensity);
                themeFadeColor.G = (byte)(255 * intensity);
                themeFadeColor.B = (byte)(255 * intensity);

                if (m_ThemeTimerLast <= 0 && m_ThemeTimer > 0)
                {
                    SwapTheme();
                }
                else
                {
                    if (m_ThemeTimer >= GameConstants.BACKGROUND_FADE)
                    {
                        m_ChangeTheme = false;
                    }
                }
            }

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied);

            // Background always goes in the back.
            m_CurrentTheme.Draw(spriteBatch, themeFadeColor);

            // draw Shadows
            spriteBatch.Draw(m_PlatformShadowTarget, new Rectangle((int)m_ShadowOffset.X, (int)m_ShadowOffset.Y, m_WindowWidth, m_WindowHeight), SHADOW_COLOR);
            // draw player shadow
            spriteBatch.Draw(m_PlayerShadowTarget, new Rectangle((int)m_ShadowOffset.X, (int)m_ShadowOffset.Y, m_WindowWidth, m_WindowHeight), SHADOW_COLOR);

            // draw Portals.
            foreach (Portal portal in m_Portals)
            {
                portal.Draw(spriteBatch);
            }
            // Platforms next.
            foreach (Platform platform in m_Platforms)
            {
                platform.Draw(spriteBatch, themeFadeColor);
            }

            foreach (MoveableObject obj in m_MiscObjects)
            {
                obj.Draw(spriteBatch);
            }


            // HUD goes BEHIND every player.
            /* foreach (Player player in m_Players)
             {
                 if (player != null)
                 {
                     player.DrawHUD(spriteBatch);
                 }
             }*/

            // Players should ALWAYS be on top.
            foreach (Player player in m_Players)
            {
                if (player != null)
                {
                    player.Draw(spriteBatch);
                }
            }

            // Title screen if nobody is playing
            Color titleColor = Color.White;
            m_TitleFade = MathHelper.Clamp(m_TitleFade, 0, 1);
            titleColor.A = (byte)(255 * m_TitleFade);
            spriteBatch.Draw(m_TitlePage, m_TitleRect, titleColor);

            spriteBatch.End();
        }

        private Theme CreateTheme(StateMachine.ThemeTextures tex)
        {
            return new Theme(tex, new Point(m_WindowWidth, m_WindowHeight));
        }

        private void CheckBounds(Player player)
        {
            if (player.State != Player.States.PLAYING)
            {
                return; 
            }

            Vector2 pos = player.Position;

            if (player.Position.Y < 0)
            {
                pos.Y = 0;
            }

            if (player.Position.X < 0)
            {
                pos.X = 0;
            }

            if (player.Position.X + player.Bounds.Width > m_WindowWidth)
            {
                pos.X = m_WindowWidth - player.Bounds.Width;
            }

            if (player.Position.Y > m_WindowHeight)
            {
                player.Kill();
                m_GameState.PlayerDied(player);
            }

            player.Position = pos;
        }

        private void CheckBounds(List<Collectable> collectables)
        {
            foreach (Collectable c in collectables)
            {
                if (c.Active)
                {
                    Vector2 pos = c.Position;
                    Vector2 vel = c.Velocity;

                    if (pos.Y < 0)
                    {
                        pos.Y = 0;
                        vel.Y *= -1;
                    }

                    if (pos.X < 0)
                    {
                        pos.X = 0;
                        vel.X *= -1;
                    }

                    if (pos.X + c.Bounds.Width > m_WindowWidth)
                    {
                        pos.X = m_WindowWidth - c.Bounds.Width;
                        vel.X *= -1;
                    }

                    if (pos.Y + c.Bounds.Height > m_WindowHeight)
                    {
                        pos.Y = m_WindowHeight - c.Bounds.Height;
                        vel.Y *= -1;
                    }

                    c.Position = pos;
                    c.Velocity = vel;
                }
            }
        }

        public void ShowTitle(bool show)
        {
            m_ShowTitle = show;
        }

        public void AddPortal(int dest, Texture2D image, Color c)
        {
            float rangeX = GameConstants.MAX_PORTAL_X - GameConstants.MIN_PORTAL_X;
            float rangeY = GameConstants.MAX_PORTAL_Y - GameConstants.MIN_PORTAL_Y;

            float randX = (float)GameConstants.RANDOM.NextDouble() * rangeX + GameConstants.MIN_PORTAL_X;
            float randY = (float)GameConstants.RANDOM.NextDouble() * rangeY + GameConstants.MIN_PORTAL_Y;

            Rectangle portalRect = new Rectangle((int)(randX * m_WindowWidth),(int)(randY * m_WindowHeight),GameConstants.PORTAL_DIM, GameConstants.PORTAL_DIM);

            foreach (Portal port in m_Portals)
            {
                if (port.Bounds.Intersects(portalRect))
                {
                    AddPortal(dest, image, c);
                    return;
                }
            }

            Portal p = new Portal(portalRect, dest, image, c);
            m_Portals.Add(p);
        }

        public void AddObject(Collectable obj)
        {
            m_MiscObjects.Add(obj);
        }

        public void AddPlayer(Player player, int from)
        {
            m_Players.Add(player);

            foreach (Portal portal in m_Portals)
            {
                if (portal.Destination == from)
                {
                    Vector2 pos = player.Position;
                    pos.X = portal.Position.X;
                    pos.Y = portal.Position.Y;
                    player.Position = pos;
                }
            }
        }

        public Point WindowSize
        {
            get
            {
                return new Point(m_WindowWidth, m_WindowHeight);
            }
            set
            {
                m_WindowWidth = value.X;
                m_WindowHeight = value.Y;
            }
        }

        public void AddPlayer(Player player)
        {
            m_Players.Add(player);
        }

        public RenderTarget2D RenderTarget
        {
            get
            {
                return m_RenderTarget;
            }
        }

        public bool RenderTargetMode
        {
            get
            {
                return m_RenderTargetMode;
            }
            set
            {
                m_RenderTargetMode = value;
            }
        }

        public void SetLight(Vector3 light)
        {
            Vector3 tan, norm;
            VectorComponents(light, m_Normal, out tan, out norm);
            light = light * -1.0f;

            float dp = Vector3.Dot(light, m_Normal);
            float offsetIntensity = MathHelper.Clamp(dp, 0, 1) * GameConstants.MAX_SHADOW;

            tan.Normalize();

            tan = Vector3.Transform(tan, m_NormalRotation);
            Vector2 dir = new Vector2(-tan.X, tan.Y);
            m_ShadowOffset = dir * offsetIntensity;
        }

        public void SetNormal(Vector3 normal)
        {
            normal.Normalize();

            Vector3 forward = normal;
            Vector3 left = Vector3.UnitX;
            Vector3 up = Vector3.Cross(left, forward);

            m_NormalRotation = Matrix.Identity;

            m_NormalRotation.Forward = forward;
            m_NormalRotation.Left = left;
            m_NormalRotation.Up = up;

            m_NormalRotation = Matrix.Invert(m_NormalRotation);

            m_Normal = normal;
        }

        public void ChangeTheme(StateMachine.ThemeTextures theme)
        {
            m_NextTheme = CreateTheme(theme);
            m_ChangeTheme = true;
            m_ThemeTimer = m_ThemeTimerLast = -GameConstants.BACKGROUND_FADE;
        }

        private void SwapTheme()
        {
            if (m_NextTheme != null)
            {
                m_CurrentTheme = m_NextTheme;
            }

            Texture2D[][] plats = m_CurrentTheme.Textures.Platforms;
            m_PlatSpawn.ChangeTheme(plats);

            foreach (Platform p in m_Platforms)
            {
                int platIndex = GameConstants.RANDOM.Next(plats.Length - 1) + 1;

                if (p.Blink)
                {
                    p.ChangeTheme(plats[0]);
                }
                else
                {
                    p.ChangeTheme(plats[platIndex]);
                }
            }

        }

        private void VectorComponents(Vector3 vector, Vector3 normal, out Vector3 tangentVec, out Vector3 normalVec)
        {
            //first, get the component of velocity along the normal.  If it is parallel, the tangental component is 0;
            float normalComponent = Vector3.Dot(vector, normal);
            normalVec = normal * normalComponent;

            //the tangent vector of the velocity vector is 2 cross products.  The first gives you an orthogonal vector to the two
            // the second gives the vector in the plane of the normal and velocity
            Vector3 vCross = Vector3.Cross(vector, normal);
            tangentVec = Vector3.Cross(normal, vCross);
            tangentVec.Normalize();
            float tangentComponent = Vector3.Dot(vector, tangentVec);
            tangentVec = tangentVec * tangentComponent;
        }
    }
}

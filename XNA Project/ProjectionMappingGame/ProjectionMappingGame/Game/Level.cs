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

        Texture2D m_Background;

        List<Game.Player> m_Players;
        List<Game.Portal> m_Portals;

        List<Game.Platform> m_Platforms;
        Game.PlatformSpawner m_PlatSpawn;


        public Level(StateMachine.GamePlayState state, int lvlNum, Game.PlatformSpawner spawner, Texture2D background, GUI.KeyboardInput keyboard, GUI.GamepadInput gamepad, int width, int height)
        {
            m_WindowHeight = height;
            m_WindowWidth = width;

            m_GameState = state;
            m_LevelNum = lvlNum;

            m_Players = new List<Game.Player>();

            m_Platforms = new List<Game.Platform>();

            m_PlatSpawn = spawner;

            m_Background = background;

            m_Gamepad = gamepad;
            m_Keyboard = keyboard;

            m_Portals = new List<Portal>();

            // Initialize render target
            m_RenderTargetMode = false;
            m_RenderTarget = new RenderTarget2D(m_GameState.Graphics, m_WindowWidth, m_WindowHeight, true, m_GameState.Graphics.DisplayMode.Format, DepthFormat.Depth24);

            m_PlatformShadowTarget = new RenderTarget2D(m_GameState.Graphics, m_WindowWidth, m_WindowHeight, true, m_GameState.Graphics.DisplayMode.Format, DepthFormat.Depth24);
            m_PlayerShadowTarget = new RenderTarget2D(m_GameState.Graphics, m_WindowWidth, m_WindowHeight, true, m_GameState.Graphics.DisplayMode.Format, DepthFormat.Depth24);

            // DEBUG
            m_ShadowOffset = new Vector2(-10, -10);

        }

        public void Update(float elapsedTime)
        {
            // DEBUG moving shadows
            //m_ShadowOffset.X += elapsedTime;
            //m_ShadowOffset.Y += elapsedTime;

            // Update any logic here
            List<Platform> newPlats = m_PlatSpawn.SpawnPlatforms(elapsedTime);

            // Add newly created platfoms to the list
            if (newPlats != null)
            {
                m_Platforms.AddRange(newPlats);
            }

            // Update all platforms FIRST
            foreach (Platform platform in m_Platforms)
            {
                platform.Update(elapsedTime);
            }

            // Update portals
            foreach (Portal portal in m_Portals)
            {
                portal.Update(elapsedTime);
            }

            // Check all player/platform collisions and player/portal collisions
            for (int i = m_Players.Count - 1; i >= 0; --i)
            {
                Game.Player player = m_Players[i];

                if (player != null && player.State == Player.States.PLAYING)
                {
                    player.CheckCollisions(m_Platforms, elapsedTime);

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

            spriteBatch.Begin();

            foreach (Platform platform in m_Platforms)
            {
                platform.Draw(spriteBatch);
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

            spriteBatch.Begin();

            // Background always goes in the back.
            spriteBatch.Draw(m_Background, new Rectangle(0, 0, m_WindowWidth, m_WindowHeight), Color.White);

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
                platform.Draw(spriteBatch);
            }

            


            // HUD goes BEHIND every player.
            foreach (Player player in m_Players)
            {
                if (player != null)
                {
                    player.DrawHUD(spriteBatch);
                }
            }

            // Players should ALWAYS be on top.
            foreach (Player player in m_Players)
            {
                if (player != null)
                {
                    player.Draw(spriteBatch);
                }
            }

            spriteBatch.End();
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
    }
}

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

            // Initialize render target
            m_RenderTargetMode = false;
            m_RenderTarget = new RenderTarget2D(m_GameState.Graphics, m_WindowWidth, m_WindowHeight, true, m_GameState.Graphics.DisplayMode.Format, DepthFormat.Depth24);

        }

        public void Update(float elapsedTime)
        {
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

            // Check all player/platform collisions
            foreach (Player player in m_Players)
            {
                if (player != null && player.State == Player.States.PLAYING)
                {
                    player.CheckCollisions(m_Platforms, elapsedTime);
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

                    if (player.Position.Y > m_WindowHeight)
                    {
                        player.Kill();
                        m_GameState.PlayerDied(player);
                    }
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

        private void RenderGame(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();

            // Background always goes in the back.
            spriteBatch.Draw(m_Background, new Rectangle(0, 0, m_WindowWidth, m_WindowHeight), Color.White);


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

        public void AddPortal(int dest, Texture2D image, Color c)
        {
            Portal p = new Portal(Rectangle.Empty, dest, image, c);
            m_Portals.Add(p);
        }

        public void AddPlayer(Player player, int from)
        {
            m_Players.Add(player);

            foreach (Portal portal in m_Portals)
            {
                if (portal.Destination == from)
                {
                    player.Position = portal.Position;
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

        public bool RendterTargetMode
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

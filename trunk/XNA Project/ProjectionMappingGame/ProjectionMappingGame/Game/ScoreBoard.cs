﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace ProjectionMappingGame.Game
{
    public class ScoreBoard
    {
        GameDriver m_Game;
        Texture2D m_RenderTargetTexture;
        Viewport m_Viewport;
        RenderTarget2D m_RenderTarget;
        Texture2D m_BestTimeBackground;
        SpriteFont m_ArialFont;

        int[] indexOfBestPlayersTime;
        int bestPlayerTimeInSeconds;
        int bestPlayerTimeInMinutes;
        int m_MaxScore;

        public ScoreBoard(GameDriver gameDriver,int x,int y,int w, int h)
        {
            this.m_Game = gameDriver;
            // Initialize viewport
            this.m_Viewport = new Viewport(x, y, w, h);
            indexOfBestPlayersTime = new int[4];
            bestPlayerTimeInSeconds = 0;
            bestPlayerTimeInMinutes = 0;
            m_MaxScore = 0;

            // Initialize render target
            m_RenderTarget = new RenderTarget2D(m_Game.GraphicsDevice, w, h, true, m_Game.GraphicsDevice.DisplayMode.Format, DepthFormat.Depth24);
        }
        public void LoadContent(Texture2D texture,SpriteFont font)
        {
            m_BestTimeBackground = texture;
            m_ArialFont = font;
        }

        public void Update(float elapsedTime, Player[] players)
        {
            for (int i = players.Length - 1; i >= 0; i--)
            {
                if (players[i].State == Player.States.PLAYING)
                {
                    if (players[i].HUD.PlayerScore > m_MaxScore)
                    {
                        m_MaxScore = players[i].HUD.PlayerScore;
                    }
                    if (players[i].HUD.TimesMinutes * 60 + players[i].HUD.TimesSeconds >= bestPlayerTimeInSeconds + bestPlayerTimeInMinutes * 60)
                    {
                        indexOfBestPlayersTime[i] = 1;
                        bestPlayerTimeInSeconds = players[i].HUD.TimesSeconds;
                        bestPlayerTimeInMinutes = players[i].HUD.TimesMinutes;
                    }
                    else
                    {
                        indexOfBestPlayersTime[i] = 0;
                    }
                }
            }
        }

        private Rectangle ScreenRect(float x, float y, float w, float h)
        {
            return new Rectangle((int)(m_Viewport.Width * x), (int)(m_Viewport.Height * y), (int)(m_Viewport.Width * w), (int)(m_Viewport.Height * h));
        }
        private Vector2 TransformVec(Vector2 v, Vector2 dim)
        {
            return new Vector2(v.X * dim.X, v.Y * dim.Y);
        }

        public void DrawRenderTarget(SpriteBatch spriteBatch, Player[] players)
        {
            Color clear = new Color(1.0f, 0.0f, 0.0f, 1.0f);
            //spriteBatch.Begin();

            // Render the quads into the render target
            m_Game.GraphicsDevice.SetRenderTarget(m_RenderTarget);
            m_Game.GraphicsDevice.Clear(clear);

            DrawGame(spriteBatch, players);

            m_Game.GraphicsDevice.SetRenderTarget(null);
            m_Game.GraphicsDevice.Clear(Color.Black);
            //m_Game.GraphicsDevice.Viewport = m_Viewport;
            m_RenderTargetTexture = (Texture2D)m_RenderTarget;
        }

        public void Draw(SpriteBatch spriteBatch, Player[] players)
        {
            DrawGame(spriteBatch, players);
        }

        private void DrawGame(SpriteBatch spriteBatch, Player[] players)
        {
            spriteBatch.Begin();

            for (int i = 0; i < players.Length; ++i)
            {
                players[i].HUD.Draw(spriteBatch, players[i].GetAnimation().getColor(), (players[i].State == Player.States.SPAWNING));

            }

            Rectangle background = ScreenRect(0.0f, 0.75f, 1.0f, 0.25f);
            Vector2 dim = new Vector2(m_Viewport.Width, m_Viewport.Height);
            //string strTimeSec = String.Format("{0:00}", bestPlayerTimeInSeconds);
            //string time = bestPlayerTimeInMinutes + ":" + strTimeSec;
            string scoreString = "Best Score " + m_MaxScore;
            spriteBatch.Draw(m_BestTimeBackground, background, Color.White);
            spriteBatch.DrawString(m_ArialFont, scoreString, TransformVec(BEST_TIME_POSITION, dim) + new Vector2(-m_ArialFont.MeasureString(scoreString).Length() / 2, 0.0f), GameConstants.HUD_COLOR);

            spriteBatch.End();
        }

        Vector2 BEST_TIME_POSITION = new Vector2(0.5f, 0.8f);

        #region Public Access TV

        public Texture2D RenderTargetTexture
        {
            get { return m_RenderTargetTexture; }
        }

        #endregion
    }

}

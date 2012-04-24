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
    public class ScoreBoard
    {
        GameDriver m_Game;
        Texture2D m_RenderTargetTexture;
        Viewport m_Viewport;
        RenderTarget2D m_RenderTarget;
        Texture2D m_BestTimeBackground;
        SpriteFont m_ArialFont;

        int m_MaxScore;
        string m_HighScoreName;
        Color m_HighScoreColor;

        public ScoreBoard(GameDriver gameDriver,int x,int y,int w, int h)
        {
            this.m_Game = gameDriver;
            // Initialize viewport
            this.m_Viewport = new Viewport(x, y, w, h);

            m_MaxScore = 0;
            m_HighScoreName = "";

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
                        m_HighScoreName = players[i].PlayerName;
                        m_HighScoreColor = players[i].PlayerColor;
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

            int max = -1;

            for (int i = 0; i < players.Length; ++i)
            {
                if (players[i].State == Player.States.PLAYING && players[i].HUD.PlayerScore > max)
                {
                    max = players[i].HUD.PlayerScore;
                }
            }

            for (int i = 0; i < players.Length; ++i)
            {
                players[i].HUD.Draw(spriteBatch, players[i].PlayerColor, players[i].State, players[i].PlayerName, players[i].NameIndex, (players[i].HUD.PlayerScore >= max));
            }

            Rectangle background = ScreenRect(0.0f, 0.75f, 1.0f, 0.25f);
            Vector2 dim = new Vector2(m_Viewport.Width, m_Viewport.Height);
            //string strTimeSec = String.Format("{0:00}", bestPlayerTimeInSeconds);
            //string time = bestPlayerTimeInMinutes + ":" + strTimeSec;
            string scoreString = "Best Score " + m_MaxScore;
            Vector2 scoreDim = m_ArialFont.MeasureString(scoreString);
            spriteBatch.Draw(m_BestTimeBackground, background, Color.White);
            spriteBatch.DrawString(m_ArialFont, scoreString, TransformVec(BEST_TIME_POSITION, dim) + new Vector2(-m_ArialFont.MeasureString(scoreString).Length() / 2, 0.0f), GameConstants.HUD_COLOR);
            spriteBatch.DrawString(m_ArialFont, m_HighScoreName, TransformVec(HIGHSCORE_NAME_POSITION, dim) + new Vector2(scoreDim.Y + 10, 0) + new Vector2(-(m_ArialFont.MeasureString(m_HighScoreName).Length() + scoreDim.Y) / 2, 0.0f), GameConstants.HUD_COLOR);
            spriteBatch.Draw(GameConstants.WHITE_TEXTURE, new Rectangle((int)(TransformVec(HIGHSCORE_NAME_POSITION, dim) + new Vector2(-(m_ArialFont.MeasureString(m_HighScoreName).Length() + scoreDim.Y) / 2, 0.0f)).X, (int)TransformVec(HIGHSCORE_NAME_POSITION, dim).Y, (int)scoreDim.Y, (int)scoreDim.Y), m_HighScoreColor);
            spriteBatch.End();
        }

        Vector2 BEST_TIME_POSITION = new Vector2(0.5f, 0.8f);
        Vector2 HIGHSCORE_NAME_POSITION = new Vector2(0.5f, 0.9f);


        #region Public Access TV

        public Texture2D RenderTargetTexture
        {
            get { return m_RenderTargetTexture; }
        }

        #endregion
    }

}

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
    class ScoreBoard
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

        public ScoreBoard(GameDriver gameDriver,int x,int y,int w, int h)
        {
            this.m_Game = gameDriver;
            // Initialize viewport
            this.m_Viewport = new Viewport(x, y, w, h);
            indexOfBestPlayersTime = new int[4];
            bestPlayerTimeInSeconds = 0;
            bestPlayerTimeInMinutes = 0;
           
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

        public void DrawRenderTarget(SpriteBatch spriteBatch, Player[] players)
        {
            Color clear = new Color(0.0f, 0.0f, 0.0f, 0.0f);
            spriteBatch.Begin();
            // Render the quads into the render target
            m_Game.GraphicsDevice.SetRenderTarget(m_RenderTarget);
            m_Game.GraphicsDevice.Clear(clear);

            for (int i = players.Length - 1; i >= 0; i--)
            {
                if (players[i].State == Player.States.PLAYING)
                {
                    players[i].HUD.DrawWithNoCharSelection(spriteBatch, players[i].GetAnimation().getColor());

                }
                else if (players[i].State == Player.States.SPAWNING)
                {
                    players[i].HUD.DrawWithCharSelection(spriteBatch, players[i].GetAnimation().getColor());
                }
                else if (players[i].State == Player.States.PORTING)
                {
                    players[i].HUD.DrawWithNoCharSelection(spriteBatch, players[i].GetAnimation().getColor());
                }
                else if (players[i].State == Player.States.DEAD)
                {

                }
            }
            Rectangle background = new Rectangle(0, 495, GameConstants.WindowWidth, GameConstants.WindowHeight - 490);
            Color whiteColor = Color.White;
            whiteColor.A = 1;

            string strTimeSec = String.Format("{0:00}", bestPlayerTimeInSeconds);
            string time = bestPlayerTimeInMinutes + ":" + strTimeSec;

            spriteBatch.Draw(m_BestTimeBackground, background, whiteColor);
            spriteBatch.DrawString(m_ArialFont, "Best Time", new Vector2(500, 500), GameConstants.HUD_COLOR);
            spriteBatch.DrawString(m_ArialFont, time, new Vector2(580, 555), GameConstants.HUD_COLOR);

            Vector2 player1pos = new Vector2(150, 610);
            Vector2 player2pos = new Vector2(400, 610);
            Vector2 player3pos = new Vector2(650, 610);
            Vector2 player4pos = new Vector2(900, 610);

            if (indexOfBestPlayersTime[0] == 1)
            {
                spriteBatch.DrawString(m_ArialFont, "Player 1", player1pos, players[0].GetAnimation().getColor());
            }
            if (indexOfBestPlayersTime[1] == 1)
            {
                spriteBatch.DrawString(m_ArialFont, "Player 2", player2pos, players[1].GetAnimation().getColor());
            }
            if (indexOfBestPlayersTime[2] == 1)
            {
                spriteBatch.DrawString(m_ArialFont, "Player 3", player3pos, players[2].GetAnimation().getColor());
            }
            if (indexOfBestPlayersTime[3] == 1)
            {
                spriteBatch.DrawString(m_ArialFont, "Player 4", player4pos, players[3].GetAnimation().getColor());
            }


            m_Game.GraphicsDevice.SetRenderTarget(null);
            m_Game.GraphicsDevice.Clear(Color.Black);
            m_Game.GraphicsDevice.Viewport = m_Viewport;
            m_RenderTargetTexture = (Texture2D)m_RenderTarget;
            spriteBatch.End();
        }

        public void Draw(SpriteBatch spriteBatch, Player[] players)
        {
            spriteBatch.Begin();

            for (int i = players.Length -1 ; i >= 0; i--)
            {
                if (players[i].State == Player.States.PLAYING)
                {
                    players[i].HUD.DrawWithNoCharSelection(spriteBatch, players[i].GetAnimation().getColor());

                }
                else if (players[i].State == Player.States.SPAWNING)
                {
                    players[i].HUD.DrawWithCharSelection(spriteBatch, players[i].GetAnimation().getColor());
                }
                else if (players[i].State == Player.States.PORTING)
                {
                    players[i].HUD.DrawWithNoCharSelection(spriteBatch, players[i].GetAnimation().getColor());
                }
                else if (players[i].State == Player.States.DEAD)
                {

                }
            }
            Rectangle background = new Rectangle(0,495,GameConstants.WindowWidth,GameConstants.WindowHeight-490);
            Color whiteColor = Color.White;
            whiteColor.A = 20;

            string strTimeSec = String.Format("{0:00}", bestPlayerTimeInSeconds);
            string time =bestPlayerTimeInMinutes + ":" + strTimeSec;

            spriteBatch.Draw(m_BestTimeBackground, background, whiteColor);
            spriteBatch.DrawString(m_ArialFont, "Best Time", new Vector2(500,500), GameConstants.HUD_COLOR);
            spriteBatch.DrawString(m_ArialFont, time, new Vector2(580, 555), GameConstants.HUD_COLOR);

            Vector2 player1pos = new Vector2(150,610);
            Vector2 player2pos = new Vector2(400,610);
            Vector2 player3pos = new Vector2(650,610);
            Vector2 player4pos = new Vector2(900,610);

            if (indexOfBestPlayersTime[0] == 1)
            {
                spriteBatch.DrawString(m_ArialFont, "Player 1", player1pos, players[0].GetAnimation().getColor());
            }
            if (indexOfBestPlayersTime[1] == 1)
            {
                spriteBatch.DrawString(m_ArialFont, "Player 2", player2pos, players[1].GetAnimation().getColor());
            }
            if (indexOfBestPlayersTime[2] == 1)
            {
                spriteBatch.DrawString(m_ArialFont, "Player 3", player3pos, players[2].GetAnimation().getColor());
            }
            if (indexOfBestPlayersTime[3] == 1)
            {
                spriteBatch.DrawString(m_ArialFont, "Player 4", player4pos, players[3].GetAnimation().getColor());
            }

            spriteBatch.End();
        }
    }
}

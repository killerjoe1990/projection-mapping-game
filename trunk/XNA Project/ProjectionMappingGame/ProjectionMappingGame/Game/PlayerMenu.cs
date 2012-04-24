//System includes
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//XNA includes
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ProjectionMappingGame.Game
{
    public class PlayerMenu
    {
        const int BUFFER = 5;
        const int BORDER = 10;

        SpriteFont m_Font;
        Texture2D m_CharColorTex;
        Texture2D m_Background;
        Texture2D m_BestTexture;

        Point m_WindowSize;

        int m_TotalPlayerScore;
        int m_PlayersDefeated;
        int m_PlayerNum;
        int m_TimeSec;
        int m_TimeMin;
        int m_LastTimeSec;
        int m_LastTimeMin;

        float m_PointCounter;
        float m_TimeCounter;

         public PlayerMenu(PlayerIndex playerNum)
         {
            m_PlayerNum = (int)playerNum;

            m_TotalPlayerScore = 0;
            m_PlayersDefeated = 0;

            m_PointCounter = 0;
            m_TimeSec = 0;
            m_TimeCounter = 0;
            m_TimeMin = 0;
            m_LastTimeMin = 0;
            m_LastTimeSec = 0;

            m_WindowSize = new Point(768, 1024);
         }
         private Rectangle ScreenRect(float x, float y, float w, float h)
         {
             return new Rectangle((int)(m_WindowSize.X * x), (int)(m_WindowSize.Y * y), (int)(m_WindowSize.X * w), (int)(m_WindowSize.Y * h));
         }
         private Vector2 TransformVec(Vector2 v, Vector2 dim)
         {
             return new Vector2(v.X * dim.X, v.Y * dim.Y);
         }
         public Point WindowSize
         {
             get { return m_WindowSize; }
             set { m_WindowSize = value; }
         }
         public int TimesSeconds
         {
             get { return m_TimeSec; }
         }
         public int TimesMinutes
         {
             get { return m_TimeMin; }
         }

         public void Reset()
         {
             m_TotalPlayerScore = 0;
             m_PlayersDefeated = 0;
             m_TimeSec = 0;
             m_PointCounter = 0;
             m_TimeCounter = 0;
             m_TimeMin = 0;
             
             
         }

         public void LoadContent(SpriteFont font, GraphicsDevice device, Texture2D background, Texture2D bestTexture)
         {
             m_BestTexture = bestTexture;
             m_Font = font;
             m_CharColorTex = new Texture2D(device, 1, 1);
             m_CharColorTex.SetData<Color>(new Color[]{Color.White});

             m_Background = background;
         }

         public void Update(float elapsedTime, int playersAlive)
         {
             m_PointCounter += elapsedTime;
             m_TimeCounter += elapsedTime;

             if (m_PointCounter >= GameConstants.POINT_INTERVAL)
             {
                 m_PointCounter -= GameConstants.POINT_INTERVAL;
                 m_TotalPlayerScore += (GameConstants.POINTS_PER_INTERVAL * (playersAlive));
             }

             m_TimeSec =(int) m_TimeCounter;

             if (m_TimeSec >= 60)
             {
                 m_TimeSec = 0;
                 m_TimeMin++;
                 m_TimeCounter = 0;
             }
         }

         Vector2 PLAYER_NAME_POSITION = new Vector2(0.3f, 0.1f);
         Vector2 COLOR_POSITION = new Vector2(0.1f, 0.1f);
         Vector2 SCORE_POSITION = new Vector2(0.05f, 0.33f);
         Vector2 TIME_POSITION = new Vector2(0.1f, 0.66f);

         public void Draw(SpriteBatch spriteBatch, Color playerColor, Player.States state, string name, int index, bool bestScore)
         {
             Rectangle background = ScreenRect(((m_PlayerNum % 2) != 0) ? 0.5f : 0.0f, (m_PlayerNum > 1) ? 0.375f : 0.0f, 0.5f, 0.375f);
             Vector2 offset = new Vector2(background.X, background.Y);
             Vector2 dim = new Vector2(background.Width, background.Height);

             Vector2 charDim = m_Font.MeasureString(name[index].ToString());
             Vector2 stringDim = m_Font.MeasureString(name.Substring(0, index));

             string score = "Score: " + m_TotalPlayerScore;
             string strLastTimeSec = String.Format("{0:00}", m_LastTimeSec);
             string time = "Last Time Alive: " + m_LastTimeMin + ":" + strLastTimeSec;

             switch(state)
             {
                 case Player.States.SPAWNING:
                     spriteBatch.Draw(m_Background, background, Color.White);
                     spriteBatch.DrawString(m_Font, name, offset + TransformVec(PLAYER_NAME_POSITION, dim), GameConstants.HUD_COLOR);
                     spriteBatch.Draw(m_CharColorTex, new Rectangle((int)offset.X + (int)(COLOR_POSITION.X * dim.X), (int)offset.Y + (int)(COLOR_POSITION.Y * dim.Y), GameConstants.HUD_ICON_DIM, GameConstants.HUD_ICON_DIM), playerColor);
                     break;
                 case Player.States.NAME_CHANGE:
                     spriteBatch.Draw(m_Background, background, Color.White);
                     spriteBatch.DrawString(m_Font, name, offset + TransformVec(PLAYER_NAME_POSITION, dim), GameConstants.HUD_COLOR);
                     spriteBatch.Draw(m_CharColorTex, new Rectangle((int)offset.X + (int)(COLOR_POSITION.X * dim.X), (int)offset.Y + (int)(COLOR_POSITION.Y * dim.Y), GameConstants.HUD_ICON_DIM, GameConstants.HUD_ICON_DIM), playerColor);
                     spriteBatch.Draw(m_CharColorTex, new Rectangle((int)(offset + TransformVec(PLAYER_NAME_POSITION, dim)).X + (int)stringDim.X, (int)(offset + TransformVec(PLAYER_NAME_POSITION, dim)).Y + (int)charDim.Y, (int)charDim.X, 10), GameConstants.HUD_COLOR);
                     break;
                 case Player.States.PLAYING:
                     spriteBatch.Draw(m_Background, background, Color.White);
                     spriteBatch.DrawString(m_Font, name, offset + TransformVec(PLAYER_NAME_POSITION, dim), GameConstants.HUD_COLOR);
                     spriteBatch.DrawString(m_Font, score, offset + TransformVec(SCORE_POSITION, dim), GameConstants.HUD_COLOR);
                     //spriteBatch.DrawString(m_Font, time, offset + TransformVec(TIME_POSITION, dim), GameConstants.HUD_COLOR);
                     spriteBatch.Draw(m_CharColorTex, new Rectangle((int)offset.X + (int)(COLOR_POSITION.X * dim.X), (int)offset.Y + (int)(COLOR_POSITION.Y * dim.Y), GameConstants.HUD_ICON_DIM, GameConstants.HUD_ICON_DIM), playerColor);

                     if (bestScore)
                     {
                         spriteBatch.Draw(m_BestTexture, background, playerColor);
                     }

                     break;
                 case Player.States.PORTING:
                     spriteBatch.Draw(m_Background, background, Color.White);
                     spriteBatch.DrawString(m_Font, name, offset + TransformVec(PLAYER_NAME_POSITION, dim), GameConstants.HUD_COLOR);
                     spriteBatch.DrawString(m_Font, score, offset + TransformVec(SCORE_POSITION, dim), GameConstants.HUD_COLOR);
                     //spriteBatch.DrawString(m_Font, time, offset + TransformVec(TIME_POSITION, dim), GameConstants.HUD_COLOR);
                     spriteBatch.Draw(m_CharColorTex, new Rectangle((int)offset.X + (int)(COLOR_POSITION.X * dim.X), (int)offset.Y + (int)(COLOR_POSITION.Y * dim.Y), GameConstants.HUD_ICON_DIM, GameConstants.HUD_ICON_DIM), playerColor);

                     if (bestScore)
                     {
                         spriteBatch.Draw(m_BestTexture, background, playerColor);
                     }
                     break;
                 case Player.States.DEAD:
                     Color deadTint = Color.DarkRed;
                     spriteBatch.Draw(m_Background, background, deadTint);
                     spriteBatch.DrawString(m_Font, name, offset + TransformVec(PLAYER_NAME_POSITION, dim), Color.White);
                     spriteBatch.DrawString(m_Font, score, offset + TransformVec(SCORE_POSITION, dim), Color.White);
                     //spriteBatch.DrawString(m_Font, time, offset + TransformVec(TIME_POSITION, dim), GameConstants.HUD_COLOR);
                     spriteBatch.Draw(m_CharColorTex, new Rectangle((int)offset.X + (int)(COLOR_POSITION.X * dim.X), (int)offset.Y + (int)(COLOR_POSITION.Y * dim.Y), GameConstants.HUD_ICON_DIM, GameConstants.HUD_ICON_DIM), playerColor);
                     break; 
            }
         }

         public void PlayerDefeated(int bonus)
         {
             m_PlayersDefeated++;
             m_TotalPlayerScore += bonus;
         }

         public int PlayerScore
         {
             get { return m_TotalPlayerScore; }
         }

    }
}

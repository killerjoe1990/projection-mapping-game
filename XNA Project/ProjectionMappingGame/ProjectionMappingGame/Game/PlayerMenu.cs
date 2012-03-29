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

            m_WindowSize = new Point(GameConstants.WindowHeight, GameConstants.WindowWidth);
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

         public void LoadContent(SpriteFont font, GraphicsDevice device, Texture2D background)
         {
             m_Font = font;
             m_CharColorTex = new Texture2D(device, 1, 1);
             m_CharColorTex.SetData<Color>(new Color[]{Color.White});

             m_Background = background;
         }

         public void Update(float elapsedTime)
         {
             m_PointCounter += elapsedTime;
             m_TimeCounter += elapsedTime;

             if (m_PointCounter >= GameConstants.POINT_INTERVAL)
             {
                 m_PointCounter -= GameConstants.POINT_INTERVAL;
                 m_TotalPlayerScore += GameConstants.POINTS_PER_INTERVAL;
             }

             m_TimeSec =(int) m_TimeCounter;

             if (m_TimeSec >= 60)
             {
                 m_TimeSec = 0;
                 m_TimeMin++;
                 m_TimeCounter = 0;
             }
         }

         public void DrawWithCharSelection(SpriteBatch spriteBatch, Color playerColor)
         {
             Vector2 dimensions = Vector2.Zero, scorePos = Vector2.Zero, iconPos = Vector2.Zero, dimensionsTime = Vector2.Zero,
                 timePos = Vector2.Zero;
             Rectangle background = Rectangle.Empty;

             string score = "Player " + (m_PlayerNum + 1) + " Score: " + m_TotalPlayerScore;
             string strLastTimeSec = String.Format("{0:00}", m_LastTimeSec);
             string time = "Last Time Alive: "+ m_LastTimeMin+":" + strLastTimeSec;
             dimensions = m_Font.MeasureString(score);
             dimensionsTime = m_Font.MeasureString(time);

             if (dimensionsTime.X >= dimensions.X)
             {
                 background.Width = (int)dimensionsTime.X + BORDER * 2 + BUFFER;
             }
             else
             {
                 background.Width = (int)dimensions.X + BORDER * 2 + BUFFER;
             }
             background.Height = BORDER * 2 + BUFFER + GameConstants.HUD_ICON_DIM + (int)dimensions.Y + (int)dimensionsTime.Y;

             switch (m_PlayerNum)
             {
                 case (int)PlayerIndex.One:
                     background.X = 0;
                     background.Y = 0;
                     break;
                 case (int)PlayerIndex.Two:
                     background.X = m_WindowSize.X - background.Width;
                     background.Y = 0;
                     break;
                 case (int)PlayerIndex.Three:
                     background.X = 0;
                     background.Y = background.Height;
                     break;
                 case (int)PlayerIndex.Four:
                     background.X = m_WindowSize.X - background.Width;
                     background.Y = background.Height;
                     break;
             }
             
             scorePos.X = background.X + BORDER;
             scorePos.Y = background.Y + BORDER;

             timePos.X = background.X + BORDER;
             timePos.Y = background.Y + scorePos.Y + dimensions.Y + BUFFER;

             iconPos.X = background.X + (dimensions.X + BORDER * 2) / 2.0f - GameConstants.HUD_ICON_DIM / 2.0f;
             iconPos.Y = dimensions.Y + timePos.Y + BUFFER;

             Color whiteColor = Color.White;
             whiteColor.A = 20;

             spriteBatch.Draw(m_Background, background, whiteColor);
             spriteBatch.DrawString(m_Font, score, scorePos, GameConstants.HUD_COLOR);
             spriteBatch.DrawString(m_Font, time, timePos, GameConstants.HUD_COLOR);
             spriteBatch.Draw(m_CharColorTex, new Rectangle((int)iconPos.X, (int)iconPos.Y, GameConstants.HUD_ICON_DIM, GameConstants.HUD_ICON_DIM), playerColor);
         }
         public void DrawWithNoCharSelection(SpriteBatch spriteBatch, Color playerColor)
         {
             Vector2 dimensions = Vector2.Zero, defeatDim = Vector2.Zero, scorePos = Vector2.Zero, iconPos = Vector2.Zero, defeatedPos = Vector2.Zero,
                 dimensionsTime = Vector2.Zero, timePos = Vector2.Zero;
             Rectangle background = Rectangle.Empty;


             string score = "Player " + (m_PlayerNum + 1) + " Score: " + m_TotalPlayerScore;
             string defeated = "Players Outlasted: " + m_PlayersDefeated;
             string strTimeSec =String.Format("{0:00}",m_TimeSec);
             string time = "Time Alive: " + m_TimeMin + ":" + strTimeSec; 
             
             dimensions = m_Font.MeasureString(score);
             defeatDim = m_Font.MeasureString(defeated);
             dimensionsTime = m_Font.MeasureString(time);

             if (dimensionsTime.X >= dimensions.X)
             {
                 background.Width = (int)dimensionsTime.X + BORDER * 2 + BUFFER +100;
             }
             else
             {
                 background.Width = (int)dimensions.X + BORDER * 2 + BUFFER +90;
             }
             
            

             background.Height = BORDER * 2 + BUFFER + (int)dimensions.Y * 3;

             switch (m_PlayerNum)
             {
                 case (int)PlayerIndex.One:
                     background.X = 0;
                     background.Y = 0;                 
                     break;
                 case (int)PlayerIndex.Two:
                     background.X = m_WindowSize.X - background.Width;
                     background.Y = 0;
                     break;
                 case (int)PlayerIndex.Three:
                     background.X = 0;
                     background.Y = background.Height;
                     break;
                 case (int)PlayerIndex.Four:
                     background.X = m_WindowSize.X - background.Width;
                     background.Y = background.Height;
                     break;
             }

             iconPos.X = background.X + BORDER;
             iconPos.Y = background.Y + BORDER;

             scorePos.X = iconPos.X + BUFFER + dimensions.Y;
             scorePos.Y = background.Y + BORDER;

             defeatedPos.X = background.X + BORDER;
             defeatedPos.Y = background.Y + scorePos.Y + dimensions.Y + BUFFER;

             timePos.X = background.X + BORDER;
             timePos.Y = background.Y + defeatedPos.Y + dimensions.Y + BUFFER;

             m_LastTimeMin = m_TimeMin;
             m_LastTimeSec = m_TimeSec;

             Color whiteColor = Color.White;
             whiteColor.A = 20;

             spriteBatch.Draw(m_Background, background, whiteColor);
             spriteBatch.DrawString(m_Font, score, scorePos, GameConstants.HUD_COLOR);
             spriteBatch.DrawString(m_Font, defeated, defeatedPos, GameConstants.HUD_COLOR);
             spriteBatch.DrawString(m_Font, time, timePos, GameConstants.HUD_COLOR);
             spriteBatch.Draw(m_CharColorTex, new Rectangle((int)iconPos.X, (int)iconPos.Y, GameConstants.HUD_ICON_DIM, GameConstants.HUD_ICON_DIM), playerColor);
         }


         public void PlayerDefeated()
         {
             m_PlayersDefeated++;
             m_TotalPlayerScore += GameConstants.POINTS_FOR_KILL;
         }

    }
}

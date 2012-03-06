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

        float m_PointCounter;

         public PlayerMenu(PlayerIndex playerNum)
         {
            m_PlayerNum = (int)playerNum;

            m_TotalPlayerScore = 0;
            m_PlayersDefeated = 0;

            m_PointCounter = 0;

            m_WindowSize = new Point(GameConstants.WindowHeight, GameConstants.WindowWidth);
         }

         public Point WindowSize
         {
             get { return m_WindowSize; }
             set { m_WindowSize = value; }
         }

         public void Reset()
         {
             m_TotalPlayerScore = 0;
             m_PlayersDefeated = 0;

             m_PointCounter = 0;
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

             if (m_PointCounter >= GameConstants.POINT_INTERVAL)
             {
                 m_PointCounter -= GameConstants.POINT_INTERVAL;
                 m_TotalPlayerScore += GameConstants.POINTS_PER_INTERVAL;
             }
         }

         public void DrawWithCharSelection(SpriteBatch spriteBatch, Color playerColor)
         {
             Vector2 dimensions = Vector2.Zero, scorePos = Vector2.Zero, iconPos = Vector2.Zero;
             Rectangle background = Rectangle.Empty;

             string score = "Player " + (m_PlayerNum + 1) + " Score: " + m_TotalPlayerScore;
             dimensions = m_Font.MeasureString(score);

             background.Width = (int)dimensions.X + BORDER * 2;
             background.Height = BORDER * 2 + BUFFER + GameConstants.HUD_ICON_DIM + (int)dimensions.Y;

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
                     background.Y = m_WindowSize.Y - background.Height;
                     break;
                 case (int)PlayerIndex.Four:
                     background.X = m_WindowSize.X - background.Width;
                     background.Y = m_WindowSize.Y - background.Height;
                     break;
             }
             
             scorePos.X = background.X + BORDER;
             scorePos.Y = background.Y + BORDER;

             iconPos.X = background.X + (dimensions.X + BORDER * 2) / 2.0f - GameConstants.HUD_ICON_DIM / 2.0f;
             iconPos.Y = scorePos.Y + dimensions.Y + BUFFER;

             spriteBatch.Draw(m_Background, background, Color.White);
             spriteBatch.DrawString(m_Font, score, scorePos, GameConstants.HUD_COLOR);
             spriteBatch.Draw(m_CharColorTex, new Rectangle((int)iconPos.X, (int)iconPos.Y, GameConstants.HUD_ICON_DIM, GameConstants.HUD_ICON_DIM), playerColor);
         }
         public void DrawWithNoCharSelection(SpriteBatch spriteBatch, Color playerColor)
         {
             Vector2 dimensions = Vector2.Zero, defeatDim = Vector2.Zero, scorePos = Vector2.Zero, iconPos = Vector2.Zero, defeatedPos = Vector2.Zero;
             Rectangle background = Rectangle.Empty;

             string score = "Player " + (m_PlayerNum + 1) + " Score: " + m_TotalPlayerScore;
             string defeated = "Players Outlasted: " + m_PlayersDefeated;

             dimensions = m_Font.MeasureString(score);
             defeatDim = m_Font.MeasureString(defeated);

             if (dimensions.X >= defeatDim.X)
             {
                 background.Width = (int)dimensions.X + BORDER * 2 + BUFFER + (int)dimensions.Y;
             }
             else
             {
                 background.Width = (int)defeatDim.X + BORDER * 2 + BUFFER + (int)dimensions.Y;
             }

             background.Height = BORDER * 2 + BUFFER + (int)dimensions.Y * 2;

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
                     background.Y = m_WindowSize.Y - background.Height;
                     break;
                 case (int)PlayerIndex.Four:
                     background.X = m_WindowSize.X - background.Width;
                     background.Y = m_WindowSize.Y - background.Height;
                     break;
             }

             iconPos.X = background.X + BORDER;
             iconPos.Y = background.Y + BORDER;

             scorePos.X = iconPos.X + BUFFER + dimensions.Y;
             scorePos.Y = background.Y + BORDER;

             defeatedPos.X = background.X + BORDER;
             defeatedPos.Y = background.Y + scorePos.Y + dimensions.Y + BUFFER;  

             spriteBatch.Draw(m_Background, background, Color.White);
             spriteBatch.DrawString(m_Font, score, scorePos, GameConstants.HUD_COLOR);
             spriteBatch.DrawString(m_Font, defeated, defeatedPos, GameConstants.HUD_COLOR);
             spriteBatch.Draw(m_CharColorTex, new Rectangle((int)iconPos.X, (int)iconPos.Y, (int)dimensions.Y, (int)dimensions.Y), playerColor);
         }


         public void PlayerDefeated()
         {
             m_PlayersDefeated++;
             m_TotalPlayerScore += GameConstants.POINTS_FOR_KILL;
         }

    }
}

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
    class PlayerMenu
    {
        const int BUFFER = 5;

        SpriteFont m_Font;
        Texture2D m_CharColorTex;
        Texture2D m_Background;

        Vector2 m_Position;

        int m_TotalPlayerScore;
        int m_PlayersDefeated;
        int m_PlayerNum;

        float m_PointCounter;

         public PlayerMenu(PlayerIndex playerNum, Vector2 position)
         {
            m_PlayerNum = (int)playerNum;
            m_Position = position;
            m_TotalPlayerScore = 0;
            m_PlayersDefeated = 0;

            m_PointCounter = 0;
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
             string score = "Player " + (m_PlayerNum + 1) + " Score: " + m_TotalPlayerScore;
             Vector2 dimensions = m_Font.MeasureString(score);
             Vector2 scorePos = m_Position;
             scorePos.X += BUFFER;
             scorePos.Y += BUFFER;
             Vector2 iconPos = m_Position;
             iconPos.X += dimensions.X / 2.0f - GameConstants.HUD_ICON_DIM / 2.0f;
             iconPos.Y += dimensions.Y + BUFFER * 2;

             Rectangle background = new Rectangle((int)m_Position.X, (int)m_Position.Y, GameConstants.HUD_WIDTH, GameConstants.HUD_HEIGHT);

             // spriteBatch.Begin();
             spriteBatch.Draw(m_Background, background, Color.White);
             spriteBatch.DrawString(m_Font, score, scorePos, GameConstants.HUD_COLOR);
             //spriteBatch.DrawString(m_ArialFont, "" + m_TotalPlayerScore, m_Position + new Vector2(132, 0), GameConstants.HUD_COLOR);
             spriteBatch.Draw(m_CharColorTex, new Rectangle((int)iconPos.X, (int)iconPos.Y, GameConstants.HUD_ICON_DIM, GameConstants.HUD_ICON_DIM), playerColor);
            // spriteBatch.End();
         }
         public void DrawWithNoCharSelection(SpriteBatch spriteBatch, Color playerColor)
         {
             string score = "Player " + (m_PlayerNum + 1) + " Score: " + m_TotalPlayerScore;
             string defeated = "Players Outlasted: " + m_PlayersDefeated;
             Vector2 dimensions = m_Font.MeasureString(score);
             
             Vector2 iconPos = m_Position;
             iconPos.X += BUFFER;
             iconPos.Y += BUFFER;
             Vector2 scorePos = iconPos;
             scorePos.X += dimensions.Y + BUFFER;
             Vector2 defeatedPos = m_Position;
             defeatedPos.X += BUFFER;
             defeatedPos.Y += 2 * BUFFER + dimensions.Y;

             Rectangle background = new Rectangle((int)m_Position.X, (int)m_Position.Y, GameConstants.HUD_WIDTH, GameConstants.HUD_HEIGHT);

             // spriteBatch.Begin();
             spriteBatch.Draw(m_Background, background, Color.White);
             spriteBatch.DrawString(m_Font, score, scorePos, GameConstants.HUD_COLOR);
             spriteBatch.DrawString(m_Font, defeated, defeatedPos, GameConstants.HUD_COLOR);
             //spriteBatch.DrawString(m_ArialFont, "" + m_TotalPlayerScore, m_Position + new Vector2(132, 0), GameConstants.HUD_COLOR);
             spriteBatch.Draw(m_CharColorTex, new Rectangle((int)iconPos.X, (int)iconPos.Y, (int)dimensions.Y, (int)dimensions.Y), playerColor);
             // spriteBatch.End();
         }


         public void PlayerDefeated()
         {
             m_PlayersDefeated++;
             m_TotalPlayerScore += GameConstants.POINTS_FOR_KILL;
         }

    }
}

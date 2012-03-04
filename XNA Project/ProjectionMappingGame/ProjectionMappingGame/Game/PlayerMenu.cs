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
        SpriteFont arialFont;
        Texture2D charColorTex;
        int totalPlayerScore;
        int playerNum;

         public PlayerMenu(PlayerIndex playerNum)
        {
            this.playerNum = (int)playerNum;
        }

         public void Reset()
         {

         }
         public void LoadContent(SpriteFont font, Texture2D playerColor)
         {
             arialFont = font;
             charColorTex = playerColor;
         }
         public void Update(float elapsedTime)
         {

         }
         public void HandleInput(float elapsedTime)
         {

         }
         public void DrawWithCharSelection(SpriteBatch spriteBatch, Vector2 originOfHud)
         {
            // spriteBatch.Begin();
             spriteBatch.DrawString(arialFont, "Player "+ (playerNum+1)+" Score: ", originOfHud, Color.White);
             spriteBatch.DrawString(arialFont, ""+totalPlayerScore,originOfHud+ new Vector2(132, 0), Color.White);
             spriteBatch.Draw(charColorTex, new Rectangle((int)originOfHud.X + 45,(int)originOfHud.Y+ 25, 32, 32), Color.White);
            // spriteBatch.End();
         }
         public void DrawWithNoCharSelection(SpriteBatch spriteBatch, Vector2 originOfHud)
         {
             //spriteBatch.Begin();
             spriteBatch.DrawString(arialFont, "Player " + (playerNum+1) + " Score: ", originOfHud, Color.White);
             spriteBatch.DrawString(arialFont, "" + totalPlayerScore, originOfHud + new Vector2(132, 0), Color.White);
            // spriteBatch.End();
         }
    
    

    }
}

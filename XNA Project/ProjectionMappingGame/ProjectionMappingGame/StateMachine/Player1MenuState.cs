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

namespace ProjectionMappingGame.StateMachine
{
    public class Player1MenuState:GameState
    {
        SpriteFont arialFont;
        Texture2D charColorTex;
        int totalP1Score;

         public Player1MenuState(GameDriver game)
         : base(game, StateType.GamePlay)
        {

        }

         public override void Reset()
         {

         }
         public override void LoadContent(ContentManager content)
         {
             arialFont = content.Load<SpriteFont>("Fonts/Arial");
             charColorTex = content.Load<Texture2D>("Sprites/Idle");
         }
         public override void Update(float elapsedTime)
         {

         }
         public override void HandleInput(float elapsedTime)
         {

         }
         public override void Draw(SpriteBatch spriteBatch)
         {
             spriteBatch.Begin();
             spriteBatch.DrawString(arialFont, "Player 1 Score: ", new Vector2(0, 0), Color.White);
             spriteBatch.DrawString(arialFont, ""+totalP1Score, new Vector2(132, 0), Color.White);
             spriteBatch.Draw(charColorTex, new Rectangle(45, 25, 32, 32), Color.White);
             spriteBatch.End();
         }
    

    }
}

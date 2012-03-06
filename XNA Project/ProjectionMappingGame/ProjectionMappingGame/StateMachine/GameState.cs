
#region File Description

/******************************************************************
 * Filename:        GameState.cs
 * Author:          Adam (A.J.) Fairfield
 * 
 * Created:         1/24/2012
 *****************************************************************/

#endregion

#region Imports

// System includes
using System;
using System.Collections.Generic;
using System.Text;

// XNA includes
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace ProjectionMappingGame.StateMachine
{
   public abstract class GameState
   {
      // Active game reference
      protected GameDriver m_Game;

      // State representation
      protected StateType m_StateType;

      // Updating/Rendering controller flags
      protected bool m_IsUpdatable;
      protected bool m_IsRenderable;

      /// <summary>
      /// Default constructor for type GameState defines a game state of
      /// a specified type and stores a reference to the main game driver.
      /// </summary>
      /// <param name="game">Main XNA reference</param>
      /// <param name="type">This state type</param>
      public GameState(GameDriver game, StateType type)
      {
         // Store fields
         m_Game = game;
         m_StateType = type;

         // Defaults
         m_IsUpdatable = true;
         m_IsRenderable = true;
      }

      // Pure virtual functions that must be implemented in each derived class
      public abstract void Reset();
      public abstract void LoadContent(ContentManager content);
      public abstract void Update(float elapsedTime);
      public abstract void HandleInput(float elapsedTime);
      public abstract void Draw(SpriteBatch spriteBatch);

      #region Public Access TV

      /// <summary>
      /// Accessor/Mutator for whether a game state should be updated every frame.
      /// </summary>
      public bool IsUpdatable
      {
         get { return m_IsUpdatable; }
         set { m_IsUpdatable = value; }
      }

      /// <summary>
      /// Accessor/Mutator for whether a game state should be drawn every frame.
      /// </summary>
      public bool IsRenderable
      {
         get { return m_IsRenderable; }
         set { m_IsRenderable = value; }
      }

      /// <summary>
      /// Accessor for this game state's type. Use for polymorphism.
      /// </summary>
      public StateType StateType
      {
         get { return m_StateType; }
      }

      #endregion

   }
}

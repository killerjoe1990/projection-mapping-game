
#region File Description

/******************************************************************
 * Filename:        FiniteStateMachine.cs
 * Author:          Adam (A.J.) Fairfield
 * 
 * Created:         1/24/2012
 *****************************************************************/

#endregion

#region Imports

// System includes
using System;
using System.Collections.Generic;

// XNA includes
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace ProjectionMappingGame.StateMachine
{
   // All possible states
   enum StateType
   {
      MainMenu,
      GamePlay,
      ProjectionEditor,
      Player1Menu,
      Count
   };

   class FiniteStateMachine
   {
      // State fields
      Stack<int> m_ActiveStates;    // Current states that are running either their Update and/or Draw functions
      GameState[] m_States;         // All possible states
      StateType[] m_StateTypes = {
         StateType.MainMenu,
         StateType.GamePlay,
         StateType.ProjectionEditor,
         StateType.Player1Menu
      };

      // Properties
      public static int NUM_STATES = (int)StateType.Count;
      bool m_GamePlayMode;

      /// <summary>
      /// Default constructor for type FiniteStateMachine.
      /// </summary>
      private FiniteStateMachine()
      {
         // Allocate an empty stack
         m_ActiveStates = new Stack<int>();

         // Allocate space for all game states
         m_States = new GameState[NUM_STATES];

         m_GamePlayMode = false;
      }

      #region Singleton Components

      // Create singleton instance of type FiniteStateMachine
      private static FiniteStateMachine _instance = new FiniteStateMachine();

      /// <summary>
      /// Accessor for singleton instance of type FiniteStateMachine
      /// </summary>
      /// <returns>static FiniteStateMachine _instance</returns>
      public static FiniteStateMachine GetInstance()
      {
         return _instance;
      }

      #endregion

      #region Initialization

      /// <summary>
      /// Configure the state machine before playing.
      /// </summary>
      /// <param name="game">Main XNA instance</param>
      /// <param name="startState">Default state</param>
      public void Configure(GameDriver game, StateType startState)
      {
         // Initialize all game states
         m_States[(int)StateType.MainMenu] = new MainMenuState(game);
         m_States[(int)StateType.GamePlay] = new GamePlayState(game);
         m_States[(int)StateType.ProjectionEditor] = new ProjectionEditorState(game);
         m_States[(int)StateType.Player1Menu] = new Player1MenuState(game);

         // Load the start state into the stack
         m_ActiveStates.Push((int)startState);
      }

      /// <summary>
      /// Load the content for each state.  This would be inefficient for a larger
      /// game, but will be OK for ours.
      /// </summary>
      /// <param name="content">Content loader</param>
      public void LoadContent(ContentManager content)
      {
         // Load the content of all states once per game at launch.
         for (int i = 0; i < m_States.Length; ++i)
         {
            m_States[i].LoadContent(content);
         }

         Player1MenuState player1Menu = (Player1MenuState)m_States[(int)StateType.Player1Menu];
         GamePlayState gameplay = (GamePlayState)m_States[(int)StateType.GamePlay];
         ProjectionEditorState projectionEditor = (ProjectionEditorState)m_States[(int)StateType.ProjectionEditor];
         projectionEditor.ProjectorInput = gameplay.RenderTarget;
      }

      #endregion

      #region State Changing

      public void StartGame()
      {
         GamePlayState gameplay = (GamePlayState)m_States[(int)StateType.GamePlay];
         gameplay.RenderTargetMode = true;
         gameplay.Reset();
         m_GamePlayMode = true;
         m_ActiveStates.Clear();
         m_ActiveStates.Push((int)StateType.GamePlay);
         m_ActiveStates.Push((int)StateType.ProjectionEditor);
      }

      public void QuitGame()
      {
         GamePlayState gameplay = (GamePlayState)m_States[(int)StateType.GamePlay];
         gameplay.RenderTargetMode = false;
         gameplay.Reset();
         m_GamePlayMode = false;
         m_ActiveStates.Clear();
         m_ActiveStates.Push((int)StateType.ProjectionEditor);
      }

      /// <summary>
      /// Add a state to the stack and set it as the current state.
      /// </summary>
      /// <param name="state">New state</param>
      public void ChangeState(StateType state)
      {
         DictateFlow(state);
         m_ActiveStates.Push((int)state);
         m_States[(int)state].Reset();
      }

      /// <summary>
      /// Clear the stack and push the state.
      /// </summary>
      /// <param name="state">New state</param>
      public void SetState(StateType state)
      {
         DictateFlow(state);
         m_ActiveStates.Clear();
         m_ActiveStates.Push((int)state);
         m_States[(int)state].Reset();
      }

      /// <summary>
      /// Pop the top state, if any, from the state.
      /// </summary>
      public void PopState()
      {
         if (m_ActiveStates.Count > 1)
            m_ActiveStates.Pop();

         DictateFlow(m_StateTypes[m_ActiveStates.Peek()]);
      }

      private void DictateFlow(StateType state)
      {
         MainMenuState mainMenu = (MainMenuState)m_States[(int)StateType.MainMenu];
         GamePlayState gameplay = (GamePlayState)m_States[(int)StateType.GamePlay];
         ProjectionEditorState projectionEditor = (ProjectionEditorState)m_States[(int)StateType.ProjectionEditor];
         Player1MenuState player1Menu = (Player1MenuState)m_States[(int)StateType.Player1Menu];

         // Handle any inter-state logic here
         switch (state)
         {
            case StateType.MainMenu:

               break;
            case StateType.GamePlay:

               break;
            case StateType.Player1Menu:

               break;
            case StateType.ProjectionEditor:
               projectionEditor.ProjectorInput = gameplay.RenderTarget;
               break;
         }
      }

      #endregion

      #region Updating

      /// <summary>
      /// Update the top game state each frame.
      /// </summary>
      /// <param name="elapsedTime">Elapsed time between frames.</param>
      public void Update(float elapsedTime)
      {
         if (m_GamePlayMode)
         {
            GamePlayState gameplay = (GamePlayState)m_States[(int)StateType.GamePlay];
            ProjectionEditorState projectionEditor = (ProjectionEditorState)m_States[(int)StateType.ProjectionEditor];
            projectionEditor.ProjectorInput = gameplay.RenderTarget;
         }

         // Render all active and updatable states
         int[] activeStates = m_ActiveStates.ToArray();
         for (int i = activeStates.Length - 1; i >= 0; --i)
         {
            // Only if a state is updatable
            if (m_States[activeStates[i]].IsUpdatable)
            {
               m_States[activeStates[i]].HandleInput(elapsedTime);
               m_States[activeStates[i]].Update(elapsedTime);
            }
         }
      }

      #endregion

      #region Rendering

      /// <summary>
      /// Render active game states each frame.
      /// </summary>
      /// <param name="spriteBatch">Spritebatch for 2D rendering</param>
      public void Draw(SpriteBatch spriteBatch)
      {
         // Render all active and renderable states
         int[] activeStates = m_ActiveStates.ToArray();
         for (int i = activeStates.Length - 1; i >= 0; --i)
         {
            // Only if a state is renderable
            if (m_States[activeStates[i]].IsRenderable)
            {
               m_States[activeStates[i]].Draw(spriteBatch);
            }
         }
      }

      #endregion

      #region Public Access TV

      /// <summary>
      /// Accessor for the current game state.
      /// </summary>
      public int CurrentState
      {
         get { return m_ActiveStates.Peek(); }
      }

      #endregion

   }
}

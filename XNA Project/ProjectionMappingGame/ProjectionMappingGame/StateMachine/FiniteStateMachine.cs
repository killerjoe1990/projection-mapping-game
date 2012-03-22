
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
using System.Windows.Forms;

// XNA includes
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace ProjectionMappingGame.StateMachine
{
   // All possible states
   public enum StateType
   {
      MainMenu,
      GamePlay,
      ProjectionEditor,
      Count
   };

   public class FiniteStateMachine
   {
      // State fields
      Stack<int> m_ActiveStates;    // Current states that are running either their Update and/or Draw functions
      GameState[] m_States;         // All possible states
      StateType[] m_StateTypes = {
         StateType.MainMenu,
         StateType.GamePlay,
         StateType.ProjectionEditor
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

         //GamePlayState gameplay = (GamePlayState)m_States[(int)StateType.GamePlay];
         Texture2D rt = content.Load<Texture2D>("Textures/Layer0_2");
         ProjectionEditorState projectionEditor = (ProjectionEditorState)m_States[(int)StateType.ProjectionEditor];
         projectionEditor.ProjectorInput = rt;
      }

      #endregion

      #region State Changing

      public void StartEditor()
      {
         ProjectionEditorState projectionEditor = (ProjectionEditorState)m_States[(int)StateType.ProjectionEditor];
         MainMenuState mainMenu = (MainMenuState)m_States[(int)StateType.MainMenu];
         
         int numProjectors = mainMenu.Windows.Count - 1;
         for (int i = 0; i < numProjectors; ++i)
         {
            Rectangle bounds = new Rectangle(System.Windows.Forms.Screen.AllScreens[i + 1].Bounds.X, System.Windows.Forms.Screen.AllScreens[i + 1].Bounds.Y, System.Windows.Forms.Screen.AllScreens[i + 1].Bounds.Width, System.Windows.Forms.Screen.AllScreens[i + 1].Bounds.Height);

            projectionEditor.AddProjector(bounds);
         }
      }

      public void QuitEditor()
      {
         ProjectionEditorState projectionEditor = (ProjectionEditorState)m_States[(int)StateType.ProjectionEditor];
         MainMenuState mainMenu = (MainMenuState)m_States[(int)StateType.MainMenu];
      }

      public void StartGame()
      {
         ProjectionEditorState projectionEditor = (ProjectionEditorState)m_States[(int)StateType.ProjectionEditor];
         GamePlayState gameplay = (GamePlayState)m_States[(int)StateType.GamePlay];
         gameplay.Reset();
         m_GamePlayMode = true;
         m_ActiveStates.Clear();
         m_ActiveStates.Push((int)StateType.GamePlay);
         m_ActiveStates.Push((int)StateType.ProjectionEditor);

         gameplay.Levels.Clear();
         for (int i = 0; i < projectionEditor.Layers.Count; ++i)
         {
            if (projectionEditor.Layers[i].Type == Editor.LayerType.Gameplay)
            {
                gameplay.AddLevel(projectionEditor.Layers[i].Width, projectionEditor.Layers[i].Height, projectionEditor.Layers[i].Normal);
            }
         }
         for (int i = 0; i < gameplay.Levels.Count; ++i)
         {
            gameplay.Levels[i].RenderTargetMode = true;
         }

         gameplay.SetMainLevel(0);
      }

      public void StartGameWithoutEditor()
      {
          GamePlayState gameplay = (GamePlayState)m_States[(int)StateType.GamePlay];
          gameplay.Reset();
          m_GamePlayMode = true;

          m_ActiveStates.Clear();
          m_ActiveStates.Push((int)StateType.GamePlay);

          gameplay.Levels.Clear();

          gameplay.AddLevel(GameConstants.WindowWidth, GameConstants.WindowHeight, Vector3.Backward);
          gameplay.Levels[0].RenderTargetMode = false;
          gameplay.SetMainLevel(0);
      }

      public void QuitGame()
      {
         GamePlayState gameplay = (GamePlayState)m_States[(int)StateType.GamePlay];
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
         

         // Handle any inter-state logic here
         switch (state)
         {
            case StateType.MainMenu:

               break;
            case StateType.GamePlay:

               break;
            case StateType.ProjectionEditor:
               //projectionEditor.ProjectorInput = gameplay.GetRenderTarget(0);
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
            Texture2D[] renderTargets = new Texture2D[gameplay.Levels.Count];
            for (int i = 0; i < gameplay.Levels.Count; ++i)
            {
               renderTargets[i] = gameplay.GetRenderTarget(i);
            }
            projectionEditor.GameplayRenderTargets = renderTargets;

            gameplay.SetLight(projectionEditor.Light.Direction);
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

      public void ResizeGame(int width, int height)
      {
         int dx = width - GameConstants.WindowWidth;
         int dy = height - GameConstants.WindowHeight;

         GameConstants.WindowWidth = width;
         GameConstants.WindowHeight = height;

         for (int i = 0; i < m_States.Length; ++i)
         {
            m_States[i].Resize(dx, dy);
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

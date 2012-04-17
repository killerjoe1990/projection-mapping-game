
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
   public class LinkLayerPair<T, U>
   {
      public LinkLayerPair()
      {
      }

      public LinkLayerPair(T first, U second)
      {
         this.First = first;
         this.Second = second;
      }

      public T First { get; set; }
      public U Second { get; set; }
   };

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

      private Screen[] GetOrderedScreens()
      {
         Screen[] screens = System.Windows.Forms.Screen.AllScreens;
         bool swapped = false;
         do
         {
            swapped = false;
            for (int i = 1; i < screens.Length; ++i)
            {
               if (screens[i - 1].Bounds.X > screens[i].Bounds.X)
               {
                  Screen temp = screens[i - 1];
                  screens[i - 1] = screens[i];
                  screens[i] = temp;
                  swapped = true;
               }
            }
         } while (swapped);
         return screens;
      }

      public void StartEditor()
      {
         ProjectionEditorState projectionEditor = (ProjectionEditorState)m_States[(int)StateType.ProjectionEditor];
         MainMenuState mainMenu = (MainMenuState)m_States[(int)StateType.MainMenu];
         Screen[] screens = GetOrderedScreens();

         int numWindowSizers = mainMenu.Windows.Count;
         if (numWindowSizers == 2 && (mainMenu.Windows[1].DividesX > 1 || mainMenu.Windows[1].DividesY > 1))
         {
            int numProjectors = mainMenu.Windows[1].DividesX * mainMenu.Windows[1].DividesY;
            int projWidth = mainMenu.Windows[1].Width / mainMenu.Windows[1].DividesX;
            int projHeight = mainMenu.Windows[1].Height / mainMenu.Windows[1].DividesY;

            for (int x = 0; x < mainMenu.Windows[1].DividesX; ++x)
            {
               for (int y = 0; y < mainMenu.Windows[1].DividesY; ++y)
               {
                  Rectangle bounds = new Rectangle(screens[1].Bounds.X + x * projWidth, screens[1].Bounds.Y + y * projHeight, projWidth, projHeight);
                  projectionEditor.AddProjector(bounds);
               }
            }
         }
         else
         {
            int numProjectors = mainMenu.Windows.Count - 1;
            for (int i = 0; i < numProjectors; ++i)
            {
               Rectangle bounds = new Rectangle(System.Windows.Forms.Screen.AllScreens[i + 1].Bounds.X, System.Windows.Forms.Screen.AllScreens[i + 1].Bounds.Y, System.Windows.Forms.Screen.AllScreens[i + 1].Bounds.Width, System.Windows.Forms.Screen.AllScreens[i + 1].Bounds.Height);

               projectionEditor.AddProjector(bounds);
            }
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

         // Determine level pairs
         List<LinkLayerPair<int, int>> uniqueLayers = new List<LinkLayerPair<int, int>>();
         
         for (int i = 0; i < projectionEditor.Layers.Count; ++i)
         {
            List<int> set = projectionEditor.Layers[i].LinkedLayers;

            for (int j = 0; j < set.Count; ++j)
            {
               int i0 = Math.Min(i, set[j]);
               int i1 = Math.Max(i, set[j]);
               LinkLayerPair<int, int> link = new LinkLayerPair<int, int>(i0, i1);
               if (!AlreadyInList(uniqueLayers, link))
                  uniqueLayers.Add(link);
            }
         }

         for (int i = 0; i < uniqueLayers.Count; ++i)
         {
            gameplay.CreatePortalLink(uniqueLayers[i].First, uniqueLayers[i].Second);
         }

         for (int i = 0; i < gameplay.Levels.Count; ++i)
         {
            gameplay.Levels[i].RenderTargetMode = true;
         }

         gameplay.SetMainLevel(0);
      }

      private bool AlreadyInList(List<LinkLayerPair<int, int>> pairs, LinkLayerPair<int, int> link)
      {
         for (int i = 0; i < pairs.Count; ++i)
         {
            if (pairs[i].First == link.First && pairs[i].Second == link.Second)
               return true;
         }
         return false;
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
            projectionEditor.ScoreboardInput = gameplay.Scoreboard.RenderTargetTexture;
             
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

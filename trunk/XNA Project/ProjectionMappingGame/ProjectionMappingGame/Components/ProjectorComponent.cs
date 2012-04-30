#region File Description

//-----------------------------------------------------------------------------
// ProjectorComponent.cs
//
// Author:          A.J. Fairfield (Adam, ajfairfi)
// Date Created:    1/30/2012
//-----------------------------------------------------------------------------

#endregion

#region Imports

// System imports
using System;
using System.Collections.Generic;
using System.Text;

// XNA imports
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

// Local imports
using ProjectionMappingGame.Editor;

#endregion

namespace ProjectionMappingGame.Components
{
   class ProjectorComponent
   {
      // View components
      Matrix m_ViewMatrix;
      Viewport m_Viewport;
      float m_Fov;
      float m_AspectRatio;
      float m_NearPlane;
      float m_FarPlane;
      Matrix m_ProjectionMatrix;

      // Local space
      Vector3 m_Position;
      Vector3 m_LookAt;
      Vector3 m_Up;
      Vector3 m_LocalX, m_LocalY, m_LocalZ;
      Vector3 m_Direction;
      float m_RotX, m_RotY, m_RotZ;

      // Graph and Grid
      UVGrid m_Grid;
      UVDualEdgeGraph m_EdgeGraph;

      // Projection components
      Texture2D m_Texture;
      bool m_IsOn;
      ModelEntity m_Entity;
      float m_Alpha;

      // Constant fields
      const float KEYBOARD_TRANSLATE_SCALAR = 5.0f;
      const float MOUSE_MOVEMENT_SCALAR = 0.5f;

      /// <summary>
      /// 
      /// </summary>
      /// <param name="displayBounds"></param>
      /// <param name="pos"></param>
      /// <param name="lookAt"></param>
      /// <param name="fov"></param>
      /// <param name="ar"></param>
      /// <param name="near"></param>
      /// <param name="far"></param>
      /// <param name="content"></param>
      public ProjectorComponent(Rectangle displayBounds, Vector3 pos, Vector3 lookAt, float fov, float ar, float near, float far, ContentManager content)
      {
         // Store settings
         m_AspectRatio = ar;
         m_Fov = fov;
         m_NearPlane = near;
         m_FarPlane = far;
         m_LookAt = lookAt;
         m_Position = pos;

         // Defaults
         m_Up = Vector3.Up;
         m_IsOn = true;
         m_RotX = 0.0f;
         m_RotY = 0.0f;
         m_RotZ = 0.0f;
         m_EdgeGraph = new UVDualEdgeGraph(content.Load<Texture2D>("Textures/Layer0_2"));
         m_Grid = new UVGrid(100, 100);
         m_Alpha = 1.0f;
         m_Viewport = new Viewport(displayBounds);

         // Create initial projection/view matrix
         UpdateProjection();
         UpdateView();
      }

      #region Orientation

      /// <summary>
      /// Reflect updates in position or rotation by recomputing
      /// the projector's view matrix.
      /// </summary>
      public void UpdateView()
      {
         // Calculate view matrix
         Matrix rotationRight = Matrix.CreateRotationY(m_RotY);
         Matrix rotationUp = Matrix.CreateRotationX(-m_RotX);
         m_ViewMatrix = Matrix.CreateLookAt(m_Position, m_LookAt, m_Up);
         m_ViewMatrix *= rotationRight;
         m_ViewMatrix *= rotationUp;

         // Update directions
         m_LocalX = new Vector3(m_ViewMatrix.M11, m_ViewMatrix.M21, m_ViewMatrix.M31);
         m_LocalY = new Vector3(m_ViewMatrix.M12, m_ViewMatrix.M22, m_ViewMatrix.M32);
         m_LocalZ = new Vector3(m_ViewMatrix.M13, m_ViewMatrix.M23, m_ViewMatrix.M33);
         m_Direction = -m_LocalZ;
         m_Direction.Normalize();
      }

      /// <summary>
      /// Reflect updates in fov or aspect ratio by recomputing
      /// the projector's projection matrix.
      /// </summary>
      public void UpdateProjection()
      {
         m_ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(m_Fov, m_AspectRatio, 0.1f, 1000f);
      }

      /// <summary>
      /// Look up/down by adding rotation in local X axis.
      /// Rotation is indicated by a pos/neg radian value.
      /// </summary>
      /// <param name="radians">Radian amount to rotate by</param>
      public void LookUp(float radians)
      {
         // Add to x rotation
         m_RotX += radians;

         // Reset rotation over 2pi
         if (m_RotX > MathHelper.TwoPi)
            m_RotX -= MathHelper.TwoPi;

         // Recompute view matrix
         UpdateView();
      }

      /// <summary>
      /// Look right/left by adding rotation in local Y axis.
      /// Rotation is indicated by a pos/neg radian value.
      /// </summary>
      /// <param name="radians">Radian amount to rotate by</param>
      public void LookRight(float radians)
      {
         // Add to y rotation
         m_RotY += radians;

         // Reset rotation over 2pi
         if (m_RotY > MathHelper.TwoPi)
            m_RotY -= MathHelper.TwoPi;

         // Recompute view matrix
         UpdateView();
      }

      /// <summary>
      /// Translate/move by a delta value in each local axis.
      /// </summary>
      /// <param name="dx">Delta x</param>
      /// <param name="dy">Delta y</param>
      /// <param name="dz">Delta z</param>
      public void Translate(float dx, float dy, float dz)
      {
         // Compute forwards direction
         Vector3 forwards = Vector3.Normalize(Vector3.Cross(Up, m_LocalX));

         // Calculate translation
         Vector3 translation = Vector3.Zero;
         translation += m_LocalX * dx;
         translation += Up * dy;
         translation += forwards * dz;

         // Translate all positional components
         m_Position += translation;
         m_LookAt += translation;

         // Recompute view matrix
         UpdateView();
      }

      #endregion

      #region Input Handling

      /// <summary>
      /// Handle projector rotation based off mouse input.
      /// </summary>
      /// <param name="mState">Current frame's mouse state</param>
      /// <param name="mStatePrev">Preview frame's mouse state</param>
      /// <param name="elapsedTime">Elapsed time since last frame</param>
      public void HandleRotation(MouseState mState, MouseState mStatePrev, float elapsedTime)
      {
         float dx, dy;

         // Mouse Input
         if (mState.LeftButton == ButtonState.Pressed)
         {
            // As long as this is the first click since a release
            if (mStatePrev != mState)
            {
               // Store change in x and y
               dx = elapsedTime * (mState.X - mStatePrev.X) * MOUSE_MOVEMENT_SCALAR;
               dy = elapsedTime * (mState.Y - mStatePrev.Y) * MOUSE_MOVEMENT_SCALAR;

               if (dx != 0)
                  LookRight(dx);  // Orbit right 'dx' radians
               if (dy != 0)
                  LookUp(-dy);     // Orbit up 'dy' radians
            }
         }
      }

      /// <summary>
      /// W, A, S, D key based movement controls for projector.  Standard
      /// key distribution is used: 
      /// 
      /// W = Forward
      /// A = Strafe Left
      /// S = Backward
      /// D = Strafe Right
      /// Q = Up
      /// E = Down
      /// </summary>
      /// <param name="kState">Current frame's keyboard state</param>
      /// <param name="elapsedTime">Elapsed time since last frame</param>
      public void HandleTranslation(KeyboardState kState, float elapsedTime)
      {
         // Compute direction based on keys down
         Vector3 direction = Vector3.Zero;
         if (kState.IsKeyDown(Keys.W)) direction.Z += 1.0f;
         if (kState.IsKeyDown(Keys.S)) direction.Z -= 1.0f;
         if (kState.IsKeyDown(Keys.D)) direction.X += 1.0f;
         if (kState.IsKeyDown(Keys.A)) direction.X -= 1.0f;
         if (kState.IsKeyDown(Keys.Q)) direction.Y -= 1.0f;
         if (kState.IsKeyDown(Keys.E)) direction.Y += 1.0f;

         // Apply movement
         Vector3 translation = direction * KEYBOARD_TRANSLATE_SCALAR * elapsedTime;
         Translate(translation.X, translation.Y, translation.Z);
      }

      #endregion

      #region Public Access TV

      /// <summary>
      /// Accessor/Mutator for the projector's output viewport dimensions.
      /// </summary>
      public Viewport Viewport
      {
         get { return m_Viewport; }
         set { m_Viewport = value; }
      }

      /// <summary>
      /// Accessor/Mutator for the projector's output texture alpha.
      /// </summary>
      public float Alpha
      {
         get { return m_Alpha; }
         set { m_Alpha = value; }
      }

      /// <summary>
      /// Accessor/Mutator for the projector's representative model entity in the
      /// editor.
      /// </summary>
      public ModelEntity Entity
      {
         get { return m_Entity; }
         set { m_Entity = value; }
      }

      /// <summary>
      /// Accessor/Mutator for the projector's on/off state.
      /// </summary>
      public bool IsOn
      {
         get { return m_IsOn; }
         set { m_IsOn = value; }
      }

      /// <summary>
      /// Accessor/Mutator for the projector's current output texture.
      /// </summary>
      public Texture2D Texture
      {
         get { return m_Texture; }
         set { m_Texture = value; }
      }

      /// <summary>
      /// Accessor/Mutator for the projector's output UV edge graph.
      /// </summary>
      public UVDualEdgeGraph EdgeGraph
      {
         get { return m_EdgeGraph; }
         set { m_EdgeGraph = value; }
      }

      /// <summary>
      /// Accessor/Mutator for the projector's output UV grid.
      /// </summary>
      public UVGrid Grid
      {
         get { return m_Grid; }
         set { m_Grid = value; }
      }

      /// <summary>
      /// Accessor/Mutator for the projector's local space rotation around the
      /// x-axis. Setting this value forces a recomputation of the view matrix.
      /// </summary>
      public float RotX
      {
         get { return m_RotX; }
         set { m_RotX = value; UpdateView(); }
      }

      /// <summary>
      /// Accessor/Mutator for the projector's local space rotation around the
      /// y-axis. Setting this value forces a recomputation of the view matrix.
      /// </summary>
      public float RotY
      {
         get { return m_RotY; }
         set { m_RotY = value; UpdateView(); }
      }

      /// <summary>
      /// Accessor/Mutator for the projector's local space rotation around the
      /// z-axis. Setting this value forces a recomputation of the view matrix.
      /// </summary>
      public float RotZ
      {
         get { return m_RotZ; }
         set { m_RotZ = value; UpdateView(); }
      }

      /// <summary>
      /// Accessor/Mutator for the projector's projection matrix fov. Setting
      /// this value forces a recomputation of the projection matrix.
      /// </summary>
      public float Fov
      {
         get { return m_Fov; }
         set { m_Fov = value; UpdateProjection(); }
      }

      /// <summary>
      /// Accessor/Mutator for the projector's projection matrix near plane. Setting
      /// this value forces a recomputation of the projection matrix.
      /// </summary>
      public float NearPlane
      {
         get { return m_NearPlane; }
         set { m_NearPlane = value; UpdateProjection(); }
      }

      /// <summary>
      /// Accessor/Mutator for the projector's projection matrix far plane. Setting
      /// this value forces a recomputation of the projection matrix.
      /// </summary>
      public float FarPlane
      {
         get { return m_FarPlane; }
         set { m_FarPlane = value; UpdateProjection(); }
      }

      /// <summary>
      /// Accessor/Mutator for the projector's projection matrix aspect ratio. Setting
      /// this value forces a recomputation of the projection matrix.
      /// </summary>
      public float AspectRatio
      {
         get { return m_AspectRatio; }
         set { m_AspectRatio = value; UpdateProjection(); }
      }

      /// <summary>
      /// Accessor/Mutator for the projector's local up orientation.
      /// </summary>
      public Vector3 Up
      {
         get { return m_Up; }
         set { m_Up = value; }
      }

      /// <summary>
      /// Accessor/Mutator for the projector's position.  Setting this value
      /// forces a recomputation of the view matrix.
      /// </summary>
      public Vector3 Position
      {
         get { return m_Position; }
         set
         {
            Vector3 translation = value - m_Position;
            m_Position += translation;
            m_LookAt += translation;
            UpdateView();
         }
      }

      /// <summary>
      /// Accessor for the projector's view matrix.
      /// </summary>
      public Matrix ViewMatrix
      {
         get { return m_ViewMatrix; }
      }

      /// <summary>
      /// Accessor for the projector's projection matrix.
      /// </summary>
      public Matrix ProjectionMatrix
      {
         get { return m_ProjectionMatrix; }
      }

      /// <summary>
      /// Accessor for the projector's local right direction.
      /// </summary>
      public Vector3 Right
      {
         get { return m_LocalX; }
      }

      /// <summary>
      /// Accessor for the projector's local forward direction.
      /// </summary>
      public Vector3 Direction
      {
         get { return m_Direction; }
      }

      /// <summary>
      /// Accessor for the projector's current focus/lookat point.
      /// </summary>
      public Vector3 LookAt
      {
         get { return m_LookAt; }
      }

      #endregion
   }
}

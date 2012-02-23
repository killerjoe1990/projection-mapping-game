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

#endregion

namespace ProjectionMappingGame.Components
{
   class ProjectorComponent
   {
      // View components
      Vector3 m_Position;
      Vector3 m_LookAt;
      Vector3 m_Up;
      Vector3 m_LocalX, m_LocalY, m_LocalZ;
      Vector3 m_Direction;
      Matrix m_ViewMatrix;
      float m_RotX, m_RotY, m_RotZ;

      // Projection components
      float m_Fov;
      float m_AspectRatio;
      float m_NearPlane;
      float m_FarPlane;
      Matrix m_ProjectionMatrix;
      Texture2D m_Texture;
      bool m_IsOn;

      // Constant fields
      const float KEYBOARD_TRANSLATE_SCALAR = 5.0f;
      const float MOUSE_MOVEMENT_SCALAR = 0.5f;

      public ProjectorComponent(Vector3 pos, Vector3 lookAt, float fov, float ar, float near, float far)
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
         
         // Create initial projection/view matrix
         UpdateProjection();
         UpdateView();
      }

      #region Orientation

      /// <summary>
      /// Reflect updates position or rotation by recomputing
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
      }

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
      /// <param name="kState">Current frame's keyboard state</param>
      /// <param name="mStatePrev">Preview frame's keyboard state</param>
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

      public Vector3 Up
      {
         get { return m_Up; }
      }

      public Vector3 Right
      {
         get { return m_LocalX; }
      }

      public Vector3 Direction
      {
         get { return m_Direction; }
      }

      public bool IsOn
      {
         get { return m_IsOn; }
         set { m_IsOn = value; }
      }

      public Texture2D Texture
      {
         get { return m_Texture; }
         set { m_Texture = value; }
      }

      /// <summary>
      /// </summary>
      public float RotX
      {
         get { return m_RotX; }
         set { m_RotX = value; UpdateView(); }
      }

      /// <summary>
      /// </summary>
      public float RotY
      {
         get { return m_RotY; }
         set { m_RotY = value; UpdateView(); }
      }

      /// <summary>
      /// </summary>
      public float RotZ
      {
         get { return m_RotZ; }
         set { m_RotZ = value; UpdateView(); }
      }

      /// <summary>
      /// </summary>
      public float Fov
      {
         get { return m_Fov; }
         set { m_Fov = value; UpdateProjection(); }
      }

      /// <summary>
      /// </summary>
      public float NearPlane
      {
         get { return m_NearPlane; }
         set { m_NearPlane = value; UpdateProjection(); }
      }

      /// <summary>
      /// </summary>
      public float FarPlane
      {
         get { return m_FarPlane; }
         set { m_FarPlane = value; UpdateProjection(); }
      }

      /// <summary>
      /// </summary>
      public float AspectRatio
      {
         get { return m_AspectRatio; }
         set { m_AspectRatio = value; UpdateProjection(); }
      }

      /// <summary>
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

      

      #endregion
   }
}

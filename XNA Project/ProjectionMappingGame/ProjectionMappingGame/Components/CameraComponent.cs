#region File Description

//-----------------------------------------------------------------------------
// CameraComponent.cs
//
// Camera implements a multi-mode camera for use in a 3d environment.  This
// camera can either be configured for 3rd person use or 1st person use.  
// Calculated camera properties are stored such as direction, position, view
// matrix, etc...and an interface for manipulating the camera is also provided
// via way of 'looking' around.
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
   class CameraComponent
   {
      // View components
      Vector3 m_Target;
      Matrix m_ViewMatrix;
      Quaternion m_Orientation;
      float m_Pitch, m_Yaw;

      // Projection components
      float m_Fov;
      float m_AspectRatio;
      float m_NearPlane;
      float m_FarPlane;
      Matrix m_ProjectionMatrix;

      // Distance from look at point
      float m_Distance;

      // Constant fields
      const float KEYBOARD_TRANSLATE_SCALAR = 15.0f;
      const float MOUSE_MOVEMENT_SCALAR = 0.5f;
      const float ZOOM_SCALAR = 100.0f;
      
      /// <summary>
      /// Main constructor for type camera creates an orbital
      /// camera with a starting position and lookat point.
      /// </summary>
      /// <param name="pos">Starting position of camera.</param>
      /// <param name="lookAt">Initial look at point of camera.</param>
      public CameraComponent(Vector3 pos, Vector3 lookAt, float fov, float ar, float near, float far)
      {
         // Store settings
         m_AspectRatio = ar;
         m_Fov = fov;
         m_NearPlane = near;
         m_FarPlane = far;
         m_Target = lookAt;

         // Calculate camera direction and distance
         m_Distance = Vector3.Distance(lookAt, pos);
         
         // Default orientation
         m_Yaw = 0.0f;
         m_Pitch = 0.0f;

         // Create initial projection/view matrix
         UpdateProjection();
         UpdateView();
      }

      #region Orientation

      /// <summary>
      /// Reflect updates position or rotation by recomputing
      /// the camera's view matrix.
      /// </summary>
      public void UpdateView()
      {
         // Calculate view matrix
         m_Orientation = Quaternion.CreateFromAxisAngle(Vector3.Up, -m_Yaw) * Quaternion.CreateFromAxisAngle(Vector3.Right, m_Pitch);
         m_ViewMatrix = Matrix.CreateLookAt(Position, Target, Up);
      }

      public void UpdateProjection()
      {
         m_ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(m_Fov, m_AspectRatio, 0.1f, 1000f);
      }

      public void Zoom(float distance)
      {
         m_Distance += distance;
         UpdateView();
      }

      /// <summary>
      /// Orbit camera right/left around a center lookat point.
      /// </summary>
      /// <param name="angle">Randians to turn right; can be negative</param>
      public void OrbitRight(float angle)
      {
         // Update yaw
         m_Yaw -= angle;

         // Restrain yaw
         m_Yaw = m_Yaw % MathHelper.TwoPi;

         // Update view matrix to apply new yaw
         UpdateView();
      }

      /// <summary>
      /// Orbit camera up/down around a center lookat point.
      /// </summary>
      /// <param name="angle">Randians to turn right; can be negative</param>
      public void OrbitUp(float angle)
      {
         // Update pitch
         m_Pitch -= angle;

         // Restrain pitch
         m_Pitch = MathHelper.Clamp(m_Pitch, -(MathHelper.PiOver2) + .0001f, (MathHelper.PiOver2) - .0001f);

         // Update view matrix to apply new pitch
         UpdateView();
      }

      #endregion

      #region Input Handling

      /// <summary>
      /// Handle camera rotation based off mouse input.
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
                  OrbitRight(dx);  // Orbit right 'dx' radians
               if (dy != 0)
                  OrbitUp(-dy);     // Orbit up 'dy' radians
            }
         }
      }

      /// <summary>
      /// W, A, S, D key based movement controls for camera.  Standard
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
         Vector3 moveVector = Vector3.Zero;

         // Forward
         if (kState.IsKeyDown(Keys.W))
               moveVector = Direction;
         // Backward
         else if (kState.IsKeyDown(Keys.S))
               moveVector = -Direction;
         // Straff Left
         else if (kState.IsKeyDown(Keys.A))
               moveVector = -Right;
         // Straff Right
         else if (kState.IsKeyDown(Keys.D))
               moveVector = Right;
         moveVector.Y = 0.0f;

         // Move Up
         if (kState.IsKeyDown(Keys.Q))
               moveVector = Vector3.Up;
         // Move Down
         else if (kState.IsKeyDown(Keys.E))
               moveVector = -Vector3.Up;

         // Apply movement
         m_Target += (moveVector * KEYBOARD_TRANSLATE_SCALAR * elapsedTime);

         // Re-Compute view matrix
         UpdateView();
      }

      /// <summary>
      /// Handle camera zoom based off keyboard or mouse scroll.
      /// </summary>
      /// <param name="mState">Current frame's mouse state</param>
      /// <param name="mStatePrev">Previous frame's mouse state</param>
      /// <param name="kState">Current frame's keyboard state</param>
      /// <param name="elapsedTime">Elapsed time since last frame.</param>
      public void HandleZoom(MouseState mState, MouseState mStatePrev, KeyboardState kState, float elapsedTime)
      {
         // Via mouse
         if ((mStatePrev.ScrollWheelValue - mState.ScrollWheelValue) != 0)
         {
               float scrollWheelChange = (float)((mStatePrev.ScrollWheelValue - mState.ScrollWheelValue) / 100.0f);
               m_Distance += scrollWheelChange * ZOOM_SCALAR * elapsedTime;
               if (m_Distance < .001f)
                  m_Distance = .001f;
         }
         // Via keyboard
         //else if (kState.IsKeyDown(Keys.PageDown))
         //      m_Distance += ZOOM_SCALAR * elapsedTime;
         //else if (kState.IsKeyDown(Keys.PageUp))
         //      m_Distance += ZOOM_SCALAR * elapsedTime;

         // Re-Compute view matrix
         UpdateView();
      }

      #endregion

      #region Calculated Camera Local Space Axis

      /// <summary>
      /// Get the forward direction vector of the camera.
      /// </summary>
      public Vector3 Direction
      {
         get
         {
               //R v R' where v = (0,0,-1,0)
               Vector3 dir = Vector3.Zero;
               dir.X = -2.0f * ((m_Orientation.X * m_Orientation.Z) + (m_Orientation.W * m_Orientation.Y));
               dir.Y = 2.0f * ((m_Orientation.W * m_Orientation.X) - (m_Orientation.Y * m_Orientation.Z));
               dir.Z = ((m_Orientation.X * m_Orientation.X) + (m_Orientation.Y * m_Orientation.Y)) -
                     ((m_Orientation.Z * m_Orientation.Z) + (m_Orientation.W * m_Orientation.W));
               return Vector3.Normalize(dir);
         }
      }

      /// <summary>
      /// Get the right direction vector of the camera.
      /// </summary>
      public Vector3 Right
      {
         get
         {
               //R v R' where v = (1,0,0,0)
               Vector3 right = Vector3.Zero;
               right.X = ((m_Orientation.X * m_Orientation.X) + (m_Orientation.W * m_Orientation.W)) -
                        ((m_Orientation.Z * m_Orientation.Z) + (m_Orientation.Y * m_Orientation.Y));
               right.Y = 2.0f * ((m_Orientation.X * m_Orientation.Y) + (m_Orientation.Z * m_Orientation.W));
               right.Z = 2.0f * ((m_Orientation.X * m_Orientation.Z) - (m_Orientation.Y * m_Orientation.W));
               return Vector3.Normalize(right);
         }
      }

      /// <summary>
      /// Get the up direction vector of the camera.
      /// </summary>
      public Vector3 Up
      {
         get
         {
               //R v R' where v = (0,1,0,0)
               Vector3 up = Vector3.Zero;
               up.X = 2.0f * ((m_Orientation.X * m_Orientation.Y) - (m_Orientation.Z * m_Orientation.W));
               up.Y = ((m_Orientation.Y * m_Orientation.Y) + (m_Orientation.W * m_Orientation.W)) -
                     ((m_Orientation.Z * m_Orientation.Z) + (m_Orientation.X * m_Orientation.X));
               up.Z = 2.0f * ((m_Orientation.Y * m_Orientation.Z) + (m_Orientation.X * m_Orientation.W));
               return Vector3.Normalize(up);
         }
      }

      #endregion

      #region Public Access TV

      /// <summary>
      /// Accessor/Mutator camera's look at point.
      /// </summary>
      public Vector3 Target
      {
         get { return m_Target; }
         set { m_Target = value; }
      }

      /// <summary>
      /// Accessor/Mutator for current distance from look at point.
      /// </summary>
      public float Distance
      {
         get { return m_Distance; }
         set { m_Distance = value; }
      }

      /// <summary>
      /// Accessor/Mutator for current projection matrix aspect ratio component.
      /// </summary>
      public float AspectRatio
      {
         get { return m_AspectRatio; }
         set { m_AspectRatio = value; }
      }

      /// <summary>
      /// Accessor for the camera's calculated position.  This is calculated
      /// by adding the camera's inverse direction, scaled by it's zoom distance,
      /// to it's target point.
      /// </summary>
      public Vector3 Position
      {
         get { return Target - (Direction * Distance); }
      }

      /// <summary>
      /// Accessor for the camera's view matrix.
      /// </summary>
      public Matrix ViewMatrix
      {
         get { return m_ViewMatrix; }
      }

      /// <summary>
      /// Accessor for the camera's projection matrix.
      /// </summary>
      public Matrix ProjectionMatrix
      {
         get { return m_ProjectionMatrix; }
      }

      public float Pitch
      {
         get { return m_Pitch; }
         set { m_Pitch = value; }
      }

      public float Yaw
      {
         get { return m_Yaw; }
         set { m_Yaw = value; }
      }

      #endregion
   }
}

#region File Description

//-----------------------------------------------------------------------------
// PointLightComponent.cs
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
   class PointLightComponent
   {
      // Input constants; change to alter speed of movement
      const float ZOOM_SCALAR = 100.0f;
      const float ROTATION_SCALAR_X = 2.0f;
      const float ROTATION_SCALAR_Y = 1.0f;

      // Light color components
      Vector4 m_Diffuse;
      Vector4 m_Specular;

      // Positioning/Orientation
      Vector3 m_Target;
      Quaternion m_Orientation;
      float m_Pitch, m_Yaw;
      float m_Distance;

      public PointLightComponent(Vector3 p, Vector3 t, Vector4 d, Vector4 s)
      {
         // Store properties
         m_Target = t;
         m_Diffuse = d;
         m_Specular = s;

         // Calculate light direction and distance
         m_Distance = Vector3.Distance(t, p);
         
         // Defaults
         m_Pitch = 0.0f;
         m_Yaw = 0.0f;
      }

      #region Orientation

      /// <summary>
      /// Reflect updates in rotation by recomputing the light's orientation quaternion.
      /// </summary>
      public void UpdateOrientation()
      {
         // Calculate orientation in quaternion form to avoid gimbol locks
         m_Orientation = Quaternion.CreateFromAxisAngle(Vector3.Up, -m_Yaw) * Quaternion.CreateFromAxisAngle(Vector3.Right, m_Pitch);
      }

      #endregion

      #region Input Handling

      /// <summary>
      /// Orbit light right/left around a center lookat point.
      /// </summary>
      /// <param name="angle">Randians to turn right; can be negative</param>
      public void OrbitRight(float angle)
      {
         // Update yaw
         m_Yaw -= angle;

         // Restrain yaw
         m_Yaw = m_Yaw % MathHelper.TwoPi;

         // Update orientation to apply new yaw
         UpdateOrientation();
      }

      /// <summary>
      /// Orbit light up/down around a center lookat point.
      /// </summary>
      /// <param name="angle">Randians to turn right; can be negative</param>
      public void OrbitUp(float angle)
      {
         // Update pitch
         m_Pitch -= angle;

         // Restrain pitch
         m_Pitch = MathHelper.Clamp(m_Pitch, -(MathHelper.PiOver2) + .0001f, (MathHelper.PiOver2) - .0001f);

         // Update orientation to apply new pitch
         UpdateOrientation();
      }

      /// <summary>
      /// Handle light rotation based off keyboard input.
      /// </summary>
      /// <param name="keyboardState">Current frame's keyboard state</param>
      /// <param name="elapsedTime">Elapsed time since last frame</param>
      public void HandleRotation(KeyboardState keyboardState, float elapsedTime)
      {
         float dx = 0.0f;
         float dy = 0.0f;

         // Accumulate directional changes based on which keys are down
         if (keyboardState.IsKeyDown(Keys.Left)) dx += -1;
         if (keyboardState.IsKeyDown(Keys.Right)) dx += 1;
         if (keyboardState.IsKeyDown(Keys.Up)) dy += 1;
         if (keyboardState.IsKeyDown(Keys.Down)) dy += -1;

         // If there was a change...
         if ((int)dx != 0 || (int)dy != 0)
         {
            // Store change in x and y
            dx = elapsedTime * dx * ROTATION_SCALAR_X;
            dy = elapsedTime * dy * ROTATION_SCALAR_Y;

            if (dx != 0) OrbitRight(dx);  // Orbit right 'dx' radians
            if (dy != 0) OrbitUp(dy);     // Orbit up 'dy' radians
         }
      }

      /// <summary>
      /// Handle light zoom based off keyboard input.
      /// </summary>
      /// <param name="keyboardState">Current frame's keyboard state</param>
      /// <param name="elapsedTime">Elapsed time since last frame.</param>
      public void HandleZoom(KeyboardState keyboardState, float elapsedTime)
      {
         // Via keyboard
         if (keyboardState.IsKeyDown(Keys.PageDown))
            m_Distance += ZOOM_SCALAR * elapsedTime;
         else if (keyboardState.IsKeyDown(Keys.PageUp))
            m_Distance -= ZOOM_SCALAR * elapsedTime;
      }

      #endregion

      #region Public Access TV

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

      /// <summary>
      /// Accessor for the light's calculated position.  This is calculated
      /// by adding the light's inverse direction, scaled by it's zoom distance,
      /// to it's target point.
      /// </summary>
      public Vector3 Position
      {
         get { return Target - (Direction * Distance); }
      }

      /// <summary>
      /// Accessor/Mutator light's look at point.
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

      public Vector4 Diffuse
      {
         get { return m_Diffuse; }
         set { m_Diffuse = value; }
      }

      public Vector4 Specular
      {
         get { return m_Specular; }
         set { m_Specular = value; }
      }

      #endregion
   }
}

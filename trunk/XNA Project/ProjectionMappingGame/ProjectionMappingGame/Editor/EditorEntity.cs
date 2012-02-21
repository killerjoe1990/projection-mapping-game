
#region File Description

//-----------------------------------------------------------------------------
// EditorEntity.cs
//
// Author:          A.J. Fairfield (Adam, ajfairfi)
// Date Created:    2/11/2012
//-----------------------------------------------------------------------------

#endregion

#region Imports

// System imports
using System;
using System.Collections.Generic;

// XNA imports
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace ProjectionMappingGame.Editor
{
   abstract class EditorEntity
   {
      // Static Identification
      public static int ENTITY_COUNTER = 0;
      public int ID;

      protected bool m_IsActive;
      protected float m_RotX, m_RotY, m_RotZ;
      protected Vector3 m_Position;
      protected Vector3 m_Scale;
      protected Vector3 m_Forward;
      protected Vector3 m_Up;
      protected Matrix m_WorldMatrix;

      public EditorEntity(Vector3 position, bool active)
      {
         // ID the entity
         ID = ENTITY_COUNTER++;

         // Store default settings
         m_Position = position;
         m_IsActive = active;

         // Defaults
         m_Up = Vector3.Up;
         m_Forward = Vector3.Forward;
         m_RotX = 0.0f;
         m_RotY = 0.0f;
         m_RotZ = 0.0f;
      }

      #region Public Inherited Abstract Functions

      public abstract void LoadContent(ContentManager content);
      public abstract void Update(float elapsedTime);
      public abstract void Draw(Effect effect, GraphicsDevice device);

      #endregion

      #region Public Access TV

      /// <summary>
      /// Computed accessor for the entity's bounding box.
      /// </summary>
      public BoundingBox BoundingBox
      {
         get { return new BoundingBox(m_Position - m_Scale, m_Position + m_Scale); }
      }

      /// <summary>
      /// Quick way to re-compute the entity's world matrix based on it's
      /// transform components that have been altered by the GizmoComponent.
      /// </summary>
      public virtual void UpdateWorld()
      {
         m_WorldMatrix = Matrix.Identity;
         m_WorldMatrix *= Matrix.CreateScale(m_Scale);
         m_WorldMatrix *= Matrix.CreateRotationX(m_RotX);
         m_WorldMatrix *= Matrix.CreateRotationY(m_RotY);
         m_WorldMatrix *= Matrix.CreateRotationZ(m_RotZ);
         m_WorldMatrix *= Matrix.CreateTranslation(m_Position);
      }

      /// <summary>
      /// Accessor/Mutator for position.
      /// </summary>
      public virtual Vector3 Position
      {
         get { return m_Position; }
         set { m_Position = value; }
      }

      /// <summary>
      /// Accessor/Mutator for scale.
      /// </summary>
      public virtual Vector3 Scale
      {
         get { return m_Scale; }
         set { m_Scale = value; }
      }

      /// <summary>
      /// Accessor/Mutator for forward.
      /// </summary>
      public virtual Vector3 Forward
      {
         get { return m_Forward; }
         set { m_Forward = value; }
      }

      /// <summary>
      /// Accessor/Mutator for up.
      /// </summary>
      public virtual Vector3 Up
      {
         get { return m_Up; }
         set { m_Up = value; }
      }

      /// <summary>
      /// Accessor/Mutator for rotation around x-axis.
      /// </summary>
      public float RotX
      {
         get { return m_RotX; }
         set { m_RotX = value; if (m_RotX < -MathHelper.TwoPi) m_RotX = 0.0f; else if (m_RotX > MathHelper.TwoPi) m_RotX = 0.0f; }
      }

      /// <summary>
      /// Accessor/Mutator for rotation around y-axis.
      /// </summary>
      public float RotY
      {
         get { return m_RotY; }
         set { m_RotY = value; if (m_RotY < -MathHelper.TwoPi) m_RotY = 0.0f; else if (m_RotY > MathHelper.TwoPi) m_RotY = 0.0f; }
      }

      /// <summary>
      /// Accessor/Mutator for rotation around z-axis.
      /// </summary>
      public float RotZ
      {
         get { return m_RotZ; }
         set { m_RotZ = value; if (m_RotZ < -MathHelper.TwoPi) m_RotZ = 0.0f; else if (m_RotZ > MathHelper.TwoPi) m_RotZ = 0.0f; }
      }

      /// <summary>
      /// Accessor/Mutator for active state.
      /// </summary>
      public bool IsActive
      {
         get { return m_IsActive; }
         set { m_IsActive = value; }
      }

      #endregion

   }
}

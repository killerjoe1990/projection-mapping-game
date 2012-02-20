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
      Vector3 m_Position;
      Vector4 m_Diffuse;
      Vector4 m_Specular;

      public PointLightComponent(Vector3 p, Vector4 d, Vector4 s)
      {
         m_Position = p;
         m_Diffuse = d;
         m_Specular = s;
      }

      #region Public Access TV

      public Vector3 Position
      {
         get { return m_Position; }
         set { m_Position = value; }
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

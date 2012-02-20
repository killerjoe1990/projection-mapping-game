#region File Description

//-----------------------------------------------------------------------------
// Material.cs
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

#endregion

namespace ProjectionMappingGame
{
   struct Material
   {
      public Vector4 Emissive;
      public Vector4 Ambient;
      public Vector4 Diffuse;
      public Vector4 Specular;
      public float Shine;

      public Material(Vector4 e, Vector4 a, Vector4 d, Vector4 s, float ms)
      {
         Emissive = e;
         Ambient = a;
         Diffuse = d;
         Specular = s;
         Shine = ms;
      }
   }
}

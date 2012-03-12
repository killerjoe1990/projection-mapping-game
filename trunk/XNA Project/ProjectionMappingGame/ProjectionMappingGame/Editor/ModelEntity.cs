
#region File Description

//-----------------------------------------------------------------------------
// BuildingEntity.cs
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
   class ModelEntity : EditorEntity
   {
      Material m_Material;
      Model m_Mesh;
      string m_MeshFilename;

      public ModelEntity(EntityType type, string meshFilename, Material mat, Vector3 position, bool active)
         : base(type, position, active)
      {
         m_Material = mat;
         m_MeshFilename = meshFilename;
         m_Scale = Vector3.One * 5;
      }

      public override void LoadContent(ContentManager content)
      {
         // Load the building mesh
         m_Mesh = content.Load<Model>(m_MeshFilename);
      }

      #region Updating

      public override void Update(float elapsedTime)
      {
         UpdateWorld();
      }

      #endregion

      #region Rendering

      public override void Draw(Effect effect, GraphicsDevice device)
      {
         // We don't know how many meshes the artist put in this model
         foreach (ModelMesh modelMesh in m_Mesh.Meshes)
         {
            // We also don't know how many parts there are to this mesh (bones)
            foreach (ModelMeshPart meshPart in modelMesh.MeshParts)
            {
               // Now we can apply the world matrix to the shader
               effect.Parameters["worldMatrix"].SetValue(m_WorldMatrix);

               // For each pass in the shader effect
               foreach (EffectPass pass in effect.CurrentTechnique.Passes)
               {
                  // Apply the shader pass.  You can have multiple
                  // passes if you want to have multiple lights.
                  pass.Apply();

                  // Bind the mesh part's vertices to the graphics device's vertex buffer.
                  device.SetVertexBuffer(meshPart.VertexBuffer);

                  // Bind the mesh part's incices to the graphics device's vertex buffer.
                  device.Indices = meshPart.IndexBuffer;

                  // Actually render the vertices as a triangle list
                  device.DrawIndexedPrimitives(PrimitiveType.TriangleList,
                                                         meshPart.VertexOffset,
                                                         0,
                                                         meshPart.NumVertices,
                                                         meshPart.StartIndex,
                                                         meshPart.PrimitiveCount);
               }
            }
         }
      }

      #endregion

      #region Public Access TV

      /// <summary>
      /// Accessor for building material components.
      /// </summary>
      public Material Material
      {
         get { return m_Material; }
         set { m_Material = value; }
      }

      public Matrix WorldMatrix
      {
         get { return m_WorldMatrix; }
      }

      #endregion

   }
}

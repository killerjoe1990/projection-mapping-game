#region File Description

//-----------------------------------------------------------------------------
// ProjectionPreviewComponent.cs
//
// Author:          A.J. Fairfield (Adam, ajfairfi)
// Date Created:    2/1/2012
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
   
   class ProjectionPreviewComponent
   {
      Viewport m_Viewport;
      GameDriver m_Game;

      // Shaders
      Effect m_ProjectionMappingShader;
      Effect m_SolidColorShader;

      // Models
      Model m_GroundPlaneMesh;

      // Materials
      Material m_GroundPlaneMaterial;

      // Projector
      ProjectorComponent m_Projector;
      bool m_RenderProjectorFrustum;

      // Fonts
      SpriteFont m_ArialFont;

      // Camera
      CameraComponent m_Camera;
      Vector3 m_CameraLastTarget;
      float m_CameraLastDistance;
      float m_CameraLastYaw;
      float m_CameraLastPitch;
      bool m_EditorMode;

      // 3rd party gizmo tool - I DID NOT WRITE THIS
      GizmoComponent m_Gizmo;

      // Lighting
      PointLightComponent m_LightSource;
      Vector4 m_AmbientLight;

      // Input
      MouseState m_PrevMouseState;
      KeyboardState m_PrevKeyboardState;

      // Entities
      ModelEntity m_BuildingEntity;
      ModelEntity m_ProjectorEntity;
      ModelEntity m_LightEntity;

      public ProjectionPreviewComponent(GameDriver game, int x, int y, int w, int h)
      {
         m_Game = game;
         m_Viewport = new Viewport(x, y, w, h);

         // Initialize building
         m_BuildingEntity = new ModelEntity(
            "Models/bldgV_4centered",
            new Material(
               new Vector4(0.0f, 0.0f, 0.0f, 1.0f),
               new Vector4(0.0f, 0.0f, 0.0f, 1.0f),
               new Vector4(0.6f, 0.6f, 0.6f, 1.0f),
               new Vector4(1.0f, 1.0f, 1.0f, 1.0f),
               14.0f
            ),
            new Vector3(0.0f, 2.5f, 0.0f),
            true
         );
         m_BuildingEntity.Scale = Vector3.One;
         m_BuildingEntity.UpdateWorld();

         // Initialize materials
         m_GroundPlaneMaterial = new Material(
            new Vector4(0.0f, 0.0f, 0.0f, 1.0f),
            new Vector4(0.0f, 0.0f, 0.0f, 1.0f),
            new Vector4(0.8f, 0.8f, 0.8f, 1.0f),
            new Vector4(1.0f, 1.0f, 1.0f, 1.0f),
            14.0f
         );

         // Initialize lighting
         m_AmbientLight = new Vector4(0.4f, 0.4f, 0.4f, 1.0f);
         m_LightSource = new PointLightComponent(
            new Vector3(20.0f, 20.0f, 20.0f),
            new Vector4(0.33f, 0.33f, 0.0f, 1.0f),
            new Vector4(0.0f, 0.0f, 0.33f, 1.0f)
         );
         m_LightEntity = new ModelEntity(
            "Models/sphere",
            new Material(
               new Vector4(0.0f, 0.0f, 0.0f, 1.0f),
               new Vector4(0.0f, 0.0f, 0.0f, 1.0f),
               new Vector4(1.0f, 1.0f, 0.0f, 1.0f),
               new Vector4(1.0f, 1.0f, 1.0f, 1.0f),
               14.0f
            ),
            m_LightSource.Position,
            true
         );
         m_LightEntity.Scale = 1.0f * Vector3.One;
         m_LightEntity.UpdateWorld();

         // Initialize camera
         m_Camera = new CameraComponent(
            new Vector3(0.0f, 0.0f, -30.0f),
            new Vector3(0.0f, 1.0f, 0.0f),
            MathHelper.ToRadians(45.0f),
            (float)m_Viewport.Width / (float)m_Viewport.Height,
            0.1f,
            100.0f
         );
         m_Camera.OrbitUp(MathHelper.ToRadians(45.0f));
         m_Camera.OrbitRight(MathHelper.ToRadians(-45.0f));
         m_Camera.UpdateView();

         // Initialize projector
         m_Projector = new ProjectorComponent(
            new Vector3(0.0f, 2.0f, 10.0f),
            new Vector3(0.0f, 2.0f, 0.0f),
            MathHelper.ToRadians(45.0f),
            1.0f,
            10.0f,
            30.0f
         );
         m_Projector.UpdateView();
         m_ProjectorEntity = new ModelEntity(
            "Models/sphere",
            new Material(
               new Vector4(0.0f, 0.0f, 0.0f, 1.0f),
               new Vector4(0.0f, 0.0f, 0.0f, 1.0f),
               new Vector4(0.8f, 0.8f, 0.8f, 1.0f),
               new Vector4(1.0f, 1.0f, 1.0f, 1.0f),
               14.0f
            ),
            m_Projector.Position,
            true
         );
         m_ProjectorEntity.Scale = 1.0f * Vector3.One;
         m_ProjectorEntity.UpdateWorld();

         // Set defaults
         m_RenderProjectorFrustum = true;
         m_EditorMode = true;
         //SnapCameraToProjector();

         // Initialize input
         m_PrevMouseState = Mouse.GetState();
         m_PrevKeyboardState = Keyboard.GetState();
      }

      public void Reset()
      {
         // Set defaults
         m_RenderProjectorFrustum = true;
         m_EditorMode = true;

         //SnapCameraToProjector();
      }

      public void LoadContent(ContentManager content)
      {
         // Load models
         m_GroundPlaneMesh = content.Load<Model>("Models/plane");

         // Load shaders
         m_ProjectionMappingShader = content.Load<Effect>("Shaders/ProjectiveMapping");
         m_ProjectionMappingShader.CurrentTechnique = m_ProjectionMappingShader.Techniques["ProjectiveTexturing"];
         m_SolidColorShader = content.Load<Effect>("Shaders/SolidColor");
         m_SolidColorShader.CurrentTechnique = m_SolidColorShader.Techniques["SolidColor"];

         // Load fonts
         m_ArialFont = content.Load<SpriteFont>("Fonts/Arial");

         // Load entities
         m_BuildingEntity.LoadContent(content);
         m_ProjectorEntity.LoadContent(content);
         m_LightEntity.LoadContent(content);

         // Initialize gizmo
         m_Gizmo = new GizmoComponent(this, content, m_Game.GraphicsDevice);
         m_Gizmo.Initialize();
      }

      public void Update(float elapsedTime)
      {
         // Update entities
         m_BuildingEntity.Update(elapsedTime);
         m_ProjectorEntity.Update(elapsedTime);
         m_LightEntity.Update(elapsedTime);

         // Update camera
         m_Camera.UpdateView();

         // Update gizmo
         m_Gizmo.Update(elapsedTime, m_Camera.ViewMatrix, m_Camera.ProjectionMatrix);

         // Reflect possible updates in light/projector entities to their actual objects
         m_LightSource.Position = m_LightEntity.Position;
         m_Projector.Position = m_ProjectorEntity.Position;
         m_Projector.RotX = m_ProjectorEntity.RotX;
         m_Projector.RotY = m_ProjectorEntity.RotY;
         m_Projector.RotZ = m_ProjectorEntity.RotZ;
      }

      #region Input Handling

      public void HandleInput(float elapsedTime)
      {
         // Get input states
         MouseState mouseState = Mouse.GetState();
         KeyboardState keyboardState = Keyboard.GetState();

         // Handle camera/projector input
         if (m_EditorMode)
         {
            if (m_Gizmo.ActiveAxis == GizmoAxis.None)
            {
               m_Camera.HandleRotation(mouseState, m_PrevMouseState, elapsedTime);
               m_Camera.HandleZoom(mouseState, m_PrevMouseState, keyboardState, elapsedTime);
            }

            // Handle gizmo input
            m_Gizmo.HandleInput(mouseState, m_PrevMouseState, keyboardState, m_PrevKeyboardState);
         }
         else
         {
            m_Projector.HandleTranslation(keyboardState, elapsedTime);
            m_Projector.HandleRotation(mouseState, m_PrevMouseState, elapsedTime);
            m_Projector.UpdateView();
            m_ProjectorEntity.Position = m_Projector.Position;
            m_ProjectorEntity.RotX = m_Projector.RotX;
            m_ProjectorEntity.RotY = m_Projector.RotY;
            m_ProjectorEntity.RotZ = m_Projector.RotZ;
         }

         // Store input
         m_PrevKeyboardState = keyboardState;
         m_PrevMouseState = mouseState;
      }

      #endregion

      #region Rendering

      public void Draw()
      {
         m_Game.GraphicsDevice.BlendState = BlendState.Opaque;
         m_Game.GraphicsDevice.DepthStencilState = DepthStencilState.Default;

         // Configure shader
         UpdateShaderParameters(m_ProjectionMappingShader);                // Shared properties
         UpdateProjectionShaderParameters(m_ProjectionMappingShader);      // Projection mapping only properties

         // Render ground plane
         Matrix groundWorld = Matrix.Identity;
         groundWorld *= Matrix.CreateScale(new Vector3(20.0f, 1.0f, 20.0f));
         m_ProjectionMappingShader.Parameters["projecting"].SetValue(false);
         UpdateShaderMaterialParameters(m_ProjectionMappingShader, m_GroundPlaneMaterial);
         DrawModel(m_GroundPlaneMesh, m_ProjectionMappingShader, groundWorld);
         m_ProjectionMappingShader.Parameters["projecting"].SetValue(m_Projector.IsOn);

         // Render building entity
         UpdateShaderMaterialParameters(m_ProjectionMappingShader, m_BuildingEntity.Material);
         m_BuildingEntity.Draw(m_ProjectionMappingShader, m_Game.GraphicsDevice);

         // Configure shader
         UpdateShaderParameters(m_SolidColorShader);                       // Solid color only properties; no more projection mapping

         // Render light source marker
         UpdateShaderMaterialParameters(m_SolidColorShader, m_LightEntity.Material);
         m_LightEntity.Draw(m_SolidColorShader, m_Game.GraphicsDevice);

         // Render projector source marker
         UpdateShaderMaterialParameters(m_SolidColorShader, m_ProjectorEntity.Material);
         m_ProjectorEntity.Draw(m_SolidColorShader, m_Game.GraphicsDevice);

         // Render projector frustum
         if (m_RenderProjectorFrustum)
         {
            DrawProjectorFrustum();
         }

         // Draw gizmo component
         m_Gizmo.Draw3D();
      }

      #endregion

      #region Rendering Utility

      private void UpdateShaderParameters(Effect effect)
      {
         // Space matrices
         if (m_EditorMode)
         {
            effect.Parameters["viewMatrix"].SetValue(m_Camera.ViewMatrix);
         }
         else
         {
            effect.Parameters["viewMatrix"].SetValue(m_Projector.ViewMatrix);
         }
         effect.Parameters["projectionMatrix"].SetValue(m_Camera.ProjectionMatrix);

         // Light
         effect.Parameters["lightPosition"].SetValue(m_LightSource.Position);
         effect.Parameters["lightAmbientColor"].SetValue(m_AmbientLight);
         effect.Parameters["lightDiffuseColor"].SetValue(m_LightSource.Diffuse);
         effect.Parameters["lightSpecularColor"].SetValue(m_LightSource.Specular);

         // Camera
         effect.Parameters["cameraPosition"].SetValue(m_Camera.Position);
      }

      private void UpdateProjectionShaderParameters(Effect effect)
      {
         // Space matrices
         effect.Parameters["projectorViewMatrix"].SetValue(m_Projector.ViewMatrix);
         effect.Parameters["projectorProjectionMatrix"].SetValue(m_Projector.ProjectionMatrix);

         // Projector
         effect.Parameters["projtex2D"].SetValue(m_Projector.Texture);
         effect.Parameters["projecting"].SetValue(m_Projector.IsOn);
      }

      private void UpdateShaderMaterialParameters(Effect effect, Material material)
      {
         effect.Parameters["materialEmissiveColor"].SetValue(material.Emissive);
         effect.Parameters["materialAmbientColor"].SetValue(material.Ambient);
         effect.Parameters["materialDiffuseColor"].SetValue(material.Diffuse);
         effect.Parameters["materialSpecularColor"].SetValue(material.Specular);
         effect.Parameters["materialShine"].SetValue(material.Shine);
      }

      private void DrawProjectorFrustum()
      {
         Vector3 projNearC = m_Projector.Direction * m_Projector.NearPlane;
         Vector3 projFarC = m_Projector.Direction * m_Projector.FarPlane;
         float nearHalfHeight = m_Projector.NearPlane * (float)Math.Tan(m_Projector.Fov);
         float farHalfHeight = m_Projector.FarPlane * (float)Math.Tan(m_Projector.Fov);
         float nearHalfWidth = nearHalfHeight * m_Projector.AspectRatio;
         float farHalfWidth = farHalfHeight * m_Projector.AspectRatio;

         Vector3 n1 = projNearC - nearHalfHeight * m_Projector.Up + nearHalfWidth * m_Projector.Right;
         Vector3 n2 = projNearC + nearHalfHeight * m_Projector.Up + nearHalfWidth * m_Projector.Right;
         Vector3 n3 = projNearC + nearHalfHeight * m_Projector.Up - nearHalfWidth * m_Projector.Right;
         Vector3 n4 = projNearC - nearHalfHeight * m_Projector.Up - nearHalfWidth * m_Projector.Right;

         VertexPositionColor[] pointList = new VertexPositionColor[5];
         pointList[0] = new VertexPositionColor(n1, Color.White);
         pointList[1] = new VertexPositionColor(n2, Color.White);
         pointList[2] = new VertexPositionColor(n3, Color.White);
         pointList[3] = new VertexPositionColor(n4, Color.White);
         pointList[4] = new VertexPositionColor(Vector3.Zero, Color.White);

         short[] lineListIndices = new short[16];
         lineListIndices[0] = (short)0;  // near-right
         lineListIndices[1] = (short)1;
         lineListIndices[2] = (short)1;  // near-top
         lineListIndices[3] = (short)2;
         lineListIndices[4] = (short)2;  // near-left
         lineListIndices[5] = (short)3;
         lineListIndices[6] = (short)3;  // near-bottom
         lineListIndices[7] = (short)0;

         lineListIndices[8] = (short)0;  // far-right
         lineListIndices[9] = (short)4;
         lineListIndices[10] = (short)1; // far-top
         lineListIndices[11] = (short)4;
         lineListIndices[12] = (short)2; // far-left
         lineListIndices[13] = (short)4;
         lineListIndices[14] = (short)3; // far-bottom
         lineListIndices[15] = (short)4;

         m_Game.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionColor>(
             PrimitiveType.LineList,
             pointList,
             0,
             pointList.Length,
             lineListIndices,
             0,
             8
         );
      }

      private void DrawModel(Model model, Effect effect, Matrix worldMatrix)
      {
         // We don't know how many meshes the artist put in this model
         foreach (ModelMesh modelMesh in model.Meshes)
         {
            // We also don't know how many parts there are to this mesh (bones)
            foreach (ModelMeshPart meshPart in modelMesh.MeshParts)
            {
               // Now we can apply the world matrix to the shader
               effect.Parameters["worldMatrix"].SetValue(worldMatrix);

               // For each pass in the shader effect
               foreach (EffectPass pass in effect.CurrentTechnique.Passes)
               {
                  // Apply the shader pass.  You can have multiple
                  // passes if you want to have multiple lights.
                  pass.Apply();

                  // Bind the mesh part's vertices to the graphics device's vertex buffer.
                  m_Game.GraphicsDevice.SetVertexBuffer(meshPart.VertexBuffer);

                  // Bind the mesh part's incices to the graphics device's vertex buffer.
                  m_Game.GraphicsDevice.Indices = meshPart.IndexBuffer;

                  // Actually render the vertices as a triangle list
                  m_Game.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList,
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

      public ModelEntity ProjectorEntity
      {
         get { return m_ProjectorEntity; }
      }

      public ModelEntity LightEntity
      {
         get { return m_LightEntity; }
      }

      public List<EditorEntity> Entities
      {
         get
         {
            List<EditorEntity> entities = new List<EditorEntity>();
            entities.Add(m_BuildingEntity);
            entities.Add(m_ProjectorEntity);
            entities.Add(m_LightEntity);
            return entities;
         }
      }

      public bool ProjectorIsOn
      {
         get { return m_Projector.IsOn; }
         set { m_Projector.IsOn = value; }
      }

      public bool RenderFrustum
      {
         get { return m_RenderProjectorFrustum; }
         set { m_RenderProjectorFrustum = value; }
      }

      public bool EditorMode
      {
         get { return m_EditorMode; }
         set { m_EditorMode = value; }
      }

      public Vector3 CameraPosition
      {
         get { return m_Camera.Position; }
      }

      public Viewport Viewport
      {
         get { return m_Viewport; }
      }

      public Texture2D ProjectorTexture
      {
         set { m_Projector.Texture = value; }
      }

      public ProjectorComponent Projector
      {
         get { return m_Projector; }
      }

      public CameraComponent Camera
      {
         get { return m_Camera; }
      }

      public GizmoComponent Gizmo
      {
         get { return m_Gizmo; }
      }

      public MouseState PrevMouseState
      {
         set { m_PrevMouseState = value; }
      }

      public ModelEntity Building
      {
         get { return m_BuildingEntity; }
      }

      #endregion

   }
}

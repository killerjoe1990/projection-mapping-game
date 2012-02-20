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
      Model m_SphereMesh;

      // Materials
      Material m_GroundPlaneMaterial;
      Material m_LightSourceMaterial;
      Material m_ProjectorSourceMaterial;

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
      bool m_MoveCamera;
      bool m_ProjectorAttached;

      // 3rd party gizmo tool - I DID NOT WRITE THIS
      GizmoComponent m_Gizmo;

      // Lighting
      PointLightComponent m_LightSource;
      Vector4 m_AmbientLight;

      // Input
      MouseState m_PrevMouseState;
      KeyboardState m_PrevKeyboardState;

      // Building
      BuildingEntity m_BuildingEntity;

      public ProjectionPreviewComponent(GameDriver game, int x, int y, int w, int h)
      {
         m_Game = game;
         m_Viewport = new Viewport(x, y, w, h);

         // Initialize building
         m_BuildingEntity = new BuildingEntity(
            "Models/cube",
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
         m_BuildingEntity.Scale = 5 * Vector3.One;
         m_BuildingEntity.UpdateWorld();

         // Initialize materials
         m_GroundPlaneMaterial = new Material(
            new Vector4(0.0f, 0.0f, 0.0f, 1.0f),
            new Vector4(0.0f, 0.0f, 0.0f, 1.0f),
            new Vector4(0.8f, 0.8f, 0.8f, 1.0f),
            new Vector4(1.0f, 1.0f, 1.0f, 1.0f),
            14.0f
         );
         m_ProjectorSourceMaterial = new Material(
            new Vector4(0.0f, 0.0f, 0.0f, 1.0f),
            new Vector4(0.0f, 0.0f, 0.0f, 1.0f),
            new Vector4(0.8f, 0.8f, 0.8f, 1.0f),
            new Vector4(1.0f, 1.0f, 1.0f, 1.0f),
            14.0f
         );
         m_LightSourceMaterial = new Material(
            new Vector4(0.0f, 0.0f, 0.0f, 1.0f),
            new Vector4(0.0f, 0.0f, 0.0f, 1.0f),
            new Vector4(1.0f, 1.0f, 0.0f, 1.0f),
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
            new Vector3(0.0f, 0.0f, -10.0f),
            new Vector3(0.0f, 1.0f, 0.0f),
            MathHelper.ToRadians(45.0f),
            1.0f,
            10.0f,
            30.0f
         );
         m_Projector.OrbitUp(MathHelper.ToRadians(30.0f));
         m_Projector.UpdateView();

         // Set defaults
         m_RenderProjectorFrustum = true;
         m_MoveCamera = true;
         m_ProjectorAttached = false;
         //SnapCameraToProjector();

         // Initialize input
         m_PrevMouseState = Mouse.GetState();
         m_PrevKeyboardState = Keyboard.GetState();
      }

      public void Reset()
      {
         // Set defaults
         m_RenderProjectorFrustum = true;
         m_MoveCamera = true;
         m_ProjectorAttached = false;

         //SnapCameraToProjector();
      }

      public void LoadContent(ContentManager content)
      {
         // Load models
         m_GroundPlaneMesh = content.Load<Model>("Models/plane");
         m_SphereMesh = content.Load<Model>("Models/sphere");

         // Load shaders
         m_ProjectionMappingShader = content.Load<Effect>("Shaders/ProjectiveMapping");
         m_ProjectionMappingShader.CurrentTechnique = m_ProjectionMappingShader.Techniques["ProjectiveTexturing"];
         m_SolidColorShader = content.Load<Effect>("Shaders/SolidColor");
         m_SolidColorShader.CurrentTechnique = m_SolidColorShader.Techniques["SolidColor"];

         // Load fonts
         m_ArialFont = content.Load<SpriteFont>("Fonts/Arial");

         // Load building
         m_BuildingEntity.LoadContent(content);

         // Initialize gizmo
         m_Gizmo = new GizmoComponent(this, content, m_Game.GraphicsDevice);
         m_Gizmo.Initialize();
      }

      #region Input Handling

      public void HandleInput(float elapsedTime)
      {
         // Get input states
         MouseState mouseState = Mouse.GetState();
         KeyboardState keyboardState = Keyboard.GetState();

         // Update the building
         m_BuildingEntity.Update(elapsedTime);

         // Handle turning the projector on/off
         if (keyboardState.IsKeyDown(Keys.P) && !m_PrevKeyboardState.IsKeyDown(Keys.P))
            m_Projector.IsOn = !m_Projector.IsOn;

         // Handle mode switching
         if (keyboardState.IsKeyDown(Keys.C) && !m_PrevKeyboardState.IsKeyDown(Keys.C))
         {
            if (m_ProjectorAttached)
            {
               m_ProjectorAttached = false;
               m_MoveCamera = true;
               SnapCameraToOrbitPosition();
            }
            else if (m_MoveCamera)
            {
               m_MoveCamera = false;
            }
            else if (!m_MoveCamera)
            {
               m_ProjectorAttached = true;
               SnapCameraToProjector();
            }
         }

         // Handle gizmo mode switching
         if (keyboardState.IsKeyDown(Keys.O) && !m_PrevKeyboardState.IsKeyDown(Keys.O))
         {
            if (m_Gizmo.ActiveMode == GizmoMode.Translate)
               m_Gizmo.ActiveMode = GizmoMode.Rotate;
            else if (m_Gizmo.ActiveMode == GizmoMode.Rotate)
               m_Gizmo.ActiveMode = GizmoMode.UniformScale;
            else if (m_Gizmo.ActiveMode == GizmoMode.UniformScale)
               m_Gizmo.ActiveMode = GizmoMode.Translate;
         }

         // Handle camera/projector input
         if (m_ProjectorAttached)
         {
            m_Camera.HandleRotation(mouseState, m_PrevMouseState, elapsedTime);
            m_Projector.HandleTranslation(keyboardState, elapsedTime);
            m_Projector.HandleRotation(mouseState, m_PrevMouseState, elapsedTime);
            m_Projector.HandleZoom(mouseState, m_PrevMouseState, keyboardState, elapsedTime);

            // Since the projector and camera have different translation and zoom speeds,
            // we use the results from the projector in this mode.
            m_Camera.Target = m_Projector.Target;
            m_Camera.Distance = m_Projector.Distance;
            m_Camera.UpdateView();
         }
         else if (m_MoveCamera)
         {
            if (m_Gizmo.ActiveAxis == GizmoAxis.None)
            {
               m_Camera.HandleRotation(mouseState, m_PrevMouseState, elapsedTime);
               m_Camera.HandleZoom(mouseState, m_PrevMouseState, keyboardState, elapsedTime);
            }

            // Handle gizmo input
            m_Gizmo.Update(elapsedTime, m_Camera.ViewMatrix, m_Camera.ProjectionMatrix);
            m_Gizmo.HandleInput(mouseState, m_PrevMouseState, keyboardState, m_PrevKeyboardState);
         }
         else
         {
            m_Projector.HandleTranslation(keyboardState, elapsedTime);
            m_Projector.HandleRotation(mouseState, m_PrevMouseState, elapsedTime);
            m_Projector.HandleZoom(mouseState, m_PrevMouseState, keyboardState, elapsedTime);
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
         UpdateShaderMaterialParameters(m_ProjectionMappingShader, m_GroundPlaneMaterial);
         DrawModel(m_GroundPlaneMesh, m_ProjectionMappingShader, groundWorld);

         // Render building entity
         UpdateShaderMaterialParameters(m_ProjectionMappingShader, m_BuildingEntity.Material);
         m_BuildingEntity.Draw(m_ProjectionMappingShader, m_Game.GraphicsDevice);

         // Configure shader
         UpdateShaderParameters(m_SolidColorShader);                       // Solid color only properties; no more projection mapping

         // Render light source marker
         Matrix lightSourceWorld = Matrix.Identity;
         lightSourceWorld *= Matrix.CreateTranslation(m_LightSource.Position);
         UpdateShaderMaterialParameters(m_SolidColorShader, m_LightSourceMaterial);
         DrawModel(m_SphereMesh, m_SolidColorShader, lightSourceWorld);

         // Render projector source marker
         Matrix projSourceWorld = Matrix.Identity;
         projSourceWorld *= Matrix.CreateTranslation(m_Projector.Position);
         UpdateShaderMaterialParameters(m_SolidColorShader, m_ProjectorSourceMaterial);
         DrawModel(m_SphereMesh, m_SolidColorShader, projSourceWorld);

         // Render projector frustum
         if (m_RenderProjectorFrustum)
         {
            DrawProjectorFrustum();
         }

         // Draw gizmo component
         m_Gizmo.Draw3D();
      }

      public void DrawGUI(SpriteBatch spriteBatch)
      {
         string projStatus = (ProjectorIsOn) ? "On" : "Off";

         string controlStatus = (ProjectorAttached) ? "Projector" : ((MoveCamera) ? "Editor (Camera)" : "Editor (Projector)");
         string orbitzoomMode = (ProjectorAttached) ? "Projector" : ((MoveCamera) ? "Camera" : "Projector");
         int column1 = m_Viewport.Width - 300;
         int y = GameConstants.WINDOW_HEIGHT - 205;

         spriteBatch.Begin();
         spriteBatch.DrawString(m_ArialFont, "Projection Editor Controls", Vector2.One + new Vector2(m_Viewport.Width - m_ArialFont.MeasureString("Projection Editor Controls").Length(), y + 5), Color.Black);
         spriteBatch.DrawString(m_ArialFont, "Projection Editor Controls", new Vector2(m_Viewport.Width - m_ArialFont.MeasureString("Projection Editor Controls").Length(), y + 5), Color.White);
         spriteBatch.DrawString(m_ArialFont, "Orbit " + orbitzoomMode, Vector2.One + new Vector2(column1, y + 30), Color.Black);
         spriteBatch.DrawString(m_ArialFont, "Orbit " + orbitzoomMode, new Vector2(column1, y + 30), Color.White);
         spriteBatch.DrawString(m_ArialFont, "Zoom " + orbitzoomMode, Vector2.One + new Vector2(column1, y + 55), Color.Black);
         spriteBatch.DrawString(m_ArialFont, "Zoom " + orbitzoomMode, new Vector2(column1, y + 55), Color.White);
         spriteBatch.DrawString(m_ArialFont, "Toggle Projector", Vector2.One + new Vector2(column1, y + 80), Color.Black);
         spriteBatch.DrawString(m_ArialFont, "Toggle Projector", new Vector2(column1, y + 80), Color.White);
         spriteBatch.DrawString(m_ArialFont, "Toggle Controls", Vector2.One + new Vector2(column1, y + 105), Color.Black);
         spriteBatch.DrawString(m_ArialFont, "Toggle Controls", new Vector2(column1, y + 105), Color.White);
         spriteBatch.DrawString(m_ArialFont, "Cycle Gizmo", Vector2.One + new Vector2(column1, y + 130), Color.Black);
         spriteBatch.DrawString(m_ArialFont, "Cycle Gizmo", new Vector2(column1, y + 130), Color.White);
         spriteBatch.DrawString(m_ArialFont, "Left-Click & Drag", Vector2.One + new Vector2(m_Viewport.Width - m_ArialFont.MeasureString("Left-Click & Drag").Length(), y + 30), Color.Black);
         spriteBatch.DrawString(m_ArialFont, "Left-Click & Drag", new Vector2(m_Viewport.Width - m_ArialFont.MeasureString("Left-Click & Drag").Length(), y + 30), Color.White);
         spriteBatch.DrawString(m_ArialFont, "Scroll-Wheel", Vector2.One + new Vector2(m_Viewport.Width - m_ArialFont.MeasureString("Scroll-Wheel").Length(), y + 55), Color.Black);
         spriteBatch.DrawString(m_ArialFont, "Scroll-Wheel", new Vector2(m_Viewport.Width - m_ArialFont.MeasureString("Scroll-Wheel").Length(), y + 55), Color.White);
         spriteBatch.DrawString(m_ArialFont, "P", Vector2.One + new Vector2(m_Viewport.Width - m_ArialFont.MeasureString("P").Length(), y + 80), Color.Black);
         spriteBatch.DrawString(m_ArialFont, "P", new Vector2(m_Viewport.Width - m_ArialFont.MeasureString("P").Length(), y + 80), Color.White);
         spriteBatch.DrawString(m_ArialFont, "C", Vector2.One + new Vector2(m_Viewport.Width - m_ArialFont.MeasureString("C").Length(), y + 105), Color.Black);
         spriteBatch.DrawString(m_ArialFont, "C", new Vector2(m_Viewport.Width - m_ArialFont.MeasureString("C").Length(), y + 105), Color.White);
         spriteBatch.DrawString(m_ArialFont, "O", Vector2.One + new Vector2(m_Viewport.Width - m_ArialFont.MeasureString("O").Length(), y + 130), Color.Black);
         spriteBatch.DrawString(m_ArialFont, "O", new Vector2(m_Viewport.Width - m_ArialFont.MeasureString("O").Length(), y + 130), Color.White);

         spriteBatch.DrawString(m_ArialFont, "Reset", Vector2.One + new Vector2(column1, y + 155), Color.Black);
         spriteBatch.DrawString(m_ArialFont, "Reset", new Vector2(column1, y + 155), Color.White);
         spriteBatch.DrawString(m_ArialFont, "Back to Main Menu", Vector2.One + new Vector2(column1, y + 180), Color.Black);
         spriteBatch.DrawString(m_ArialFont, "Back to Main Menu", new Vector2(column1, y + 180), Color.White);
         spriteBatch.DrawString(m_ArialFont, "R", Vector2.One + new Vector2(m_Viewport.Width - m_ArialFont.MeasureString("R").Length(), y + 155), Color.Black);
         spriteBatch.DrawString(m_ArialFont, "R", new Vector2(m_Viewport.Width - m_ArialFont.MeasureString("R").Length(), y + 155), Color.White);
         spriteBatch.DrawString(m_ArialFont, "M", Vector2.One + new Vector2(m_Viewport.Width - m_ArialFont.MeasureString("M").Length(), y + 180), Color.Black);
         spriteBatch.DrawString(m_ArialFont, "M", new Vector2(m_Viewport.Width - m_ArialFont.MeasureString("M").Length(), y + 180), Color.White);

         // Scene status
         string mode = "Mode: " + controlStatus; 
         spriteBatch.DrawString(m_ArialFont, mode, Vector2.One + new Vector2(5, 5), Color.Black);
         spriteBatch.DrawString(m_ArialFont, mode, new Vector2(5, 5), Color.White);
         spriteBatch.DrawString(m_ArialFont, "Projector: " + projStatus, Vector2.One + new Vector2(5, 30), Color.Black);
         spriteBatch.DrawString(m_ArialFont, "Projector: " + projStatus, new Vector2(5, 30), Color.White);
         spriteBatch.DrawString(m_ArialFont, "Gizmo Tool: " + m_Gizmo.ActiveMode.ToString(), Vector2.One + new Vector2(5, 55), Color.Black);
         spriteBatch.DrawString(m_ArialFont, "Gizmo Tool: " + m_Gizmo.ActiveMode.ToString(), new Vector2(5, 55), Color.White);
         spriteBatch.End();
      }

      #endregion

      #region Rendering Utility

      private void SnapCameraToOrbitPosition()
      {
         // Restore camera state
         m_Camera.Target = m_CameraLastTarget;
         m_Camera.Distance = m_CameraLastDistance;
         m_Camera.Yaw = m_CameraLastYaw;
         m_Camera.Pitch = m_CameraLastPitch;
         m_Camera.UpdateView();
      }

      private void SnapCameraToProjector()
      {
         // Save camera state
         m_CameraLastTarget = m_Camera.Target;
         m_CameraLastDistance = m_Camera.Distance;
         m_CameraLastYaw = m_Camera.Yaw;
         m_CameraLastPitch = m_Camera.Pitch;

         // Snap to projector
         m_Camera.Target = m_Projector.Target;
         m_Camera.Distance = m_Projector.Distance;
         m_Camera.Yaw = m_Projector.Yaw;
         m_Camera.Pitch = m_Projector.Pitch;
         m_Camera.UpdateView();
      }

      private void UpdateShaderParameters(Effect effect)
      {
         // Space matrices
         effect.Parameters["viewMatrix"].SetValue(m_Camera.ViewMatrix);
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

      public List<EditorEntity> Entities
      {
         get
         {
            List<EditorEntity> entities = new List<EditorEntity>();
            entities.Add(m_BuildingEntity);
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

      public bool ProjectorAttached
      {
         get { return m_ProjectorAttached; }
      }

      public bool MoveCamera
      {
         get { return m_MoveCamera; }
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

      public MouseState PrevMouseState
      {
         set { m_PrevMouseState = value; }
      }

      #endregion

   }
}

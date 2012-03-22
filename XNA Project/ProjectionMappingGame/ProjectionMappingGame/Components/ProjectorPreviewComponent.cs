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
      const string NORMALS_MESSAGE = "Use the mouse cursor to select a face normal from the model mesh. Right-click to cancel.";
      const string QUIT_MESSAGE = "Click the \"Pause\" button or press the \"Escape\" key to pause gameplay.";

      // Projector fields
      Viewport m_Viewport;
      GameDriver m_Game;
      Effect m_ProjectionMappingShader;      // Shaders
      Effect m_SolidColorShader;
      Effect m_PhongShader;
      Effect m_NormalsShader;
      Model m_GroundPlaneMesh;               // Models
      Vector3 m_SelectedNormal;
      Material m_GroundPlaneMaterial;        // Materials
      List<ProjectorComponent> m_Projectors; // Projectors
      bool m_RenderProjectorFrustum;
      SpriteFont m_ArialFont;                // Fonts
      CameraComponent m_Camera;              // Camera
      bool m_EditorMode;                     // States
      bool m_RenderNormals;
      GizmoComponent m_Gizmo;                // 3rd party gizmo tool - I DID NOT WRITE THIS - AJ
      PointLightComponent m_LightSource;     // Lighting
      Vector4 m_AmbientLight;
      MouseState m_PrevMouseState;           // Input
      KeyboardState m_PrevKeyboardState;
      int m_SelectedProjector;
      int m_SelectedBuilding;
      List<ModelEntity> m_Buildings;          // Entities
      ModelEntity m_LightEntity;
      RenderTarget2D m_RenderTarget;         // Render target
      Texture2D m_RenderTargetTexture;
      BasicEffect m_FrustumEffect;
      GridComponent m_Grid;

      public ProjectionPreviewComponent(GameDriver game, int x, int y, int w, int h)
      {
         m_Game = game;
         m_Viewport = new Viewport(x, y, w, h);

         // Initialize building
         m_Buildings = new List<ModelEntity>();
         AddBuilding();

         // Initialize grid
         m_Grid = new GridComponent(m_Game.GraphicsDevice, 5, 200, true);

         m_FrustumEffect = new BasicEffect(m_Game.GraphicsDevice);
         
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
            new Vector3(0.0f, 0.0f, 100.0f),
            new Vector3(0.0f, 0.0f, 0.0f),
            new Vector4(1.0f, 1.0f, 1.0f, 1.0f),
            new Vector4(0.0f, 0.0f, 0.33f, 1.0f)
         );
         m_LightSource.OrbitRight(MathHelper.PiOver4);
         m_LightSource.OrbitUp(MathHelper.PiOver4);
         m_LightSource.UpdateOrientation();
         m_LightEntity = new ModelEntity(
            EntityType.Light,
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
         m_LightEntity.Scale = GameConstants.LIGHT_SCALE * Vector3.One;
         m_LightEntity.UpdateWorld();
         
         // Initialize camera
         m_Camera = new CameraComponent(
            GameConstants.DEFAULT_CAMERA_POSITION,
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
         m_Projectors = new List<ProjectorComponent>();
         
         // Set defaults
         m_RenderProjectorFrustum = true;
         m_EditorMode = true;
         m_RenderNormals = false;
         m_SelectedNormal = Vector3.Zero;
         m_SelectedProjector = -1;
         m_SelectedBuilding = -1;

         // Initialize input
         m_PrevMouseState = Mouse.GetState();
         m_PrevKeyboardState = Keyboard.GetState();

         // Initialize render target
         m_RenderTarget = new RenderTarget2D(m_Game.GraphicsDevice, w, h, true, m_Game.GraphicsDevice.DisplayMode.Format, DepthFormat.Depth24);
      }

      public void Reset()
      {
         // Set defaults
         m_RenderProjectorFrustum = true;
         m_EditorMode = true;
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
         m_PhongShader = content.Load<Effect>("Shaders/Phong");
         m_PhongShader.CurrentTechnique = m_PhongShader.Techniques["PhongDiffuseOnly"];
         m_NormalsShader = content.Load<Effect>("Shaders/Normals");
         m_NormalsShader.CurrentTechnique = m_NormalsShader.Techniques["NormalsOnly"];

         // Load fonts
         m_ArialFont = content.Load<SpriteFont>("Fonts/Arial10");

         // Load entities
         for (int i = 0; i < m_Buildings.Count; ++i)
         {
            m_Buildings[i].LoadContent(content);
         }
         for (int i = 0; i < m_Projectors.Count; ++i)
         {
            m_Projectors[i].Entity.LoadContent(content);
         }
         m_LightEntity.LoadContent(content);

         // Initialize gizmo
         m_Gizmo = new GizmoComponent(this, content, m_Game.GraphicsDevice);
         m_Gizmo.Initialize();
      }

      public void Update(float elapsedTime)
      {
         // Update entities
         for (int i = 0; i < m_Buildings.Count; ++i)
         {
            m_Buildings[i].Update(elapsedTime);
         }
         m_LightEntity.Update(elapsedTime);
         for (int i = 0; i < m_Projectors.Count; ++i)
         {
            m_Projectors[i].Entity.Update(elapsedTime);
         }

         // Update camera
         m_Camera.UpdateView();

         // Update gizmo
         m_Gizmo.Update(elapsedTime, m_Camera.ViewMatrix, m_Camera.ProjectionMatrix);

         // Reflect possible updates in light/projector entities to their actual objects
         //m_LightSource.Position = m_LightEntity.Position;
         for (int i = 0; i < m_Projectors.Count; ++i)
         {
            m_Projectors[i].Position = m_Projectors[i].Entity.Position;
            m_Projectors[i].RotX = m_Projectors[i].Entity.RotX;
            m_Projectors[i].RotY = m_Projectors[i].Entity.RotY;
            m_Projectors[i].RotZ = m_Projectors[i].Entity.RotZ;
         }
      }

      #region Input Handling

      public void HandleInput(bool inGame, float elapsedTime)
      {
         // Get input states
         MouseState mouseState = Mouse.GetState();
         KeyboardState keyboardState = Keyboard.GetState();
         
         if (m_RenderNormals)
         {
            if (mouseState.LeftButton == ButtonState.Released && m_PrevMouseState.LeftButton == ButtonState.Pressed)
            {
               // Handle mesh picking to leave normal rendering
               bool picked = false;
               Vector3 normal = Vector3.Zero;

               // Get the color the mouse clicked on
               Color[] colors = new Color[m_RenderTargetTexture.Width * m_RenderTargetTexture.Height];
               m_RenderTargetTexture.GetData(colors);
               Color colorPicked = Color.Black;
               Vector2 mousePos = new Vector2(mouseState.X - m_Viewport.X, mouseState.Y - m_Viewport.Y);
               if (mousePos.X < m_RenderTargetTexture.Width && mousePos.Y < m_RenderTargetTexture.Height)
               {
                  colorPicked = colors[((int)mousePos.X + (int)mousePos.Y * m_RenderTargetTexture.Width)];
               }
               picked = (colorPicked != Color.Black);

               if (picked)
               {
                  // Translate the color to the normal value; see shader for why I do this
                  normal = new Vector3(colorPicked.R / 255.0f, colorPicked.G / 255.0f, colorPicked.B / 255.0f);
                  normal -= new Vector3(0.5f);
                  normal /= 0.5f;
                  normal.Normalize();

                  m_SelectedNormal = normal;
                  if (m_OnLeaveNormalSelectionMode != null)
                     m_OnLeaveNormalSelectionMode(this, new EventArgs());
               }
            }
         }

         if (inGame)
         {
            m_LightSource.HandleRotation(keyboardState, elapsedTime);
            m_LightSource.HandleZoom(keyboardState, elapsedTime);
            m_LightSource.UpdateOrientation();
            m_LightEntity.Position = m_LightSource.Position;
         }

         // Handle camera/projector input
         if (m_EditorMode)
         {
            if (m_Gizmo.SelectedID == -1 || m_Gizmo.ActiveAxis == GizmoAxis.None)
            {
               m_Camera.HandleRotation(mouseState, m_PrevMouseState, elapsedTime);
               m_Camera.HandleZoom(mouseState, m_PrevMouseState, keyboardState, elapsedTime);
            }

            if (!inGame && !m_RenderNormals)
            {
               // Handle gizmo input
               m_Gizmo.HandleInput(mouseState, m_PrevMouseState, keyboardState, m_PrevKeyboardState);
            }
         }
         else
         {
            if (m_SelectedProjector >= 0)
            {
               m_Projectors[m_SelectedProjector].HandleTranslation(keyboardState, elapsedTime);
               m_Projectors[m_SelectedProjector].HandleRotation(mouseState, m_PrevMouseState, elapsedTime);
               m_Projectors[m_SelectedProjector].UpdateView();
               m_Projectors[m_SelectedProjector].Entity.Position = m_Projectors[m_SelectedProjector].Position;
               m_Projectors[m_SelectedProjector].Entity.RotX = m_Projectors[m_SelectedProjector].RotX;
               m_Projectors[m_SelectedProjector].Entity.RotY = m_Projectors[m_SelectedProjector].RotY;
               m_Projectors[m_SelectedProjector].Entity.RotZ = m_Projectors[m_SelectedProjector].RotZ;
            }
         }

         // Store input
         m_PrevKeyboardState = keyboardState;
         m_PrevMouseState = mouseState;
      }

      EventHandler m_OnLeaveNormalSelectionMode;
      public void RegisterOnLeaveNormalSelectionMode(EventHandler handler)
      {
         m_OnLeaveNormalSelectionMode += handler;
      }

      #endregion

      #region Rendering

      public void DrawRenderTarget(SpriteBatch spriteBatch)
      {
         m_Game.GraphicsDevice.BlendState = BlendState.NonPremultiplied;
         m_Game.GraphicsDevice.DepthStencilState = DepthStencilState.Default;

         UpdateShaderParameters(m_NormalsShader);
         
            
         // Set the render target to capture a still of the screen
         m_Game.GraphicsDevice.SetRenderTarget(m_RenderTarget);
         //m_Game.GraphicsDevice.Viewport = m_Viewport;
         m_Game.GraphicsDevice.Clear(Color.Black);

         // Render building to the render target
         for (int i = 0; i < m_Buildings.Count; ++i)
         {
            UpdateShaderMaterialParameters(m_NormalsShader, m_Buildings[i].Material);
            m_Buildings[i].Draw(m_NormalsShader, m_Game.GraphicsDevice);
         }

         // Extract and store the contents of the render target in a texture
         m_Game.GraphicsDevice.SetRenderTarget(null);
         m_Game.GraphicsDevice.Clear(Color.Black);
         m_Game.GraphicsDevice.Viewport = m_Viewport;
         m_RenderTargetTexture = (Texture2D)m_RenderTarget;
      }

      public void Draw(SpriteBatch spriteBatch, bool inGame)
      {
         m_Game.GraphicsDevice.BlendState = BlendState.NonPremultiplied;
         m_Game.GraphicsDevice.DepthStencilState = DepthStencilState.Default;

         if (m_RenderNormals)
         {
            UpdateShaderParameters(m_NormalsShader);
            
            // Render building to the screen now
            for (int i = 0; i < m_Buildings.Count; ++i)
            {
               UpdateShaderMaterialParameters(m_NormalsShader, m_Buildings[i].Material);
               m_Buildings[i].Draw(m_NormalsShader, m_Game.GraphicsDevice);
            }

            // Render a message about how to select normals and how to cancel
            spriteBatch.Begin();
            spriteBatch.DrawString(m_ArialFont, NORMALS_MESSAGE, new Vector2(5, m_Viewport.Height - 20) + Vector2.One, Color.Black);
            spriteBatch.DrawString(m_ArialFont, NORMALS_MESSAGE, new Vector2(5, m_Viewport.Height - 20), Color.White);
            spriteBatch.End();
         }
         else
         {
            m_Grid.Draw(m_Camera.ViewMatrix, m_Camera.ProjectionMatrix);

            // Configure shaders
            UpdateShaderParameters(m_PhongShader);
            UpdateShaderParameters(m_SolidColorShader);

            //if (!inGame)
            //{
               // Render ground plane
               Matrix groundWorld = Matrix.Identity;
               groundWorld *= Matrix.CreateScale(new Vector3(GameConstants.PLANE_SCALE, 1.0f, GameConstants.PLANE_SCALE));
               UpdateShaderMaterialParameters(m_PhongShader, m_GroundPlaneMaterial);
               DrawModel(m_GroundPlaneMesh, m_PhongShader, groundWorld);
            //}

            // Render building entity
            for (int i = 0; i < m_Buildings.Count; ++i)
            {
               UpdateShaderMaterialParameters(m_PhongShader, m_Buildings[i].Material);
               m_Buildings[i].Draw(m_PhongShader, m_Game.GraphicsDevice);
            }
            /*for (int i = 0; i < m_Projectors.Count; ++i)
            {
               if (m_Projectors[i].IsOn)
               {
                  UpdateShaderParameters(m_ProjectionMappingShader);                // Shared properties
                  UpdateProjectionShaderParameters(m_ProjectionMappingShader, m_Projectors[i]);      // Projection mapping only properties
                  for (int j = 0; j < m_Buildings.Count; ++j)
                  {
                     UpdateShaderMaterialParameters(m_ProjectionMappingShader, m_Buildings[j].Material);
                     m_Buildings[j].Draw(m_ProjectionMappingShader, m_Game.GraphicsDevice);
                  }
               }
            }*/

            //if (!inGame)
            //{
            // Render light source marker
            UpdateShaderMaterialParameters(m_SolidColorShader, m_LightEntity.Material);
            m_LightEntity.Draw(m_SolidColorShader, m_Game.GraphicsDevice);

            for (int i = 0; i < m_Projectors.Count; ++i)
            {
               // Render projector source marker
               UpdateShaderMaterialParameters(m_SolidColorShader, m_Projectors[i].Entity.Material);
               m_Projectors[i].Entity.Draw(m_SolidColorShader, m_Game.GraphicsDevice);

               // Render projector frustum
               //if (m_Gizmo.SelectedID == m_Projectors[i].Entity.ID)
               //   DrawProjectorFrustum(m_FrustumEffect, m_Projectors[i]);
            }

            // Draw gizmo component
            m_Gizmo.Draw3D();
            //}

            if (inGame)
            {
               spriteBatch.Begin();
               spriteBatch.DrawString(m_ArialFont, QUIT_MESSAGE, new Vector2(5, m_Viewport.Height - 20) + Vector2.One, Color.Black);
               spriteBatch.DrawString(m_ArialFont, QUIT_MESSAGE, new Vector2(5, m_Viewport.Height - 20), Color.White);
               spriteBatch.End();
            }
         }
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
            effect.Parameters["viewMatrix"].SetValue(m_Projectors[0].ViewMatrix);
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

      private void UpdateProjectionShaderParameters(Effect effect, ProjectorComponent projector)
      {
         // Space matrices
         effect.Parameters["projectorViewMatrix"].SetValue(projector.ViewMatrix);
         effect.Parameters["projectorProjectionMatrix"].SetValue(projector.ProjectionMatrix);

         // Projector
         effect.Parameters["projtex2D"].SetValue(projector.Texture);
         effect.Parameters["projecting"].SetValue(projector.IsOn);
         effect.Parameters["projectorPosition"].SetValue(projector.Position);
         m_ProjectionMappingShader.Parameters["projectorAlpha"].SetValue(projector.Alpha);
      }

      private void UpdateShaderMaterialParameters(Effect effect, Material material)
      {
         effect.Parameters["materialEmissiveColor"].SetValue(material.Emissive);
         effect.Parameters["materialAmbientColor"].SetValue(material.Ambient);
         effect.Parameters["materialDiffuseColor"].SetValue(material.Diffuse);
         effect.Parameters["materialSpecularColor"].SetValue(material.Specular);
         effect.Parameters["materialShine"].SetValue(material.Shine);
      }

      private void DrawProjectorFrustum(BasicEffect effect, ProjectorComponent projector)
      {
         Matrix projectorWorld = projector.Entity.WorldMatrix;
         effect.World = projectorWorld;
         effect.View = m_Camera.ViewMatrix;
         effect.Projection = m_Camera.ProjectionMatrix;

         Vector3 projNearC = Vector3.Forward * projector.NearPlane;
         Vector3 projFarC = Vector3.Forward * projector.FarPlane;
         float nearHalfHeight = projector.NearPlane * (float)Math.Tan(projector.Fov);
         float farHalfHeight = projector.FarPlane * (float)Math.Tan(projector.Fov);
         float nearHalfWidth = nearHalfHeight * projector.AspectRatio;
         float farHalfWidth = farHalfHeight * projector.AspectRatio;

         Vector3 n1 = projNearC + (-nearHalfHeight * Vector3.Up + nearHalfWidth * Vector3.Right);
         Vector3 n2 = projNearC + (nearHalfHeight * Vector3.Up + nearHalfWidth * Vector3.Right);
         Vector3 n3 = projNearC + (nearHalfHeight * Vector3.Up - nearHalfWidth * Vector3.Right);
         Vector3 n4 = projNearC + (-nearHalfHeight * Vector3.Up - nearHalfWidth * Vector3.Right);

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

         // For each pass in the shader effect
         foreach (EffectPass pass in effect.CurrentTechnique.Passes)
         {
            pass.Apply();
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

      #region Public Interaction

      public void ResetSelectedBuilding()
      {
         m_Buildings[m_SelectedBuilding].Scale = Vector3.One * 5.0f;
         m_Buildings[m_SelectedBuilding].RotX = 0.0f;
         m_Buildings[m_SelectedBuilding].RotY = 0.0f;
         m_Buildings[m_SelectedBuilding].RotZ = 0.0f;
         m_Buildings[m_SelectedBuilding].Position = new Vector3(0f, 2.5f, 0f);
      }

      public void AddBuilding()
      {
         ModelEntity building = new ModelEntity(
            EntityType.Building,
            "Models/cube",
            new Material(
               new Vector4(0.0f, 0.0f, 0.0f, 1.0f),
               new Vector4(0.0f, 0.0f, 0.0f, 1.0f),
               new Vector4(0.6f, 0.6f, 0.6f, 1.0f),
               new Vector4(1.0f, 1.0f, 1.0f, 1.0f),
               14.0f
            ),
            GameConstants.DEFAULT_BUILDING_POSITION,
            true
         );
         building.Scale = Vector3.One * GameConstants.DEFAULT_CUBE_SCALE;
         building.UpdateWorld();
         building.LoadContent(m_Game.Content);
         m_Buildings.Add(building);
      }

      public void AddProjector(Rectangle bounds)
      {
         Vector3 pos = Vector3.Zero;
         switch (m_Projectors.Count)
         {
            case 0:
               pos = GameConstants.DEFAULT_PROJECTOR_POSITION1;
               break;
            case 1:
               pos = GameConstants.DEFAULT_PROJECTOR_POSITION2;
               break;
            case 2:
               pos = GameConstants.DEFAULT_PROJECTOR_POSITION3;
               break;
         }
         ProjectorComponent projector = new ProjectorComponent(
            bounds,
            pos,
            new Vector3(0.0f, 2.0f, 0.0f),
            MathHelper.ToRadians(45.0f),
            1.0f,//(float)GameConstants.WindowWidth / (float)GameConstants.WindowHeight,
            10.0f,
            30.0f,
            m_Game.Content
         );
         projector.UpdateView();
         projector.Entity = new ModelEntity(
            EntityType.Projector,
            "Models/sphere",
            new Material(
               new Vector4(0.0f, 0.0f, 0.0f, 1.0f),
               new Vector4(0.0f, 0.0f, 0.0f, 1.0f),
               new Vector4(0.8f, 0.8f, 0.8f, 1.0f),
               new Vector4(1.0f, 1.0f, 1.0f, 1.0f),
               14.0f
            ),
            projector.Position,
            true
         );
         projector.Entity.Scale = GameConstants.PROJECTOR_SCALE * Vector3.One;
         projector.Entity.UpdateWorld();
         projector.Entity.LoadContent(m_Game.Content);
         m_Projectors.Add(projector);
      }

      public void DeleteProjector(int projectorIndex)
      {
         if (m_Gizmo.SelectedID == m_Projectors[projectorIndex].Entity.ID)
         {
            m_Gizmo.DeselectAll();
         }
         m_Projectors.RemoveAt(projectorIndex);
         m_SelectedProjector = -1;
      }

      public void DeleteBuilding(int buildingIndex)
      {
         if (m_Gizmo.SelectedID == m_Buildings[buildingIndex].ID)
         {
            m_Gizmo.DeselectAll();
         }
         m_Buildings.RemoveAt(buildingIndex);
         m_SelectedBuilding = -1;
      }

      public void SelectProjector(int id)
      {
         for (int i = 0; i < m_Projectors.Count; ++i)
         {
            if (id == m_Projectors[i].Entity.ID)
            {
               m_SelectedProjector = i;
               break;
            }
         }
      }

      public void SelectBuilding(int id)
      {
         for (int i = 0; i < m_Buildings.Count; ++i)
         {
            if (id == m_Buildings[i].ID)
            {
               m_SelectedBuilding = i;
               break;
            }
         }
      }

      public void DeSelectProjector()
      {
         m_SelectedProjector = -1;
      }

      public void DeSelectBuilding()
      {
         m_SelectedBuilding = -1;
      }

      public bool IsProjectorID(int id)
      {
         for (int i = 0; i < m_Projectors.Count; ++i)
         {
            if (id == m_Projectors[i].Entity.ID)
               return true;
         }
         return false;
      }

      public bool IsBuildingID(int id)
      {
         for (int i = 0; i < m_Buildings.Count; ++i)
         {
            if (id == m_Buildings[i].ID)
               return true;
         }
         return false;
      }

      public void Resize(int dx, int dy)
      {
         m_Viewport.Width += dx;
         m_Viewport.Height += dy;
         m_Camera.AspectRatio = (float)m_Viewport.Width / (float)m_Viewport.Height;
         m_Camera.UpdateProjection();
         m_RenderTarget = new RenderTarget2D(m_Game.GraphicsDevice, m_Viewport.Width, m_Viewport.Height, true, m_Game.GraphicsDevice.DisplayMode.Format, DepthFormat.Depth24);
      }

      #endregion

      #region Public Access TV

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

      public bool RenderNormals
      {
         get { return m_RenderNormals; }
         set { m_RenderNormals = value; if (m_RenderNormals) m_Gizmo.DeselectAll(); }
      }

      public Viewport Viewport
      {
         get { return m_Viewport; }
         set { m_Viewport = value; }
      }

      public int SelectedProjector
      {
         get { return m_SelectedProjector; }
         set { m_SelectedProjector = value; }
      }

      public int SelectedBuilding
      {
         get { return m_SelectedBuilding; }
         set { m_SelectedBuilding = value; }
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
            for (int i = 0; i < m_Buildings.Count; ++i)
            {
               entities.Add(m_Buildings[i]);
            }
            for (int i = 0; i < m_Projectors.Count; ++i)
            {
               entities.Add(m_Projectors[i].Entity);
            }
            //entities.Add(m_LightEntity);
            return entities;
         }
      }

      public float ProjectorAlpha
      {
         set 
         {
            if (IsProjectorSelected)
            {
               m_Projectors[m_SelectedProjector].Alpha = value;
            }
         }
      }

      public Vector3 SelectedNormal
      {
         get { return m_SelectedNormal; }
      }

      public Vector3 CameraPosition
      {
         get { return m_Camera.Position; }
      }

      public bool IsProjectorSelected
      {
         get { return (m_SelectedProjector >= 0); }
      }

      public List<ProjectorComponent> Projectors
      {
         get { return m_Projectors; }
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

      public bool IsBuildingSelected
      {
         get { return (m_SelectedBuilding >= 0); }
      }

      public List<ModelEntity> Buildings
      {
         get { return m_Buildings; }
      }

      #endregion

   }
}

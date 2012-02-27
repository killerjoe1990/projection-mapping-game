#region File Description

//-----------------------------------------------------------------------------
// ColorPickerComponent.cs
//
// Author:          A.J. Fairfield (Adam, ajfairfi)
// Date Created:    2/24/2012
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
   class ColorPickerComponent
   {
      // Event fired when color is picked
      EventHandler m_ColorPickedEvent;
      
      // Fields required to render the color picker
      Effect m_Effect;
      VertexPositionColor[] m_QuadVertexBuffer;
      VertexPositionColor[] m_TriangleVertexBuffer;

      // Input
      bool m_Dragging;

      // Colors
      const float HUE_WHEEL_MIN = 0.7f;
      const float HUE_WHEEL_MAX = 0.9f; 
      float CHORD_LEN = (2.0f * HUE_WHEEL_MIN * (float)Math.Sin(MathHelper.TwoPi / 3.0f) / 2.0f) - 0.2f;
      const float HUE_STEP = MathHelper.TwoPi / 6.0f;
      Vector3[] m_Hue;
      float m_SelectedHue;
      Color m_SelectedColor;

      // Positioning
      Viewport m_Viewport;

      // States
      bool m_IsActive;

      public ColorPickerComponent(Rectangle bounds, GraphicsDevice graphics, ContentManager content, bool active)
      {
         // Store defaults
         m_Viewport = new Viewport(bounds);
         m_IsActive = active;
         
         // Defaults
         m_SelectedColor = Color.White;

         // Initialize effect
         m_Effect = content.Load<Effect>("Shaders/ColorPicker");
         m_QuadVertexBuffer = new VertexPositionColor[4];
         m_QuadVertexBuffer[0].Position = new Vector3(-1, -1,  0);
         m_QuadVertexBuffer[1].Position = new Vector3(-1,  1,  0);
         m_QuadVertexBuffer[2].Position = new Vector3( 1, -1,  0);
         m_QuadVertexBuffer[3].Position = new Vector3(1, 1, 0);
         m_TriangleVertexBuffer = new VertexPositionColor[4];
         m_Hue = new Vector3[7];
         m_Hue[0] = new Vector3(1, 0, 0);
         m_Hue[1] = new Vector3(1, 1, 0);
         m_Hue[2] = new Vector3(0, 1, 0);
         m_Hue[3] = new Vector3(0, 1, 1);
         m_Hue[4] = new Vector3(0, 0, 1);
         m_Hue[5] = new Vector3(1, 0, 1);
         m_Hue[6] = new Vector3(1, 0, 0);
         m_Dragging = false;
      }

      #region Input Handling

      public void HandleInput(MouseState mouseState, MouseState prevMouseState)
      {
         Vector2 mousePos = new Vector2(mouseState.X, mouseState.Y);
         mousePos.X -= m_Viewport.X;
         mousePos.Y -= m_Viewport.Y;

         m_Dragging = mouseState.LeftButton == ButtonState.Pressed;

         if (mousePos.X >= 0 && mousePos.X <= m_Viewport.Width &&
             mousePos.Y >= 0 && mousePos.Y <= m_Viewport.Height)
         {
            mousePos = ToLocalCoordinateSpace(mousePos);
            if (m_Dragging && OnHueWheel(mousePos))
            {
               m_SelectedHue = GetHueUnderMouse(mousePos);
               Vector3 c = HueToColor(m_SelectedHue);
               m_SelectedColor = VectorToColor(c);
               if (m_ColorPickedEvent != null) m_ColorPickedEvent(this, new EventArgs());
            }
            else if (m_Dragging && OnSaturationTriangle(mousePos))
            {
               Vector3 c = SaturationToColor(mousePos, m_SelectedHue);
               m_SelectedColor = VectorToColor(c);
               if (m_ColorPickedEvent != null) m_ColorPickedEvent(this, new EventArgs());
            }
         }
      }

      bool OnHueWheel(Vector2 mousePos)
      {
         float len = mousePos.Length();
         return (len > HUE_WHEEL_MIN && len < HUE_WHEEL_MAX);
      }

      bool OnSaturationTriangle(Vector2 mousePos)
      {
         CalculateTrianglePoints(m_SelectedHue);
         Vector2[] local = new Vector2[3] {
            new Vector2(m_TriangleVertexBuffer[0].Position.X, m_TriangleVertexBuffer[0].Position.Y),
            new Vector2(m_TriangleVertexBuffer[1].Position.X, m_TriangleVertexBuffer[1].Position.Y),
            new Vector2(m_TriangleVertexBuffer[2].Position.X, m_TriangleVertexBuffer[2].Position.Y)
         };
         if (Vec2Cross(local[1] - local[0], mousePos - local[0]) > 0) return false;
         if (Vec2Cross(local[2] - local[1], mousePos - local[1]) > 0) return false;
         if (Vec2Cross(local[0] - local[2], mousePos - local[2]) > 0) return false;
         return true;
      }

      float GetHueUnderMouse(Vector2 mousePos)
      {
         Vector2 mouseNorm = Vector2.Normalize(mousePos);
         Vector2 xAxis = new Vector2(1, 0);
         float dp = MathHelper.Clamp(Vector2.Dot(mouseNorm, xAxis), -1.0f, 1.0f);
         float theta = (float)Math.Acos(dp);
         if (Vec2Cross(mouseNorm, xAxis) < 0.0f)
            theta = (MathHelper.TwoPi - theta);
         if (theta < 0.0f)
            theta = (MathHelper.TwoPi + theta);
         float hue = theta / HUE_STEP;
         return hue;
      }

      Vector2 ToLocalCoordinateSpace(Vector2 coord)
      {
         coord.Y = m_Viewport.Height - coord.Y;
         coord /= new Vector2(m_Viewport.Width, m_Viewport.Height);
         coord *= 2.0f;
         coord -= Vector2.One;
         return coord;
      }

      #endregion

      #region Rendering

      public void Draw(GraphicsDevice graphics, SpriteBatch sprite)
      {
         m_Effect.CurrentTechnique = m_Effect.Techniques["DrawHueWheel"];
         m_Effect.CurrentTechnique.Passes[0].Apply();
         graphics.DrawUserPrimitives(PrimitiveType.TriangleStrip, m_QuadVertexBuffer, 0, 2);

         CalculateTrianglePoints(m_SelectedHue);
         m_Effect.Parameters["m_HuePoint"].SetValue(new Vector2(m_TriangleVertexBuffer[0].Position.X, m_TriangleVertexBuffer[0].Position.Y));
         m_Effect.Parameters["m_BlackPoint"].SetValue(new Vector2(m_TriangleVertexBuffer[1].Position.X, m_TriangleVertexBuffer[1].Position.Y));
         m_Effect.Parameters["m_WhitePoint"].SetValue(new Vector2(m_TriangleVertexBuffer[2].Position.X, m_TriangleVertexBuffer[2].Position.Y));
         m_Effect.Parameters["m_Hue"].SetValue(m_SelectedHue);
         m_Effect.CurrentTechnique = m_Effect.Techniques["DrawSaturationTriangle"];
         m_Effect.CurrentTechnique.Passes[0].Apply();
         graphics.DrawUserPrimitives(PrimitiveType.TriangleStrip, m_TriangleVertexBuffer, 0, 1);
      }

      #endregion

      #region Color Calculations

      Color VectorToColor(Vector3 c)
      {
         return new Color(c.X, c.Y, c.Z);
      }

      Vector3 HueToColor(float h)
      {
         int hue = (int)h;
         float t = h - hue;
         Vector3 c = m_Hue[hue] * (1.0f - t) + m_Hue[hue + 1] * t;
         return c;
      }

      Vector3 SaturationToColor(Vector2 mousePos, float h)
      {
         CalculateTrianglePoints(h);
         Vector3 hue = HueToColor(h);
         Vector2 huePoint = new Vector2(m_TriangleVertexBuffer[0].Position.X, m_TriangleVertexBuffer[0].Position.Y);
         Vector2 whitePoint = new Vector2(m_TriangleVertexBuffer[2].Position.X, m_TriangleVertexBuffer[2].Position.Y);
         Vector2 hueToMouse = huePoint - mousePos;
         Vector2 whiteToMouse = whitePoint - mousePos;
         float hueToMouseLen = hueToMouse.Length();
         float whiteToMouseLen = whiteToMouse.Length();

         float hue_t = MathHelper.Clamp((hueToMouseLen / CHORD_LEN) - 0.1f, 0.0f, 1.0f);
         float white_t = MathHelper.Clamp((whiteToMouseLen / CHORD_LEN) - 0.1f, 0.0f, 1.0f);

         Vector3 c = hue * (1.0f - hue_t);
         c += Vector3.One * (1.0f - white_t);
         c.X = MathHelper.Clamp(c.X, 0.0f, 1.0f);
         c.Y = MathHelper.Clamp(c.Y, 0.0f, 1.0f);
         c.Z = MathHelper.Clamp(c.Z, 0.0f, 1.0f);
         return c;
      }

      #endregion

      #region Utility

      float Vec2Cross(Vector2 l, Vector2 r)
      {
         return (r.Y * l.X) - (r.X * l.Y);
      }

      void CalculateTrianglePoints(float hue)
      {
         Vector3 p = new Vector3(HUE_WHEEL_MIN, 0.0f, 0.0f);
         m_TriangleVertexBuffer[0].Position = Vector3.Transform(p, Matrix.CreateRotationZ(-(hue * HUE_STEP)));
         m_TriangleVertexBuffer[1].Position = Vector3.Transform(p, Matrix.CreateRotationZ(-((hue + 2) * HUE_STEP)));
         m_TriangleVertexBuffer[2].Position = Vector3.Transform(p, Matrix.CreateRotationZ(-((hue + 4) * HUE_STEP)));
      }

      #endregion

      #region Public Interaction

      public void ShowPickerDialog()
      {
         if (m_IsActive) return;
         m_IsActive = true;
      }

      public void HidePickerDialog()
      {
         if (!m_IsActive) return;
         m_IsActive = false;
      }

      #endregion

      #region Public Access TV

      public Color SelectedColor
      {
         get { return m_SelectedColor; }
      }

      public bool IsActive
      {
         get { return m_IsActive; }
      }

      public Viewport Viewport
      {
         get { return m_Viewport; }
      }

      #endregion

   }
}

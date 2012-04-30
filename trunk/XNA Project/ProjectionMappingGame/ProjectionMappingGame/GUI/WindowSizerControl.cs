
#region File Description

/******************************************************************
 * Filename:        WindowSizerControl.cs
 * Author:          Adam (A.J.) Fairfield
 * 
 * Created:         3/14/2012
 *****************************************************************/

#endregion

#region Imports

// System includes
using System;
using System.Collections.Generic;
using System.Text;

// XNA includes
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

#endregion

namespace ProjectionMappingGame.GUI
{
   class WindowSizerControl
   {
      public const int TITLE_X = DISPLAY_BORDER + 2;
      public const int TITLE_Y = DISPLAY_BORDER + 2;
      public const int WIDTH_LABEL_Y = 175;
      public const int HEIGHT_LABEL_Y = WIDTH_LABEL_Y;
      public const int WIDTH_LABEL_X = 10;
      public const int HEIGHT_LABEL_X = 108;
      public const int WIDTH_SPINBOX_X = 50;
      public const int HEIGHT_SPINBOX_X = 150;
      public const int X_LABEL_Y = 200;
      public const int Y_LABEL_Y = X_LABEL_Y;
      public const int X_LABEL_X = 10;
      public const int Y_LABEL_X = 108;
      public const int X_SPINBOX_X = 50;
      public const int Y_SPINBOX_X = 150;

      const int DISPLAY_BORDER = 10;

      Rectangle m_DisplayBounds;
      Texture2D m_BackgroundTexture;
      Texture2D m_BorderTexture;
      SpriteFont m_Font;
      Label m_WidthLabel;
      Label m_HeightLabel;
      Label m_XLabel;
      Label m_YLabel;
      NumUpDown m_WidthSpinBox;
      NumUpDown m_HeightSpinBox;
      NumUpDown m_XSpinBox;
      NumUpDown m_YSpinBox;
      Viewport m_Viewport;
      MouseInput m_MouseInput;

      string m_Name;
      int m_Width;
      int m_Height;
      int m_DividesX;
      int m_DividesY;
      float m_Scale;
      float m_AspectRatio;

      public WindowSizerControl(string name, SpriteFont font, MouseInput mouse, Viewport viewport, Rectangle bounds, Texture2D backgroundTexture, Texture2D borderTexture, Label widthLabel, Label heightLabel, Label xLabel, Label yLabel, NumUpDown widthSpinBox, NumUpDown heightSpinBox, NumUpDown xSpinBox, NumUpDown ySpinBox, float scale)
      {
         m_DividesX = 1;
         m_DividesY = 1;
         m_Name = name;
         m_BackgroundTexture = backgroundTexture;
         m_BorderTexture = borderTexture;
         m_Font = font;
         m_HeightLabel = heightLabel;
         m_WidthLabel = widthLabel;
         m_WidthSpinBox = widthSpinBox;
         m_HeightSpinBox = heightSpinBox;
         m_WidthSpinBox.RegisterOnValueChanged(WidthSpinBox_OnValueChanged);
         m_HeightSpinBox.RegisterOnValueChanged(HeightSpinBox_OnValueChanged);
         m_WidthSpinBox.Value = bounds.Width;
         m_HeightSpinBox.Value = bounds.Height;
         m_XLabel = xLabel;
         m_YLabel = yLabel;
         m_XSpinBox = xSpinBox;
         m_YSpinBox = ySpinBox;
         m_XSpinBox.RegisterOnValueChanged(XSpinBox_OnValueChanged);
         m_YSpinBox.RegisterOnValueChanged(YSpinBox_OnValueChanged);
         m_XSpinBox.Value = m_DividesX;
         m_YSpinBox.Value = m_DividesY;
         m_Viewport = viewport;
         m_DisplayBounds = new Rectangle(DISPLAY_BORDER, DISPLAY_BORDER, (int)(bounds.Width * scale), (int)(bounds.Height * scale));
         m_Scale = scale;
         m_AspectRatio = m_WidthSpinBox.Value / m_HeightSpinBox.Value;
         m_Width = bounds.Width;
         m_Height = bounds.Height;

         m_MouseInput = mouse;
      }

      #region Input Handling

      public void HandleInput(float elapsed)
      {
         m_MouseInput.HandleInput(PlayerIndex.One);
         m_WidthSpinBox.Update(elapsed);
         m_HeightSpinBox.Update(elapsed);
         m_Width = (int)m_WidthSpinBox.Value;
         m_Height = (int)m_HeightSpinBox.Value;
      }

      private void WidthSpinBox_OnValueChanged(object sender, EventArgs e)
      {
         m_Width = (int)m_WidthSpinBox.Value;
         m_DisplayBounds.Width = (int)(m_WidthSpinBox.Value * m_Scale);
         m_AspectRatio = m_WidthSpinBox.Value / m_HeightSpinBox.Value;
      }

      private void HeightSpinBox_OnValueChanged(object sender, EventArgs e)
      {
         m_Height = (int)m_HeightSpinBox.Value;
         m_DisplayBounds.Height = (int)(m_HeightSpinBox.Value * m_Scale);
         m_AspectRatio = m_WidthSpinBox.Value / m_HeightSpinBox.Value;
      }

      private void XSpinBox_OnValueChanged(object sender, EventArgs e)
      {
         m_DividesX = (int)m_XSpinBox.Value;
      }

      private void YSpinBox_OnValueChanged(object sender, EventArgs e)
      {
         m_DividesY = (int)m_YSpinBox.Value;
      }

      #endregion

      #region Rendering

      public void Draw(GraphicsDevice graphics, SpriteBatch spriteBatch)
      {
         Viewport defaultViewport = graphics.Viewport;
         graphics.Viewport = m_Viewport;

         spriteBatch.Begin();
         spriteBatch.Draw(m_BackgroundTexture, m_DisplayBounds, Color.White);
         spriteBatch.Draw(m_BorderTexture, new Rectangle(m_DisplayBounds.X, m_DisplayBounds.Y, m_DisplayBounds.Width, 2), Color.White);                     // Top
         spriteBatch.Draw(m_BorderTexture, new Rectangle(m_DisplayBounds.X, m_DisplayBounds.Y, 2, m_DisplayBounds.Height), Color.White);                    // Left
         spriteBatch.Draw(m_BorderTexture, new Rectangle(m_DisplayBounds.X, m_DisplayBounds.Y + m_DisplayBounds.Height - 2, m_DisplayBounds.Width, 2), Color.White); // Bottom
         spriteBatch.Draw(m_BorderTexture, new Rectangle(m_DisplayBounds.X + m_DisplayBounds.Width - 2, m_DisplayBounds.Y, 2, m_DisplayBounds.Height), Color.White); // Right
         spriteBatch.Draw(m_BorderTexture, new Rectangle(m_DisplayBounds.X, m_DisplayBounds.Y, m_DisplayBounds.Width, 20), Color.White);
         spriteBatch.DrawString(m_Font, m_Name, new Vector2(TITLE_X, TITLE_Y), Color.White);
         m_WidthLabel.Draw(graphics, spriteBatch);
         m_HeightLabel.Draw(graphics, spriteBatch);
         m_WidthSpinBox.Draw(graphics, spriteBatch);
         m_HeightSpinBox.Draw(graphics, spriteBatch);
         m_XLabel.Draw(graphics, spriteBatch);
         m_YLabel.Draw(graphics, spriteBatch);
         m_XSpinBox.Draw(graphics, spriteBatch);
         m_YSpinBox.Draw(graphics, spriteBatch);
         spriteBatch.End();

         graphics.Viewport = defaultViewport;
      }

      #endregion

      #region Public Access TV

      public int Width
      {
         get { return m_Width; }
      }

      public int Height
      {
         get { return m_Height; }
      }

      public int DividesX
      {
         get { return m_DividesX; }
      }

      public int DividesY
      {
         get { return m_DividesY; }
      }

      public int DisplayWidth
      {
         get { return m_Viewport.Width; }
      }

      public int DisplayHeight
      {
         get { return m_Viewport.Height; }
      }

      public float AspectRatio
      {
         get { return m_AspectRatio; }
      }

      public Vector2 Location
      {
         set { m_Viewport.X = (int)value.X; m_Viewport.Y = (int)value.Y; m_MouseInput.Offset = new Vector2(-m_Viewport.X, -m_Viewport.Y); }
         get { return new Vector2(m_Viewport.X, m_Viewport.Y); }
      }

      #endregion

   }
}

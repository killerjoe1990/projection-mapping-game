using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// XNA imports
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

// Local imports
using ProjectionMappingGame.GUI;

namespace ProjectionMappingGame.Editor
{
   public enum LayerType
   {
      Gameplay,
      Texture,
      Particle
   };

   public abstract class Layer
   {
      public const int LAYER_HEIGHT = 42;
      public const int LAYER_WIDTH = 192;
      public const int LAYER_PADDING_Y = 2;
      public const int NAME_X = 2;
      public const int VISIBILITY_BUTTON_X = 2;
      public const int VISIBILITY_BUTTON_WIDTH = 16;
      public const int VISIBILITY_BUTTON_HEIGHT = 16;
      public const int EDIT_BUTTON_X = LAYER_WIDTH - EDIT_BUTTON_WIDTH - 4;
      public const int EDIT_BUTTON_WIDTH = 16;
      public const int EDIT_BUTTON_HEIGHT = 16;
      public const int WIDTH_X = 20;
      public const int WIDTH_WIDTH = 40;
      public const int HEIGHT_X = WIDTH_X + WIDTH_WIDTH + 48;
      public const int HEIGHT_WIDTH = 40;
      public const int NORMALS_X = HEIGHT_X;
      public const int LABEL_HEIGHT = 20;
      public const int WIDTH_SPINBOX_X = WIDTH_X + WIDTH_WIDTH;
      public const int HEIGHT_SPINBOX_X = HEIGHT_X + HEIGHT_WIDTH;


      // Static Identification
      public static int LAYER_COUNTER = 0;
      public int ID;
      public int TYPE_ID;

      protected string m_LayerName;
      protected bool m_IsEditable;
      protected bool m_IsSelected;
      protected bool m_IsVisible;
      protected bool m_NeedToSelectNormal;
      protected LayerType m_Type;
      protected int m_Width;
      protected int m_Height;
      protected Vector3 m_Normal;

      protected Label m_NameLabel;
      //protected Button m_VisibleButton;
      protected Button m_EditButton;
      protected Label m_WidthLabel;
      protected Label m_HeightLabel;
      protected Label m_NormalsLabel;
      protected NumUpDown m_WidthSpinBox;
      protected NumUpDown m_HeightSpinBox;

      protected Texture2D m_ColorTexture;
      protected Texture2D m_RenderTarget;

      public Layer(LayerType type, Label nameLabel, Label widthLabel, Label heightLabel, Label normalsLabel, NumUpDown widthSpinBox, NumUpDown heightSpinBox, Button editBtn, Texture2D colorTexture)
      {
         // ID the layer
         ID = LAYER_COUNTER++;

         // Store settings
         m_Type = type;
         m_NameLabel = nameLabel;
         m_WidthLabel = widthLabel;
         m_HeightLabel = heightLabel;
         m_NormalsLabel = normalsLabel;
         m_WidthSpinBox = widthSpinBox;
         m_HeightSpinBox = heightSpinBox;
         //m_VisibleButton = visibleBtn;
         m_EditButton = editBtn;
         //m_VisibleButton.RegisterOnClick(VisibleButton_OnClicked);
         m_EditButton.RegisterOnClick(EditButton_OnClicked);
         m_WidthSpinBox.RegisterOnValueChanged(WidthSpinBox_OnValueChanged);
         m_HeightSpinBox.RegisterOnValueChanged(HeightSpinBox_OnValueChanged);
         m_Width = (int)m_WidthSpinBox.Value;
         m_Height = (int)m_HeightSpinBox.Value;
         m_ColorTexture = colorTexture;

         // Defaults
         m_RenderTarget = null;
         m_IsSelected = false;
         m_IsVisible = true;
         m_NeedToSelectNormal = false;
         m_Normal = Vector3.Up;
      }

      private void WidthSpinBox_OnValueChanged(object sender, EventArgs e)
      {
         m_Width = (int)m_WidthSpinBox.Value;
      }

      private void HeightSpinBox_OnValueChanged(object sender, EventArgs e)
      {
         m_Height = (int)m_HeightSpinBox.Value;
      }

      private void VisibleButton_OnClicked(object sender, EventArgs e)
      {

      }

      private void EditButton_OnClicked(object sender, EventArgs e)
      {
         m_NeedToSelectNormal = true;
      }

      public void Draw(GraphicsDevice graphics, SpriteBatch spriteBatch)
      {
         m_NameLabel.Draw(graphics, spriteBatch);
         m_EditButton.Draw(graphics, spriteBatch);
         //m_VisibleButton.Draw(graphics, spriteBatch);
         m_WidthLabel.Draw(graphics, spriteBatch);
         m_HeightLabel.Draw(graphics, spriteBatch);
         m_NormalsLabel.Draw(graphics, spriteBatch);
         m_WidthSpinBox.Draw(graphics, spriteBatch);
         m_HeightSpinBox.Draw(graphics, spriteBatch);

         Vector3 n = m_Normal;
         n *= 0.5f;
         n += new Vector3(0.5f);
         Color normalColor = new Color(n);
         spriteBatch.Draw(m_ColorTexture, new Rectangle((int)m_EditButton.Location.X + 3, (int)m_EditButton.Location.Y + 3, m_EditButton.Width - 6, m_EditButton.Height - 6), normalColor);
      }

      #region Public Access TV

      public void Offset(Vector2 offset)
      {
         m_NameLabel.Location += offset;
         m_WidthLabel.Location += offset;
         m_HeightLabel.Location += offset;
         //m_VisibleButton.Location += offset;
         m_NormalsLabel.Location += offset;
         m_EditButton.Location += offset;
         m_WidthSpinBox.Location += offset;
         m_WidthSpinBox.Children[0].Location += offset;
         m_WidthSpinBox.Children[1].Location += offset;
         m_HeightSpinBox.Location += offset;
         m_HeightSpinBox.Children[0].Location += offset;
         m_HeightSpinBox.Children[1].Location += offset;
      }

      public Rectangle Bounds
      {
         get { return new Rectangle((int)m_NameLabel.Location.X - 2, (int)m_NameLabel.Location.Y - 2, LAYER_WIDTH, LAYER_HEIGHT); }
      }

      public Label NameLabel
      {
         get { return m_NameLabel; }
      }

      public NumUpDown WidthSpinBox
      {
         get { return m_WidthSpinBox; }
      }

      public NumUpDown HeightSpinBox
      {
         get { return m_HeightSpinBox; }
      }

      //public Button VisibleButton
      //{
      //   get { return m_VisibleButton; }
      //}

      public Button EditButton
      {
         get { return m_EditButton; }
      }

      public int Width
      {
         get { return m_Width; }
      }

      public int Height
      {
         get { return m_Height; }
      }

      public Vector3 Normal
      {
         get { return m_Normal; }
         set { m_Normal = value; }
      }

      public Texture2D RenderTarget
      {
         get { return m_RenderTarget; }
         set { m_RenderTarget = value; }
      }

      public LayerType Type
      {
         get { return m_Type; }
         set { m_Type = value; }
      }

      public bool Visible
      {
         get { return m_IsVisible; }
         set { m_IsVisible = value; }
      }

      public bool Selected
      {
         get { return m_IsSelected; }
         set { m_IsSelected = value; }
      }

      public bool Editable
      {
         get { return m_IsEditable; }
         set { m_IsEditable = value; }
      }

      public bool NeedToSelectNormal
      {
         get { return m_NeedToSelectNormal; }
         set { m_NeedToSelectNormal = value; }
      }

      public string Name
      {
         get { return m_LayerName; }
         set { 
            m_LayerName = value;
         m_NameLabel.Text = m_LayerName;
         }
      }

      #endregion

   }

   public class GameplayLayer : Layer
   {
      // Static Identification
      public static int TYPE_COUNTER = 0;

      public GameplayLayer(Label nameLabel, Label widthLabel, Label heightLabel, Label normalsLabel, NumUpDown widthSpinBox, NumUpDown heightSpinBox, Button editBtn, Texture2D colorTexture)
         : base(LayerType.Gameplay, nameLabel, widthLabel, heightLabel, normalsLabel, widthSpinBox, heightSpinBox, editBtn, colorTexture)
      {
         TYPE_ID = TYPE_COUNTER++;
         m_IsEditable = false;
         //m_LayerName = LayerType.Gameplay.ToString() + " " + (TYPE_ID + 1).ToString();
         //m_NameLabel.Text = m_LayerName;
      }
   }

   /*
   public class TextureLayer : Layer
   {
      // Static Identification
      public static int TYPE_COUNTER = 0;

      public TextureLayer(Label nameLabel, Button visibleBtn, Button editBtn)
         : base(LayerType.Texture, nameLabel, visibleBtn, editBtn)
      {
         TYPE_ID = TYPE_COUNTER++;
         m_IsEditable = true;
         m_LayerName = LayerType.Texture.ToString() + " " + TYPE_ID.ToString();
      }
   }

   public class ParticleLayer : Layer
   {
      // Static Identification
      public static int TYPE_COUNTER = 0;

      public ParticleLayer(Label nameLabel, Button visibleBtn, Button editBtn)
         : base(LayerType.Particle, nameLabel, visibleBtn, editBtn)
      {
         TYPE_ID = TYPE_COUNTER++;
         m_IsEditable = true;
         m_LayerName = LayerType.Particle.ToString() + " " + TYPE_ID.ToString();
      }
   }*/
}

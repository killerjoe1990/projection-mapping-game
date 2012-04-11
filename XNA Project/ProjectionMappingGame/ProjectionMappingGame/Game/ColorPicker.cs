using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace ProjectionMappingGame.Game
{
    struct GameColor
    {
        public bool Available
        {
            get;
            set;
        }
        public Color Color
        {
            get;
            set;
        }
    }
    public class ColorPicker
    {
        GameColor[] m_Colors;

        public ColorPicker(Color[] colors)
        {
            m_Colors = new GameColor[colors.Length];

            for (int i = 0; i < m_Colors.Length; ++i)
            {
                //m_Colors[i] = new GameColor();
                m_Colors[i].Color = colors[i];
                m_Colors[i].Available = true;
            }
        }

        public void ReturnColor(Color color)
        {
            for(int i = 0; i < m_Colors.Length; ++i)
            {
                if (m_Colors[i].Color == color)
                {
                    m_Colors[i].Available = true;
                }
            }
        }

        public Color TryColor(Color color)
        {
            for (int i = 0; i < m_Colors.Length; ++i)
            {
                if (m_Colors[i].Color == color)
                {
                    if (m_Colors[i].Available)
                    {
                        m_Colors[i].Available = false;
                        return color;
                    }
                    else
                    {
                        return GetNextColor(color);
                    }
                }
            }

            return Color.Black;
        }

        public Color GetNextColor(Color color)
        {
            for (int i = 0; i < m_Colors.Length; ++i)
            {
                if (m_Colors[i].Color == color)
                {
                    int j = i;
                    do
                    {
                        j = (j + 1) % m_Colors.Length;

                        if (j == i)
                        {
                            return Color.Black;
                        }
                    }
                    while (!m_Colors[j].Available);

                    m_Colors[j].Available = false;
                    return m_Colors[j].Color;
                }
            }

            return Color.Black;
        }

        public Color GetLastColor(Color color)
        {
            for (int i = 0; i < m_Colors.Length; ++i)
            {
                if (m_Colors[i].Color == color)
                {

                    int j = i;
                    do
                    {
                        j = (j - 1);

                        if (j < 0)
                        {
                            j = m_Colors.Length - 1;
                        }

                        if (j == i)
                        {
                            return Color.Black;
                        }
                    }
                    while (!m_Colors[j].Available);

                    m_Colors[j].Available = false;
                    return m_Colors[j].Color;
                }
            }

            return Color.Black;
        }

        public void Reset()
        {
            for (int i = 0; i < m_Colors.Length; ++i)
            {
                m_Colors[i].Available = true;
            }
        }
    }
}

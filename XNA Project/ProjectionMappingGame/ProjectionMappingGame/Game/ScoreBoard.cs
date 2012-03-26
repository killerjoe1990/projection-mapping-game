﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace ProjectionMappingGame.Game
{
    class ScoreBoard
    {
        GameDriver m_Game;
        Texture2D m_RenderTargetTexture;
        Viewport m_Viewport;
        RenderTarget2D m_RenderTarget;

        public ScoreBoard(GameDriver gameDriver,int x,int y,int w, int h)
        {
            this.m_Game = gameDriver;
            // Initialize viewport
            this.m_Viewport = new Viewport(x, y, w, h);
        }

        public void Update()
        {

        }

        public void DrawRenderTarget(SpriteBatch spriteBatch, Player[] players)
        {
            Color clear = new Color(0.0f, 0.0f, 0.0f, 0.0f);

            // Render the quads into the render target
            m_Game.GraphicsDevice.SetRenderTarget(m_RenderTarget);
            m_Game.GraphicsDevice.Clear(clear);

            for (int i = 0; i < players.Length; i++)
            {
                if (players[i].State == Player.States.PLAYING)
                {
                    players[i].HUD.DrawWithNoCharSelection(spriteBatch, players[i].GetAnimation().getColor());
                }
                else if (players[i].State == Player.States.SPAWNING)
                {
                    players[i].HUD.DrawWithCharSelection(spriteBatch, players[i].GetAnimation().getColor());
                }
                else if (players[i].State == Player.States.PORTING)
                {
                    players[i].HUD.DrawWithNoCharSelection(spriteBatch, players[i].GetAnimation().getColor());
                }
                else if (players[i].State == Player.States.DEAD)
                {

                }
            }

            m_Game.GraphicsDevice.SetRenderTarget(null);
            m_Game.GraphicsDevice.Clear(Color.Black);
            m_Game.GraphicsDevice.Viewport = m_Viewport;
            m_RenderTargetTexture = (Texture2D)m_RenderTarget;
        }

        public void Draw(SpriteBatch spriteBatch, Player[] players)
        {
            spriteBatch.Begin();

            for (int i = 0; i < players.Length; i++)
            {
                if (players[i].State == Player.States.PLAYING)
                {
                    players[i].HUD.DrawWithNoCharSelection(spriteBatch, players[i].GetAnimation().getColor());
                }
                else if (players[i].State == Player.States.SPAWNING)
                {
                    players[i].HUD.DrawWithCharSelection(spriteBatch, players[i].GetAnimation().getColor());
                }
                else if (players[i].State == Player.States.PORTING)
                {
                    players[i].HUD.DrawWithNoCharSelection(spriteBatch, players[i].GetAnimation().getColor());
                }
                else if (players[i].State == Player.States.DEAD)
                {

                }
            }

            spriteBatch.End();
        }
    }
}

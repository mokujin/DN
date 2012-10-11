﻿using Blueberry.Graphics;
using Blueberry.Graphics.Fonts;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Blueberry.Diagnostics
{
    public class DiagnosticsCenter : IDiagnosable
    {
        private static DiagnosticsCenter instance;
        public static DiagnosticsCenter Instance
        {
            get
            {
                if (instance == null)
                    instance = new DiagnosticsCenter();
                return instance;
            }
        }

        List<IDiagnosable> objects;
        StringBuilder buffer;

        int fps, ups;
        int fps_counter, ups_counter;
        float update_time_counter;
        float draw_time_counter;
        BitmapFont font;
        RectangleF drawArea;

        public DiagnosticsCenter()
        {
            objects = new List<IDiagnosable>();
            Add(this);
            buffer = new StringBuilder(1024);
            font = new BitmapFont(new Font("Consolas", 14));
            Init();
        }

        public void Add(IDiagnosable obj)
        {
            if (!objects.Contains(obj))
                objects.Add(obj);
        }
        public void Remove(IDiagnosable obj)
        {
            objects.Remove(obj);
        }

        private void Init()
        {
            instance = this;
            drawArea.Width = 400;
        }
        public void UpdateBuffer()
        {
            buffer.Clear();
            int maxW = 0;
            for (int i = 0; i < objects.Count; i++)
            {
                buffer.Append("["+objects[i].DebugName+"]");
                buffer.Append(';');
                string s = objects[i].DebugInfo();
                if(s.Length > maxW) maxW = s.Length;
                buffer.Append(s);
            }
            drawArea.Height = (int)(font.MonoSpaceWidth * maxW + 20);
            if (state == PanelState.hide)
                drawArea.Y = -drawArea.Height - 10;
        }
        public void Update(float dt)
        {
            
            update_time_counter += dt;
            ups_counter++;
            if (update_time_counter >= 1)
            {
                update_time_counter -= 1;
                ups = ups_counter;
                ups_counter = 0;
            }
            UpdateBuffer();
            if (state == PanelState.showing)
            {
                if(Math.Abs(drawArea.Y - 10) < 0.1f)
                {
                    drawArea.Y = 10;
                    state = PanelState.shown;
                }
                else
                {
                    drawArea.Y = MathUtils.Lerp(drawArea.Y, 10, dt*8);
                }
            }
            if(state == PanelState.hiding)
            {
                if (Math.Abs(drawArea.Y + drawArea.Height + 10) < 0.1f)
                {
                    drawArea.Y = -drawArea.Height - 10;
                    state = PanelState.hide;
                }
                else
                {
                    drawArea.Y = MathUtils.Lerp(drawArea.Y, -drawArea.Height - 10, dt*8);
                }
            }
        }

        private enum PanelState { shown, hide, showing, hiding }
        private PanelState state = PanelState.hide;
        public bool Visible { get { return state == PanelState.shown; } }
        public void Show()
        {
            if (state == PanelState.hide)
            {
                state = PanelState.showing;
            }
        }
        public void Hide()
        {
            if (state == PanelState.shown)
            {
                state = PanelState.hiding;
            }
        }

        
        public void Draw(float dt)
        {
            draw_time_counter += dt;
            fps_counter++;
            if (draw_time_counter >= 1f)
            {
                draw_time_counter -= 1f;
                fps = fps_counter;
                fps_counter = 0;
            }
            if (state == PanelState.hide) return;

            SpriteBatch.Instance.Begin();
            SpriteBatch.Instance.FillRectangle(drawArea, new OpenTK.Graphics.Color4(0, 0, 0, 0.3f));

            int start = 0, line = 0;
            for (int i = 0; i < buffer.Length; i++)
            {
                if (buffer[i] == ';')
                {
                    SpriteBatch.Instance.PrintText(font, buffer.ToString(start, i - start), drawArea.Left + 10, drawArea.Top + 10 + line * font.LineSpacing, Color.White);
                    line++;
                    start = ++i;
                }
            }
            SpriteBatch.Instance.End();

        }

        public void DebugAction()
        {
            
        }

        public string DebugInfo()
        {
            return "FPS: " + fps + ";UPS: " + ups + ";";
        }

        public string DebugName
        {
            get { return "General"; }
        }
    }
}
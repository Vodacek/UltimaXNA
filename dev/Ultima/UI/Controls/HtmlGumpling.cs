﻿/***************************************************************************
 *   HtmlGumpling.cs
 *   Copyright (c) 2015 UltimaXNA Development Team
 * 
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region using
using Microsoft.Xna.Framework;
using System;
using UltimaXNA.Core.Graphics;
using UltimaXNA.Core.Input.Windows;
using UltimaXNA.Core.UI;
using UltimaXNA.Core.UI.HTML;
using UltimaXNA.Ultima.UI.Interfaces;
#endregion

namespace UltimaXNA.Ultima.UI.Controls
{
    public class HtmlGumpling : AControl
    {
        // private variables
        private IScrollBar m_Scrollbar;
        private RenderedText m_RenderedText;
        private bool m_IsMouseDown = false;
        private int m_MouseDownHREF = -1;
        private int m_MouseOverHREF = -1;
        // public variables
        public int ScrollX = 0;
        public int ScrollY = 0;
        // public events
        public Action<string> OnDragHRef;

        public string Text
        {
            get { return m_RenderedText.Text; }
            set { m_RenderedText.Text = value; }
        }

        public int Hue
        {
            get;
            set;
        }

        public bool HasBackground
        {
            get;
            set;
        }

        public bool HasScrollbar
        {
            get;
            set;
        }

        public bool UseFlagScrollbar
        {
            get;
            set;
        }

        public override int Width
        {
            get
            {
                return base.Width;
            }
            set
            {
                if (value != base.Width)
                {
                    base.Width = value;
                }
            }
        }

        public HtmlGumpling(AControl owner, string[] arguements, string[] lines)
            : base(owner)
        {
            int x, y, width, height, textIndex, background, scrollbar;
            x = Int32.Parse(arguements[1]);
            y = Int32.Parse(arguements[2]);
            width = Int32.Parse(arguements[3]);
            height = Int32.Parse(arguements[4]);
            textIndex = Int32.Parse(arguements[5]);
            background = Int32.Parse(arguements[6]);
            scrollbar = Int32.Parse(arguements[7]);

            buildGumpling(x, y, width, height, background, scrollbar, "<font color=#000>" + lines[textIndex]);
        }

        public HtmlGumpling(AControl owner, int x, int y, int width, int height, int background, int scrollbar, string text)
            : base(owner)
        {
            buildGumpling(x, y, width, height, background, scrollbar, text);
        }

        void buildGumpling(int x, int y, int width, int height, int background, int scrollbar, string text)
        {
            Position = new Point(x, y);
            base.Width = width;
            Size = new Point(width, height);
            HasBackground = (background == 1) ? true : false;
            HasScrollbar = (scrollbar != 0) ? true : false;
            UseFlagScrollbar = (HasScrollbar && scrollbar == 2) ? true : false;
            m_RenderedText = new RenderedText(text, width - (HasScrollbar ? 15 : 0) - (HasBackground ? 8 : 0));

            if (HasBackground)
            {
                this.AddControl(new ResizePic(this, 0, 0, 0x2486, Width - (HasScrollbar ? 15 : 0), Height));
                LastControl.HandlesMouseInput = false;
            }

            if (HasScrollbar)
            {
                if (UseFlagScrollbar)
                    AddControl(new ScrollFlag(this));
                else
                    AddControl(new ScrollBar(this));
                m_Scrollbar = LastControl as IScrollBar;
                m_Scrollbar.Position = new Point(Width - 14, 0);
                m_Scrollbar.Height = Height;
                m_Scrollbar.MinValue = 0;
                m_Scrollbar.MaxValue = m_RenderedText.Height - Height + (HasBackground ? 8 : 0);
                ScrollY = m_Scrollbar.Value;
            }
        }

        public override void Update(double totalMS, double frameMS)
        {
            m_MouseOverHREF = -1; // this value is changed every frame if we mouse over a region.

            HandlesMouseInput = (m_RenderedText.Regions.Count > 0);

            if (HasScrollbar)
            {
                m_Scrollbar.Height = Height;
                m_Scrollbar.MinValue = 0;
                m_Scrollbar.MaxValue = m_RenderedText.Height - Height + (HasBackground ? 8 : 0);
                ScrollY = m_Scrollbar.Value;
            }

            base.Update(totalMS, frameMS);
        }

        public override void Draw(SpriteBatchUI spriteBatch, Point position)
        {
            base.Draw(spriteBatch, position);

            m_RenderedText.ActiveRegion = m_MouseOverHREF;
            m_RenderedText.ActiveRegion_UseDownHue = m_IsMouseDown;
            m_RenderedText.Draw(spriteBatch,
                new Rectangle(position.X + (HasBackground ? 4 : 0), position.Y + (HasBackground ? 4 : 0),
                    Width - (HasBackground ? 8 : 0), Height - (HasBackground ? 8 : 0)), ScrollX, ScrollY);
        }

        protected override bool IsPointWithinControl(int x, int y)
        {
            if (HasScrollbar)
            {
                if (m_Scrollbar.PointWithinControl(x - m_Scrollbar.Position.X,  y - m_Scrollbar.Position.Y))
                    return true;
            }

            if (m_RenderedText.Regions.Count > 0)
            {
                Region region = m_RenderedText.Regions.RegionfromPoint(new Point(x + ScrollX, y + ScrollY));
                if (region != null)
                {
                    m_MouseOverHREF = region.Index;
                    return true;
                }
            }
            return false;
        }

        protected override void OnMouseDown(int x, int y, MouseButton button)
        {
            m_IsMouseDown = true;
            m_MouseDownHREF = m_MouseOverHREF;
        }

        protected override void OnMouseUp(int x, int y, MouseButton button)
        {
            m_IsMouseDown = false;
            m_MouseDownHREF = -1;
        }

        protected override void OnMouseClick(int x, int y, MouseButton button)
        {
            if (m_MouseOverHREF != -1 && m_MouseDownHREF == m_MouseOverHREF)
            {
                if (button == MouseButton.Left)
                {
                    if (m_RenderedText.Regions.Region(m_MouseOverHREF).HREF != null)
                        ActivateByHREF(m_RenderedText.Regions.Region(m_MouseOverHREF).HREF.HREF);
                }
            }
        }

        protected override void OnMouseOver(int x, int y)
        {
            if (m_IsMouseDown && m_MouseDownHREF != -1 && m_MouseDownHREF != m_MouseOverHREF)
            {
                if (OnDragHRef != null)
                    OnDragHRef(m_RenderedText.Regions.Region(m_MouseDownHREF).HREF.HREF);
            }
        }
    }
}

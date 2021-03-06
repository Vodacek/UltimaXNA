﻿/***************************************************************************
 *   ColorPicker.cs
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UltimaXNA.Core.Graphics;
using UltimaXNA.Core.Input.Windows;
using UltimaXNA.Core.UI;
#endregion

namespace UltimaXNA.Ultima.UI.Controls
{
    class ColorPicker : AControl
    {
        protected Texture2D m_huesTexture;
        protected Texture2D m_selectedIndicator;
        protected Rectangle m_openArea;

        protected int m_hueWidth, m_hueHeight;
        protected int[] m_hues;

        protected ColorPicker m_ChildColorPicker;

        public int Index
        {
            get;
            set;
        }

        public bool IsChild = false;
        public ColorPicker Parent = null;

        public int HueValue
        {
            get { return m_hues[Index]; }
            set
            {
                for (int i = 0; i < m_hues.Length; i++)
                {
                    if (value == m_hues[i])
                    {
                        Index = i;
                        break;
                    }
                }
            }
        }

        UserInterfaceService m_UserInterface;

        public ColorPicker(AControl owner)
            : base(owner)
        {
            HandlesMouseInput = true;

            m_UserInterface = ServiceRegistry.GetService<UserInterfaceService>();
        }

        public ColorPicker(AControl owner, Rectangle area, int swatchWidth, int swatchHeight, int[] hues)
            : this(owner)
        {
            buildGumpling(area, swatchWidth, swatchHeight, hues);
        }

        public ColorPicker(AControl owner, Rectangle closedArea, Rectangle openArea, int swatchWidth, int swatchHeight, int[] hues)
            : this(owner)
        {
            m_openArea = openArea;
            buildGumpling(closedArea, swatchWidth, swatchHeight, hues);
        }

        void buildGumpling(Rectangle area, int swatchWidth, int swatchHeight, int[] hues)
        {
            m_hueWidth = swatchWidth;
            m_hueHeight = swatchHeight;
            Position = new Point(area.X, area.Y);
            Size = new Point(area.Width, area.Height);
            m_hues = hues;
            Index = 0;
        }

        protected override void OnInitialize()
        {
            if (m_huesTexture == null)
            {
                if (IsChild) // is a child
                {
                    m_huesTexture = IO.HueData.CreateHueSwatch(m_hueWidth, m_hueHeight, m_hues);
                    m_selectedIndicator = IO.GumpData.GetGumpXNA(6000);
                }
                else
                {
                    m_huesTexture = IO.HueData.CreateHueSwatch(1, 1, new int[1] { m_hues[Index] });
                }
            }
        }

        public override void Draw(SpriteBatchUI spriteBatch, Point position)
        {
            spriteBatch.Draw2D(m_huesTexture, new Rectangle(position.X, position.Y, Width, Height), Vector3.Zero);
            if (IsChild && IsMouseOver)
            {
                spriteBatch.Draw2D(m_selectedIndicator, new Vector3(
                    (int)(X + (float)(Width / m_hueWidth) * ((Index % m_hueWidth) + 0.5f) - m_selectedIndicator.Width / 2),
                    (int)(Y + (float)(Height / m_hueHeight) * ((Index / m_hueWidth) + 0.5f) - m_selectedIndicator.Height / 2),
                    0), Vector3.Zero);
            }
            base.Draw(spriteBatch, position);
        }

        protected override void OnMouseClick(int x, int y, MouseButton button)
        {
            if (IsChild) // is a child
            {
                Parent.Index = this.Index;
                Parent.CloseChildPicker();
            }
            else
            {
                if (m_ChildColorPicker == null)
                {
                    m_ChildColorPicker = new ColorPicker(Owner, m_openArea, m_hueWidth, m_hueHeight, m_hues);
                    m_ChildColorPicker.IsChild = true;
                    m_ChildColorPicker.Parent = this;
                    Owner.AddControl(m_ChildColorPicker, this.Page);
                }
                else
                {
                    m_ChildColorPicker.Dispose();
                    m_ChildColorPicker = null;
                }

            }
        }

        protected override void OnMouseOver(int x, int y)
        {
            if (IsChild)
            {
                int clickRow = x / (Width / m_hueWidth);
                int clickColumn = y / (Height / m_hueHeight);
                Parent.Index = Index = clickRow + clickColumn * m_hueWidth;
            }
        }

        protected override void OnMouseOut(int x, int y)
        {
        }

        protected void CloseChildPicker()
        {
            if (m_ChildColorPicker != null)
            {
                m_ChildColorPicker.Dispose();
                m_ChildColorPicker = null;
                m_huesTexture = IO.HueData.CreateHueSwatch(1, 1, new int[1] { m_hues[Index] });
            }
        }
    }
}

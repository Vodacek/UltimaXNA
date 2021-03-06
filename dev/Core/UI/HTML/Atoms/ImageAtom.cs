﻿/***************************************************************************
 *   ImageAtom.cs
 *   Copyright (c) 2015 UltimaXNA Development Team
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/

namespace UltimaXNA.Core.UI.HTML.Atoms
{
    public class ImageAtom : AAtom
    {
        public Image AssociatedImage
        {
            get;
            set;
        }

        private int m_overrideWidth = -1;
        public override int Width
        {
            set
            {
                m_overrideWidth = value;
            }
            get
            {
                if (m_overrideWidth != -1)
                    return m_overrideWidth;
                return AssociatedImage.Texture.Width;
            }
        }

        private int m_overrideHeight = -1;
        public override int Height
        {
            set
            {
                m_overrideHeight = value;
            }
            get
            {
                if (m_overrideHeight != -1)
                    return m_overrideHeight;
                return AssociatedImage.Texture.Height;
            }
        }

        public ImageAtom(StyleState style)
            : base(style)
        {

        }

        public override string ToString()
        {
            return string.Format("<gImg {0}>", Style.GumpImgSrc);
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UltimaXNA.UltimaData;

namespace UltimaXNA.Entity
{
    public class StaticItem : Item
    {
        public int SortInfluence = 0;

        public StaticItem(int itemID, int sortInfluence)
            : base(Serial.Null)
        {
            ItemID = itemID;
            SortInfluence = sortInfluence;
        }
    }
}
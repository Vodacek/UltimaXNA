﻿/***************************************************************************
 *   ClintSort.cs
 *   Based on code from ClintXNA's renderer.
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using System.Collections.Generic;
using UltimaXNA.Entity;
using UltimaXNA.Entity.EntityViews;
#endregion

namespace UltimaXNA.UltimaWorld.Model
{
    class ClintSort
    {
        public static void Sort(List<AEntity> items)
        {
            for (int i = 0; i < items.Count - 1; i++)
            {
                int j = i + 1;

                while (j > 0)
                {
                    int result = Compare(items[j - 1], items[j]);
                    if (result > 0)
                    {
                        AEntity temp = items[j - 1];
                        items[j - 1] = items[j];
                        items[j] = temp;

                    }
                    j--;
                }
            }
        }

        public static int Compare(AEntity x, AEntity y)
        {
            int result = InternalGetSortZ(x) - InternalGetSortZ(y);

            if (result == 0)
                result = InternalGetTypeSortValue(x) - InternalGetTypeSortValue(y);

            return result;
        }

        private static int InternalGetSortZ(AEntity entity)
        {
            int sort = entity.GetView().SortZ;
            if (entity is Ground)
                sort--;
            else if (entity is Item)
            {
                UltimaData.ItemData itemdata = ((Item)entity).ItemData;
                if (!itemdata.IsBackground)
                    sort++;
                if (!(itemdata.Height == 0))
                    sort++;
                if (itemdata.IsSurface)
                    sort--;
            }
            else if (entity is Mobile)
            {
                sort ++;
            }
            return sort;
        }

        private static int InternalGetTypeSortValue(AEntity mapobject)
        {
            if (mapobject is Ground)
                return 0;
            else if (mapobject is StaticItem)
                return 1;
            else if (mapobject is Item)
                return 2;
            else if (mapobject is Mobile)
                return 3;
            //else if (type == typeof(MapObjectText))
            //    return 4;
            //else if (type == typeof(MapObjectDynamic))
            //    return 5;
            return -100;
        }
    }
}
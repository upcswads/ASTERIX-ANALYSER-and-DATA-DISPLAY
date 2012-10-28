/*
 * ge-kml: A .NET 2.0 class library that implements the Google Earth 2.1 API
 * Copyright (C) 2006  Ryan M. Johnston (SourceForge-boseefus00001)
 * Questions, Comments, Praise, and Flame can be sent to ryan-gekml@homieshouse.com
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
 */

// **** Modification History **** //
/*
 * 2006-12-19: boseefus00001 - Initial release
 * 
 */

using System;
using System.Collections.Generic;
using System.Xml;

namespace Google.KML
{
    /// <summary>
    /// A container for one or more geometry objects
    /// </summary>
    public class geMultiGeometry : geGeometry
    {
        public List<geGeometry> Geometries = new List<geGeometry>();

        /// <summary>
        /// Renders the object to KML
        /// </summary>
        /// <param name="kml"></param>
        public override void ToKML(XmlTextWriter kml)
        {
            kml.WriteStartElement("MultiGeometry");
            if ((ID != null) && (ID.Length > 0))
                kml.WriteAttributeString("id", ID);
            
            foreach (geGeometry geo in Geometries)
            {
                geo.ToKML(kml);
            }
            kml.WriteEndElement();
        }
    }
}

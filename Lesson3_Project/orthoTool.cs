//****** Author: Michael Bowser   
//******** Date: 2/7/2017 

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Desktop.AddIns;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Geodatabase;
using System.Windows.Forms;
using ESRI.ArcGIS.DataSourcesFile;
using ESRI.ArcGIS.DataSourcesRaster;


namespace Lesson3_Project
{
    public class orthoTool : ESRI.ArcGIS.Desktop.AddIns.Tool
    {
        IMxDocument m_pMxDoc;
        IMap m_pMap;
        IEnumLayer m_pLayers;
        ILayer m_pLoopLayer;
        IFeatureLayer m_pOrthoIndexLayer;

        protected override void OnMouseUp(MouseEventArgs arg)
        {
            try
            {
                IPoint pPoint; //Point object for clicked location
                pPoint = m_pMxDoc.CurrentLocation;

                //Create string variable to hold the field name in the ortho index
                string orthoField = "ORTHOID";

                //Call method to get value of clicked index
                string strFieldValue;
                strFieldValue = getFeatureVal.featureUtility(pPoint, m_pOrthoIndexLayer, orthoField);

                //Create a Raster Layer object
                IRasterLayer pRLayer;
                pRLayer = new RasterLayer();               

                //Set the path to the selected image
                pRLayer.CreateFromFilePath("C:/Users/micha/Documents/GEOG 489/Lesson_3/Data/orthos/" + strFieldValue + ".tif");
                pRLayer.Name = "Tile " + strFieldValue; //Update layer name
                
                //Add the ortho image to the map
                m_pMap.AddLayer(pRLayer);

                //Move the image to the bottom in the TOC
                m_pMap.MoveLayer(pRLayer, m_pMap.LayerCount - 1);

                //Update the map contents
                m_pMxDoc.UpdateContents();
            }

            catch (System.Exception ex)
            {
                MessageBox.Show("Error: This index does not have an orthophoto");
            }
        }

        protected override void OnUpdate()
        {
            m_pMxDoc = (IMxDocument)ArcMap.Application.Document;
            m_pMap = m_pMxDoc.FocusMap;

            //** If a brand new data frame, reading Layers property will generate an error
            if (m_pMap.LayerCount > 0)
            {
                m_pLayers = m_pMap.Layers;
                m_pLayers.Reset();
                m_pLoopLayer = m_pLayers.Next();
                m_pOrthoIndexLayer = null;

                //** Loop through all layers
                while (!(m_pLoopLayer == null))
                {
                    if (m_pLoopLayer.Name == "orth_idx")
                    {
                        //** Ortho index found.  Set the ortho var, enable the tool and exit the loop
                        m_pOrthoIndexLayer = (IFeatureLayer)m_pLoopLayer;
                        Enabled = true;
                        break;
                    }
                    else
                    {
                        //** STATES not found yet.  Move to next layer
                        m_pLoopLayer = m_pLayers.Next();
                    }
                }
            }
            else
            {
                m_pOrthoIndexLayer = null;
            }

            if (m_pOrthoIndexLayer == null)
            {
                //** Ortho Index layer must not be in data frame, disable the tool
                Enabled = false;
            }
        }
    }

}

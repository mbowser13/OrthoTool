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
using ESRI.ArcGIS.Display;

namespace Lesson3_Project
{
    public class streetTool : ESRI.ArcGIS.Desktop.AddIns.Tool
    {
        //********class level variables********
        IMxDocument m_pMxDoc;
        IMap m_pMap;
        IEnumLayer m_pLayers;
        ILayer m_pLoopLayer;
        IFeatureLayer m_pStreetIndexLayer;

        protected override void OnMouseUp(MouseEventArgs arg)
        {
            try
            {
                IPoint pPoint; //Point object for clicked location
                pPoint = m_pMxDoc.CurrentLocation;

                //Create string variable to hold the field name in the ortho index
                string streetField = "NEWID";

                //Call method to get value of clicked index
                string strFieldStreet;
                strFieldStreet = getFeatureVal.featureUtility(pPoint, m_pStreetIndexLayer, streetField);

                //Create a Raster Layer object
                IFeatureLayer pFLayer;
                pFLayer = new FeatureLayer();

                //Create workspace object to hold the file location
                IWorkspaceFactory pWSFactory;
                pWSFactory = new ShapefileWorkspaceFactory();

                //Set filepath to workspace
                IWorkspace pWorkspace;
                pWorkspace = pWSFactory.OpenFromFile("C:/Users/micha/Documents/GEOG 489/Lesson_3/Data/roads/", ArcMap.Application.hWnd);

                //Create feature object
                IFeatureWorkspace pFWorkspace;
                pFWorkspace = (IFeatureWorkspace)pWorkspace;

                //Set name of feature to the selected roads name
                IFeatureClass pFClass;
                pFClass = pFWorkspace.OpenFeatureClass("roads_" + strFieldStreet + ".shp");

                pFLayer.FeatureClass = pFClass;

                //Change name of layer to "Roads - selected field"
                pFLayer.Name = "Roads - " + strFieldStreet;


                //*********Symbology for roads**************

                //Create RGB object
                IRgbColor pRGBColor;
                pRGBColor = new RgbColor();

                //Set RGB values to the new object
                pRGBColor.Red = 0;
                pRGBColor.Green = 255;
                pRGBColor.Blue = 255;

                //Create simple line object for road cl
                ISimpleLineSymbol pSimpleLine;
                pSimpleLine = new SimpleLineSymbol();

                //Set the width, style and color of the streets
                pSimpleLine.Width = 0.5;
                pSimpleLine.Style = ESRI.ArcGIS.Display.esriSimpleLineStyle.esriSLSDot;
                pSimpleLine.Color = pRGBColor;

                //Create the renderer
                ISimpleRenderer pSimpleRenderer;
                pSimpleRenderer = new SimpleRenderer();
                pSimpleRenderer.Symbol = (ISymbol)pSimpleLine;

                //Create geo feature to apply renderer
                IGeoFeatureLayer pGeoFLayer;
                pGeoFLayer = (IGeoFeatureLayer)pFLayer;

                pGeoFLayer.Renderer = (IFeatureRenderer)pSimpleRenderer;

                //Add layer to map
                m_pMap.AddLayer(pFLayer);

                //Update the map contents
                m_pMxDoc.UpdateContents();
            }

            catch (System.Exception ex)
            {
                MessageBox.Show("Error: This index does not have any streets");
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
                m_pStreetIndexLayer = null;

                //** Loop through all layers
                while (!(m_pLoopLayer == null))
                {
                    if (m_pLoopLayer.Name == "road_idx")
                    {
                        //** Ortho index found.  Set the ortho var, enable the tool and exit the loop
                        m_pStreetIndexLayer = (IFeatureLayer)m_pLoopLayer;
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
                m_pStreetIndexLayer = null;
            }

            if (m_pStreetIndexLayer == null)
            {
                //** Ortho Index layer must not be in data frame, disable the tool
                Enabled = false;
            }
        }
    }

}

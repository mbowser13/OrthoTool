//****** Author: Michael Bowser   
//******** Date: 2/7/2017   

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;

namespace Lesson3_Project
{
    class getFeatureVal
    {
        public static string featureUtility(IPoint pPoint, IFeatureLayer pFLayer, string strField)
        {
            string functionReturnValue;
            ISpatialFilter pSpatialFilter;
            pSpatialFilter = new SpatialFilter();
            pSpatialFilter.Geometry = pPoint;
            pSpatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
 
            IFeatureCursor pFeatureCursor;
            pFeatureCursor = pFLayer.Search(pSpatialFilter, true);


            IFeature pFeature;
            pFeature = pFeatureCursor.NextFeature();


            if ((pFeature != null))
            {
                functionReturnValue = pFeature.Value[pFeatureCursor.FindField(strField)].ToString();
            }
            else
            {
                functionReturnValue = "Undefined";
            }
            return functionReturnValue;
        }
    }
}

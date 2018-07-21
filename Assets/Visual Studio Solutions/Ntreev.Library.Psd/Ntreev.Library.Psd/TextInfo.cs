using Ntreev.Library.Psd.Structures;
using System.Collections;
using System;

namespace Ntreev.Library.Psd
{
    public class TextInfo
    {
        public string text;
        public float[] color;
        public int fontSize;
        public string fontName;
        public TextInfo(DescriptorStructure text)
        {
            this.text = text["Txt"].ToString();
            //颜色路径EngineData/EngineDict/StyleRun/RunArray/StyleSheet/StyleSheetData/FillColor
            //UnityEngine.Debug.Log(Newtonsoft.Json.JsonConvert.SerializeObject(text));
            var engineData = text["EngineData"] as StructureEngineData;
            var engineDict = engineData["EngineDict"] as Properties;

            var stylerun = engineDict["StyleRun"] as Properties;
            var runarray = stylerun["RunArray"] as ArrayList;
            var styleSheet = (runarray[0] as Properties)["StyleSheet"] as Properties;
            var styleSheetsData = (styleSheet as Properties)["StyleSheetData"] as Properties;
            if (styleSheetsData.Contains("FontSize"))
            {
                fontSize = (int)(System.Single)styleSheetsData["FontSize"];
            }
            if (styleSheetsData.Contains("Font"))
            {
                fontName = styleSheetsData["Font"] as string;
            }

            if (styleSheetsData.Contains("FillColor"))
            {
                var strokeColorProp = styleSheetsData["FillColor"] as Properties;
                var strokeColor = strokeColorProp["Values"] as ArrayList;
                if (strokeColor != null && strokeColor.Count >= 4)
                {
                    color = new float[] {
                        float.Parse(strokeColor[1].ToString()),
                        float.Parse(strokeColor[2].ToString()),
                        float.Parse(strokeColor[3].ToString()),
                        float.Parse(strokeColor[0].ToString())};
                }
                else
                {
                    color =new float[4] { 0,0,0,1};
                }
            }
            else
            {
                color = new float[4] { 0, 0, 0, 1 };
            }
        }
    }
}

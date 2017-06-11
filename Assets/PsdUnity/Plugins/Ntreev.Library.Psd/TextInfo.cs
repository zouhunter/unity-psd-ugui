using Ntreev.Library.Psd.Structures;
using System.Collections;

namespace Ntreev.Library.Psd
{
    public class TextInfo
    {
        public string text;
        public UnityEngine.Color color;
        public int fontSize;
        public string fontName;
        public TextInfo(DescriptorStructure text)
        {
            this.text = text["Txt"].ToString();
            //EngineData/DocumentResources/StyleSheetSet/StyleSheetData

            var engineData = text["EngineData"] as StructureEngineData;
            var document = engineData["DocumentResources"] as Properties;
            var styleSheets = document["StyleSheetSet"] as ArrayList;
            var styleSheetsData = (styleSheets[0] as Properties)["StyleSheetData"] as Properties;
            fontSize = (int)(System.Single)styleSheetsData["FontSize"];
            var strokeColorProp = styleSheetsData["StrokeColor"] as Properties;
            var strokeColor = strokeColorProp["Values"] as ArrayList;
            UnityEngine.Debug.Log(strokeColor[0]);
            color = new UnityEngine.Color(float.Parse(strokeColor[0].ToString()), float.Parse(strokeColor[1].ToString()), float.Parse(strokeColor[2].ToString()), float.Parse(strokeColor[3].ToString()));
        }
    }
}

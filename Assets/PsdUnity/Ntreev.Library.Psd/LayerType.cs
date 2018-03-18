using System.Collections;

namespace Ntreev.Library.Psd
{
    public enum LayerType
    {
        Normal,//智能对象
        Color,//纯色块
        Text,//文字
        Group,//文件夹
        Divider,//</LayerGroup>
        Other//其他层级不支持
    }
}
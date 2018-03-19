using System.Collections;

namespace Ntreev.Library.Psd
{
    public enum LayerType
    {
        Normal,//智能对象
        Color,//纯色块
        Text,//文字
        Group,//文件夹
        Overflow,//超过6层
        Complex//复杂类型
    }
}
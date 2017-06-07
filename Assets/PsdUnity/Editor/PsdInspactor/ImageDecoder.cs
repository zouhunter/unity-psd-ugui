using System;
using UnityEngine;
using PhotoshopFile;

/// <summary>
/// Used to decode an image from a PSD layer.
/// </summary>
public static class ImageDecoder
{
    /// <summary>
    /// Decodes a <see cref="Layer"/> into a <see cref="Texture2D"/>.
    /// </summary>
    /// <param name="layer">The <see cref="Layer"/> to decode.</param>
    /// <returns>The <see cref="Texture2D"/> decoded from the layer.</returns>
    public static Texture2D DecodeImage(NodeLayer layer)
    {
        var rect = PsdUtility.GetRectByRectRange(layer.Rect);
        if (rect.width == 0 || rect.height == 0)
        {
            return null;
        }

        Texture2D texture = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.ARGB32, false);

        Color32[] colors = new Color32[(int)(rect.width * rect.height)];

        for (int y = 0; y < rect.height; ++y)
        {
            int layerRow = y * (int)rect.width;

            // we need to reverse the Y position for the Unity texture
            int textureRow = ((int)rect.height - 1 - y) * (int)rect.width;

            for (int x = 0; x < rect.width; ++x)
            {
                int layerPosition = layerRow + x;
                int texturePosition = textureRow + x;

                colors[texturePosition] = GetColor(layer, layerPosition);

                // set the alpha
                if (layer.SortedChannels.ContainsKey(-2))
                {
                    byte color = GetColor(layer.MaskData, x, y);
                    colors[texturePosition].a = (byte)(colors[texturePosition].a * color);
                }
            }
        }

        texture.SetPixels32(colors);
        return texture;
    }

    /// <summary>
    /// Gets the color at the given position in the given <see cref="Layer"/>.
    /// </summary>
    /// <param name="layer">The <see cref="Layer"/> to sample.</param>
    /// <param name="position">The position to sample.</param>
    /// <returns>The sampled color.</returns>
    private static Color32 GetColor(NodeLayer layer, int position)
    {
        Color32 baseColor = new Color32(1, 1, 1, 1);
        switch (layer.PsdFile.ColorMode)
        {
            case PsdColorMode.Grayscale:
            case PsdColorMode.Duotone:
                baseColor = new Color32(layer.SortedChannels[0].ImageData[position], layer.SortedChannels[0].ImageData[position], layer.SortedChannels[0].ImageData[position], 1);
                break;
            case PsdColorMode.Indexed:
                int index = layer.SortedChannels[0].ImageData[position];
                baseColor = new Color32(layer.PsdFile.ColorModeData[index], layer.PsdFile.ColorModeData[index + 256], layer.PsdFile.ColorModeData[index + 512], 1);
                break;
            case PsdColorMode.RGB:
                baseColor = new Color32(layer.SortedChannels[0].ImageData[position], layer.SortedChannels[1].ImageData[position], layer.SortedChannels[2].ImageData[position], 1);
                break;
            case PsdColorMode.CMYK:
                baseColor = CMYKToRGB(layer.SortedChannels[0].ImageData[position], layer.SortedChannels[1].ImageData[position], layer.SortedChannels[2].ImageData[position], layer.SortedChannels[3].ImageData[position]);
                break;
            case PsdColorMode.Multichannel:
                baseColor = CMYKToRGB(layer.SortedChannels[0].ImageData[position], layer.SortedChannels[1].ImageData[position], layer.SortedChannels[2].ImageData[position], 0);
                break;
            case PsdColorMode.Lab:
                baseColor = LabToRGB(layer.SortedChannels[0].ImageData[position], layer.SortedChannels[1].ImageData[position], layer.SortedChannels[2].ImageData[position]);
                break;
        }

        // set the alpha
        if (layer.SortedChannels.ContainsKey(-1))
        {
            baseColor.a = layer.SortedChannels[-1].ImageData[position];
        }

        return baseColor;
    }

    /// <summary>
    /// Gets the color at the given pixel position in the given mask.
    /// </summary>
    /// <param name="mask">The mask to sample.</param>
    /// <param name="x">The x position.</param>
    /// <param name="y">The y position.</param>
    /// <returns>The mask color.</returns>
    private static byte GetColor(NodeMask mask, int x, int y)
    {
        var rect = PsdUtility.GetRectByRectRange(mask.Rect);
        byte num = byte.MaxValue;
        if (mask.PositionIsRelative)
        {
            x -= (int)rect.x;
            y -= (int)rect.y;
        }
        else
        {
            x = x + (int)rect.x - (int)rect.x;
            y = y + (int)rect.y - (int)rect.y;
        }

        if (y >= 0 && (y < rect.height && x >= 0) && x < rect.width)
        {
            int index = (y * (int)rect.width) + x;
            num = index >= mask.ImageData.Length ? byte.MaxValue : mask.ImageData[index];
        }

        return num;
    }

    /// <summary>
    /// Converts Lab color to RGB color.
    /// </summary>
    /// <param name="lb">The lb channel.</param>
    /// <param name="ab">The ab channel.</param>
    /// <param name="bb">The bb channel.</param>
    /// <returns>The RGB color.</returns>
    private static Color32 LabToRGB(byte lb, byte ab, byte bb)
    {
        double num1 = lb;
        double num2 = ab;
        double num3 = bb;
        double num4 = 2.56;
        double num5 = 1.0;
        double num6 = 1.0;
        int num7 = (int)(num1 / num4);
        int num8 = (int)((num2 / num5) - 128.0);
        int num9 = (int)((num3 / num6) - 128.0);
        double x1 = (num7 + 16.0) / 116.0;
        double x2 = (num8 / 500.0) + x1;
        double x3 = x1 - (num9 / 200.0);
        double num10 = Math.Pow(x1, 3.0) <= 0.008856 ? (x1 - 0.0) / 7.787 : Math.Pow(x1, 3.0);
        double num11 = Math.Pow(x2, 3.0) <= 0.008856 ? (x2 - 0.0) / 7.787 : Math.Pow(x2, 3.0);
        double num12 = Math.Pow(x3, 3.0) <= 0.008856 ? (x3 - 0.0) / 7.787 : Math.Pow(x3, 3.0);
        return XYZToRGB(95.047 * num11, 100.0 * num10, 108.883 * num12);
    }

    /// <summary>
    /// Converts XYZ color to RGB color.
    /// </summary>
    /// <param name="x">The x channel.</param>
    /// <param name="y">The y channel.</param>
    /// <param name="z">The z channel.</param>
    /// <returns>The RGB color.</returns>
    private static Color32 XYZToRGB(double x, double y, double z)
    {
        double num1 = x / 100.0;
        double num2 = y / 100.0;
        double num3 = z / 100.0;

        double x1 = (num1 * 3.2406) + (num2 * -1.5372) + (num3 * -0.4986);
        double x2 = (num1 * -0.9689) + (num2 * 1.8758) + (num3 * 0.0415);
        double x3 = (num1 * 0.0557) + (num2 * -0.204) + (num3 * 1.057);

        double num4 = x1 <= 0.0031308 ? 12.92 * x1 : (1.055 * Math.Pow(x1, 5.0 / 12.0)) - 0.055;
        double num5 = x2 <= 0.0031308 ? 12.92 * x2 : (1.055 * Math.Pow(x2, 5.0 / 12.0)) - 0.055;
        double num6 = x3 <= 0.0031308 ? 12.92 * x3 : (1.055 * Math.Pow(x3, 5.0 / 12.0)) - 0.055;

        byte red = (byte)(num4 * 256.0);
        byte green = (byte)(num5 * 256.0);
        byte blue = (byte)(num6 * 256.0);

        if (red > byte.MaxValue)
        {
            red = byte.MaxValue;
        }

        if (green > byte.MaxValue)
        {
            green = byte.MaxValue;
        }

        if (blue > byte.MaxValue)
        {
            blue = byte.MaxValue;
        }

        return new Color32(red, green, blue, 1);
    }

    /// <summary>
    /// Converts CMYK color to RGB color.
    /// </summary>
    /// <param name="c">The c channel.</param>
    /// <param name="m">The m channel.</param>
    /// <param name="y">The y channel.</param>
    /// <param name="k">The k channel.</param>
    /// <returns>The RGB color.</returns>
    private static Color32 CMYKToRGB(byte c, byte m, byte y, byte k)
    {
        double num1 = Math.Pow(2.0, 8.0);
        double num6 = 1.0 - (c / num1);
        double num7 = 1.0 - (m / num1);
        double num8 = 1.0 - (y / num1);
        double num9 = 1.0 - (k / num1);

        byte red = (byte)((1.0 - ((num6 * (1.0 - num9)) + num9)) * byte.MaxValue);
        byte green = (byte)((1.0 - ((num7 * (1.0 - num9)) + num9)) * byte.MaxValue);
        byte blue = (byte)((1.0 - ((num8 * (1.0 - num9)) + num9)) * byte.MaxValue);

        if (red > byte.MaxValue)
        {
            red = byte.MaxValue;
        }

        if (green > byte.MaxValue)
        {
            green = byte.MaxValue;
        }

        if (blue > byte.MaxValue)
        {
            blue = byte.MaxValue;
        }

        return new Color32(red, green, blue, 1);
    }
}
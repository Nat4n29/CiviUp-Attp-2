using UnityEngine;

public static class ColorUtils
{
    /// <summary>
    /// Retorna uma versão mais clara da cor original.
    /// amount = 0 → cor original
    /// amount = 1 → branco
    /// </summary>
    public static Color Lighten(Color color, float amount = 0.3f)
    {
        amount = Mathf.Clamp01(amount);
        return Color.Lerp(color, Color.white, amount);
    }

    /// <summary>
    /// Retorna uma versão mais escura da cor original.
    /// amount = 0 → cor original
    /// amount = 1 → preto
    /// </summary>
    public static Color Darken(Color color, float amount = 0.3f)
    {
        amount = Mathf.Clamp01(amount);
        return Color.Lerp(color, Color.black, amount);
    }
}

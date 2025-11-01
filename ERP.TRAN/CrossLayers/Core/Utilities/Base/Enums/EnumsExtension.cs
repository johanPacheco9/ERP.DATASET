using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ERP.TRAN.CrossLayers.Core.Utilities.Base.Enums;

public static class EnumExtensions
{
    /// <summary>
    /// Obtiene el DisplayName del enum, si no tiene devuelve el nombre del enum
    /// </summary>
    public static string GetDisplayName(this Enum enumValue)
    {
        var field = enumValue.GetType().GetField(enumValue.ToString());
        var displayAttribute = field?.GetCustomAttribute<DisplayAttribute>();

        if (displayAttribute?.GetName() != null)
            return displayAttribute.GetName();

        var displayNameAttribute = field?.GetCustomAttribute<DisplayNameAttribute>();
        if (displayNameAttribute != null)
            return displayNameAttribute.DisplayName;

        var descriptionAttribute = field?.GetCustomAttribute<DescriptionAttribute>();
        if (descriptionAttribute != null)
            return descriptionAttribute.Description;

        return enumValue.ToString();
    }

    /// <summary>
    /// Obtiene la descripción del enum, si no tiene devuelve el nombre del enum
    /// </summary>
    public static string GetDescription(this Enum enumValue)
    {
        var field = enumValue.GetType().GetField(enumValue.ToString());
        var attribute = field?.GetCustomAttribute<DescriptionAttribute>();
        return attribute?.Description ?? enumValue.ToString();
    }

    /// <summary>
    /// Obtiene el nombre simple del enum (sin atributos)
    /// </summary>
    public static string GetName(this Enum enumValue)
    {
        return enumValue.ToString();
    }

    /// <summary>
    /// Convierte un string a enum (case insensitive)
    /// </summary>
    public static T? ToEnum<T>(this string value) where T : struct, Enum
    {
        if (Enum.TryParse<T>(value, true, out var result))
            return result;
        return null;
    }

    /// <summary>
    /// Obtiene todos los valores de un enum con sus display names
    /// </summary>
    public static Dictionary<T, string> ToDictionary<T>() where T : struct, Enum
    {
        return Enum.GetValues(typeof(T))
                  .Cast<T>()
                  .ToDictionary(k => k, v => v.GetDisplayName());
    }

    /// <summary>
    /// Obtiene lista para dropdowns (id, nombre)
    /// </summary>
    public static List<KeyValuePair<int, string>> ToSelectList<T>() where T : struct, Enum
    {
        return Enum.GetValues(typeof(T))
                  .Cast<T>()
                  .Select(e => new KeyValuePair<int, string>((int)(object)e, e.GetDisplayName()))
                  .ToList();
    }
}


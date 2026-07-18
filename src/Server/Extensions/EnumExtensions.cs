using System.ComponentModel;
using System.Reflection;

namespace MiniRedis.Commands.Extensions;

public static class EnumExtensions
{
    public static string GetDescription<T>(this T enumValue) where T : Enum
    {
        var fieldInfo = enumValue.GetType().GetField(enumValue.ToString());
        var descriptionAttribute = fieldInfo?.GetCustomAttribute<DescriptionAttribute>(false);
        return descriptionAttribute?.Description ?? enumValue.ToString().ToLower();
    }
}
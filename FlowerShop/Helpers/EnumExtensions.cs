using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace FlowerShop.Helpers;

public static class EnumExtensions
{
    public static string GetDisplayName(this Enum enumValue)
    {
        return enumValue.GetType()
            .GetMember(enumValue.ToString())[0]
            .GetCustomAttribute<DisplayAttribute>()?
            .GetName() ?? enumValue.ToString();
    }

    public static List<string> GetDisplayNames<T>() where T : Enum
    {
        return Enum.GetValues(typeof(T))
            .Cast<T>()
            .Select(e =>
            {
                var display = e.GetType()
                    .GetField(e.ToString())
                    ?.GetCustomAttribute<DisplayAttribute>();

                return display?.Name ?? e.ToString();
            })
            .ToList();
    }

}
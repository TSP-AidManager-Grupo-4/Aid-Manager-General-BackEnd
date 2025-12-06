namespace AidManager.API.Shared.Infraestructure.Interfaces.ASP.Configuration.Extensions;

using System.ComponentModel;

public static class EnumExtension
{
    public static string GetDescription(this Enum value)
    {
        var fieldInfo = value.GetType().GetField(value.ToString());
        if (fieldInfo == null)
        {
            return value.ToString();
        }

        var attributes = fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
        if (attributes.Length > 0)
        {
            return ((DescriptionAttribute)attributes[0]).Description;
        }

        return value.ToString();
    }
}
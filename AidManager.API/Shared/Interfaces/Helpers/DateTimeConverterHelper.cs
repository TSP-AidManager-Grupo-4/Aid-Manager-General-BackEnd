namespace AidManager.API.Shared.Interfaces.Helpers;

public class DateTimeConverterHelper
{
    public static DateTime ToNormalizeFormat(DateTime dateTime)
    {
        return dateTime.ToUniversalTime().ToLocalTime();
    }
    
}
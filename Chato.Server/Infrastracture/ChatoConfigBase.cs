namespace Chato.Server.Infrastracture;

public class ChatoConfigBase<TClass> where TClass : class
{
    public static string ApiName => typeof(TClass).Name;
    public string ConfigurationName => ApiName;
}

public class AppSettingConfig : ChatoConfigBase<AppSettingConfig>
{
    public string Token { get; set; }
}
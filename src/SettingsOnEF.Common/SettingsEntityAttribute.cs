namespace SettingsOnEF;

[AttributeUsage(AttributeTargets.Class)]
public class SettingsEntityAttribute : Attribute
{
    public const string IdShadowProperty = "Id";
}

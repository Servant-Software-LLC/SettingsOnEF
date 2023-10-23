namespace SettingsOnEF.Json;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class JsonDescriptionAttribute : Attribute
{
    public JsonDescriptionAttribute(string description)
    {
        Description = !string.IsNullOrEmpty(description) ? description : throw new ArgumentNullException(nameof(description));
    }

    public string Description { get; }
}

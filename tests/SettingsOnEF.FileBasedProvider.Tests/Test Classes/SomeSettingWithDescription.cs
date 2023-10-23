using SettingsOnEF.Json;

namespace SettingsOnEF.FileBasedProvider.Tests.Test_Classes;

[SettingsEntity]
public class SomeSettingWithDescription
{
    [JsonDescription("The property to beat 'em all.")]
    public string SomeProperty { get; set; }

}

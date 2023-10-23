using Data.Common.FileStatements;
using Data.Common.Interfaces;
using Data.Json.JsonIO;
using System.Data;
using System.Reflection;
using System.Text.Json;

namespace SettingsOnEF.Json;

public class JsonDataSetWriterEx : JsonDataSetWriter
{
    private readonly Dictionary<string, Type> settingsTypes;

    public JsonDataSetWriterEx(IFileConnection fileConnection, FileStatement fileStatement, Dictionary<string, Type> settingsTypes)
        : base(fileConnection, fileStatement)
    {
        this.settingsTypes = settingsTypes ?? throw new ArgumentNullException(nameof(settingsTypes));
    }

    protected override void WriteCommentValue(Utf8JsonWriter jsonWriter, DataColumn column)
    {
        //There is no way to write a description for the shadow property "Id".
        if (column.ColumnName == SettingsEntityAttribute.IdShadowProperty)
            return;

        var tableName = column.Table!.TableName;

        if (!settingsTypes.TryGetValue(tableName, out var settingsType))
            throw new Exception($"No {nameof(settingsTypes)} for data type {tableName}");

        var property = settingsType.GetProperty(column.ColumnName);
        if (property == null)
            throw new Exception($"The settings type, {settingsType.FullName} does not contain the property, {column.ColumnName}");

        //See if the settings property has the JsonDescriptionAttribute
        var jsonDescriptionAttribute = property.GetCustomAttribute<JsonDescriptionAttribute>();
        if (jsonDescriptionAttribute != null)
        {
            jsonWriter.WriteCommentValue(jsonDescriptionAttribute.Description);
        }
    }
}

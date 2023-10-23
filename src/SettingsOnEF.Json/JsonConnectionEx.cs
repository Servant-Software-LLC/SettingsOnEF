using Data.Common.FileStatements;
using Data.Common.Interfaces;
using System.Data.JsonClient;

namespace SettingsOnEF.Json;

public class JsonConnectionEx : JsonConnection
{
    private readonly Dictionary<string, Type> settingsTypes;

    public JsonConnectionEx(string connectionString, List<Type> settingsTypes)
        : base(connectionString)
    {
        this.settingsTypes = settingsTypes.ToDictionary(type => type.Name, type => type);    
    }

    protected override Func<FileStatement, IDataSetWriter> CreateDataSetWriter => fileStatement => new JsonDataSetWriterEx(this, fileStatement, settingsTypes);
}

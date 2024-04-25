using Microsoft.Extensions.Localization;
using Newtonsoft.Json;

namespace Jumia.Mvc
{
    public class JsonStringLocalizer : IStringLocalizer
    {
        private readonly JsonSerializer _serializer=new();
        public LocalizedString this[string name]
        {
            get
            {
                var value =getString(name);
                return new LocalizedString(name,value);
            }
        }

        public LocalizedString this[string name, params object[] arguments]
        {
            get
            {
                var actualValue = this[name];
                return actualValue = actualValue.ResourceNotFound ? new LocalizedString(name, string.Format(actualValue.Value, arguments))
                    : actualValue;

            }
        }

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            var fileBath = $"Resources/{Thread.CurrentThread.CurrentCulture.Name}.json";
            using FileStream stream = new(fileBath, FileMode.Open, FileAccess.Read);
            using StreamReader streamReader = new(stream);
            using JsonTextReader reader = new(streamReader);
            while(reader.Read())
            {
                if(reader.TokenType != JsonToken.PropertyName)
                {
                    continue;
                }
                else
                {
                    var key=reader.Value as string;
                    reader.Read();
                    var value = _serializer.Deserialize<string>(reader);
                    yield return new LocalizedString(key, value);
                }
            }
        }
        private string getString (string key)
        {
            //ar.eg.json
            //en.us.json
            var fileBath = $"Resources/{Thread.CurrentThread.CurrentCulture.Name}.json";
            var fullFilePath=Path.GetFullPath (fileBath);

            if(File.Exists (fullFilePath) )
            {
                var result=getVlueFromJson(key,fullFilePath);
                return result;
            }
            return string.Empty;
        }
        private string getVlueFromJson(string propertyName,string fileBath)
        {
            if(string.IsNullOrEmpty(propertyName)|| string.IsNullOrEmpty(fileBath))
                return string.Empty;
            using FileStream stream=new(fileBath,FileMode.Open,FileAccess.Read);
            using StreamReader streamReader=new(stream);
            using JsonTextReader reader=new(streamReader);

            while(reader.Read())
            {
                if (reader.TokenType == JsonToken.PropertyName && reader.Value as string == propertyName)
                {
                    reader.Read();
                    return _serializer.Deserialize<string>(reader);
                }
            }
            return string.Empty;
        }

    }
}

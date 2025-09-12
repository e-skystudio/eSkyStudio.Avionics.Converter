using AvionicConverter.Converter.BinaryNumberRepresentation;
using AvionicConverter.Converter.Models.JsonModel;
using System.Collections;
using System.Text.Json;

namespace AvionicConverter.Converter.Services;

public class ConverterManagementService
{
    private Dictionary<string, IBnrConverter> _converters = [];

    public int LoadFromJson(string path)
    {
        string data = File.ReadAllText(path);
        var convertersModels = JsonSerializer.Deserialize<List<ConverterModel>>(data);
        int count = 0;
        if (convertersModels != null)
        {
            foreach (var model in convertersModels)
            {
                var res = CreateConverterFromModel(model);
                if (res == null)  continue;
                (string name, IBnrConverter converter) = res.Value;
                _converters.Add(name, converter);
                count++;
            }
        }
        return count;
    }

    public Dictionary<string, IBnrConverter> GetAllConverters() => new(_converters);
    public IEnumerable<string> GetAllConverterNames() => _converters.Keys;
    public IBnrConverter? GetConveterByName(string name) => _converters.TryGetValue(name, out IBnrConverter? value) ? value : null;

    private (string, IBnrConverter)? CreateConverterFromModel(ConverterModel model)
    {
        if(model is BnrConverterModel bnrModel)
        {
            return (model.Name ?? "", bnrModel.ToBnrConverter());
        }
        else if(model is BnrConverterCompositeModel compositeModel)
        {
            return (model.Name ?? "", compositeModel.ToBnrCompositeConverter());
        }
        return null;
    }
}

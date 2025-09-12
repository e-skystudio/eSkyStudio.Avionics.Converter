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

    public async Task SaveToJson(string path, bool minify = true)
    {
        var stream = new FileStream(path, FileMode.OpenOrCreate);
        stream.Position = 0;

        var options = new JsonSerializerOptions
        {
            WriteIndented = !minify,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        };

        var models = _converters.Select(kv =>
        {
            if (kv.Value is BnrConverter bnrConverter)
            {
                return new BnrConverterModel
                {
                    Name = kv.Key,
                    DataBitLength = bnrConverter.DataBitLength,
                    Offset = bnrConverter.Offset,
                    IsSigned = bnrConverter.IsSigned,
                    Resolution = bnrConverter.Resolution,
                    Range = bnrConverter.Range
                } as ConverterModel;
            }
            else if (kv.Value is BnrCompositeConverter compositeConverter)
            {
                var compositeModel = new BnrConverterCompositeModel
                {
                    Name = kv.Key,
                    Converters = []
                };

                foreach(var ordered in compositeConverter.OrderedSource)
                {
                    var conveter = compositeConverter.GetConverter(ordered.Value);
                    if (conveter == null || conveter is not BnrConverter subBnrConverter)
                        return null;

                    BnrConverterModel model = new()
                    {
                        AvionicSource = new AvionicSourceModel
                        {
                            Bus = ordered.Value.Bus!.Value,
                            Label = ordered.Value.Label!.Value
                        },
                        Order = ordered.Key,
                        Name = kv.Key,
                        DataBitLength = subBnrConverter.DataBitLength,
                        Offset = subBnrConverter.Offset,
                        IsSigned = subBnrConverter.IsSigned,
                        Resolution = subBnrConverter.Resolution,
                        Range = subBnrConverter.Range
                    };
                    compositeModel.Converters.Add(model);
                }

                return compositeModel as ConverterModel;
            }
            throw new InvalidOperationException("Unknown converter type.");
        }).ToList();

        await JsonSerializer.SerializeAsync(stream, models, options);
        await stream.FlushAsync();
    }

    public void AddConverter(string name, IBnrConverter converter)
    {
        if (!_converters.ContainsKey(name))
        {
            _converters.Add(name, converter);
        }
        else
        {
            throw new ArgumentException($"Converter with name '{name}' already exists.");
        }
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

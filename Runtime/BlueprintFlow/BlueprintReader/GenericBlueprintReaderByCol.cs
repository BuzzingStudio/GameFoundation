namespace GameFoundation.BlueprintFlow.BlueprintReader
{
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using GameFoundation.BlueprintFlow.BlueprintReader.CsvHelper;
    using Sylvan.Data.Csv;

    /// <summary> An abstraction class for databases with column-based header fields </summary>
    public abstract class GenericBlueprintReaderByCol : IGenericBlueprintReader
    {
        public async Task DeserializeFromCsv(string rawCsv)
        {
            await using var csv = await CsvDataReader.CreateAsync(new StringReader(rawCsv), CsvHelper.CsvHelper.CsvDataReaderOptions);

            var allMembers = this.GetType().GetAllFieldAndProperties().ToDictionary(info => info.MemberName, info => info);

            while (await csv.ReadAsync())
            {
                if (allMembers.TryGetValue(csv.GetString(0), out var memberInfo))
                {
                    memberInfo.SetValue(this, CsvHelper.CsvHelper.TypeConverterCache.GetConverter(memberInfo.MemberType).ConvertFromString(csv.GetString(1), memberInfo.MemberType));
                }
            }
        }
    }
}
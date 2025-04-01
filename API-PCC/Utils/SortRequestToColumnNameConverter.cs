using API_PCC.ApplicationModels;
using static System.Net.Mime.MediaTypeNames;
using System.Text;

namespace API_PCC.Utils
{
    public class SortRequestToColumnNameConverter
    {
        public static void convert(SortByModel sortByModel)
        {
            if (sortByModel.Field == null)
            {
                throw new ArgumentNullException(nameof(sortByModel.Field));
            }
            if (sortByModel.Field.Length < 2)
            {
                sortByModel.Field = sortByModel.Field.ToLowerInvariant();
            }
            var sb = new StringBuilder();
            sb.Append(char.ToLowerInvariant(sortByModel.Field[0]));
            for (int i = 1; i < sortByModel.Field.Length; ++i)
            {
                char c = sortByModel.Field[i];
                if (char.IsUpper(c))
                {
                    sb.Append('_');
                    sb.Append(char.ToLowerInvariant(c));
                }
                else
                {
                    sb.Append(c);
                }
            }
            sortByModel.Field = sb.ToString();
        }
    }
}

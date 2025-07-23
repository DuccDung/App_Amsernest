namespace WebSearchLink.Service
{
    using System.Globalization;
    using System.Text;

    public static class StringHelper
    {
        public static string NormalizeVietnameseName(string input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;
            var firstPart = input.Split('|')[0].Trim();
            string noDiacritics = RemoveDiacritics(firstPart);
            return noDiacritics.ToLower().Replace(" ", "");
        }

        private static string RemoveDiacritics(string text)
        {
            var normalized = text.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();

            foreach (var c in normalized)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(c);
                }
            }

            return sb.ToString().Normalize(NormalizationForm.FormC);
        }
        public static int DetectTypeFromTopic(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return 3;

            input = input.Trim().ToLower();

            if (input.StartsWith("ac "))
                return 1;

            if (input.StartsWith("coach "))
                return 2;

            return 3;
        }
    }
}

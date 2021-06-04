/// <summary>
/// general data to string converter
/// </summary>
public static class DataStringConverter {
    /// <summary>
    /// convert long number to items singular or plural style
    /// exemple: 1 view or 2 views
    /// </summary>
    public static string GetItems(long countViews, string singularValue, string pluralValue) {
        return countViews < 1 ? "" : NumberToKMBConverter.ToKMB(countViews) + " " +
            (countViews == 1 ? singularValue : pluralValue);
    }
}

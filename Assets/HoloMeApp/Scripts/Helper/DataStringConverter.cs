public static class DataStringConverter {
    public static string GetViews(long countViews) {
        return countViews < 1 ? "" : NumberToKMBConverter.ToKMB(countViews) +
            (countViews == 1 ? " view" : " views");
    }
}

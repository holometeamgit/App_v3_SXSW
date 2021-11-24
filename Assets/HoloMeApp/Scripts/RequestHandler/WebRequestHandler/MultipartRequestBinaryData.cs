public class MultipartRequestBinaryData {
    public string FieldName;
    public byte[] Content;
    public string FileName;

    public MultipartRequestBinaryData(string fieldName, byte[] content, string fileName) {
        FieldName = fieldName;
        Content = content;
        FileName = fileName;
    }
}
using System.IO;
using System.Web;

public class CustomPostedFiles : HttpPostedFileBase
{
    private readonly byte[] fileBytes;
    MemoryStream stream;

    public CustomPostedFiles(byte[] fileBytes, string filename = null)
    {
        this.fileBytes = fileBytes;
        this.FileName = filename;
        this.ContentType = "image/png";
        this.stream = new MemoryStream(fileBytes);
    }

    public override int ContentLength => fileBytes.Length;
    public override string FileName { get; }
    public override Stream InputStream
    {
        get { return stream; }
    }
    public override string ContentType { get; }
}
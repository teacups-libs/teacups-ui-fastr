namespace WPFastr.Demo;

public static class DemoFileGenerator
{
    private static readonly string[] Extensions =
    {
        "txt",
        "pdf",
        "docx",
        "xlsx",
        "png",
        "jpg",
        "cs",
        "json",
        "zip"
    };

    private static readonly string[] NameParts =
    {
        "Budget",
        "Meeting notes",
        "Invoice",
        "Screenshot",
        "Spec",
        "Draft",
        "Report",
        "Archive",
        "Source",
        "Reference",
        "Export",
        "Plan"
    };

    public static List<FileItem> GenerateFiles(string folder)
    {
        var fileCount = Random.Shared.Next(10, 501);
        var files = new List<FileItem>(fileCount);
        var folderNumber = folder.Replace("Folder ", string.Empty, StringComparison.Ordinal);

        for (var i = 1; i <= fileCount; i++)
        {
            var extension = Extensions[Random.Shared.Next(Extensions.Length)];
            var namePart = NameParts[Random.Shared.Next(NameParts.Length)];
            var size = Random.Shared.Next(12, 50_000);
            var modified = DateTime.Today.AddDays(-Random.Shared.Next(0, 365));

            files.Add(new FileItem(
                $"{namePart} {folderNumber}-{i:000}.{extension}",
                extension.ToUpperInvariant(),
                FormatSize(size),
                modified.ToString("MMM d"),
                $"https://picsum.photos/seed/wpfastr-{folderNumber}-{i}/320/180"));
        }

        return files;
    }

    private static string FormatSize(int kilobytes)
    {
        return kilobytes < 1024
            ? $"{kilobytes} KB"
            : $"{kilobytes / 1024.0:0.#} MB";
    }
}

using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;

namespace Watermark;

public class FolderScanner(string watermarkFile)
{
    private readonly Watermark _watermark = new();
    private const string BACKUP = "Backup";

    public async Task ScanFiles(string directory)
    {
        try
        {
            // Process files in the current directory
            Directory.CreateDirectory($"{directory}/{BACKUP}/");

            foreach (var file in Directory.GetFiles(directory))
            {
                // Process the file path as needed

                var extension = Path.GetExtension(file)?.TrimStart('.').ToLowerInvariant();
                var name = Path.GetFileName(file);
                var format = GetImageFormatFromExtension(extension);

                if (format is not null)
                {

                    File.Copy(file, $"{directory}/{BACKUP}/{name}", true);

                    using var res = await _watermark.PrepareForWatermark(file,
                        watermarkFile);

                    await using var resultStream = new FileStream(file, FileMode.Create);
                    await res.SaveAsync(resultStream, format, default);

                    Console.WriteLine($"{directory}/{BACKUP}/{name}");
                }
            }

            // Recursively scan subdirectories
            foreach (var subdirectory in Directory.GetDirectories(directory))
            {
                var dirInfo = new DirectoryInfo(subdirectory);

                if (dirInfo.Name != BACKUP)
                {
                    await ScanFiles(subdirectory);
                }
            }
        }
        catch (Exception ex)
        {
            // Handle exceptions, if any
            Console.WriteLine($"Error: {ex.Message}");
            Console.WriteLine($"Error: {ex.InnerException}");
        }
    }


    private static ImageEncoder GetImageFormatFromExtension(string extension)
    {
        // Map file extensions to ImageSharp formats
        switch (extension)
        {
            case "jpg":
            case "jpeg":
                return new JpegEncoder();
            case "png":
                return new PngEncoder();
            // Add more cases as needed
            default:
                return null;
            //throw new FormatException($"This '{extension}' Format Not Supported");
        }
    }
}
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;


namespace Watermark;
public sealed class Watermark
{
    public async Task<Image> PrepareForWatermark(string fileName,string watermarkFileName)
    {
        await using var originalImageStream =
            new FileStream(fileName, FileMode.Open, FileAccess.Read);
        await using var watermarkImageStream =
            new FileStream(watermarkFileName, FileMode.Open, FileAccess.Read);

        using var original = new MemoryStream();
        using var watermark = new MemoryStream();


        await originalImageStream.CopyToAsync(original);
        await watermarkImageStream.CopyToAsync(watermark);

        originalImageStream.Close();
        watermarkImageStream.Close();

        original.Position = 0;
        watermark.Position = 0;

       // using var result = AddWatermark(original, watermark);
        //
        // var encoder = new JpegEncoder();
        //
        // using var resultStream = new MemoryStream();
        // resultStream.Position = 0;

        ////   await result.SaveAsync(resultStream, encoder);
       // await result.SaveAsync("UploadMarked.jpg");

       return AddWatermark(original, watermark);
    }

    private static Image AddWatermark(Stream originalImageStream, Stream watermarkImageStream)
    {
        var image = Image.Load(originalImageStream);
        using var watermark = Image.Load(watermarkImageStream);
        // Resize the watermark to a percentage of the original image size
        const float scale = 0.2f; // Adjust the scale as needed
        watermark.Mutate(x => x.Resize(new ResizeOptions
        {
            Size =
                new Size
                (
                    (int) ( image.Width * scale ),
                    (int) ( image.Height * scale )
                ),
            Mode = ResizeMode.Max
        }));

        // Position the watermark at the bottom-right corner with a margin
        int margin = 10; // Adjust the margin as needed
        int xPos = image.Width / 2 ;
        int yPos = image.Height / 2;

        // Draw the watermark onto the original image
        image.Mutate(x =>
            {
                x.DrawImage(watermark, new Point(xPos, yPos), 0.5f);
                x.Resize(new ResizeOptions
                {
                    Size =
                        new Size
                        (
                            (int) ( 1920 ),
                            (int) ( 1080 )
                        ),
                    Mode = ResizeMode.Max
                });
            });

        return image;
    }
}
// var b = new Watermark.Watermark();
// await b.PrepareForWatermark();


using System.Reflection.Metadata.Ecma335;
using Watermark;

var scanner = new FolderScanner("/watermark.png");


scanner.ScanFiles("/Downloads/test");


Console.ReadKey();

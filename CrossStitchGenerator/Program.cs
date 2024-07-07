using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Drawing;
using PdfSharp.Drawing;
using PdfSharp.Pdf;

class Program
{
    static readonly Dictionary<int, (Rgba32 color, char symbol)> DMCColors30 = new()
    {
        { 3713, (new Rgba32(255, 226, 226), 'A') },
        { 761, (new Rgba32(255, 201, 201), 'B') },
        { 760, (new Rgba32(245, 173, 173), 'C') },
        { 3712, (new Rgba32(241, 135, 135), 'D') },
        { 3328, (new Rgba32(227, 109, 109), 'E') },
        { 347, (new Rgba32(191, 45, 45), 'F') },
        { 817, (new Rgba32(187, 5, 31), 'G') },
        { 814, (new Rgba32(123, 0, 27), 'H') },
        { 820, (new Rgba32(14, 54, 92), 'I') },
        { 939, (new Rgba32(27, 40, 83), 'J') },
        { 310, (new Rgba32(0, 0, 0), 'K') },
        { 5200, (new Rgba32(255, 255, 255), 'L') },
        { 208, (new Rgba32(184, 121, 184), 'M') },
        { 209, (new Rgba32(204, 132, 204), 'N') },
        { 210, (new Rgba32(221, 160, 221), 'O') },
        { 211, (new Rgba32(233, 186, 233), 'P') },
        { 221, (new Rgba32(136, 62, 62), 'Q') },
        { 223, (new Rgba32(186, 91, 91), 'R') },
        { 224, (new Rgba32(255, 223, 213), 'S') },
        { 225, (new Rgba32(255, 227, 227), 'T') },
        { 300, (new Rgba32(111, 47, 0), 'U') },
        { 301, (new Rgba32(179, 95, 43), 'V') },
        { 302, (new Rgba32(255, 111, 79), 'W') },
        { 307, (new Rgba32(255, 243, 85), 'X') },
        { 309, (new Rgba32(111, 35, 42), 'Y') },
        { 311, (new Rgba32(32, 55, 125), 'Z') },
        { 312, (new Rgba32(42, 74, 137), 'a') },
        { 315, (new Rgba32(129, 58, 62), 'b') },
        { 316, (new Rgba32(183, 115, 127), 'c') },
        { 317, (new Rgba32(198, 198, 198), 'd') }
    };

    static readonly Dictionary<int, (Rgba32 color, char symbol)> DMCColors50 = new()
    {
        { 3713, (new Rgba32(255, 226, 226), 'A') },
        { 761, (new Rgba32(255, 201, 201), 'B') },
        { 760, (new Rgba32(245, 173, 173), 'C') },
        { 3712, (new Rgba32(241, 135, 135), 'D') },
        { 3328, (new Rgba32(227, 109, 109), 'E') },
        { 347, (new Rgba32(191, 45, 45), 'F') },
        { 817, (new Rgba32(187, 5, 31), 'G') },
        { 814, (new Rgba32(123, 0, 27), 'H') },
        { 820, (new Rgba32(14, 54, 92), 'I') },
        { 939, (new Rgba32(27, 40, 83), 'J') },
        { 310, (new Rgba32(0, 0, 0), 'K') },
        { 5200, (new Rgba32(255, 255, 255), 'L') },
        { 208, (new Rgba32(184, 121, 184), 'M') },
        { 209, (new Rgba32(204, 132, 204), 'N') },
        { 210, (new Rgba32(221, 160, 221), 'O') },
        { 211, (new Rgba32(233, 186, 233), 'P') },
        { 221, (new Rgba32(136, 62, 62), 'Q') },
        { 223, (new Rgba32(186, 91, 91), 'R') },
        { 224, (new Rgba32(255, 223, 213), 'S') },
        { 225, (new Rgba32(255, 227, 227), 'T') },
        { 300, (new Rgba32(111, 47, 0), 'U') },
        { 301, (new Rgba32(179, 95, 43), 'V') },
        { 302, (new Rgba32(255, 111, 79), 'W') },
        { 307, (new Rgba32(255, 243, 85), 'X') },
        { 309, (new Rgba32(111, 35, 42), 'Y') },
        { 311, (new Rgba32(32, 55, 125), 'Z') },
        { 312, (new Rgba32(42, 74, 137), 'a') },
        { 315, (new Rgba32(129, 58, 62), 'b') },
        { 316, (new Rgba32(183, 115, 127), 'c') },
        { 317, (new Rgba32(198, 198, 198), 'd') },
        { 318, (new Rgba32(171, 171, 171), 'e') },
        { 321, (new Rgba32(199, 43, 59), 'f') },
        { 322, (new Rgba32(50, 102, 124), 'g') },
        { 333, (new Rgba32(92, 84, 148), 'h') },
        { 334, (new Rgba32(115, 159, 196), 'i') },
        { 335, (new Rgba32(238, 84, 110), 'j') },
        { 336, (new Rgba32(37, 57, 77), 'k') },
        { 340, (new Rgba32(173, 167, 199), 'l') },
        { 341, (new Rgba32(173, 192, 214), 'm') },
        { 349, (new Rgba32(210, 16, 53), 'n') },
        { 350, (new Rgba32(224, 72, 72), 'o') },
        { 351, (new Rgba32(233, 106, 103), 'p') },
        { 352, (new Rgba32(253, 156, 151), 'q') },
        { 353, (new Rgba32(254, 215, 204), 'r') },
        { 355, (new Rgba32(152, 66, 54), 's') },
        { 356, (new Rgba32(197, 94, 86), 't') },
        { 367, (new Rgba32(97, 118, 82), 'u') },
        { 368, (new Rgba32(163, 186, 143), 'v') },
        { 369, (new Rgba32(215, 237, 204), 'w') }
    };

    static void Main(string[] args)
    {
        string inputDirectory = "./in/";
        string outputDirectory = "./out/";

        if (!Directory.Exists(inputDirectory))
        {
            Console.WriteLine($"Input directory '{inputDirectory}' does not exist.");
            return;
        }

        if (!Directory.Exists(outputDirectory))
        {
            Directory.CreateDirectory(outputDirectory);
        }

        foreach (string filePath in Directory.GetFiles(inputDirectory))
        {
            string extension = System.IO.Path.GetExtension(filePath).ToLower();
            if (extension == ".png" || extension == ".jpg" || extension == ".jpeg" || extension == ".webp")
            {
                Console.WriteLine($"Processing {filePath}...");
                ProcessImage(filePath, outputDirectory, DMCColors30, 80);
                ProcessImage(filePath, outputDirectory, DMCColors50, 120);
                ProcessImage(filePath, outputDirectory, DMCColors50, 180);
            }
        }

        Console.WriteLine("Processing complete.");
    }

    static void ProcessImage(string inputPath, string outputDirectory, Dictionary<int, (Rgba32 color, char symbol)> DMCColors, int size)
    {
        // Load the image
        using Image<Rgba32> originalImage = Image.Load<Rgba32>(inputPath);

        // Resize the image to the desired size
        originalImage.Mutate(x => x.Resize(size, size));

        // Convert to a limited color palette (e.g., DMC floss colors for cross-stitching)
        Image<Rgba32> patternImage = ConvertToCrossStitchPalette(originalImage, DMCColors);

        // Save the pattern image
        string patternFilePath = System.IO.Path.Combine(outputDirectory, System.IO.Path.GetFileNameWithoutExtension(inputPath) + $"_pattern_{size}.png");
        patternImage.Save(patternFilePath);

        // Create and save the grid image
        string gridFilePath = System.IO.Path.Combine(outputDirectory, System.IO.Path.GetFileNameWithoutExtension(inputPath) + $"_grid_{size}.png");
        CreateGridImage(patternImage, gridFilePath, DMCColors);

        // Create and save the legend image
        string legendFilePath = System.IO.Path.Combine(outputDirectory, System.IO.Path.GetFileNameWithoutExtension(inputPath) + $"_legend_{size}.png");
        CreateLegendImage(patternImage, legendFilePath, DMCColors);

        // Create and save the PDF
        string pdfFilePath = System.IO.Path.Combine(outputDirectory, System.IO.Path.GetFileNameWithoutExtension(inputPath) + $"_{size}.pdf");
        CreatePDF(gridFilePath, legendFilePath, pdfFilePath);
    }

    static Image<Rgba32> ConvertToCrossStitchPalette(Image<Rgba32> image, Dictionary<int, (Rgba32 color, char symbol)> DMCColors)
    {
        Image<Rgba32> result = new Image<Rgba32>(image.Width, image.Height);

        for (int y = 0; y < image.Height; y++)
        {
            for (int x = 0; x < image.Width; x++)
            {
                Rgba32 originalColor = image[x, y];
                Rgba32 closestColor = FindClosestColor(originalColor, DMCColors);
                result[x, y] = closestColor;
            }
        }

        return result;
    }

    static Rgba32 FindClosestColor(Rgba32 originalColor, Dictionary<int, (Rgba32 color, char symbol)> palette)
    {
        var closest = palette.Values.First();
        double closestDistance = ColorDistance(originalColor, closest.color);

        foreach (var entry in palette.Values)
        {
            double distance = ColorDistance(originalColor, entry.color);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closest = entry;
            }
        }

        return closest.color;
    }

    static double ColorDistance(Rgba32 c1, Rgba32 c2)
    {
        int rDiff = c1.R - c2.R;
        int gDiff = c1.G - c2.G;
        int bDiff = c1.B - c2.B;
        return Math.Sqrt(rDiff * rDiff + gDiff * gDiff + bDiff * bDiff);
    }

    static void CreateGridImage(Image<Rgba32> patternImage, string outputPath, Dictionary<int, (Rgba32 color, char symbol)> DMCColors)
    {
        int gridSize = 32;
        int width = patternImage.Width * gridSize;
        int height = patternImage.Height * gridSize;
        var font = SystemFonts.CreateFont("Arial", 24);

        using var gridImage = new Image<Rgba32>(width, height);
        gridImage.Mutate(ctx =>
        {
            for (int y = 0; y < patternImage.Height; y++)
            {
                for (int x = 0; x < patternImage.Width; x++)
                {
                    var color = patternImage[x, y];
                    var symbol = GetSymbolForColor(color, DMCColors);
                    var rect = new Rectangle(x * gridSize, y * gridSize, gridSize, gridSize);

                    ctx.Fill(color, rect);
                    ctx.DrawText(symbol.ToString(), font, new Rgba32(0, 0, 0), new PointF(x * gridSize + 8, y * gridSize + 2));
                }
            }
        });

        gridImage.Save(outputPath);
    }

    static char GetSymbolForColor(Rgba32 color, Dictionary<int, (Rgba32 color, char symbol)> DMCColors)
    {
        foreach (var entry in DMCColors.Values)
        {
            if (entry.color == color)
            {
                return entry.symbol;
            }
        }
        return ' ';
    }

    static void CreateLegendImage(Image<Rgba32> patternImage, string outputPath, Dictionary<int, (Rgba32 color, char symbol)> DMCColors)
    {
        int gridSize = 32;
        int fontSize = 24;
        int columns = 3;

        // Gather unique colors from the pattern image
        HashSet<Rgba32> uniqueColors = new HashSet<Rgba32>();
        for (int y = 0; y < patternImage.Height; y++)
        {
            for (int x = 0; x < patternImage.Width; x++)
            {
                uniqueColors.Add(patternImage[x, y]);
            }
        }

        // Filter the DMCColors dictionary to only include colors found in the image
        var filteredDMCColors = DMCColors
            .Where(entry => uniqueColors.Contains(entry.Value.color))
            .ToDictionary(entry => entry.Key, entry => entry.Value);

        int rows = (int)Math.Ceiling(filteredDMCColors.Count / (double)columns);
        int legendHeight = (rows + 1) * gridSize; // +1 for the top row
        int legendWidth = columns * 300; // Adjust the width according to the number of columns
        var font = SystemFonts.CreateFont("Arial", fontSize);

        using var legendImage = new Image<Rgba32>(legendWidth, legendHeight);
        legendImage.Mutate(ctx =>
        {
            ctx.Clear(new Rgba32(255, 255, 255)); // Set background to white
            ctx.DrawText($"Total colors needed: {filteredDMCColors.Count}", font, new Rgba32(0, 0, 0), new PointF(8, 2));

            int i = 0;
            foreach (var entry in filteredDMCColors)
            {
                int col = i % columns;
                int row = i / columns + 1; // +1 to account for the top row
                var color = entry.Value.color;
                var symbol = entry.Value.symbol;
                var rect = new Rectangle(col * 300, row * gridSize, gridSize, gridSize);

                ctx.Fill(color, rect);
                ctx.DrawText(symbol.ToString(), font, new Rgba32(0, 0, 0), new PointF(col * 300 + 8, row * gridSize + 2));
                ctx.DrawText($"{entry.Key} (#{color.ToHex()})", font, new Rgba32(0, 0, 0), new PointF(col * 300 + gridSize + 10, row * gridSize + 2));

                i++;
            }
        });

        legendImage.Save(outputPath);
    }

    static void CreatePDF(string gridFilePath, string legendFilePath, string pdfFilePath)
    {
        using var document = new PdfDocument();

        // Create temporary files for the images
        string tempGridFilePath = System.IO.Path.GetTempFileName();
        string tempLegendFilePath = System.IO.Path.GetTempFileName();

        try
        {
            // Copy the images to the temporary files
            File.Copy(gridFilePath, tempGridFilePath, true);
            File.Copy(legendFilePath, tempLegendFilePath, true);

            // Add grid image page
            PdfPage gridPage = document.AddPage();
            using (XGraphics gfx = XGraphics.FromPdfPage(gridPage))
            {
                XImage gridImage = XImage.FromFile(tempGridFilePath);
                DrawImageCentered(gfx, gridImage, gridPage.Width, gridPage.Height);
            }

            // Add legend image page
            PdfPage legendPage = document.AddPage();
            using (XGraphics gfx = XGraphics.FromPdfPage(legendPage))
            {
                XImage legendImage = XImage.FromFile(tempLegendFilePath);
                DrawImageCentered(gfx, legendImage, legendPage.Width, legendPage.Height);
            }

            // Save PDF document
            document.Save(pdfFilePath);
        }
        finally
        {
            // Clean up temporary files
            if (File.Exists(tempGridFilePath)) File.Delete(tempGridFilePath);
            if (File.Exists(tempLegendFilePath)) File.Delete(tempLegendFilePath);
        }
    }

    static void DrawImageCentered(XGraphics gfx, XImage image, double pageWidth, double pageHeight)
    {
        double imageWidth = image.PixelWidth;
        double imageHeight = image.PixelHeight;

        // Calculate scaling factor to maintain aspect ratio
        double scalingFactor = Math.Min(pageWidth / imageWidth, pageHeight / imageHeight);

        double scaledWidth = imageWidth * scalingFactor;
        double scaledHeight = imageHeight * scalingFactor;

        // Calculate offsets to center the image
        double xOffset = (pageWidth - scaledWidth) / 2;
        double yOffset = (pageHeight - scaledHeight) / 2;

        gfx.DrawImage(image, xOffset, yOffset, scaledWidth, scaledHeight);
    }

}

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
    static readonly Dictionary<int, (Rgba32 color, char symbol, string name)> DMCColors30 = new()
    {
        { 5200, (new Rgba32(255, 255, 255), 'A', "White") },
        { 150, (new Rgba32(207, 19, 19), 'B', "Red - BRIGHT") },
        { 310, (new Rgba32(0, 0, 0), 'C', "Black") },
        { 730, (new Rgba32(130, 120, 56), 'D', "Olive Green - VY DK") },
        { 732, (new Rgba32(134, 126, 54), 'E', "Olive Green") },
        { 740, (new Rgba32(255, 131, 19), 'F', "Tangerine") },
        { 742, (new Rgba32(255, 211, 118), 'G', "Tangerine - LT") },
        { 743, (new Rgba32(255, 224, 150), 'H', "Yellow - MED") },
        { 745, (new Rgba32(255, 233, 173), 'I', "Yellow - LT PALE") },
        { 823, (new Rgba32(31, 30, 51), 'J', "Blue - DK") },
        { 826, (new Rgba32(51, 91, 128), 'K', "Blue - MED") },
        { 600, (new Rgba32(222, 36, 61), 'L', "Cranberry - VY DK") },
        { 603, (new Rgba32(255, 164, 182), 'M', "Cranberry") },
        { 606, (new Rgba32(250, 50, 3), 'N', "Orange-red - BRIGHT") },
        { 608, (new Rgba32(253, 93, 53), 'O', "Orange - BRIGHT") },
        { 500, (new Rgba32(4, 57, 53), 'P', "Blue Green - VY DK") },
        { 502, (new Rgba32(91, 144, 113), 'Q', "Blue Green") },
        { 699, (new Rgba32(7, 111, 33), 'R', "Green") },
        { 701, (new Rgba32(85, 145, 70), 'S', "Green - LT") },
        { 703, (new Rgba32(123, 181, 71), 'T', "Chartreuse") },
        { 550, (new Rgba32(92, 24, 78), 'U', "Violet - VY DK") },
        { 553, (new Rgba32(176, 103, 166), 'V', "Violet") },
        { 435, (new Rgba32(184, 119, 72), 'W', "Brown - VY LT") },
        { 437, (new Rgba32(250, 206, 173), 'X', "Tan - LT") },
        { 445, (new Rgba32(255, 251, 139), 'Y', "Lemon - LT") },
        { 208, (new Rgba32(162, 89, 165), 'Z', "Lavender - VY DK") },
        { 210, (new Rgba32(195, 143, 192), 'a', "Lavender - MED") },
        { 347, (new Rgba32(191, 45, 45), 'b', "Salmon - VY DK") },
        { 350, (new Rgba32(224, 72, 72), 'c', "Coral - MED") },
        { 352, (new Rgba32(253, 156, 151), 'd', "Coral - LT") },
        { 829, (new Rgba32(126, 95, 33), 'e', "Golden Olive - VY DK") },
        { 831, (new Rgba32(141, 126, 62), 'f', "Golden Olive - MED") }
    };

    static readonly Dictionary<int, (Rgba32 color, char symbol, string name)> DMCColors64 = new()
    {
        { 5200, (new Rgba32(255, 255, 255), 'A', "White") },
        { 150, (new Rgba32(207, 19, 19), 'B', "Red - BRIGHT") },
        { 310, (new Rgba32(0, 0, 0), 'C', "Black") },
        { 730, (new Rgba32(130, 120, 56), 'D', "Olive Green - VY DK") },
        { 731, (new Rgba32(134, 124, 58), 'E', "Olive Green - DK") },
        { 732, (new Rgba32(134, 126, 54), 'F', "Olive Green") },
        { 733, (new Rgba32(159, 149, 100), 'G', "Olive Green - MED") },
        { 734, (new Rgba32(187, 179, 123), 'H', "Olive Green - LT") },
        { 740, (new Rgba32(255, 131, 19), 'I', "Tangerine") },
        { 741, (new Rgba32(255, 163, 43), 'J', "Tangerine - MED") },
        { 742, (new Rgba32(255, 211, 118), 'K', "Tangerine - LT") },
        { 743, (new Rgba32(255, 224, 150), 'L', "Yellow - MED") },
        { 744, (new Rgba32(255, 239, 172), 'M', "Yellow - PALE") },
        { 745, (new Rgba32(255, 233, 173), 'N', "Yellow - LT PALE") },
        { 823, (new Rgba32(31, 30, 51), 'O', "Blue - DK") },
        { 824, (new Rgba32(57, 105, 135), 'P', "Blue - VY DK") },
        { 825, (new Rgba32(72, 129, 161), 'Q', "Blue - DK") },
        { 826, (new Rgba32(104, 161, 194), 'R', "Blue - MED") },
        { 827, (new Rgba32(189, 221, 237), 'S', "Blue - VY LT") },
        { 828, (new Rgba32(220, 237, 249), 'T', "Blue - ULT VY LT") },
        { 600, (new Rgba32(222, 36, 61), 'U', "Cranberry - VY DK") },
        { 601, (new Rgba32(255, 84, 120), 'V', "Cranberry - DK") },
        { 602, (new Rgba32(255, 145, 172), 'W', "Cranberry - MED") },
        { 603, (new Rgba32(255, 164, 182), 'X', "Cranberry") },
        { 604, (new Rgba32(255, 194, 204), 'Y', "Cranberry - LT") },
        { 605, (new Rgba32(255, 232, 232), 'Z', "Cranberry - VY LT") },
        { 606, (new Rgba32(250, 50, 3), 'a', "Orange-red - BRIGHT") },
        { 608, (new Rgba32(253, 93, 53), 'b', "Orange - BRIGHT") },
        { 500, (new Rgba32(4, 57, 53), 'c', "Blue Green - VY DK") },
        { 501, (new Rgba32(57, 111, 100), 'd', "Blue Green - DK") },
        { 502, (new Rgba32(91, 144, 113), 'e', "Blue Green") },
        { 503, (new Rgba32(123, 172, 148), 'f', "Blue Green - MED") },
        { 504, (new Rgba32(196, 222, 204), 'g', "Blue Green - VY LT") },
        { 699, (new Rgba32(7, 111, 33), 'h', "Green") },
        { 700, (new Rgba32(0, 127, 55), 'i', "Green - BRIGHT") },
        { 701, (new Rgba32(85, 145, 70), 'j', "Green - LT") },
        { 702, (new Rgba32(0, 158, 96), 'k', "Kelly Green") },
        { 703, (new Rgba32(123, 181, 71), 'l', "Chartreuse") },
        { 704, (new Rgba32(0, 194, 108), 'm', "Chartreuse - BRIGHT") },
        { 550, (new Rgba32(92, 24, 78), 'n', "Violet - VY DK") },
        { 552, (new Rgba32(128, 58, 111), 'o', "Violet - MED") },
        { 553, (new Rgba32(176, 103, 166), 'p', "Violet") },
        { 554, (new Rgba32(228, 185, 209), 'q', "Violet - LT") },
        { 435, (new Rgba32(184, 119, 72), 'r', "Brown - VY LT") },
        { 436, (new Rgba32(198, 144, 96), 's', "Tan") },
        { 437, (new Rgba32(250, 206, 173), 't', "Tan - LT") },
        { 444, (new Rgba32(255, 239, 0), 'u', "Lemon - DK") },
        { 445, (new Rgba32(255, 251, 139), 'v', "Lemon - LT") },
        { 208, (new Rgba32(162, 89, 165), 'w', "Lavender - VY DK") },
        { 209, (new Rgba32(174, 110, 184), 'x', "Lavender - DK") },
        { 210, (new Rgba32(195, 143, 192), 'y', "Lavender - MED") },
        { 211, (new Rgba32(227, 203, 227), 'z', "Lavender - LT") },
        { 347, (new Rgba32(191, 45, 45), 'A', "Salmon - VY DK") },
        { 349, (new Rgba32(210, 16, 53), 'B', "Coral - DK") },
        { 350, (new Rgba32(224, 72, 72), 'C', "Coral - MED") },
        { 351, (new Rgba32(233, 106, 103), 'D', "Coral") },
        { 352, (new Rgba32(253, 156, 151), 'E', "Coral - LT") },
        { 353, (new Rgba32(254, 215, 204), 'F', "Peach") },
        { 829, (new Rgba32(126, 95, 33), 'G', "Golden Olive - VY DK") },
        { 830, (new Rgba32(142, 118, 68), 'H', "Golden Olive - DK") },
        { 831, (new Rgba32(141, 126, 62), 'I', "Golden Olive - MED") },
        { 832, (new Rgba32(189, 162, 107), 'J', "Golden Olive") },
        { 833, (new Rgba32(200, 174, 116), 'K', "Golden Olive - LT") },
        { 834, (new Rgba32(219, 190, 127), 'L', "Golden Olive - VY LT") }
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
                ProcessImage(filePath, outputDirectory, DMCColors64, 120);
                ProcessImage(filePath, outputDirectory, DMCColors64, 180);
            }
        }

        Console.WriteLine("Processing complete.");
    }

    static void ProcessImage(string inputPath, string outputDirectory, Dictionary<int, (Rgba32 color, char symbol, string name)> DMCColors, int size)
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
        CreateGridImage(patternImage, gridFilePath, DMCColors, outputDirectory);

        // Create and save the legend image
        string legendFilePath = System.IO.Path.Combine(outputDirectory, System.IO.Path.GetFileNameWithoutExtension(inputPath) + $"_legend_{size}.png");
        CreateLegendImage(patternImage, legendFilePath, DMCColors);

        // Create and save the PDF
        string pdfFilePath = System.IO.Path.Combine(outputDirectory, System.IO.Path.GetFileNameWithoutExtension(inputPath) + $"_{size}.pdf");
        CreatePDF(gridFilePath, legendFilePath, pdfFilePath);
    }

    static Image<Rgba32> ConvertToCrossStitchPalette(Image<Rgba32> image, Dictionary<int, (Rgba32 color, char symbol, string name)> DMCColors)
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

    static Rgba32 FindClosestColor(Rgba32 originalColor, Dictionary<int, (Rgba32 color, char symbol, string name)> palette)
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

    static void CreateGridImage(Image<Rgba32> patternImage, string outputPath, Dictionary<int, (Rgba32 color, char symbol, string name)> DMCColors, string outputDirectory)
    {
        int gridSize = 32;
        int width = patternImage.Width * gridSize;
        int height = patternImage.Height * gridSize;
        var font = SystemFonts.CreateFont("Arial", 20);

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

            // Draw center lines
            ctx.DrawLine(new Rgba32(0, 0, 0), 1, new PointF(width / 2, 0), new PointF(width / 2, height));
            ctx.DrawLine(new Rgba32(0, 0, 0), 1, new PointF(0, height / 2), new PointF(width, height / 2));
        });

        gridImage.Save(outputPath);

        // Create and save quarter images
        CreateQuarterImages(gridImage, outputDirectory);
    }

    static void CreateQuarterImages(Image<Rgba32> gridImage, string outputPath)
    {
        int halfWidth = gridImage.Width / 2;
        int halfHeight = gridImage.Height / 2;

        // Define regions for each quarter
        var regions = new[]
        {
        new Rectangle(0, 0, halfWidth, halfHeight),
        new Rectangle(halfWidth, 0, halfWidth, halfHeight),
        new Rectangle(0, halfHeight, halfWidth, halfHeight),
        new Rectangle(halfWidth, halfHeight, halfWidth, halfHeight)
    };

        // Create quarter images
        for (int i = 0; i < regions.Length; i++)
        {
            using var quarterImage = gridImage.Clone(ctx => ctx.Crop(regions[i]).Resize(regions[i].Width * 2, regions[i].Height * 2));
            quarterImage.Save(System.IO.Path.Combine(outputPath, $"quarter_{i + 1}.png"));
        }
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

            // Add quarter images
            for (int i = 0; i < 4; i++)
            {
                string quarterFilePath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(gridFilePath), $"quarter_{i + 1}.png");
                PdfPage quarterPage = document.AddPage();
                using (XGraphics gfx = XGraphics.FromPdfPage(quarterPage))
                {
                    XImage quarterImage = XImage.FromFile(quarterFilePath);
                    DrawImageCentered(gfx, quarterImage, quarterPage.Width, quarterPage.Height);
                }
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

    static char GetSymbolForColor(Rgba32 color, Dictionary<int, (Rgba32 color, char symbol, string name)> DMCColors)
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

    static void CreateLegendImage(Image<Rgba32> patternImage, string outputPath, Dictionary<int, (Rgba32 color, char symbol, string name)> DMCColors)
    {
        int gridSize = 28;
        int fontSize = 20;
        int columns = 2;

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
                var name = entry.Value.name;
                var rect = new Rectangle(col * 300, row * gridSize, gridSize, gridSize);

                ctx.Fill(color, rect);
                ctx.DrawText(symbol.ToString(), font, new Rgba32(0, 0, 0), new PointF(col * 300 + 8, row * gridSize + 2));
                ctx.DrawText($"{entry.Key} - {name}", font, new Rgba32(0, 0, 0), new PointF(col * 300 + gridSize + 10, row * gridSize + 2));

                i++;
            }
        });

        legendImage.Save(outputPath);
    }
}

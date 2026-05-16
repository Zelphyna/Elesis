#!/usr/bin/env bash
set -euo pipefail

if [[ $# -ne 1 ]]; then
  echo "Usage: scripts/create-energy-assets.sh <source-png>" >&2
  exit 1
fi

source_png="$1"
repo_root="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
dotnet_bin="${DOTNET_BIN:-/mnt/HC_Volume_105232828/shared/tools/dotnet/dotnet}"
tool_dir="${TMPDIR:-/tmp}/sts2-template-energy-assets"

if [[ ! -f "$source_png" ]]; then
  echo "Source image not found: $source_png" >&2
  exit 1
fi

mkdir -p "$tool_dir"

cat > "$tool_dir/sts2-template-energy-assets.csproj" <<'CSProj'
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net9.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="SixLabors.ImageSharp" Version="3.1.11" />
  </ItemGroup>
</Project>
CSProj

cat > "$tool_dir/Program.cs" <<'CS'
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

if (args.Length != 3)
{
    Console.Error.WriteLine("Usage: <source-png> <big-output-png> <text-output-png>");
    return 1;
}

var sourcePath = args[0];
var bigOutputPath = args[1];
var textOutputPath = args[2];

using var source = Image.Load<Rgba32>(sourcePath);
using var keyed = FloodRemovePreviewBackground(source);
var bounds = FindOpaqueBounds(keyed, minimumAlpha: 16);
if (bounds.Width <= 0 || bounds.Height <= 0)
{
    Console.Error.WriteLine("No foreground pixels found after background removal.");
    return 1;
}

var pad = Math.Max(bounds.Width, bounds.Height) / 14;
bounds = Expand(bounds, pad, keyed.Width, keyed.Height);

using var cropped = keyed.Clone(ctx => ctx.Crop(bounds));
SaveContained(cropped, bigOutputPath, 74, 74, scale: 0.92f);
SaveContained(cropped, textOutputPath, 24, 24, scale: 0.92f);

Console.WriteLine($"source={source.Width}x{source.Height}");
Console.WriteLine($"crop={bounds.X},{bounds.Y} {bounds.Width}x{bounds.Height}");
Console.WriteLine(bigOutputPath);
Console.WriteLine(textOutputPath);
return 0;

static Image<Rgba32> FloodRemovePreviewBackground(Image<Rgba32> source)
{
    var pixels = new Rgba32[source.Width * source.Height];
    var outputPixels = new Rgba32[pixels.Length];
    source.CopyPixelDataTo(pixels);
    Array.Copy(pixels, outputPixels, pixels.Length);

    var key = SampleBorderKey(source);
    var visited = new bool[source.Width * source.Height];
    var queue = new Queue<Point>();

    void Enqueue(int x, int y)
    {
        if (x < 0 || y < 0 || x >= source.Width || y >= source.Height)
        {
            return;
        }

        var index = y * source.Width + x;
        if (visited[index])
        {
            return;
        }

        visited[index] = true;
        queue.Enqueue(new Point(x, y));
    }

    for (var x = 0; x < source.Width; x++)
    {
        Enqueue(x, 0);
        Enqueue(x, source.Height - 1);
    }

    for (var y = 0; y < source.Height; y++)
    {
        Enqueue(0, y);
        Enqueue(source.Width - 1, y);
    }

    while (queue.Count > 0)
    {
        var point = queue.Dequeue();
        var index = point.Y * source.Width + point.X;
        var pixel = pixels[index];
        if (!IsPreviewBackground(pixel, key))
        {
            continue;
        }

        outputPixels[index] = new Rgba32(pixel.R, pixel.G, pixel.B, 0);
        Enqueue(point.X + 1, point.Y);
        Enqueue(point.X - 1, point.Y);
        Enqueue(point.X, point.Y + 1);
        Enqueue(point.X, point.Y - 1);
    }

    return Image.LoadPixelData<Rgba32>(outputPixels, source.Width, source.Height);
}

static bool IsPreviewBackground(Rgba32 pixel, Rgba32 key)
{
    var distance = Distance(pixel, key);
    var max = Math.Max(pixel.R, Math.Max(pixel.G, pixel.B));
    var min = Math.Min(pixel.R, Math.Min(pixel.G, pixel.B));
    var saturation = max - min;

    return distance <= 34 && max <= 64 && saturation <= 18;
}

static Rgba32 SampleBorderKey(Image<Rgba32> source)
{
    long r = 0;
    long g = 0;
    long b = 0;
    long count = 0;
    var inset = Math.Max(8, Math.Min(source.Width, source.Height) / 48);

    source.ProcessPixelRows(accessor =>
    {
        for (var y = 0; y < accessor.Height; y++)
        {
            var row = accessor.GetRowSpan(y);
            for (var x = 0; x < row.Length; x++)
            {
                var onBorder = x < inset || y < inset || x >= row.Length - inset || y >= accessor.Height - inset;
                if (!onBorder)
                {
                    continue;
                }

                var pixel = row[x];
                r += pixel.R;
                g += pixel.G;
                b += pixel.B;
                count++;
            }
        }
    });

    return count == 0
        ? new Rgba32(16, 18, 22, 255)
        : new Rgba32((byte)(r / count), (byte)(g / count), (byte)(b / count), 255);
}

static Rectangle FindOpaqueBounds(Image<Rgba32> image, byte minimumAlpha)
{
    var minX = image.Width;
    var minY = image.Height;
    var maxX = -1;
    var maxY = -1;

    image.ProcessPixelRows(accessor =>
    {
        for (var y = 0; y < accessor.Height; y++)
        {
            var row = accessor.GetRowSpan(y);
            for (var x = 0; x < row.Length; x++)
            {
                if (row[x].A < minimumAlpha)
                {
                    continue;
                }

                minX = Math.Min(minX, x);
                minY = Math.Min(minY, y);
                maxX = Math.Max(maxX, x);
                maxY = Math.Max(maxY, y);
            }
        }
    });

    return maxX < minX || maxY < minY
        ? Rectangle.Empty
        : new Rectangle(minX, minY, maxX - minX + 1, maxY - minY + 1);
}

static Rectangle Expand(Rectangle rectangle, int amount, int maxWidth, int maxHeight)
{
    var x = Math.Max(0, rectangle.X - amount);
    var y = Math.Max(0, rectangle.Y - amount);
    var right = Math.Min(maxWidth, rectangle.Right + amount);
    var bottom = Math.Min(maxHeight, rectangle.Bottom + amount);
    return new Rectangle(x, y, right - x, bottom - y);
}

static void SaveContained(Image<Rgba32> source, string path, int width, int height, float scale)
{
    using var canvas = new Image<Rgba32>(width, height, Color.Transparent);
    var fit = Math.Min(width / (float)source.Width, height / (float)source.Height) * scale;
    var resizedWidth = Math.Max(1, (int)Math.Round(source.Width * fit));
    var resizedHeight = Math.Max(1, (int)Math.Round(source.Height * fit));

    using var resized = source.Clone(ctx => ctx.Resize(resizedWidth, resizedHeight, KnownResamplers.Lanczos3));
    var x = (width - resizedWidth) / 2;
    var y = (height - resizedHeight) / 2;
    canvas.Mutate(ctx => ctx.DrawImage(resized, new Point(x, y), 1f));

    Directory.CreateDirectory(Path.GetDirectoryName(path)!);
    canvas.SaveAsPng(path);
}

static double Distance(Rgba32 a, Rgba32 b)
{
    var dr = a.R - b.R;
    var dg = a.G - b.G;
    var db = a.B - b.B;
    return Math.Sqrt(dr * dr + dg * dg + db * db);
}
CS

"$dotnet_bin" run --project "$tool_dir/sts2-template-energy-assets.csproj" -- \
  "$source_png" \
  "$repo_root/Elesis/images/charui/big_energy.png" \
  "$repo_root/Elesis/images/charui/text_energy.png"

#!/usr/bin/env bash
set -euo pipefail

if [[ $# -lt 2 || $# -gt 3 ]]; then
  echo "Usage: scripts/remove-chroma-key.sh <source-png> <output-png> [#rrggbb]" >&2
  exit 1
fi

source_png="$1"
output_png="$2"
key_color="${3:-auto}"
repo_root="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
dotnet_bin="${DOTNET_BIN:-/mnt/HC_Volume_105232828/shared/tools/dotnet/dotnet}"
tool_dir="${TMPDIR:-/tmp}/sts2-template-remove-chroma-key"

if [[ ! -f "$source_png" ]]; then
  echo "Source image not found: $source_png" >&2
  exit 1
fi

mkdir -p "$tool_dir"

cat > "$tool_dir/sts2-template-remove-chroma-key.csproj" <<'CSProj'
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

if (args.Length != 3)
{
    Console.Error.WriteLine("Usage: <source-png> <output-png> <#rrggbb>");
    return 1;
}

var src = args[0];
var output = args[1];
var key = args[2].Equals("auto", StringComparison.OrdinalIgnoreCase)
    ? SampleBorderKey(src)
    : ParseColor(args[2]);

using var image = Image.Load<Rgba32>(src);
using var outputImage = new Image<Rgba32>(image.Width, image.Height);
long transparent = 0;
long partial = 0;
long opaque = 0;
outputImage.ProcessPixelRows(image, (outAccessor, inAccessor) =>
{
    for (var y = 0; y < inAccessor.Height; y++)
    {
        var inRow = inAccessor.GetRowSpan(y);
        var outRow = outAccessor.GetRowSpan(y);
        for (var x = 0; x < inRow.Length; x++)
        {
            var pixel = inRow[x];
            var distance = Distance(pixel, key);

            if (distance <= 24)
            {
                outRow[x] = new Rgba32(pixel.R, pixel.G, pixel.B, 0);
                transparent++;
                continue;
            }

            if (distance < 150)
            {
                var alpha = (byte)Math.Clamp((distance - 24) / 126.0 * 255.0, 0, 255);
                pixel.A = Math.Min(pixel.A, alpha);
                pixel.G = (byte)Math.Min(pixel.G, Math.Max(pixel.R, pixel.B) + 24);
                outRow[x] = pixel;
                partial++;
                continue;
            }

            if (pixel.G > pixel.R + 20 && pixel.G > pixel.B + 20)
            {
                pixel.G = (byte)Math.Max(Math.Max(pixel.R, pixel.B), pixel.G - 36);
            }
            outRow[x] = pixel;
            opaque++;
        }
    }
});

Directory.CreateDirectory(Path.GetDirectoryName(output)!);
outputImage.SaveAsPng(output);
Console.WriteLine($"key=#{key.R:x2}{key.G:x2}{key.B:x2} transparent={transparent} partial={partial} opaque={opaque}");
Console.WriteLine(output);
return 0;

static Rgba32 ParseColor(string value)
{
    value = value.TrimStart('#');
    if (value.Length != 6)
    {
        throw new ArgumentException("Key color must be #rrggbb.");
    }

    return new Rgba32(
        Convert.ToByte(value[0..2], 16),
        Convert.ToByte(value[2..4], 16),
        Convert.ToByte(value[4..6], 16),
        255);
}

static double Distance(Rgba32 a, Rgba32 b)
{
    var dr = a.R - b.R;
    var dg = a.G - b.G;
    var db = a.B - b.B;
    return Math.Sqrt(dr * dr + dg * dg + db * db);
}

static Rgba32 SampleBorderKey(string path)
{
    using var source = Image.Load<Rgba32>(path);
    long r = 0;
    long g = 0;
    long b = 0;
    long count = 0;
    const int inset = 12;

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
        ? new Rgba32(0, 255, 0, 255)
        : new Rgba32((byte)(r / count), (byte)(g / count), (byte)(b / count), 255);
}
CS

"$dotnet_bin" run --project "$tool_dir/sts2-template-remove-chroma-key.csproj" -- "$source_png" "$repo_root/$output_png" "$key_color"

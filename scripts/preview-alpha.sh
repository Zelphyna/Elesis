#!/usr/bin/env bash
set -euo pipefail

if [[ $# -ne 2 ]]; then
  echo "Usage: scripts/preview-alpha.sh <source-png> <output-png>" >&2
  exit 1
fi

source_png="$1"
output_png="$2"
repo_root="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
dotnet_bin="${DOTNET_BIN:-/mnt/HC_Volume_105232828/shared/tools/dotnet/dotnet}"
tool_dir="${TMPDIR:-/tmp}/sts2-template-alpha-preview"

if [[ ! -f "$source_png" ]]; then
  echo "Source image not found: $source_png" >&2
  exit 1
fi

mkdir -p "$tool_dir"

cat > "$tool_dir/sts2-template-alpha-preview.csproj" <<'CSProj'
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

if (args.Length != 2)
{
    Console.Error.WriteLine("Usage: <source-png> <output-png>");
    return 1;
}

using var image = Image.Load<Rgba32>(args[0]);
using var preview = new Image<Rgba32>(image.Width, image.Height);

long transparent = 0;
long partial = 0;
long opaque = 0;
const int tile = 32;

preview.ProcessPixelRows(image, (outAccessor, inAccessor) =>
{
    for (var y = 0; y < outAccessor.Height; y++)
    {
        var outRow = outAccessor.GetRowSpan(y);
        var inRow = inAccessor.GetRowSpan(y);
        for (var x = 0; x < outRow.Length; x++)
        {
            var src = inRow[x];
            if (src.A == 0) transparent++;
            else if (src.A == 255) opaque++;
            else partial++;

            var checker = ((x / tile) + (y / tile)) % 2 == 0
                ? new Rgba32(224, 224, 224, 255)
                : new Rgba32(128, 128, 128, 255);

            var alpha = src.A / 255f;
            outRow[x] = new Rgba32(
                (byte)Math.Round(src.R * alpha + checker.R * (1 - alpha)),
                (byte)Math.Round(src.G * alpha + checker.G * (1 - alpha)),
                (byte)Math.Round(src.B * alpha + checker.B * (1 - alpha)),
                255);
        }
    }
});

Directory.CreateDirectory(Path.GetDirectoryName(args[1])!);
preview.SaveAsPng(args[1]);
Console.WriteLine($"transparent={transparent} partial={partial} opaque={opaque}");
Console.WriteLine(args[1]);
return 0;
CS

"$dotnet_bin" run --project "$tool_dir/sts2-template-alpha-preview.csproj" -- "$source_png" "$repo_root/$output_png"

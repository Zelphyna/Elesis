#!/usr/bin/env bash
set -euo pipefail

if [[ $# -lt 4 || $# -gt 5 ]]; then
  echo "Usage: scripts/resize-image.sh <source-png> <output-png> <width> <height> [cover|contain]" >&2
  exit 1
fi

source_png="$1"
output_png="$2"
target_width="$3"
target_height="$4"
mode="${5:-cover}"
repo_root="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
dotnet_bin="${DOTNET_BIN:-/mnt/HC_Volume_105232828/shared/tools/dotnet/dotnet}"
tool_dir="${TMPDIR:-/tmp}/sts2-template-image-resize-generic"

if [[ ! -f "$source_png" ]]; then
  echo "Source image not found: $source_png" >&2
  exit 1
fi

if [[ "$mode" != "cover" && "$mode" != "contain" ]]; then
  echo "Mode must be 'cover' or 'contain'." >&2
  exit 1
fi

mkdir -p "$tool_dir"

cat > "$tool_dir/sts2-template-image-resize-generic.csproj" <<'CSProj'
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
using SixLabors.ImageSharp.Processing;

if (args.Length != 5)
{
    Console.Error.WriteLine("Usage: <source-png> <output-png> <width> <height> <cover|contain>");
    return 1;
}

var src = args[0];
var output = args[1];
var width = int.Parse(args[2]);
var height = int.Parse(args[3]);
var mode = args[4];

using var image = Image.Load(src);
using var clone = image.Clone(ctx =>
{
    if (mode == "cover")
    {
        var targetAspect = (double)width / height;
        var sourceAspect = (double)image.Width / image.Height;
        Rectangle crop;
        if (sourceAspect > targetAspect)
        {
            var cropWidth = (int)Math.Round(image.Height * targetAspect);
            crop = new Rectangle((image.Width - cropWidth) / 2, 0, cropWidth, image.Height);
        }
        else
        {
            var cropHeight = (int)Math.Round(image.Width / targetAspect);
            crop = new Rectangle(0, (image.Height - cropHeight) / 2, image.Width, cropHeight);
        }
        ctx.Crop(crop).Resize(width, height, KnownResamplers.Lanczos3);
    }
    else
    {
        ctx.Resize(new ResizeOptions
        {
            Size = new Size(width, height),
            Mode = ResizeMode.Pad,
            Sampler = KnownResamplers.Lanczos3,
            PadColor = Color.Transparent
        });
    }
});

Directory.CreateDirectory(Path.GetDirectoryName(output)!);
clone.SaveAsPng(output);
Console.WriteLine($"{output}: {width}x{height} ({mode})");
return 0;
CS

"$dotnet_bin" run --project "$tool_dir/sts2-template-image-resize-generic.csproj" -- "$source_png" "$repo_root/$output_png" "$target_width" "$target_height" "$mode"

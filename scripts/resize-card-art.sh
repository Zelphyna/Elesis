#!/usr/bin/env bash
set -euo pipefail

if [[ $# -ne 3 ]]; then
  echo "Usage: scripts/resize-card-art.sh <source-png> <big-output-png> <small-output-png>" >&2
  exit 1
fi

source_png="$1"
big_output="$2"
small_output="$3"
repo_root="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
dotnet_bin="${DOTNET_BIN:-/mnt/HC_Volume_105232828/shared/tools/dotnet/dotnet}"
tool_dir="${TMPDIR:-/tmp}/sts2-template-image-resize"

if [[ ! -f "$source_png" ]]; then
  echo "Source image not found: $source_png" >&2
  exit 1
fi

mkdir -p "$tool_dir"

cat > "$tool_dir/sts2-template-image-resize.csproj" <<'CSProj'
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

if (args.Length != 3)
{
    Console.Error.WriteLine("Usage: <source-png> <big-output-png> <small-output-png>");
    return 1;
}

ResizeCard(args[0], args[1], args[2]);
return 0;

static void ResizeCard(string src, string big, string small)
{
    using var image = Image.Load(src);
    SaveCropped(image, 1000, 760, big);
    SaveCropped(image, 250, 190, small);
}

static void SaveCropped(Image image, int targetWidth, int targetHeight, string output)
{
    var targetAspect = (double)targetWidth / targetHeight;
    var sourceAspect = (double)image.Width / image.Height;

    Rectangle crop;
    if (sourceAspect > targetAspect)
    {
        var width = (int)Math.Round(image.Height * targetAspect);
        var x = (image.Width - width) / 2;
        crop = new Rectangle(x, 0, width, image.Height);
    }
    else
    {
        var height = (int)Math.Round(image.Width / targetAspect);
        var y = (image.Height - height) / 2;
        crop = new Rectangle(0, y, image.Width, height);
    }

    using var clone = image.Clone(ctx => ctx.Crop(crop).Resize(targetWidth, targetHeight, KnownResamplers.Lanczos3));
    Directory.CreateDirectory(Path.GetDirectoryName(output)!);
    clone.SaveAsPng(output);
    Console.WriteLine($"{output}: {targetWidth}x{targetHeight}");
}
CS

"$dotnet_bin" run --project "$tool_dir/sts2-template-image-resize.csproj" -- "$source_png" "$repo_root/$big_output" "$repo_root/$small_output"

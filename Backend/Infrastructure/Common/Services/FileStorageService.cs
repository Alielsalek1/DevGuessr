using Application.Common.Options;
using Application.Services.Interfaces.Misc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Infrastructure.Common.Services;

/// <summary>
/// Saves uploaded images to the configured directory on the server and generates
/// 6 progressively-obfuscated variants. All returned paths are relative URLs that
/// can be served statically (e.g. /uploads/...).
/// </summary>
public class FileStorageService(IOptions<FileStorageOptions> options) : IFileStorageService
{
    private readonly FileStorageOptions _options = options.Value;

    private static readonly Color LogodleMatteColor = Color.ParseHex("#0c0d0e");

    // Stored least -> most blurred. Gameplay reads this in reverse (hardest first).
    // Keep sigma <= 250 to avoid ImageSharp kernel sampling failures.
    private static readonly float[] BlurSigmas = [0.01f, 18f, 62f, 142f, 220f, 250f];

    // Aggressive per-tier pixelation jumps for stronger visual separation.
    private static readonly float[] PixelationScales = [1f, 0.985f, 0.78f, 0.34f, 0.08f, 0.008f];

    // Allow extremely small mosaics only on hardest tiers.
    private static readonly int[] MinPixelDimsPerTier = [64, 48, 32, 16, 8, 4];

    /// <inheritdoc />
    public async Task<string> SaveOriginalImageAsync(IFormFile file, string targetName, CancellationToken ct)
    {
        var sanitised = SanitiseName(targetName);
        var folder = Path.Combine(_options.UploadPath, sanitised);
        
        // If the directory exists but we are saving a "new" original, 
        // it means we are re-injecting or fixing a previous corrupted state.
        // Nuke it to ensure no old/low-quality artifacts remain.
        if (Directory.Exists(folder))
        {
            Directory.Delete(folder, true);
        }
        
        Directory.CreateDirectory(folder);

        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (string.IsNullOrEmpty(ext)) ext = ".png";

        var originalFileName = $"original{ext}";
        var originalDisk = Path.Combine(folder, originalFileName);

        await using (var stream = File.Create(originalDisk))
        {
            await file.CopyToAsync(stream, ct);
        }

        return ToRelativeUrl(sanitised, originalFileName);
    }

    /// <inheritdoc />
    public async Task<List<string>> GenerateBlurredImagesAsync(string targetName, string ext, CancellationToken ct)
    {
        var sanitised = SanitiseName(targetName);
        var folder = Path.Combine(_options.UploadPath, sanitised);
        var originalDisk = Path.Combine(folder, $"original{ext}");

        using var original = await Image.LoadAsync(originalDisk, ct);
        using var matte = new Image<Rgba32>(original.Width, original.Height, LogodleMatteColor.ToPixel<Rgba32>());
        matte.Mutate(ctx => ctx.DrawImage(original, 1f));

        var blurredImageUrls = new List<string>(BlurSigmas.Length);

        for (var i = 0; i < BlurSigmas.Length; i++)
        {
            var blurredFileName = $"blur_{i + 1}{ext}";
            var blurredDisk = Path.Combine(folder, blurredFileName);

            using var blurred = matte.Clone(ctx => ctx.GaussianBlur(BlurSigmas[i]));

            var pixelScale = PixelationScales[i];
            if (pixelScale < 1f)
            {
                var minDim = MinPixelDimsPerTier[i];
                var downscaledWidth = Math.Max(minDim, (int)Math.Round(blurred.Width * pixelScale));
                var downscaledHeight = Math.Max(minDim, (int)Math.Round(blurred.Height * pixelScale));

                blurred.Mutate(ctx => ctx.Resize(new ResizeOptions
                {
                    Size = new Size(downscaledWidth, downscaledHeight),
                    Sampler = KnownResamplers.Box,
                    Mode = ResizeMode.Stretch
                }));

                blurred.Mutate(ctx => ctx.Resize(new ResizeOptions
                {
                    Size = new Size(matte.Width, matte.Height),
                    Sampler = KnownResamplers.NearestNeighbor,
                    Mode = ResizeMode.Stretch
                }));
            }

            await blurred.SaveAsync(blurredDisk, ct);

            blurredImageUrls.Add(ToRelativeUrl(sanitised, blurredFileName));
        }

        return blurredImageUrls;
    }

    /// <inheritdoc />
    public Task DeleteImagesAsync(string targetName, CancellationToken ct)
    {
        var folder = Path.Combine(_options.UploadPath, SanitiseName(targetName));
        if (Directory.Exists(folder))
            Directory.Delete(folder, recursive: true);

        return Task.CompletedTask;
    }

    /// <summary>Converts a sanitised folder name + filename into a relative URL.</summary>
    private string ToRelativeUrl(string sanitisedName, string fileName)
    {
        var baseUrl = _options.BaseUrl.TrimEnd('/');
        return $"{baseUrl}/{sanitisedName}/{fileName}";
    }

    private static string SanitiseName(string name) =>
        string.Concat(name.ToLowerInvariant().Split(Path.GetInvalidFileNameChars()))
              .Replace(' ', '_');
}

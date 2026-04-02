using Application.Common.Options;
using Application.Services.Interfaces.Misc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace Infrastructure.Common.Services;

/// <summary>
/// Saves uploaded images to the configured directory on the server and generates
/// 5 progressively-blurred variants. All returned paths are relative URLs that
/// can be served statically (e.g. /uploads/...).
/// </summary>
public class FileStorageService(IOptions<FileStorageOptions> options) : IFileStorageService
{
    private readonly FileStorageOptions _options = options.Value;

    // The 5 sigma levels (least → most blurred) used for the logodle game.
    private static readonly float[] BlurSigmas = [1f, 4f, 8f, 14f, 22f];

    /// <inheritdoc />
    public async Task<string> SaveOriginalImageAsync(IFormFile file, string targetName, CancellationToken ct)
    {
        var sanitised = SanitiseName(targetName);
        var folder = Path.Combine(_options.UploadPath, sanitised);
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

        var blurredImageUrls = new List<string>(BlurSigmas.Length);

        for (var i = 0; i < BlurSigmas.Length; i++)
        {
            var blurredFileName = $"blur_{i + 1}{ext}";
            var blurredDisk = Path.Combine(folder, blurredFileName);

            using var blurred = original.Clone(ctx => ctx.GaussianBlur(BlurSigmas[i]));
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

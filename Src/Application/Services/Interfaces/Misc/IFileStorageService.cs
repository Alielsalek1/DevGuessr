using Microsoft.AspNetCore.Http;

namespace Application.Services.Interfaces.Misc;

public interface IFileStorageService
{
    /// <summary>
    /// Saves the original image file to the server under a per-target folder.
    /// </summary>
    /// <param name="file">The uploaded image file.</param>
    /// <param name="targetName">Folder key derived from the logodle-target name.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The relative URL of the saved original image.</returns>
    Task<string> SaveOriginalImageAsync(IFormFile file, string targetName, CancellationToken ct);

    /// <summary>
    /// Generates exactly 5 progressively-blurred variants from the already-saved original
    /// and stores them alongside it.
    /// </summary>
    /// <param name="targetName">Folder key matching the one used in <see cref="SaveOriginalImageAsync"/>.</param>
    /// <param name="ext">File extension of the original (e.g. ".png").</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Ordered list of 5 relative URLs (least → most blurred).</returns>
    Task<List<string>> GenerateBlurredImagesAsync(string targetName, string ext, CancellationToken ct);

    /// <summary>
    /// Deletes all images (original + blurred) stored under the given target folder.
    /// </summary>
    Task DeleteImagesAsync(string targetName, CancellationToken ct);
}

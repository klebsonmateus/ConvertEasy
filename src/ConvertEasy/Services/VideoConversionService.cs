using System.Diagnostics;
using System.IO;
using System.Text;

namespace ConvertEasy.Services;

public sealed class VideoConversionService
{
    private static readonly HashSet<string> VideoExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".mp4", ".mov", ".avi", ".mkv", ".wmv", ".webm", ".m4v",
        ".mpg", ".mpeg", ".mts", ".m2ts"
    };

    public static bool IsSupportedVideo(string path) =>
        File.Exists(path) && VideoExtensions.Contains(Path.GetExtension(path));

    public async Task<string> ConvertAsync(string inputPath)
    {
        string? temporaryPath = null;

        try
        {
            if (!IsSupportedVideo(inputPath))
            {
                throw new InvalidOperationException("O arquivo selecionado não é um vídeo disponível para conversão.");
            }

            var sourceDirectory = Path.GetDirectoryName(inputPath)
                ?? throw new InvalidOperationException("Não foi possível localizar a pasta do vídeo.");
            var resultDirectory = Path.Combine(sourceDirectory, "Resultado");
            Directory.CreateDirectory(resultDirectory);

            // FFmpeg grava primeiro em um arquivo temporário. A movimentação final não substitui
            // arquivos existentes, inclusive se outro processo criar o nome durante a conversão.
            var encodedPath = Path.Combine(resultDirectory, $".{Guid.NewGuid():N}.tmp.mp4");
            temporaryPath = encodedPath;
            using var process = new Process { StartInfo = CreateStartInfo(inputPath, encodedPath) };
            process.Start();
            var errorOutput = process.StandardError.ReadToEndAsync();
            await process.WaitForExitAsync();
            var details = await errorOutput;

            if (process.ExitCode != 0 || !File.Exists(encodedPath))
            {
                throw new InvalidOperationException(
                    $"FFmpeg terminou com código {process.ExitCode}.{Environment.NewLine}{details}");
            }

            var baseName = Path.GetFileNameWithoutExtension(inputPath);
            for (var number = 1; ; number++)
            {
                var fileName = number == 1 ? $"{baseName}.mp4" : $"{baseName} ({number}).mp4";
                var outputPath = Path.Combine(resultDirectory, fileName);

                try
                {
                    File.Move(encodedPath, outputPath);
                    temporaryPath = null;
                    return outputPath;
                }
                catch (IOException) when (File.Exists(outputPath))
                {
                    // O nome já existe; tente o próximo sem sobrescrever.
                }
            }
        }
        catch (Exception exception)
        {
            var logPath = await WriteErrorLogAsync(inputPath, exception);
            throw new VideoConversionException(
                "Não foi possível converter este vídeo. Confira o arquivo e tente novamente.",
                logPath,
                exception);
        }
        finally
        {
            if (temporaryPath is not null)
            {
                try { File.Delete(temporaryPath); }
                catch (IOException) { /* A falha original já foi registrada. */ }
                catch (UnauthorizedAccessException) { /* A falha original já foi registrada. */ }
            }
        }
    }

    private static ProcessStartInfo CreateStartInfo(string inputPath, string outputPath)
    {
        var startInfo = new ProcessStartInfo("ffmpeg")
        {
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardError = true
        };

        string[] arguments =
        [
            "-hide_banner", "-nostdin", "-y",
            "-i", inputPath,
            "-map", "0:v:0", "-map", "0:a:0?",
            "-vf", "scale=w='min(1920,iw)':h='min(1080,ih)':force_original_aspect_ratio=decrease:force_divisible_by=2",
            "-c:v", "libx264", "-pix_fmt", "yuv420p", "-preset", "medium", "-crf", "23",
            "-c:a", "aac", "-ar", "48000", "-ac", "2",
            "-sn", "-dn", "-movflags", "+faststart", "-f", "mp4", outputPath
        ];

        foreach (var argument in arguments)
        {
            startInfo.ArgumentList.Add(argument);
        }

        return startInfo;
    }

    private static async Task<string> WriteErrorLogAsync(string inputPath, Exception exception)
    {
        var logDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "ConvertEasy", "Logs");
        var logPath = Path.Combine(logDirectory, "conversao.log");

        try
        {
            Directory.CreateDirectory(logDirectory);
            var entry = $"[{DateTimeOffset.Now:O}] Arquivo: {inputPath}{Environment.NewLine}" +
                        $"{exception}{Environment.NewLine}{Environment.NewLine}";
            await File.AppendAllTextAsync(logPath, entry, Encoding.UTF8);
            return logPath;
        }
        catch
        {
            // Uma falha ao gravar o log não deve encerrar o aplicativo.
            return string.Empty;
        }
    }
}

public sealed class VideoConversionException(string message, string logPath, Exception innerException)
    : Exception(message, innerException)
{
    public string LogPath { get; } = logPath;
}

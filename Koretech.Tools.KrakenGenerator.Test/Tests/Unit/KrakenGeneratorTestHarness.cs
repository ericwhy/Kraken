using System.Text;

namespace Koretech.Tools.KrakenGenerator.Test.Tests.Unit;

/// <summary>
/// Provides a disposable harness for running the generator against repository test metadata.
/// </summary>
internal sealed class KrakenGeneratorTestHarness : IDisposable
{
    private readonly DirectoryInfo _outputDirectory;

    private KrakenGeneratorTestHarness(string kamlBoFileName, DirectoryInfo outputDirectory)
    {
        KamlBoFileName = kamlBoFileName;
        _outputDirectory = outputDirectory;
    }

    /// <summary>
    /// Gets the KAMLBO file name used for the generation run.
    /// </summary>
    public string KamlBoFileName { get; }

    /// <summary>
    /// Creates a harness and runs the generator for the supplied KAMLBO file.
    /// </summary>
    /// <param name="kamlBoFileName">The KAMLBO file to load from the generator test metadata folder.</param>
    /// <returns>A harness that exposes the generated output and cleans it up when disposed.</returns>
    public static KrakenGeneratorTestHarness Generate(string kamlBoFileName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(kamlBoFileName);

        DirectoryInfo repoRoot = FindRepoRoot();
        FileInfo sourceFile = new(Path.Combine(
            repoRoot.FullName,
            "Koretech.Tools.KrakenGenerator",
            "kamlbo",
            kamlBoFileName));

        string outputPath = Path.Combine(
            Path.GetTempPath(),
            "kraken-generator-tests",
            Guid.NewGuid().ToString("N"));
        DirectoryInfo outputDirectory = new(outputPath);

        KrakenGenerator generator = new(sourceFile, outputDirectory);
        generator.Generate();

        return new KrakenGeneratorTestHarness(kamlBoFileName, outputDirectory);
    }

    /// <summary>
    /// Gets a generated file relative to the harness output root.
    /// </summary>
    /// <param name="relativePath">The file path relative to the output root.</param>
    /// <returns>The generated file information.</returns>
    public FileInfo GetOutputFile(string relativePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(relativePath);
        return new FileInfo(Path.Combine(_outputDirectory.FullName, relativePath));
    }

    /// <summary>
    /// Reads a generated file as UTF-8 text.
    /// </summary>
    /// <param name="relativePath">The file path relative to the output root.</param>
    /// <returns>The generated file contents.</returns>
    public string ReadOutputFile(string relativePath)
    {
        FileInfo file = GetOutputFile(relativePath);
        Assert.That(file.Exists, Is.True, $"Expected generated file '{relativePath}' to exist for '{KamlBoFileName}'.");
        return File.ReadAllText(file.FullName, Encoding.UTF8);
    }

    /// <summary>
    /// Gets whether a generated directory exists relative to the output root.
    /// </summary>
    /// <param name="relativePath">The directory path relative to the output root.</param>
    /// <returns><c>true</c> when the directory exists; otherwise <c>false</c>.</returns>
    public bool DirectoryExists(string relativePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(relativePath);
        return Directory.Exists(Path.Combine(_outputDirectory.FullName, relativePath));
    }

    /// <summary>
    /// Deletes the temporary output directory created for the test run.
    /// </summary>
    public void Dispose()
    {
        if (_outputDirectory.Exists)
        {
            _outputDirectory.Delete(recursive: true);
        }
    }

    /// <summary>
    /// Locates the repository root from the current test execution directory.
    /// </summary>
    /// <returns>The repository root directory.</returns>
    private static DirectoryInfo FindRepoRoot()
    {
        DirectoryInfo? current = new(TestContext.CurrentContext.TestDirectory);
        while (current != null)
        {
            string solutionPath = Path.Combine(current.FullName, "Koretech.Kraken.sln");
            if (File.Exists(solutionPath))
            {
                return current;
            }

            current = current.Parent;
        }

        throw new DirectoryNotFoundException("Could not locate the Kraken repository root from the test directory.");
    }
}

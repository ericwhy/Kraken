using System.CommandLine;

namespace Koretech.Kraken.Kaml;
class Program
{
    static async Task Main(string[] args)
    {
        var kamlSourceOption = new Option<FileInfo>("--source-file");
        kamlSourceOption.Description = "The source kamlbo file to use";
        kamlSourceOption.AddAlias("-s");
        kamlSourceOption.IsRequired = true;
        var outputDirectoryOption = new Option<DirectoryInfo>("--output-directory");
        outputDirectoryOption.AddAlias("-o");
        outputDirectoryOption.IsRequired = false;
        outputDirectoryOption.Description = "The directory to write the generated files to";

        var rootCommand = new RootCommand("KamlBoGen");
        rootCommand.AddOption(kamlSourceOption);
        rootCommand.AddOption(outputDirectoryOption);

        rootCommand.SetHandler( (kamlSource, outputDirectory) => 
        {
            ProcessKamlBo(kamlSource, outputDirectory);
        }, 
        kamlSourceOption, outputDirectoryOption);
        await rootCommand.InvokeAsync(args);
    }

    public static void ProcessKamlBo(FileInfo kamlSource, DirectoryInfo outputDirectory) {
        Console.WriteLine($"Source kaml file specified was {kamlSource.FullName}");
        if(!kamlSource.Exists) {
            Console.WriteLine("Specified file does not exist!");
        }
        if(outputDirectory == null) {
            outputDirectory = new DirectoryInfo(Directory.GetCurrentDirectory());
            Console.WriteLine($"Output directory was not specified, using current directory: {outputDirectory.FullName}");
        } else {
            Console.WriteLine($"Output directory is {outputDirectory.FullName}");
            if(!outputDirectory.Exists) {
                Console.WriteLine("Output directory does not exist!");
            }
        }
        KamlBoGen.KamlBoGen gen = new(kamlSource, outputDirectory);
        gen.Generate();
    }
}

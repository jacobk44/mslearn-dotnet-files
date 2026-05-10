using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Text;

//FIX: correct project root (mslearn-dotnet-files)
var currentDirectory = Path.GetFullPath(
    Path.Combine(AppContext.BaseDirectory, "..", "..", "..")
);

var storesDirectory = Path.Combine(currentDirectory, "stores");

var salesTotalDir = Path.Combine(currentDirectory, "salesTotalDir");
Directory.CreateDirectory(salesTotalDir);

var salesFiles = FindFiles(storesDirectory);

// Generate report
GenerateSalesSummary(salesFiles, salesTotalDir);

foreach (var file in salesFiles)
{
    Console.WriteLine(file);
}

IEnumerable<string> FindFiles(string folderName)
{
    List<string> salesFiles = new List<string>();

    if (!Directory.Exists(folderName))
    {
        Console.WriteLine("Stores directory not found: " + folderName);
        return salesFiles;
    }

    var foundFiles = Directory.EnumerateFiles(folderName, "*.json", SearchOption.AllDirectories);

    foreach (var file in foundFiles)
    {
        salesFiles.Add(file);
    }

    return salesFiles;
}

void GenerateSalesSummary(IEnumerable<string> salesFiles, string outputDirectory)
{
    Directory.CreateDirectory(outputDirectory);

    double grandTotal = 0;

    StringBuilder report = new StringBuilder();

    report.AppendLine("Sales Summary");
    report.AppendLine("----------------------------");
    report.AppendLine();

    report.AppendLine("Details:");

    foreach (var file in salesFiles)
    {
        string salesJson = File.ReadAllText(file);

        SalesData? data = JsonConvert.DeserializeObject<SalesData>(salesJson);

        double fileTotal = data?.Total ?? 0;

        grandTotal += fileTotal;

        string fileName = Path.GetFileName(file);

        report.AppendLine($"  {fileName}: {fileTotal:C}");
    }

    report.Insert(
        report.ToString().IndexOf("Details:"),
        $"Total Sales: {grandTotal:C}\n\n"
    );

    string reportPath = Path.Combine(outputDirectory, "sales-summary.txt");

    File.WriteAllText(reportPath, report.ToString());

    Console.WriteLine("Sales summary report created.");
    Console.WriteLine($"Saved at: {reportPath}");
}

record SalesData(double Total);
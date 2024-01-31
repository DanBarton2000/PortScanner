using PortScanner;
using System.CommandLine;

var rootCommand = new RootCommand("PortScanner");
var address = new Argument<string>(
	name: "address", 
	description: "Target address, can be IP or URL");
var includePorts = new Option<string>(
	aliases: new string[] { "-p", "--ports", "--include" },
	description: "Ports to scan: -p 8080, -p 1-1000",
	getDefaultValue: () => "1-1000");
var excludePorts = new Option<string>(
	aliases: new string[] { "-x", "--exclude" },
	description: "Ports to exclude from the scan: -x 8080, -x 25-50",
	getDefaultValue: () => "");
rootCommand.AddArgument(address);
rootCommand.AddOption(includePorts);
rootCommand.AddOption(excludePorts);
rootCommand.SetHandler(Handler, address, includePorts, excludePorts);
await rootCommand.InvokeAsync(args);

static async Task Handler(string address, string includePorts, string excludePorts)
{
	TcpScanner tcpScanner = new();

	try
	{
		await tcpScanner.SetAddress(address);
		tcpScanner.SetPorts(includePorts, excludePorts);
		tcpScanner.Scan();
	}
	catch (Exception ex)
	{
		Console.WriteLine(ex.Message);
	}

	Console.ReadLine();
}
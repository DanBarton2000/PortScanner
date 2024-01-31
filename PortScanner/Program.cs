using PortScanner;
using System.CommandLine;
using System.Net;
using System.Net.Sockets;

var rootCommand = new RootCommand("PortScanner");
var address = new Argument<string>(
	name: "address", 
	description: "Target address, can be IP or URL");
var includePorts = new Option<string>(
	aliases: new string[] { "-p", "--ports" },
	description: "Ports to scan: -p 8080, -p 1-1000",
	getDefaultValue: () => "1-1000");
rootCommand.AddArgument(address);
rootCommand.AddOption(includePorts);
rootCommand.SetHandler(Handler, address, includePorts);
await rootCommand.InvokeAsync(args);

static async Task Handler(string address, string includePorts)
{
	TcpScanner tcpScanner = new();

	try
	{
		await tcpScanner.SetAddress(address);
		tcpScanner.SetIncludePorts(includePorts);
		tcpScanner.Scan();
	}
	catch (Exception ex)
	{
		Console.WriteLine(ex.Message);
	}

	Console.ReadLine();
}
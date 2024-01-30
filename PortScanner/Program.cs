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
	Console.WriteLine($"{address} - {includePorts}");

	if (IPAddress.TryParse(address, out IPAddress? ip))
	{
		Console.WriteLine("Valid IP address!");
	}
	else
	{
		try
		{
			var ipHostInfo = await Dns.GetHostEntryAsync(address);
		}
		catch (ArgumentException)
		{
			Console.WriteLine($"{address} is an invalid IP address.");
			return;
		}
		catch (SocketException)
		{
			Console.WriteLine($"Could not resolve {address}");
			return;
		}
		catch (Exception ex) 
		{
			Console.WriteLine($"An unknown error as occurred: {ex}");
			return;
		}
	}
}
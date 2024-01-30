using System.CommandLine;
using System.Net;
using System.Net.Sockets;
using System.Text;

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
	if (IPAddress.TryParse(address, out IPAddress ip))
	{
		Console.WriteLine("Valid IP address!");
	}
	else
	{
		try
		{
			var ipHostInfo = await Dns.GetHostEntryAsync(address);
			ip = ipHostInfo.AddressList[1];
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
			Console.WriteLine($"An unknown error has occurred: {ex}");
			return;
		}
	}

	Console.WriteLine($"IP address: {ip}");

	var split = includePorts.Split('-');
	List<int> ports = new();

	if (split.Length == 1)
	{
		if (!int.TryParse(split[0], out int port))
		{
			Console.WriteLine($"Could not parse port {port}");
			return;
		}
		ports.Add(port);
	}
	else if (split.Length == 2)
	{
		if (!int.TryParse(split[0], out int port1))
		{
			Console.WriteLine($"Could not parse port {port1}");
			return;
		}
		if (!int.TryParse(split[1], out int port2))
		{
			Console.WriteLine($"Could not parse port {port2}");
			return;
		}
		ports.AddRange(Enumerable.Range(port1, port2 - port1 + 1));
	}
	else
	{
		Console.WriteLine($"Too many arguments: {includePorts}");
		return;
	}

	foreach (var port in ports) 
	{
		using TcpClient tcpClient = new();
		try
		{
			tcpClient.Connect(ip.ToString(), port);
			Console.WriteLine($"Connected to port {port}");
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
		}
	}

	Console.WriteLine("Finished scan");
	Console.ReadLine();
}
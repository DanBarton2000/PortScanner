using System.Net;
using System.Net.Sockets;

namespace PortScanner
{
	internal class TcpScanner
	{
        private IPAddress _address = IPAddress.Loopback;
        private List<int> _ports = new();

		private int _openPorts = 0;
		private int _closedPorts = 0;

		public void Scan()
		{
			Console.WriteLine("Starting scan");

			foreach (var port in _ports)
			{
				using TcpClient tcpClient = new();
				try
				{
					tcpClient.Connect(_address.ToString(), port);
					Console.WriteLine($"Connected to port {port}");
					_openPorts++;
				}
				catch (SocketException ex)
				{
					Console.WriteLine($"{ex.Message} {ex.ErrorCode}");
					_closedPorts++;
				}
			}

			Console.WriteLine("Finished scan");
			Console.WriteLine($"Open ports: {_openPorts}");
			Console.WriteLine($"Closed ports: {_closedPorts}");
		}

        public async Task SetAddress(string address)
        {
			if (IPAddress.TryParse(address, out IPAddress? ip))
			{
				Console.WriteLine("Valid IP address!");
				_address = ip;
			}
			else
			{
				try
				{
					var ipHostInfo = await Dns.GetHostEntryAsync(address);
					_address = ipHostInfo.AddressList[1];
				}
				catch
				{
					throw;
				}
			}

			Console.WriteLine($"IP address: {_address}");
		}

		public void SetPorts(string includePorts, string excludePorts) 
		{
			_ports = ParsePortString(includePorts);

			foreach (int port in ParsePortString(excludePorts))
			{
				_ports.Remove(port);
			}
		}

		private static List<int> ParsePortString(string portString)
		{
			List<int> ports = new();

			var split = portString.Split('-');

			if (split.Length == 1)
			{
				if (!int.TryParse(split[0], out int port))
				{
					throw new ArgumentException($"Could not parse port '{split[0]}'");
				}
				ports.Add(port);
			}
			else if (split.Length == 2)
			{
				if (!int.TryParse(split[0], out int port1))
				{
					throw new ArgumentException($"Could not parse port '{split[0]}'");
				}
				if (!int.TryParse(split[1], out int port2))
				{
					throw new ArgumentException($"Could not parse port '{split[1]}'");
				}
				ports.AddRange(Enumerable.Range(port1, port2 - port1 + 1));
			}
			else
			{
				throw new ArgumentException($"Too many arguments: '{ports}'");
			}

			return ports;
		}
	}
}

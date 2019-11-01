using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Thrift;
using Thrift.Protocol;
using Thrift.Transport;

namespace Client
{
	class Program
	{
		static void Main(string[] args)
		{
			// Create the transport socket 
			// The socket port must be the same port from server
			TTransport transport = new TSocket("localhost", 2320);

			// Wrap socket in a protocol
			TProtocol protocol = new TBinaryProtocol(transport);

			// Create the client
			FileTransferService.Client client = new FileTransferService.Client(protocol);

			try
			{
				// Connect to server !!!
				transport.Open();
			}
			catch (Exception e)
			{
				Console.WriteLine("Server is not running! Please start server first!");
				Console.WriteLine("Press any key to close ... ");

				// Wait user input for closure
				Console.ReadKey();

				Environment.Exit(0);
			}

			Console.WriteLine("Send a message to server:");

			string userInput = Console.ReadLine();

			// Call a function from server
			// In this case "Transfer" function
			Console.WriteLine(client.Transfer(userInput));

			// Handle console closure
			Console.WriteLine("\n\n\nPress any key to close ... ");
			Console.ReadKey();

			// Close server connection
			transport.Close();
		}
	}
}

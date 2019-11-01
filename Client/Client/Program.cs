using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Thrift;
using Thrift.Protocol;
using Thrift.Transport;

namespace Client
{
	class Program
	{
		static void Main(string[] args)
		{
			var (client, transport) = CreateClient();

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

			bool switchOptions = true;

			while (switchOptions)
			{
				Console.WriteLine("Choose am option by inserting the option number:");
				Console.WriteLine("1. Communicate with server by a string");
				Console.WriteLine("2. Upload an XML file to server database");
				
				// read user input
				string userInput = Console.ReadLine();

				switch (userInput)
				{
					case "1":
						// Call TransferString function
						CallTransfer(client);
						switchOptions = false;
						break;
					case "2":
						// Call TransferInnerXml function
						CallUploadInnerXml(client);
						switchOptions = false;
						break;
					default:
						Console.WriteLine("Wrong option! Please try again!");
						break;
				}
			}

			// Handle console closure
			Console.WriteLine("\n\n\nPress any key to close ... ");
			Console.ReadKey();

			// Close server connection
			transport.Close();
		}



		private static Tuple<FileTransferService.Client, TTransport> CreateClient()
		{
			// Create the transport socket 
			// The socket port must be the same port from server
			TTransport transport = new TSocket("localhost", 2320);

			// Wrap socket in a protocol
			TProtocol protocol = new TBinaryProtocol(transport);

			// Create the client
			FileTransferService.Client client = new FileTransferService.Client(protocol);

			return new Tuple<FileTransferService.Client, TTransport>(client, transport);
		}
		
		private static void CallTransfer(FileTransferService.Client client)
		{
			Console.WriteLine("Send a message to server:");

			// read user input
			string userInput = Console.ReadLine();

			// Call a function from server
			// In this case "Transfer" function
			Console.WriteLine(client.Transfer(userInput));
		}

		private static void CallUploadInnerXml(FileTransferService.Client client)
		{
			Console.WriteLine("Insert file name:");
			string saveName = Console.ReadLine();

			Console.WriteLine("Insert full xml local path:");
			string filePath = Console.ReadLine();
			
			// create a XML document
			XmlDocument doc = new XmlDocument();

			try
			{
				// load the document
				doc.Load($"{filePath}");

				// check if is a valid xml
				doc.LoadXml(doc.InnerXml);
			}
			catch (Exception e)
			{
				Console.WriteLine($"ERROR! Cannot find a valid xml file on this path: {filePath}");
				return;
			}

			// Call a function from server
			// In this case "UploadInnerXml" function
			Console.WriteLine(client.UploadInnerXml(saveName, doc.InnerXml));

		}
	}
}

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

			bool keepConnectionAlive = true;

			while (keepConnectionAlive)
			{
				bool switchOptions = true;

				while (switchOptions)
				{
					Console.WriteLine("\n\nChoose am option by inserting the option number:");
					Console.WriteLine("1. Communicate with server by a string");
					Console.WriteLine("2. Upload an XML file to server database");
					Console.WriteLine("3. Download an XML file from server database");

					// read user input
					string userInput = Console.ReadLine();

					switch (userInput)
					{
						case "1":
							// Call Transfer function
							CallTransfer(client);
							switchOptions = false;
							break;
						case "2":
							// Call UploadInnerXml function
							CallUploadInnerXml(client);
							switchOptions = false;
							break;
						case "3":
							// Call DownloadInnerXml function
							CallDownloadInnerXml(client);
							switchOptions = false;
							break;
						default:
							Console.WriteLine("Wrong option! Please try again!");
							break;
					}
				}

				Console.WriteLine("\n\nKeep connection alive? - Y/N");
				var answer = Console.ReadKey();

				// check user answer
				if (answer.KeyChar.ToString().ToUpper() != "Y")
				{
					keepConnectionAlive = false;
				}
			}

			// Handle console closure
			Console.WriteLine("\n\n\nPress any key to close connection ... ");
			Console.ReadKey();

			// Close server connection
			transport.Close();
		}

		/// <summary>
		/// Create Client 
		/// </summary>
		/// <returns></returns>
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
			Console.WriteLine("\nSend a message to server:");

			// read user input
			string userInput = Console.ReadLine();

			// Call a function from server
			// In this case "Transfer" function
			Console.WriteLine(client.Transfer(userInput));
		}

		/// <summary>
		/// Upload a xml file to server database folder
		/// </summary>
		/// <param name="client"></param>
		private static void CallUploadInnerXml(FileTransferService.Client client)
		{
			Console.WriteLine("\nInsert file name:");
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


		/// <summary>
		/// Download a xml file from server database folder
		/// </summary>
		/// <param name="client"></param>
		/// <returns></returns>
		private static void CallDownloadInnerXml(FileTransferService.Client client)
		{
			var xmlServerFiles = GetXmlFIleFromServer(client);

			if (!xmlServerFiles.Item1)
			{
				Console.WriteLine("\nNo xml files server database!");
				return;
			}

			Console.WriteLine("\nXml file on server:");

			foreach (var xmlFile in xmlServerFiles.Item2)
			{
				Console.WriteLine($"{xmlFile}");
			}

			Console.WriteLine("\nPlease insert the fully qualified name of the file you want to download:");
			var xmlFileName = Console.ReadLine();

			// create a XML document
			XmlDocument doc = new XmlDocument();

			// get innerXml from server
			var innerXml = client.DownloadInnerXml(xmlFileName);

			try
			{
				// check if received document is a valid xml
				doc.LoadXml(innerXml);
			}
			catch (Exception e)
			{
				Console.WriteLine($"\nERROR! Cannot find a valid xml file with this name: {xmlFileName}");
				return;
			}

			// set saving path
			string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
			var pathToSave = desktopPath + $"\\{xmlFileName}";

			// save the document
			doc.Save(pathToSave);

			Console.WriteLine($"The xml file was correctly downloaded at this path: '{pathToSave}'.") ;
		}


		/// <summary>
		/// Get xml list from server database folder
		/// </summary>
		/// <param name="client"></param>
		/// <returns>
		///	Tuple.Item1 = true - if are xml files on server database folder
		/// Tuple.Item2 - xml files from server database folder
		/// </returns>
		private static Tuple<bool, List<string>> GetXmlFIleFromServer(FileTransferService.Client client)
		{
			var xmlList = client.GetServerXmlList();

			if (xmlList.Count == 0)
			{
				return new Tuple<bool, List<string>>(false, xmlList);
			}

			return new Tuple<bool, List<string>>(true, xmlList);
		}
	}
}

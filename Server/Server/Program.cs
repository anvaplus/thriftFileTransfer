using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Thrift;
using Thrift.Server;
using Thrift.Transport;

namespace Server
{
	class Program
	{
		static void Main(string[] args)
		{
			try
			{
				FileTransferServer transferServer = new FileTransferServer();
				FileTransferService.Processor processor = new FileTransferService.Processor(transferServer);

				// Set server listening port
				TServerTransport serverTransport = new TServerSocket(2320);

				// Create server object
				TServer server = new TSimpleServer(processor, serverTransport);

				Console.WriteLine("Starting is up and running ... ");

				// Start server
				server.Serve();

			}
			catch (Exception e)
			{
				Console.WriteLine(e.StackTrace);
			}

			Console.WriteLine("Server is down");

		}
	}
}

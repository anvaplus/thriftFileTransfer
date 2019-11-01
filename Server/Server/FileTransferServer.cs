using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
	class FileTransferServer : FileTransferService.Iface
	{
		// implement FileTransferService interface
		// this is server side implementation for client call

		public string Transfer(string name)
		{
			Console.WriteLine($"Client call Transfer function with parameter: {name}");

			return $"The server response: I received this message: '{name}'. My answer is 'Hello client!'";

		}
	}
}

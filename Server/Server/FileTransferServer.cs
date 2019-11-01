using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace Server
{
	class FileTransferServer : FileTransferService.Iface
	{
		// implement FileTransferService interface
		// this is server side implementation for client call

		/// <summary>
		/// Transfer a simple string message from client to server
		/// </summary>
		/// <param name="message">Message sent from client</param>
		/// <returns>Server response with the same message plus "Hello client!"</returns>
		public string Transfer(string message)
		{
			Console.WriteLine($"Client call Transfer function with parameter: {message}");

			return $"Server response: I received this message: '{message}'. My answer is 'Hello client!'";
		}

		/// <summary>
		/// Upload a xml file to server 
		/// </summary>
		/// <param name="name"> Name of saving file</param>
		/// <param name="innerXml">xml file string</param>
		/// <returns>Server response with the path where the xml file is saved</returns>
		public string UploadInnerXml(string name, string innerXml)
		{
			Console.WriteLine($"Client call UploadInnerXml function");

			// create a XML document
			XmlDocument doc = new XmlDocument();

			// load xml document from a specific string (xml format)
			doc.LoadXml(innerXml);

			// set saving path
			string serverDataBasePath = GetServeDataBasePath();

			if (serverDataBasePath == string.Empty)
			{
				return "Corrupt project directory";
			}

			var pathToSave = serverDataBasePath + name + ".xml";
			
			// save the document
			doc.Save(pathToSave);

			return $"Server response: The xml file was correctly uploaded at this path: '{pathToSave}'.";
		}

		/// <summary>
		/// Get a list of xml files from server database folder
		/// </summary>
		/// <returns>Empty list if no xml files are in server database folder</returns>
		public List<string> GetServerXmlList()
		{
			// create list of xml files
			List<string> xmlFileList = new List<string>();

			string serverDataBasePath = GetServeDataBasePath();
			if (serverDataBasePath == string.Empty)
			{
				return xmlFileList;
			}

			// add xml file name to list
			DirectoryInfo d = new DirectoryInfo($"{serverDataBasePath}");
			FileInfo[] files = d.GetFiles("*.xml"); //Getting XML files
			foreach (FileInfo file in files)
			{
				xmlFileList.Add(file.Name);
			}

			return xmlFileList;
		}

		/// <summary>
		/// Download a xml file from server database folder
		/// </summary>
		/// <param name="name">Xml file name to be downloaded</param>
		/// <returns>Error with string empty if no file name coincide with the parameter</returns>
		public string DownloadInnerXml(string name)
		{
			var filePath = GetServeDataBasePath() + name;

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
				// return a not valid xml
				return string.Empty;
			}

			return doc.InnerXml;
		}

		/// <summary>
		/// Get server database folder path
		/// </summary>
		/// <returns></returns>
		private string GetServeDataBasePath()
		{
			string serverDataBasePath = string.Empty;

			// Get the current WORKING directory (i.e. \bin\Debug)
			string workingDirectory = Directory.GetCurrentDirectory();

			// Get the current PROJECT directory
			var directoryInfo = Directory.GetParent(workingDirectory).Parent;
			if (directoryInfo != null)
			{
				string projectDirectory = directoryInfo.FullName;
				serverDataBasePath = Path.GetFullPath(Path.Combine(projectDirectory, @"..\..\ServerDataBase\"));
			}
			else
			{
				return serverDataBasePath;
			}

			return serverDataBasePath;
		}

	}
}

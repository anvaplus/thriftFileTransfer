namespace csharp Client

service FileTransferService {
    string Transfer (1:string message)
    string UploadInnerXml (1:string name, 2:string innerXml)
    list <string> GetServerXmlList ()
     string DownloadInnerXml (1:string xmlName)
}
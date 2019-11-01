namespace csharp Server

service FileTransferService {
    string Transfer (1:string message)
    string UploadInnerXml (1:string name, 2:string innerXml)
}
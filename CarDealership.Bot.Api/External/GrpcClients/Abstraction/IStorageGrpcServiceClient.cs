namespace CarDealership.Bot.Api.External.GrpcClients.Abstraction
{
    public interface IStorageGrpcServiceClient
    {
        Task<Stream> DownloadFile(string directory, string filename);
    }
}

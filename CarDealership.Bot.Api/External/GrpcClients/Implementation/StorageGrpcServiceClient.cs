using CarDealership.Bot.Api.External.GrpcClients.Abstraction;
using Grpc.Core;
using Storage;
using static Storage.FileStorage;

namespace CarDealership.Bot.Api.External.GrpcClients.Implementation
{
    public class StorageGrpcServiceClient : IStorageGrpcServiceClient
    {
        private readonly FileStorageClient _grpcClient;

        public StorageGrpcServiceClient(FileStorageClient grpcClient)
        {
            _grpcClient = grpcClient;
        }

        public async Task<Stream> DownloadFile(string directory, string filename)
        {
            var request = new DownloadRequest
            {
                Directory = directory,
                FileName = filename
            };

            Stream stream = new MemoryStream();
            await foreach (var response in _grpcClient.DownloadFile(request).ResponseStream.ReadAllAsync())
            {
                await stream.WriteAsync(response.Content.Memory);
            }

            stream.Position = 0;

            return stream;
        }
    }
}

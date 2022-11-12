using Grpc.Net.Client;
using Practices.gRPC.Server;

namespace Practices.gRPC.Client.Services;

public class BooksWebService : IDisposable
{
    private readonly GrpcChannel _channel;
    private readonly BookRepository.BookRepositoryClient _client; 
    
    public BooksWebService()
    {
        // Grpc.Net.Client.Factory helps with DI client initialization
#if DEBUG
        AppContext.SetSwitch(
            "System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true); // Return `true` to allow certificates that are untrusted/invalid
        var httpHandler = new HttpClientHandler();
        httpHandler.ServerCertificateCustomValidationCallback =
            HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
        _channel = GrpcChannel.ForAddress("https://127.0.0.1:5054",
            new GrpcChannelOptions { HttpHandler = httpHandler });
#else
        _channel = GrpcChannel.ForAddress("https://127.0.0.1:7249");
#endif
        
        _client = new BookRepository.BookRepositoryClient(_channel);
    }

    public async Task<Book> Get(int id)
    {
        var req = new GetBookRequest
        {
            Id = id
        };
        return await _client.GetAsync(req);
    }

    public async Task<Book> Create(string title, string description, int authorId)
    {
        var req = new CreateBookRequest
        {
            Title = title,
            Description = description,
            AuthorId = authorId
        };
        return await _client.CreateAsync(req);
    }

    public async Task<bool> Delete(int id)
    {
        var req = new DeleteBookRequest
        {
            Id = id
        };
        var result = await _client.DeleteAsync(req);
        return result.Success;
    }

    public void Dispose()
    {
        _channel.Dispose();
    }
}
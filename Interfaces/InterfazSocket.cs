namespace Interfaz
{
    public interface SocketGeneral
    {
        Task ConnectAsync(string host, int port);
        Task SendAsync(string message);
        Task<string> ReceiveAsync(CancellationToken cancellationToken = default);
        void Disconnect();
    }
}

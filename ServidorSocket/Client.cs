using System.Net.Sockets;
using System.Text;
using InterfazClient;

namespace Client
{
    public class SocketClient : ClientGeneral
    {
        private TcpClient tcpClient;
        private NetworkStream stream;

        public async Task Conectar(string host, int port)
        {
            try
            {
                tcpClient = new TcpClient();
                await tcpClient.ConnectAsync(host, port);
                stream = tcpClient.GetStream();
            }
            catch (SocketException ex)
            {
                
                throw new Exception($"Error al conectar: {ex.Message}", ex);
            }
        }

        public async Task Enviar(string message)
        {
            try
            {
                if (stream == null) throw new InvalidOperationException("No conectado.");
                byte[] buffer = Encoding.UTF8.GetBytes(message);
                await stream.WriteAsync(buffer);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al enviar mensaje: {ex.Message}", ex);
            }
        }

        public async Task<string> Recibir(CancellationToken cancellationToken = default)
        {
            try
            {
                if (stream == null) throw new InvalidOperationException("No conectado.");
                byte[] buffer = new byte[1024];
                int bytesRead = await stream.ReadAsync(buffer.AsMemory(0, buffer.Length), cancellationToken);
                return Encoding.UTF8.GetString(buffer, 0, bytesRead);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al recibir mensaje: {ex.Message}", ex);
            }
        }

        public void Desconectar()
        {
            try
            {
                stream?.Close();
                tcpClient?.Close();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al desconectar: {ex.Message}", ex);
            }
        }
    }
}

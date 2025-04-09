using System.Net;
using System.Net.Sockets;
using System.Text;
namespace SocketLibrary.Server
{
    public class ServidorSocket 
    {
        private TcpListener listener;
        private bool activo;
        public event Action<string, string> OnMessageReceived;

        public void Arranque(int puerto)
        {
            try
            {
                listener = new TcpListener(IPAddress.Any, puerto);
                listener.Start();
                activo = true;
                _ = Escuchar();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al iniciar el servidor: {ex.Message}", ex);
            }
        }

        private async Task Escuchar()
        {
            try
            {
                while (activo)
                {
                    TcpClient client = await listener.AcceptTcpClientAsync();
                    _ = HandleClient(client);
                }
            }
            catch (Exception ex)
            {
                
                Console.WriteLine($"Error en el servidor: {ex.Message}");
            }
        }

        private async Task HandleClient(TcpClient client)
        {
            try
            {
                var stream = client.GetStream();
                var buffer = new byte[1024];

                while (activo)
                {
                    int bytesRead = await stream.ReadAsync(buffer);
                    if (bytesRead == 0) break;

                    string mensaje = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    var ipusuario = ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString();
                    OnMessageReceived?.Invoke(ipusuario, mensaje);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al manejar cliente: {ex.Message}");
            }
            finally
            {
                client?.Close();
            }
        }

        public void Parar()
        {
            try
            {
                activo = false;
                listener.Stop();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al detener el servidor: {ex.Message}", ex);
            }
        }
    }
}


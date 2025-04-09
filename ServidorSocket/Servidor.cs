using System.Net;
using System.Net.Sockets;
using System.Text;
using InterfazSocket;

namespace Servidor
{
    public class ServidorSocket : SocketGeneral
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
                Console.WriteLine($"Servidor iniciado en el puerto {puerto}");
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
                    var ip = ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString(); //Obtener ip
                    Console.WriteLine($"Cliente conectado: {ip}");
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
            string ipusuario = ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString();

            try
            {
                var stream = client.GetStream();
                var buffer = new byte[1024];

                while (activo)
                {
                    var lectura = stream.ReadAsync(buffer).AsTask();
                    var tiempofinal = await Task.WhenAny(lectura, Task.Delay(20000));// 20 segundos timeout

                    if (tiempofinal != lectura)
                    {
                        Console.WriteLine($"Timeout de lectura para cliente {ipusuario}");
                        break;
                    }

                    int leido = await lectura;
                    if (leido == 0) break;

                    string mensaje = Encoding.UTF8.GetString(buffer, 0, leido);
                    OnMessageReceived?.Invoke(ipusuario, mensaje);

                    string respuesta = "Mensaje recibido";
                    byte[] datos = Encoding.UTF8.GetBytes(respuesta);
                    await stream.WriteAsync(datos, 0, datos.Length);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al manejar cliente {ipusuario}: {ex.Message}");
            }
            finally
            {
                client?.Close();
                Console.WriteLine($"Cliente desconectado: {ipusuario}");
            }
        }

        public void Parar()
        {
            try
            {
                activo = false;
                listener.Stop();
                Console.WriteLine("Servidor detenido.");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al detener el servidor: {ex.Message}", ex);
            }
        }
    }
}



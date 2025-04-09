using System;
using System.Threading.Tasks;
using Client;
using Servidor;

class Program
{
    static async Task Main(string[] args)
    {
        var servidor = new ServidorSocket();
        servidor.OnMessageReceived += (ip, mensaje) =>
        {
            Console.WriteLine($"[Servidor] Mensaje de {ip}: {mensaje}");
        };

        servidor.Arranque(5000); 
        Console.WriteLine("Servidor iniciado.");


        await Task.Delay(500); 

        var cliente = new SocketClient();
        try
        {
            Console.WriteLine("Conectando al servidor...");
            await cliente.Conectar("127.0.0.1", 5000);

            Console.WriteLine("Enviando mensaje...");
            await cliente.Enviar("Hola desde el cliente");


            var respuesta = await cliente.Recibir();

            Console.WriteLine($"[Cliente] Respuesta del servidor: {respuesta}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
        finally
        {
            cliente.Desconectar();
            servidor.Parar();
            Console.WriteLine("Conexión cerrada y servidor detenido.");
        }
    }
}


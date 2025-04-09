namespace InterfazClient;

public interface ClientGeneral
{
    Task Conectar(string host, int port);
    Task Enviar(string message);
    Task<string> Recibir(CancellationToken cancellationToken = default);
    void Desconectar();
}

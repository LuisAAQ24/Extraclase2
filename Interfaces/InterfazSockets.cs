using System;

namespace InterfazSocket
{
    public interface SocketGeneral
    {
        void Arranque(int puerto);
        void Parar();
        event Action<string, string> OnMessageReceived;
    }
}


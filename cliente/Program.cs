using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

class Client
{
    private int listenPort;
    private int connectPort;

    public Client(int listenPort, int connectPort)
    {
        this.listenPort = listenPort;
        this.connectPort = connectPort;
    }

    public void Start()
    {
        // Iniciar hilo para escuchar
        Thread listenThread = new Thread(new ThreadStart(Listen));
        listenThread.Start();

        // Conectar al otro cliente
        Connect();
    }

    private void Listen()
    {
        // Configurar el endpoint de escucha
        IPEndPoint endpoint = new IPEndPoint(IPAddress.Any, listenPort);
        Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        listener.Bind(endpoint);
        listener.Listen(10);

        Console.WriteLine($"Escuchando en el puerto {listenPort}...");

        while (true)
        {
            Socket handler = listener.Accept();

            // Recibir datos
            byte[] buffer = new byte[1024];
            int bytesRec = handler.Receive(buffer);
            string data = Encoding.UTF8.GetString(buffer, 0, bytesRec);

            Console.WriteLine("Datos recibidos: {0}", data);

            // Enviar respuesta
            byte[] msg = Encoding.UTF8.GetBytes("Datos recibidos.");
            handler.Send(msg);

            handler.Shutdown(SocketShutdown.Both);
            handler.Close();
        }
    }

    private void Connect()
    {
        // Configurar el endpoint del otro cliente
        IPEndPoint endpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), connectPort);
        Socket sender = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        while (true)
        {
            try
            {
                sender.Connect(endpoint);
                break;
            }
            catch (SocketException)
            {
                // Intentar conectar nuevamente si falla
                Thread.Sleep(1000);
            }
        }

        Console.WriteLine($"Conectado al puerto {connectPort}...");

        while (true)
        {
            // Enviar datos
            Console.Write("Escribe un mensaje para enviar al otro cliente: ");
            string message = Console.ReadLine();
            if (string.IsNullOrEmpty(message)) break;
            byte[] msg = Encoding.UTF8.GetBytes(message);
            sender.Send(msg);

            // Recibir respuesta
            byte[] buffer = new byte[1024];
            int bytesRec = sender.Receive(buffer);
            string data = Encoding.UTF8.GetString(buffer, 0, bytesRec);

            Console.WriteLine("Respuesta del otro cliente: {0}", data);
        }

        sender.Shutdown(SocketShutdown.Both);
        sender.Close();
    }

    static void Main(string[] args)
    {
        if (args.Length != 2)
        {
            Console.WriteLine("Uso: dotnet run <nombre-del-proyecto> -port <puerto-escucha> <puerto-otro-cliente>");
            return;
        }

        int listenPort = int.Parse(args[0]);
        int connectPort = int.Parse(args[1]);

        Client client = new Client(listenPort, connectPort);
        client.Start();
    }
}

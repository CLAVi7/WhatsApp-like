using System; 
using System.Net; 
using System.Net.Sockets; 
using System.Text; 
using System.Threading; 

class Client
{
    private int listenPort; // Define un campo para almacenar el puerto de escucha
    private int connectPort; // Define un campo para almacenar el puerto del otro cliente

    // Constructor que inicializa los puertos de escucha y de conexión
    public Client(int listenPort, int connectPort)
    {
        this.listenPort = listenPort; // Inicializa el puerto de escucha
        this.connectPort = connectPort; // Inicializa el puerto del otro cliente
    }

    // Método para iniciar el cliente
    public void Start()
    {
        // Iniciar un hilo para el método de escucha
        Thread listenThread = new Thread(new ThreadStart(Listen));
        listenThread.Start(); // Comienza la ejecución del hilo de escucha

        // Conectar al otro cliente
        Connect();
    }

    // Método para escuchar conexiones entrantes
    private void Listen()
    {
        // Configura el endpoint de escucha en cualquier dirección IP y el puerto especificado
        IPEndPoint endpoint = new IPEndPoint(IPAddress.Any, listenPort);
        // Crea un socket para escuchar conexiones TCP
        Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        listener.Bind(endpoint); // Vincula el socket al endpoint de escucha
        listener.Listen(1000); // Comienza a escuchar hasta 10 conexiones entrantes

        Console.WriteLine($"Escuchando en el puerto {listenPort}...");

        while (true)
        {
            // Acepta una conexión entrante
            Socket handler = listener.Accept();

            // Recibir datos del cliente conectado
            byte[] buffer = new byte[1024];
            int bytesRec = handler.Receive(buffer); // Lee los datos del cliente
            string data = Encoding.UTF8.GetString(buffer, 0, bytesRec); // Convierte los bytes en una cadena

            Console.WriteLine("Datos recibidos: {0}", data);

            // Enviar respuesta al cliente
            byte[] msg = Encoding.UTF8.GetBytes("Datos recibidos.");
            handler.Send(msg); // Envía los datos al cliente

            handler.Shutdown(SocketShutdown.Both); // Cierra la conexión
            handler.Close(); // Libera los recursos asociados al socket
        }
    }

    // Método para conectarse a otro cliente
    private void Connect()
    {
        // Configura el endpoint del otro cliente usando la dirección IP local y el puerto especificado
        IPEndPoint endpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), connectPort);
        // Crea un socket para la conexión TCP
        Socket sender = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        while (true)
        {
            try
            {
                sender.Connect(endpoint); // Intenta conectarse al otro cliente
                break; // Sale del bucle si la conexión es exitosa
            }
            catch (SocketException)
            {
                // Si la conexión falla, espera 1 segundo y reintenta
                Thread.Sleep(1000);
            }
        }

        Console.WriteLine($"Conectado al puerto {connectPort}...");

        while (true)
        {
            // Envía datos al otro cliente
            Console.Write("Escribe un mensaje para enviar al otro cliente: ");
            string message = Console.ReadLine(); // Lee el mensaje de la consola
            if (string.IsNullOrEmpty(message)) break; // Si el mensaje está vacío, termina el bucle
            byte[] msg = Encoding.UTF8.GetBytes(message); // Convierte el mensaje en bytes
            sender.Send(msg); // Envía el mensaje al otro cliente

            // Recibe la respuesta del otro cliente
            byte[] buffer = new byte[1024];
            int bytesRec = sender.Receive(buffer); // Lee los datos del otro cliente
            string data = Encoding.UTF8.GetString(buffer, 0, bytesRec); // Convierte los bytes en una cadena

            Console.WriteLine("Respuesta del otro cliente: {0}", data);
        }

        sender.Shutdown(SocketShutdown.Both); // Cierra la conexión
        sender.Close(); // Libera los recursos asociados al socket
    }

    // Método principal que se ejecuta al iniciar el programa
    static void Main(string[] args)
    {
        // Verifica que se pasen dos argumentos (puertos)
        if (args.Length != 2)
        {
            Console.WriteLine("Uso: dotnet run <puerto-escucha> <puerto-otro-cliente>");
            return;
        }

        // Intenta convertir los argumentos a enteros
        int listenPort = int.Parse(args[0]);
        int connectPort = int.Parse(args[1]);

        // Crea una instancia del cliente con los puertos especificados
        Client client = new Client(listenPort, connectPort);
        client.Start(); // Inicia el cliente
    }
}

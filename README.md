como se usa el programa
en la terminal del proyecto escribiremos dotnet run "puerto de escucha" "puerto del otro cliente"
por ejemplo dotnet run 1 3, significa que al puerto a donde le llegaran los mensajes a este cliente sera el 1 y a donde los enviara
sera al puerto 3, si iniciamos otra terminal y colocamos dotnet run 3 1, significa que estara recibiendo los mensajes en el puerto 3
y los enviara al 1, de esta manera se conectan los clientes 

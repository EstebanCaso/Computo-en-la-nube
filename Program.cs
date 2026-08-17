using System;
using System.Threading;

public class Program
{
    public static void Main(string[] args)
    {
        // Num de filosofos/tenedores
        const int NUMERO_FILOSOFOS = 5;

        // Creacion de tenedores
        var tenedores = new Tenedor[NUMERO_FILOSOFOS];
        for (int i = 0; i < NUMERO_FILOSOFOS; i++)
        {
            tenedores[i] = new Tenedor(i);
        }

        // creacion de filosofos y sus hilos
        var filosofos = new Filosofo[NUMERO_FILOSOFOS];
        for (int i = 0; i < NUMERO_FILOSOFOS; i++)
        {
            Tenedor tenedorIzquierdo = tenedores[i];
            Tenedor tenedorDerecho = tenedores[(i + 1) % NUMERO_FILOSOFOS];

            // Creacion del filosofo con sus tenedores
            filosofos[i] = new Filosofo($"Filósofo {i + 1}", tenedorIzquierdo, tenedorDerecho);

            // creacion e inicio de los hilos
            var hiloFilosofo = new Thread(filosofos[i].CicloDeVida);
            hiloFilosofo.Start();
        }

        Console.WriteLine("Comienzo de la cena");
        Console.ReadLine();
    }
}
public class Tenedor
{
    public int Id { get; }

    private readonly object lockObject = new object();

    public Tenedor(int id)
    {
        Id = id;
    }
    public void Tomar()
    {
        Monitor.Enter(lockObject);
    }
    public void Dejar()
    {
        Monitor.Exit(lockObject);
    }
}

public class Filosofo
{
    public string Nombre { get; }
    private readonly Tenedor _tenedorIzquierdo;
    private readonly Tenedor _tenedorDerecho;
    private readonly Random _random = new Random();
    private readonly Tenedor _primerTenedor;
    private readonly Tenedor _segundoTenedor;

    public Filosofo(string nombre, Tenedor tenedorIzquierdo, Tenedor tenedorDerecho)
    {
        Nombre = nombre;
        _tenedorIzquierdo = tenedorIzquierdo;
        _tenedorDerecho = tenedorDerecho;

        // ordenar los recursos (tenedores) por su ID.
        if (tenedorIzquierdo.Id < tenedorDerecho.Id)
        {
            _primerTenedor = tenedorIzquierdo;
            _segundoTenedor = tenedorDerecho;
        }
        else
        {
            _primerTenedor = tenedorDerecho;
            _segundoTenedor = tenedorIzquierdo;
        }
    }

    public void CicloDeVida()
    {
        while (true)
        {
            Pensar();
            Comer();
        }
    }

    private void Pensar()
    {
        Console.WriteLine($"{Nombre} está pensando.");
        Thread.Sleep(_random.Next(1000, 5000)); 
        Console.WriteLine($"{Nombre} tiene hambre.");
    }

    private void Comer()
    {
        Console.WriteLine($"{Nombre} intenta tomar el tenedor {_primerTenedor.Id}.");
        _primerTenedor.Tomar();
        Console.WriteLine($"{Nombre} tomó el tenedor {_primerTenedor.Id}.");

        Console.WriteLine($"{Nombre} intenta tomar el tenedor {_segundoTenedor.Id}.");
        _segundoTenedor.Tomar();
        Console.WriteLine($"{Nombre} tomó el tenedor {_segundoTenedor.Id}.");

        Console.WriteLine($"{Nombre} está comiendo.");
        Thread.Sleep(_random.Next(1000, 3000));

        Console.WriteLine($"{Nombre} terminó de comer. Deja los tenedores.");
        _primerTenedor.Dejar();
        _segundoTenedor.Dejar();
    }
}

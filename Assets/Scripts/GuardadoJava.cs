using UnityEngine;
using System.IO;
using System.Net.Sockets;

public class GuardadoJava
{
    // ============================
    // GUARDAR POSICIÓN EN MYSQL
    // ============================
    public static void Guardar(float x, float y)
    {
        try
        {
            TcpClient cliente = new TcpClient("127.0.0.1", 5000);
            StreamWriter sw = new StreamWriter(cliente.GetStream());
            sw.AutoFlush = true;

            string ordenador = SystemInfo.deviceName;

            // Formato: guardar;PC;X;Y
            sw.WriteLine("guardar;" + ordenador + ";" + x + ";" + y);

            cliente.Close();
        }
        catch (System.Exception e)
        {
            Debug.Log("Error guardando: " + e.Message);
        }
    }

    // ============================
    // CARGAR POSICIÓN DESDE MYSQL
    // ============================
    public static Vector2 Cargar()
    {
        try
        {
            TcpClient cliente = new TcpClient("127.0.0.1", 5000);
            StreamWriter sw = new StreamWriter(cliente.GetStream());
            StreamReader sr = new StreamReader(cliente.GetStream());
            sw.AutoFlush = true;

            string ordenador = SystemInfo.deviceName;

            // Formato: cargar;PC
            sw.WriteLine("cargar;" + ordenador);

            string respuesta = sr.ReadLine();
            cliente.Close();

            // Respuesta: X;Y
            string[] partes = respuesta.Split(';');
            float x = float.Parse(partes[0]);
            float y = float.Parse(partes[1]);

            return new Vector2(x, y);
        }
        catch (System.Exception e)
        {
            Debug.Log("Error cargando: " + e.Message);
            return Vector2.zero;
        }
    }
}

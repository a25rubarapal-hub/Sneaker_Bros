using UnityEngine;
using System.Diagnostics;
using System.IO;

public static class GuardadoJava
{
    private static string EjecutarJava(string argumentos)
    {
        Process p = new Process();
        p.StartInfo.FileName = "java";
        p.StartInfo.Arguments = "-cp .;mysql-connector.jar JavaMySQL " + argumentos;

        p.StartInfo.RedirectStandardOutput = true;
        p.StartInfo.UseShellExecute = false;
        p.StartInfo.CreateNoWindow = true;

        p.Start();

        string salida = p.StandardOutput.ReadLine();
        return salida;
    }

    public static void Guardar(float x, float y)
    {
        EjecutarJava("guardar " + x + " " + y);
    }

    public static Vector2 Cargar()
    {
        string salida = EjecutarJava("cargar");

        if (string.IsNullOrEmpty(salida))
            return Vector2.zero;

        string[] datos = salida.Split(',');

        float x = float.Parse(datos[0]);
        float y = float.Parse(datos[1]);

        return new Vector2(x, y);
    }
}

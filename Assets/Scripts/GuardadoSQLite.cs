using UnityEngine;
using SQLite4Unity3d;

public class GuardadoSQLite
{
    private static SQLiteConnection db;

    public static void Init()
    {
        string ruta = System.IO.Path.Combine(Application.streamingAssetsPath, "guardado.db");
        db = new SQLiteConnection(ruta, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);
    }

    public static void Guardar(float x, float y)
    {
        string ordenador = SystemInfo.deviceName;

        var existente = db.Table<PosicionJugador>().Where(p => p.ordenador == ordenador).FirstOrDefault();

        if (existente == null)
        {
            db.Insert(new PosicionJugador { ordenador = ordenador, posicionX = x, posicionY = y });
        }
        else
        {
            existente.posicionX = x;
            existente.posicionY = y;
            db.Update(existente);
        }
    }

    public static Vector2 Cargar()
    {
        string ordenador = SystemInfo.deviceName;

        var existente = db.Table<PosicionJugador>().Where(p => p.ordenador == ordenador).FirstOrDefault();

        if (existente == null)
            return Vector2.zero;

        return new Vector2(existente.posicionX, existente.posicionY);
    }
}

public class PosicionJugador
{
    [PrimaryKey, AutoIncrement]
    public int id { get; set; }
    public string ordenador { get; set; }
    public float posicionX { get; set; }
    public float posicionY { get; set; }
}

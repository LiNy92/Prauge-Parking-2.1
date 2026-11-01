using Prague_Parking_Library;
using System.Diagnostics;
using System.Text.Json;

namespace Prague_Parking_2._0;

public class Konfigurationshanterare
{
    private static readonly string Filnamn = Path.Combine(AppContext.BaseDirectory, "config.json");

    public static AppConfig LäsaJsonFil()
    {
        if (!File.Exists(Filnamn))
        {
            Console.Error.WriteLine("config.json saknas. Standardinställningar används.");
            return new AppConfig();
        }

        try
        {
            string jsonString = File.ReadAllText(Filnamn);
            return JsonSerializer.Deserialize<AppConfig>(jsonString) ?? new AppConfig();
        }
        catch (JsonException)
        {
            Console.Error.WriteLine("config.json innehåller ogiltig JSON. Standardinställningar används.");
            return new AppConfig();
        }
        catch (IOException)
        {
            Console.Error.WriteLine("config.json kunde inte läsas. Standardinställningar används.");
            return new AppConfig();
        }
        catch (UnauthorizedAccessException)
        {
            Console.Error.WriteLine("Programmet saknar behörighet att läsa config.json. Standardinställningar används.");
            return new AppConfig();
        }
    }

    public static AppConfig ÖppnaOchLäsJsonFil()
    {
        var startInfo = new ProcessStartInfo("notepad.exe")
        {
            UseShellExecute = false
        };
        startInfo.ArgumentList.Add(Filnamn);

        if (Process.Start(startInfo) == null)
        {
            throw new InvalidOperationException("Kunde inte öppna config.json i Anteckningar.");
        }

        Console.WriteLine("Spara filen och tryck Enter för att läsa in ändringarna.");
        Console.WriteLine(Filnamn);
        Console.ReadLine();
        return LäsaJsonFil();
    }
}

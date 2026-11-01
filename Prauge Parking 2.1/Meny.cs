using Prague_Parking_Library;
using Spectre.Console;

namespace Prague_Parking_2._0;

public class Meny
{
    public void StartaMeny(AppConfig config)
    {

        var garage = new ParkeringsGarage(config);
        var parkeringsvakt = new ParkeringsVakt();

        //DataLagring.TestDataOmFilSaknas(garage, config);
        DataLagring.LaddaGarage(garage, config);

        while (true)
        {
            var menyVal = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[LightPink3][bold]=== Meny - Prauge Parking ===[/][/]")
                    .HighlightStyle(new Style(foreground: Color.LightPink3))
                    .AddChoices(
                    "Checka in fordon",
                    "Checka ut fordon",
                    "Söka fordon",
                    "Flytta fordon",
                    "Visa parkeringsgarage",
                    "Administration",
                    "Avsluta program"));

            AnsiConsole.MarkupLine($"Du har valt: [palegreen3_1]{menyVal}[/]");

            switch (menyVal)
            {
                case "Checka in fordon":
                    {
                        parkeringsvakt.Incheckning(garage, config);
                        DataLagring.SparaGarage(garage);
                        break;
                    }
                case "Checka ut fordon":
                    {
                        parkeringsvakt.Utcheckning(garage, config);
                        DataLagring.SparaGarage(garage);
                        break;
                    }
                case "Söka fordon":
                    {
                        parkeringsvakt.SökaFordonOchPlats(garage);
                        break;
                    }
                case "Flytta fordon":
                    {
                        parkeringsvakt.FlyttaFordon(garage);
                        DataLagring.SparaGarage(garage);
                        break;
                    }
                case "Visa parkeringsgarage":
                    {
                        parkeringsvakt.VisaParkeringsGarage(garage);
                        break;
                    }
                case "Administration":
                    {
                        var val = AnsiConsole.Prompt(
                            new SelectionPrompt<string>()
                                .Title("[LightPink3]=== Administration ===[/]")
                                .AddChoices("Uppdatera konfigurationsfilen", "Avbryt"));

                        if (val == "Uppdatera konfigurationsfilen")
                        {
                            try
                            {
                                DataLagring.SparaGarage(garage);
                                var nyConfig = Konfigurationshanterare.ÖppnaOchLäsJsonFil();
                                var nyttGarage = new ParkeringsGarage(nyConfig);
                                DataLagring.LaddaGarage(nyttGarage, nyConfig);
                                config = nyConfig;
                                garage = nyttGarage;
                                AnsiConsole.MarkupLine("[LightGreen]Konfigurationen har uppdaterats.[/]");
                            }
                            catch (Exception ex)
                            {
                                AnsiConsole.MarkupLine($"[LightCoral]Konfigurationen kunde inte läsas in: {ex.Message}[/]");
                            }
                        }
                        break;
                    }
                case "Avsluta program":
                    {
                        DataLagring.SparaGarage(garage);
                        return;
                    }
            }
            Console.WriteLine();
            AnsiConsole.Prompt(
    new TextPrompt<string>("Tryck [PaleGreen3_1]Enter[/] för att återgå till meny.")
    .AllowEmpty());
            Console.WriteLine();
        }
    }
}
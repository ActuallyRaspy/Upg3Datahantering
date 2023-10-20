using System.Globalization;
using System.Net;
bool appRun = true;
Uppgift[] uppgifter = new Uppgift[0]; //Skapa en tom Uppgift array för att lagra uppgifter 
int antalUppgifter = uppgifter.Length; // räknare 
LäsaFrånFil(); // kallar på metoden vid uppstart där den antigen skapar csv.filen om den inte finns eller läser upp befintlig fil

Console.WriteLine("Välkommen till ToDo-listan");

while (appRun) // Huvudprogrammet, appRun flaggan bestämmer om programmet körs
{
    string? användareInmatning = "";

    while (!valideraInput(användareInmatning, 5) && appRun)
    {
        Console.WriteLine("\nVälj åtgärd: " +
        "\n1. Lägg till ny uppgift" +
        "\n2. Markera uppgift som klar" +
        "\n3. Visa aktuella uppgifter" +
        "\n4. Avsluta program");
        användareInmatning = Console.ReadLine();

        if (valideraInput(användareInmatning, 5))
        {
            switch (användareInmatning) // Show list
            {
                case "1": // Lägg till uppgift
                    LäggTillNyUppgift();
                    LäggTillFil();
                    break;

                case "2": // Markera uppgift som klar
                    if (uppgifter.Length == 0)
                    {
                        Console.WriteLine("Inga uppgifter att markera");
                        break;
                    }
                    else
                    {

                        if (MarkeraUppgiftSomKlar() < 0)
                        {
                            break;
                        }
                        else
                            LäggTillFil();
                        break;
                    }

                case "3": //Visa uppgift
                    if (uppgifter.Length == 0)
                    {
                        Console.WriteLine("Inga uppgifter, snälla lägg till!");
                        break;
                    }
                    else
                    {
                        VisaUppgifter();
                        användareInmatning = null;
                        break;
                    }

                case "4": // stäng av program

                    while (!valideraInput(användareInmatning, 4))
                    {
                        Console.WriteLine("Are you sure you want to exit? Y/N");
                        användareInmatning = Console.ReadLine().ToLower();
                        if (användareInmatning == "y")
                        {
                            appRun = false;
                        }

                        else if (användareInmatning == "n")
                        {
                            användareInmatning = null;
                            break;
                        }
                    }
                    break;
            }
        }
        else
        {
            Console.WriteLine("Ogiltig inmatning!");
        }
    }

}

void LäggTillNyUppgift() //Lägger till ny uppgift i uppgift vektorn
{
    antalUppgifter++;
    Array.Resize(ref uppgifter, antalUppgifter);
    Uppgift uppgift = new Uppgift();
    string? inmatning = null;

    while (!valideraInput(inmatning, 1)) // Beskriv uppgiften
    {
        Console.WriteLine("Skriv uppgiftsbeskrivning: ");
        inmatning = Console.ReadLine();

        if (valideraInput(inmatning, 1)) // Skriv uppgiftsbeskrivning 
        {
            uppgift.Task = inmatning;
        }

        else
        {
            Console.WriteLine("Ogiltig inmatning!");
            continue;
        }

        while (!valideraInput(inmatning, 2)) // Skriv deadline
        {

            Console.WriteLine("Skriv datum i formatet 2023-01-01: ");
            inmatning = Console.ReadLine();

            if (valideraInput(inmatning, 2))
            {
                uppgift.Deadline = DateTime.Parse(inmatning);
            }
            else
            {
                Console.WriteLine("Ogiltig inmatning!");
                continue;
            }
        }

        while (!valideraInput(inmatning, 3)) // Skriv tidsåtgången
        {
            Console.WriteLine("Skriv tidsåtgång: ");
            inmatning = Console.ReadLine();

            if (valideraInput(inmatning, 3))
            {
                uppgift.EstimatedHours = double.Parse(inmatning);
            }
        }
        uppgift.IsCompleted = false;
        uppgifter[antalUppgifter - 1] = uppgift;
        SorteraEfterDatum();
    }
}

void VisaUppgifter() // Visa uppgifter i uppgift vektorn 
{
    Console.WriteLine("Dina aktuella uppgifter:\n");
    Console.WriteLine($"{"ID",-6} {"Deadline",-14} {"Tid",-8} {"Vad"}");
    for (int i = 0; i < uppgifter.Length; i++)
    {

        SättFärg(uppgifter[i].Deadline, uppgifter[i].IsCompleted);
        Console.WriteLine($"{i + 1 + ".",-6} {uppgifter[i].Deadline.ToShortDateString(),-14} {uppgifter[i].EstimatedHours + "h",-9}" +
        $"{uppgifter[i].Task}");

        Console.ForegroundColor = ConsoleColor.White;
    }
}

void LäggTillFil() // skriver till .csv-filen
{
    StreamWriter streamWriter = new StreamWriter("UppgiftLista.csv");

    for (int i = 0; i < uppgifter.Length; i++)
    {
        streamWriter.WriteLine(uppgifter[i].Task + ";" +
        uppgifter[i].Deadline.ToString("yyyy'-'MM'-'dd") + ";" +
        uppgifter[i].EstimatedHours + ";" +
        uppgifter[i].IsCompleted);
    }
    streamWriter.Close();
}

void LäsaFrånFil() // läser från .csv filen
{

    bool file = File.Exists("UppgiftLista.csv");
    if (file) // om filen  existerar 
    {

        StreamReader streamReader1 = new StreamReader("UppgiftLista.csv");
        string? textLinje;

        while ((textLinje = streamReader1.ReadLine()) != null)
        {
            string[] splitText = textLinje.Split(";");
            antalUppgifter++;
            Array.Resize(ref uppgifter, antalUppgifter);
            Uppgift nyUppgift = new Uppgift();
            nyUppgift.Task = splitText[0];
            nyUppgift.Deadline = DateTime.Parse(splitText[1]);
            nyUppgift.EstimatedHours = double.Parse(splitText[2]);
            nyUppgift.IsCompleted = bool.Parse(splitText[3]);
            uppgifter[antalUppgifter - 1] = nyUppgift;
        }
        streamReader1.Close();
        SorteraEfterDatum();
    }
    else
    {
        File.Create("UppgiftLista.csv").Close();

    }
}

int MarkeraUppgiftSomKlar()
{
    int uppgift = VäljUppgift();
    if (uppgift < 0)
    {
        return uppgift;
    }
    else
    {
        uppgifter[uppgift].IsCompleted = true;
        return uppgift;
    }

}

void SättFärg(DateTime deadline, bool complete) //färghantering beorende på datum och slutföringsstatus (isCompleted)
{
    DateTime today = DateTime.Now;
    DateTime threeDays = today.AddDays(3);

    if (complete)
    {
        Console.ForegroundColor = ConsoleColor.DarkGray;
        return;
    }

    else if (DateTime.Now > deadline)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        return;
    }

    else if (deadline < threeDays)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        return;
    }
}

int VäljUppgift() // väljer uppgift i vektorn utifrån användarens input
{
    VisaUppgifter();
    Console.Write("Välj uppgift (skriv 0 för att ): ");
    int användarInput;
    while (true)
    {
        if (int.TryParse(Console.ReadLine(), out användarInput))
        {
            if (användarInput == 0)
            {
                break;
            }
            if (användarInput - 1 < uppgifter.Length && användarInput - 1 >= 0)
            {
                break;
            }
            else
            {
                Console.WriteLine($"Det finns ingen uppgift på plats {användarInput}");
            }
        }
        else
        {
            Console.WriteLine("Kunde inte bearbeta svaret, försök igen:");
        }
    }
    return användarInput - 1;
}

void SorteraEfterDatum()
{
    uppgifter = uppgifter.OrderBy(x => x.Deadline).ToArray();
}

bool valideraInput(string? input, int type)// Validera användarens input och returnera true eller false
{
    switch (type) //Depending on the type of validation needed, select the appropriate case
    {
        case 1: //Input is going to be assigned to Task
            if (!string.IsNullOrWhiteSpace(input) && input.Length > 0 && !input.Contains(";"))
            {
                return true;
            }
            else return false;

        case 2: // Input is going to be assigned to Deadline
            if (!string.IsNullOrWhiteSpace(input) && DateTime.TryParse(input, out DateTime dt))
            {
                return true;
            }
            else return false;

        case 3: // Input is going to be assigned to EstimatedHours
            if (!string.IsNullOrWhiteSpace(input) && !input.Contains(" ") && double.TryParse(input, out double d))
            {
                return true;
            }
            else return false;

        case 4: // Input is going to be assigned to IsCompleted
            if (!string.IsNullOrWhiteSpace(input) && (input.ToLower() == "y" || input.ToLower() == "n"))
            {
                return true;
            }
            else return false;

        case 5: // Input is going to be assigned to 
            string[] str = { "1", "2", "3", "4", "5" };
            if (!string.IsNullOrWhiteSpace(input) && str.Any(input.Contains) && input.Length == 1)
            {
                return true;
            }
            else return false;
    }
    return false;
}

struct Uppgift
{
    public string Task;
    public DateTime Deadline;
    public double EstimatedHours;
    public bool IsCompleted;
}
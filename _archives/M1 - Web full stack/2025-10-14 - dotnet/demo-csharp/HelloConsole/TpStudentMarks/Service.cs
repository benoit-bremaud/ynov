namespace HelloConsole.TpStudentMarks;

using System.Globalization; // for CultureInfo


public class Service
{
    public int AskStudentCount()
    {
        while (true)
        {
            Console.Write("Combien d'élèves ? ");
            var input = Console.ReadLine();
            if (int.TryParse(input, out var n) && n >= 1)
            {
                // Console.WriteLine($"Nombre d'élèves : {n}");
                return n;
            }
            Console.WriteLine("⚠️  Entrez un nombre entier positif (≥ 1).");
        }
    }


    public List<string> CollectNames(int count)
    {
        var names = new List<string>(capacity: count);

        for (int i = 1; i <= count; i++)
        {
            while (true)
            {
                Console.Write($"Nom de l'élève {i}/{count} : ");
                var raw = Console.ReadLine();
                var name = (raw ?? string.Empty).Trim();

                if (!string.IsNullOrEmpty(name))
                {
                    names.Add(name);
                    break;
                }

                Console.WriteLine("⚠️  Le nom ne peut pas être vide. Réessayez.");
            }
        }

        return names;
    }


    public double AskMarkFor(string name)
    {
        while (true)
        {
            Console.Write($"Entrez la note de {name} (0–20) : ");
            var raw = Console.ReadLine()?.Trim();
            if (string.IsNullOrEmpty(raw))
            {
                Console.WriteLine("⚠️  La note ne peut pas être vide.");
                continue;
            }

            var normalized = raw.Replace(',', '.');
            if (double.TryParse(normalized, CultureInfo.InvariantCulture, out var mark)
                && mark >= 0 && mark <= 20)
            {
                return mark;
            }

            Console.WriteLine("⚠️  Entrée invalide. Veuillez entrer une note entre 0 et 20.");
        }
    }   

    public List<(string Name, double Mark)> CollectMarksForNames(List<string> names)
    {
        var list = new List<(string Name, double Mark)>(names.Count);
        foreach (var name in names)
        {
            var mark = AskMarkFor(name);
            list.Add((name, mark));
        }
        return list;
    }

    public double AverageMark(IEnumerable<(string Name, double Mark)> students)
    {
        var data = students as IList<(string Name, double Mark)> ?? students.ToList();
        return data.Count == 0 ? 0 : data.Average(s => s.Mark);
    }

    public void PrintSummary(IEnumerable<(string Name, double Mark)> students, double average)
    {
        Console.WriteLine("\nRécapitulatif :");
        foreach (var s in students)
            Console.WriteLine($" - {s.Name,-15} {s.Mark,5:0.00}");

        Console.WriteLine($"\nMoyenne de classe : {average:0.00}/20");
    }

    public record Student(string Name, double Mark);
    
}


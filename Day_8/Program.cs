using System.Text.RegularExpressions;

var points = new InputProvider<Point3D?>("Input.txt", GetPont3D).Where(w => w != null).Cast<Point3D>().ToList();

Dictionary<(Point3D, Point3D), double> distances = new();

foreach (var point1 in points)
{
    foreach (var point2 in points)
    {
        if (distances.ContainsKey((point1, point2)))
            continue;
        if (distances.ContainsKey((point2, point1)))
            continue;

        var distance = Distance(point1, point2);
        distances[(point1, point2)] = distance;
    }
}

var pairs = distances.Where(w => w.Value > 0).OrderBy(w => w.Value).Select(w => w.Key).ToList();

List<HashSet<Point3D>> circuits = points.Select(w => new HashSet<Point3D>() { w }).ToList();
List<(Point3D, Point3D)> directConnections = new();

for (int i = 0; true; i++)
{
    if (i == 1001)
    {
        var largest = circuits.OrderByDescending(w => w.Count).Take(3).ToList();

        Console.WriteLine($"Part 1: {largest[0].Count * largest[1].Count * largest[2].Count}");
    }

    var (point1, point2) = pairs[i];

    directConnections.Add((point1, point2));

    var circuit1 = circuits.First(c => c.Contains(point1));
    var circuit2 = circuits.First(c => c.Contains(point2));

    if (circuit1 == circuit2)
    {
        continue;
    }
    else
    {
        // move all points from circuit 2 to circuit1
        foreach(var point in circuit2)
        {
            circuit1.Add(point);
        }
        circuits.Remove(circuit2);

        if (circuits.Count == 1)
        {
            Console.WriteLine($"Part 2: {(long)point1.X * (long)point2.X}");
            break;
        }
    }
}

static bool GetPont3D(string? input, out Point3D? value)
{
    value = null;

    if (input == null) return false;

    Regex numRegex = new(@"-?\d+");

    var numbers = numRegex.Matches(input).Select(w => int.Parse(w.Value)).ToList();

    if (numbers.Count != 3) throw new Exception();

    value = new Point3D(numbers[0], numbers[1], numbers[2]);

    return true;
}

static double Distance(Point3D point, Point3D other)
{
    return Math.Sqrt(
        Math.Pow(point.X - other.X, 2) +
        Math.Pow(point.Y - other.Y, 2) +
        Math.Pow(point.Z - other.Z, 2));
}

record Point3D(int X, int Y, int Z);
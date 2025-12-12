var factory = new UniqueFactory<string, Device>(w => new Device(w, new List<Device>()));

var devices = new InputProvider<Device?>("Input.txt", GetDevice).Where(w => w != null).Cast<Device>().ToList();

var memCache = new Dictionary<string, long>();

var start = factory.GetInstanceOrThrow("you");
var end = factory.GetInstanceOrThrow("out");

var count = GetPathsBFSCount(start, end, string.Empty);

Console.WriteLine($"Part 1: {count}");

start = factory.GetInstanceOrThrow("svr");
var mandatory1 = factory.GetInstanceOrThrow("fft");
var mandatory2 = factory.GetInstanceOrThrow("dac");


var part1 = GetPathsBFSCount(start, mandatory1, string.Empty);
var part2 = GetPathsBFSCount(mandatory1, mandatory2, string.Empty);
var part3 = GetPathsBFSCount(mandatory2, end, string.Empty);
Console.WriteLine($"Part 2: {part1 * part2 * part3}");


long GetPathsBFSCount(Device current, Device goal, string pathHere)
{
    var key = $"{current.Name}-{goal.Name}";

    if (!memCache.ContainsKey(key))
    {
        memCache[key] = GetPathsBFSCount_Internal(current, goal, pathHere);
    }

    return memCache[key];

    long GetPathsBFSCount_Internal(Device current, Device goal, string pathHere)
    {
        if (current == goal)
        {
            return 1;
        }

        var newPath = pathHere + "," + current.Name;

        long sum = 0;

        foreach (var next in current.Outputs)
        {
            if (pathHere.Contains(next.Name))
                continue;

            sum += GetPathsBFSCount(next, goal, newPath);
        }

        return sum;
    }
}

bool GetDevice(string? input, out Device? value)
{
    value = null;

    if (input == null) return false;

    var parts = input.Split([" ", ":"], StringSplitOptions.RemoveEmptyEntries);

    value = factory.GetOrCreateInstance(parts[0]);

    for (var i = 1; i < parts.Length; i++)
    {
        value.Outputs.Add(factory.GetOrCreateInstance(parts[i]));
    }

    return true;
}

record Device(string Name, List<Device> Outputs);
using System.Text.RegularExpressions;

var machines = new InputProvider<Machine?>("Input.txt", GetMachine).Where(w => w != null).Cast<Machine>().ToList();

int sum = 0;

foreach (var machine in machines)
{
    var path = AStarPathfinder.FindPath(new MachineLightState(machine), MachineLightState.CreateEndState(machine), w => 0, MachineLightState.GetNewStates);

    sum += path.Count - 1;
}

Console.WriteLine(sum);

sum = 0;

MachineCounterState endState;

foreach (var machine in machines)
{
    endState = MachineCounterState.CreateEndState(machine);
    var path = AStarPathfinder.FindPath(new MachineCounterState(machine), endState, GetMachineStateHeuristicScore, MachineCounterState.GetNewStates);

    sum += path.Count - 1;

    Console.WriteLine(sum);
}

Console.WriteLine(sum);


static bool GetMachine(string? input, out Machine? value)
{
    value = null;

    if (input == null) return false;

    Regex numRegex = new(@"-?\d+");

    int indexOfCloseLights = input.IndexOf("]");
    int indexOfStartJoltage = input.IndexOf("{");

    var lights = input[1..indexOfCloseLights].Select(w => w == '.' ? 0 : 1).ToArray();

    List<int[]> buttons = new List<int[]>();
    var buttonsStrings = input[(indexOfCloseLights + 2)..(indexOfStartJoltage)].Split(" ", StringSplitOptions.RemoveEmptyEntries);

    foreach (var buttonStr in buttonsStrings)
    {
        var numbers = numRegex.Matches(buttonStr).Select(w => int.Parse(w.Value)).ToArray();
        buttons.Add(numbers);
    }

    var joltage = numRegex.Matches(input[(indexOfStartJoltage + 1)..^1]).Select(w => int.Parse(w.Value)).ToArray();

    value = new Machine(lights, buttons, joltage);
    
    return true;
}

int? GetMachineStateHeuristicScore(MachineCounterState state)
{
    if (state.NumState.Length != endState.NumState.Length)
        throw new Exception();

    for (int i = 0; i < endState.NumState.Length; i++)
    {
        if (state.NumState[i] > endState.NumState[i])
            return null;
    }

    return Enumerable.Range(0, state.NumState.Length).Select(w => endState.NumState[w] - state.NumState[w]).Max();
}

record Machine(int[] LightsGoal, List<int[]> Buttons, int[] JoltageRequirements);

class MachineLightState : INode, IEquatable<MachineLightState>
{
    private readonly Machine machine;

    public string CurrentState { get; }

    public MachineLightState(Machine machine)
    {
        this.machine = machine;

        this.CurrentState = new string(machine.LightsGoal.Select(w => '.').ToArray());
    }

    public MachineLightState(int[] button, MachineLightState previous)
    {
        this.machine = previous.machine;

        var state = previous.CurrentState.ToCharArray();

        foreach (var lightIndex in button)
        {
            state[lightIndex] = state[lightIndex] switch
            {
                '.' => '#',
                '#' => '.',
                _ => throw new Exception()
            }; 
        }

        this.CurrentState = new string(state);
    }

    public int Cost => 1;

    public static MachineLightState CreateEndState(Machine machine)
    {
        return new MachineLightState(new string(machine.LightsGoal.Select(w => w == 1 ? '#' : '.').ToArray()));
    }

    private MachineLightState(string state)
    {
        this.CurrentState = state;
    }

    public static IEnumerable<MachineLightState> GetNewStates(MachineLightState current)
    {
        foreach (var button in current.machine.Buttons)
        {
            yield return new MachineLightState(button, current);
        }
    }

    public bool Equals(MachineLightState? other)
    {
        if (other == null)
            return false;

        return this.CurrentState.Equals(other.CurrentState);
    }

    public override bool Equals(object? obj)
    {
        if (obj is MachineLightState other)
        {
            return Equals(other);
        }
        else return false;
    }

    public override int GetHashCode()
    {
        return this.CurrentState.GetHashCode();
    }
}

class MachineCounterState : INode, IEquatable<MachineCounterState>
{
    private readonly Machine machine;

    public string CurrentState { get; }
    public int Cost => 1;

    public int Total { get; }

    public int[] NumState { get; }

    public MachineCounterState(Machine machine)
    {
        this.machine = machine;

        this.NumState = Enumerable.Repeat(0, machine.JoltageRequirements.Length).ToArray();
        this.CurrentState = string.Join(",", NumState.Select(w => w.ToString()));
    }

    public MachineCounterState(int[] button, MachineCounterState previous)
    {
        this.machine = previous.machine;

        this.NumState = previous.CurrentState.Split(",").Select(w => int.Parse(w)).ToArray();

        foreach (var lightIndex in button)
        {
            this.NumState[lightIndex]++;
        }

        this.CurrentState = string.Join(",", this.NumState.Select(w => w.ToString()));

        this.Total = this.NumState.Sum();
    }

    
    public static MachineCounterState CreateEndState(Machine machine)
    {
        return new MachineCounterState(string.Join(",", machine.JoltageRequirements.Select(w => w.ToString())));
    }

    private MachineCounterState(string state)
    {
        this.CurrentState = state;
        this.NumState = state.Split(",").Select(w => int.Parse(w)).ToArray();
        this.Total = this.NumState.Sum();
    }

    public static IEnumerable<MachineCounterState> GetNewStates(MachineCounterState current)
    {
        foreach (var button in current.machine.Buttons)
        {
            yield return new MachineCounterState(button, current);
        }
    }

    public bool Equals(MachineCounterState? other)
    {
        if (other == null)
            return false;

        return this.CurrentState.Equals(other.CurrentState);
    }

    public override bool Equals(object? obj)
    {
        if (obj is MachineCounterState other)
        {
            return Equals(other);
        }
        else return false;
    }

    public override int GetHashCode()
    {
        return this.CurrentState.GetHashCode();
    }
}
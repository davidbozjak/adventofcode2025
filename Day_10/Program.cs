using Microsoft.Z3;
using System.Text.RegularExpressions;

var machines = new InputProvider<Machine?>("Input.txt", GetMachine).Where(w => w != null).Cast<Machine>().ToList();

int sum = 0;

foreach (var machine in machines)
{
    var path = AStarPathfinder.FindPath(new MachineLightState(machine), MachineLightState.CreateEndState(machine), w => 0, MachineLightState.GetNewStates);

    sum += path.Count - 1;
}

Console.WriteLine($"Part 1: {sum}");

sum = 0;

foreach (var machine in machines)
{
    sum += Z3Solve(machine);
}

Console.WriteLine($"Part 2: {sum}");

static int Z3Solve(Machine machine)
{
    // BFS approach works but the search space is too large. Solving with Z3 as it is describing a set of linear equations
    // Each equation is buttons that are pressed = joltage requirement
    // for example (3) (1,3) (2) (2,3) (0,2) (0,1) {3,5,4,7} becomes the set
    // 3 == v_4 + v_5
    // 5 == v_1 + v_5
    // 4 == v_2 + v_3 + v_4
    // 7 == v_0 + v_1 + v_3
    // we add a variable v_presses = v_0 + v_1 + v_2 + v_3 + v_4 + v_5 and then minimize it

    var context = new Context();

    var solver = context.MkOptimize();

    var presses = context.MkIntConst("v_presses");
    var constants = Enumerable.Range(0, machine.Buttons.Count).Select(w => context.MkIntConst($"v_{w}")).ToArray();
    
    foreach (var c in constants)
    {
        solver.Add(c >= 0);
    }

    for (int i = 0; i < machine.JoltageRequirements.Length; i++)
    {
        var equations = Enumerable.Range(0, machine.Buttons.Count).Where(w => machine.Buttons[w].Contains(i)).Select(w => constants[w]).ToArray();
        solver.Add(context.MkEq(context.MkAdd(equations), context.MkInt(machine.JoltageRequirements[i])));
    }

    solver.Add(context.MkEq(presses,context.MkAdd(constants)));
    
    solver.MkMinimize(presses);

    var status = solver.Check();

    if (status != Status.SATISFIABLE)
        throw new Exception();

    return ((IntNum)solver.Model.Evaluate(presses)).Int;
}

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

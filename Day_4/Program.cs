var map = new StringInputProvider("Input.txt").ToList();

var tileWorld = new TileWorld(map, true, GetTile);

var printer = new WorldPrinter();

printer.Print(tileWorld);

var loadTiles = tileWorld.WorldObjects.Where(w => w is LoadTile).Cast<LoadTile>().ToList();

Console.WriteLine($"Part 1: {loadTiles.Count(w => w.TraversibleNeighbours.Where(ww => ww is LoadTile).Count() < 4)}");

int noRemoved = 0;
int totalRemoved = 0;

do
{
    var toRemove = loadTiles
        .Where(w => !w.IsRemoved)
        .Where(w => w.TraversibleNeighbours.Count(ww => ww is LoadTile loadNeighbour && !loadNeighbour.IsRemoved) < 4)
        .ToList();

    toRemove.ForEach(w => w.IsRemoved = true);

    noRemoved = toRemove.Count;
    totalRemoved += noRemoved;
    Console.WriteLine($"Removed {noRemoved}, new total {totalRemoved}");
} while (noRemoved > 0);

Console.WriteLine($"Part 2: {totalRemoved}");


Tile GetTile(int x, int y, char c, Func<Tile, IEnumerable<Tile>> func)
{
    if (c == '@')
    {
        return new LoadTile(x, y, func);
    }
    else
    {
        return new Tile(x, y, true, func);
    }
}

class LoadTile : Tile
{
    public override char CharRepresentation => '@';

    public bool IsRemoved { get; set; }

    public LoadTile(int x, int y, Func<Tile, IEnumerable<Tile>> fillTraversibleNeighboursFunc)
        : base(x, y, true, fillTraversibleNeighboursFunc)
    {

    }
}
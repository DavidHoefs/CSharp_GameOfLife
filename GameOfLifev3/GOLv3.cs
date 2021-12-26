using GameOfLifeConsole;
using System.Collections;
using System.Diagnostics;




int ModuloOperation(int a, int b)
{
    var floatA = (float)a;
    var floatB = (float)b;
    return (int)(floatA - floatB * Math.Floor(floatA / floatB));
}

List<(int, int)> QueueCells(Hashtable gameMatrix, int numCols, int numRows)
{

    List<(int, int)> cells = new();
    int counter = 0;
    foreach (var cell in gameMatrix.Keys)
    {
        var coords = (ValueTuple<int,int>)cell;
        int x = coords.Item1;
        int y = coords.Item2;
        int size = numCols * numRows;
        int[] xCellsToCheck = new int[3];
        int[] yCellsToCheck = new int[3];


        
        

        xCellsToCheck[0] = ModuloOperation(x - 1, numCols);
        xCellsToCheck[1] = x;
        xCellsToCheck[2] = ModuloOperation(x + 1, numCols);
        yCellsToCheck[0] = ModuloOperation(y - 1, numRows);
        yCellsToCheck [1] = y;
        yCellsToCheck[2] = ModuloOperation(y + 1, numRows);
        foreach (var xCoord in xCellsToCheck)
        {
            foreach (var yCoord in yCellsToCheck)
            {
                var cellToCheck = (xCoord, yCoord);

                cells.Add(cellToCheck);


            }
        }
    }

    return cells;
}
List<List<Tuple<int, int>>> CheckCells(int x, int y, Hashtable matrix, int numCols, int numRows)
{


    List<int> xCellsToCheck = new();
    List<int> yCellsToCheck = new();

    xCellsToCheck.AddRange(new int[] { ModuloOperation(x - 1, numCols), x, ModuloOperation(x + 1, numCols) });
    yCellsToCheck.AddRange(new int[] { ModuloOperation(y - 1, numRows), y, ModuloOperation(y + 1, numRows) });

    int total = 0;
    foreach (var xCoord in xCellsToCheck)
    {
        foreach (var yCoord in yCellsToCheck)
        {
            if (matrix.ContainsKey((xCoord, yCoord)) && (xCoord, yCoord) != (x, y))
            {
                total++;

            }

        }
    }

    List<Tuple<int, int>> alive = new();
    List<Tuple<int, int>> dead = new();
    int cell = 0;

    if (matrix.ContainsKey((x, y)))
        cell = 1;
    else
        cell = 0;

    if (total % 2 == 0 && cell == 0 && total != 0)
    {
        var temp = Tuple.Create(x, y);

        alive.Add(temp);
    }
    else if ((total == 2 || total == 4 || total == 3) && cell == 1)
    {
        var temp = Tuple.Create(x, y);

        alive.Add(temp);
    }
    else if (cell == 1)
    {
        var temp = Tuple.Create(x, y);

        dead.Add(temp);
    }
    else
    {
        var temp = Tuple.Create(x, y);

        dead.Add(temp);
    }
    var output = new List<List<Tuple<int, int>>> { alive, dead };

    return output;
}

int iteration = 0;

// read in file to matrix
string inputFile = @"C:\Users\David Hoefs\source\repos\GameOfLifeConsole\GameOfLifeConsole\1000x1000step.dat";

FileHandler file = new("", "");

int[] gameSize = file.GetSize(inputFile);
Hashtable gameMatrix = new();
gameMatrix = file.ReadMatrixHashTable(inputFile);
Console.WriteLine();

  (int, int)[]? Alive = new (int, int)[gameSize[0]*gameSize[1]];
 (int, int)[]? Dead = new (int, int)[gameSize[0] * gameSize[1]];

Stopwatch stopwatch = Stopwatch.StartNew();
while (iteration < 100)
{
    
    //var queuedCells = QueueCells(gameMatrix,gameSize[0],gameSize[1]);
    int counter = 0;
    foreach (var cell in QueueCells(gameMatrix, gameSize[0], gameSize[1]))
    {
        counter++;
        var x = cell.Item1;
        var y = cell.Item2;
        var output = CheckCells(x, y, gameMatrix, gameSize[0], gameSize[1]);
        int aliveCounter = 0;
        int deadCounter = 0;
        foreach (var item in output[0])
        {
            //Alive = new (int, int)[output[0].Count];
            var coords = (Tuple<int, int>)item;
            int xAlive = coords.Item1;
            int yAlive = coords.Item2;
            Alive[aliveCounter] = (xAlive, yAlive);
            aliveCounter++;


        }

        foreach (var item in output[1])
        {
            //test.Dead = new (int, int)[output[1].Count];
            var coords = (Tuple<int, int>)item;
            int xAlive = coords.Item1;
            int yAlive = coords.Item2;
            Dead[deadCounter] = (xAlive, yAlive);
            deadCounter++;
        }

    }
    if (Dead.Length > 0)
    {
        foreach (var item in Dead)
        {
            gameMatrix.Remove((item.Item1, item.Item2));
        }
    }
    if (Alive.Length > 0)
    {
        foreach (var item in Alive)
        {
                if(!gameMatrix.ContainsKey((item.Item1, item.Item2)))
                    gameMatrix.Add((item.Item1, item.Item2), 1);
                
 
        }
    }








    Console.WriteLine($"Iteration: {iteration}");
    iteration++;

}

stopwatch.Stop();
Console.WriteLine($"Time : {stopwatch.ElapsedMilliseconds}");
Console.WriteLine("Final Iteration:");
// print matrix
//file.PrintHashtableToConsole(gameMatrix, gameSize[0], gameSize[1]);


struct Test
{
    public (int, int)[] Alive = null;
    public (int, int)[] Dead = null;
}
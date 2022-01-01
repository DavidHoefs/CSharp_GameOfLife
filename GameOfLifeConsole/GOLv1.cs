using GameOfLifeConsole;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

int ModuloOperation(int a, int b)
{
    var floatA = (float)a;
    var floatB = (float)b;
    return Convert.ToInt32(floatA - floatB * Math.Floor(floatA / floatB));
}

List<(int, int)> QueueCells(int[,] gameMatrix, int numCols, int numRows)
{
    List<(int, int)> cells = new();
    int counter = 0;
    foreach (var cell in gameMatrix)
    {
        for (int i = 0; i < gameMatrix.GetLength(0); i++)
        {
            for (int j = 0; j < gameMatrix.GetLength(1); j++)
            {
                int x = i;
                int y = j;
                int size = numCols * numRows;
                int[] xCellsToCheck = new int[3];
                int[] yCellsToCheck = new int[3];

                xCellsToCheck[0] = ModuloOperation(x - 1, numCols);
                xCellsToCheck[1] = x;
                xCellsToCheck[2] = ModuloOperation(x + 1, numCols);
                yCellsToCheck[0] = ModuloOperation(y - 1, numRows);
                yCellsToCheck[1] = y;
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
        }
    }

    return cells;
}
List<List<Tuple<int, int>>> CheckCells(int x, int y, int[,] matrix)
{
    var numCols = matrix.GetLength(0);
    var numRows = matrix.GetLength(1);

    List<int> xCellsToCheck = new();
    List<int> yCellsToCheck = new();

    xCellsToCheck.AddRange(new int[] { ModuloOperation(x - 1, numCols), x, ModuloOperation(x + 1, numCols) });
    yCellsToCheck.AddRange(new int[] { ModuloOperation(y - 1, numRows), y, ModuloOperation(y + 1, numRows) });

    int total = 0;
    int counter = 0;
    foreach (var xCoord in xCellsToCheck)
    {
        foreach (var yCoord in yCellsToCheck)
        {
            if (matrix[xCoord, yCoord] == 1 && (xCoord, yCoord) != (x, y))
            {
                total++;
            }
            counter++;
        }
    }

    List<Tuple<int, int>> alive = new();
    List<Tuple<int, int>> dead = new();
    int cell = 0;

    if (matrix[x, y] == 1)
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
int[,]? prevMatrix = null;
int[] gameSize = file.GetSize(inputFile);
int[,] gameMatrix = file.ReadMatrix(inputFile, gameSize[0], gameSize[1]);
prevMatrix = gameMatrix;

//Console.WriteLine($"Before Start: ");
//file.PrintMatrixToConsole(gameMatrix);
var watch = new Stopwatch();
watch.Start();

while (iteration < 100)
{
    IterationData test = new IterationData();

    for (int i = 0; i < gameMatrix.GetLength(0); i++)
    {
        for (int j = 0; j < gameMatrix.GetLength(1); j++)
        {

            var output = CheckCells(i, j, gameMatrix);
            foreach (var item in output[0])
            {
                test.Alive.Add(item);
            }
            foreach (var item in output[1])
            {
                test.Dead.Add(item);
            }
        }
    }

    if (test.Alive.Count != 0)
    {
        foreach (var update in test.Alive)
        {
            var coords = (Tuple<int, int>)update;
            gameMatrix[coords.Item1, coords.Item2] = 1;
        }
    }
    if (test.Dead.Count != 0)
    {
        foreach (var update in test.Dead)
        {
            var coords = (Tuple<int, int>)update;
            gameMatrix[coords.Item1, coords.Item2] = 0;
        }
    }
    Console.WriteLine($"Iteration: {iteration}");
    iteration++;
}
watch.Stop();

Console.WriteLine("Final Iteration:");
// print matrix
//file.PrintMatrixToConsole(gameMatrix);
file.PrintMatrixToFile(gameMatrix);
Console.WriteLine($"Execution Time: {watch.ElapsedMilliseconds} ms");
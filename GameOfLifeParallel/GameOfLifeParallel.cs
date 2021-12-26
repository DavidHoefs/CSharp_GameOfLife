using GameOfLifeConsole;
using System.Diagnostics;

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

async Task<List<List<Tuple<int, int>>>> CheckCellsAsync(int x, int[] y, int[][] matrix)
{
    var output = new List<List<Tuple<int, int>>>();

    return await Task.Factory.StartNew(() =>
    {
        foreach (var yCoord in y)
        {
            output.AddRange(CheckCells(x, yCoord, matrix));
        }

        //output.AddRange(CheckCells(x, y, matrix));
        return output;
    });
}
List<List<Tuple<int, int>>> CheckCells(int x, int y, int[][] matrix)
{
    var numCols = matrix.Length;
    var numRows = matrix.Length;

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
            if (matrix[xCoord][yCoord] == 1 && (xCoord, yCoord) != (x, y))
            {
                total++;
            }
            counter++;
        }
    }
    int[][] gameUpdate = new int[numRows][];
    List<Tuple<int, int>> alive = new();
    List<Tuple<int, int>> dead = new();
    int cell = 0;

    if (matrix[x][y] == 1)
        cell = 1;
    else
        cell = 0;

    if (total % 2 == 0 && cell == 0 && total != 0)
    {
        gameUpdate[x][y] = 1;
        var temp = Tuple.Create(x, y);
        alive.Add(temp);
    }
    else if ((total == 2 || total == 4 || total == 3) && cell == 1)
    {
        gameUpdate[x][y] = 1;
        var temp = Tuple.Create(x, y);
        alive.Add(temp);
    }
    else if (cell == 1)
    {
        gameUpdate[x][y] = 0;
        var temp = Tuple.Create(x, y);
        dead.Add(temp);
    }
    else
    {
        gameUpdate[x][y] = 0;
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
//int[,] gameMatrix = file.ReadMatrix(inputFile, gameSize[0], gameSize[1]);

int[][] matrix = file.ReadMatrixToJaggedArray(inputFile, gameSize[0], gameSize[1]);

var watch = new Stopwatch();
watch.Start();
const int NUMPARTS = 4;
int divider = matrix.Length / NUMPARTS;
int counter = 1;
//while (counter <= divider)
//{
//    for (int x = counter == 1 ? 0 : (divider * counter) - divider; x < matrix.Length; x++)
//    {
//        if(x == (counter == 1 ? divider : divider * counter) )
//        {
//            break;
//        }
//        for (int y = 0; y < gameSize[1]; y++)
//        {
//            Console.Write($"{x},{y} : ");
//        }
        
        
//    }
//    Console.WriteLine();
//    counter++;
//}


// Console.WriteLine();
while (iteration < 100)
{
    IterationData test = new IterationData();
    List<List<List<Tuple<int, int>>>> outputAll = new List<List<List<Tuple<int, int>>>>();

    Parallel.For(0, divider, i =>
    {
        for (int x = i == 0 ? 0 : (divider * i) - divider; x < matrix.Length; x++)
        {
            if (x == (i == 1 ? divider : divider * i))
            {
                counter++;
                break;
            }
            for (int y = 0; y < matrix.Length; y++)
            {
                var output = CheckCells(x, y, matrix);
                lock (outputAll)
                    outputAll.Add(output);
            }
        }
    });

    foreach (var record in outputAll)
    {
        if (record is not null)
        {
            if (record[0].Count > 0)
            {
                foreach (var alive in record[0])
                {
                    var coords = (Tuple<int, int>)alive;
                    matrix[coords.Item1][coords.Item2] = 1;
                }
            }

            if (record[1].Count > 0)
            {
                foreach (var dead in record[1])
                {
                    var coords = (Tuple<int, int>)dead;
                    matrix[coords.Item1][coords.Item2] = 0;
                }
            }
        }
    }

    Console.WriteLine($"Iteration: {iteration}");
    iteration++;
}
watch.Stop();
Console.WriteLine($"Execution Time: {watch.ElapsedMilliseconds} ms");
Console.WriteLine("Final Iteration:");
// print matrix
//file.PrintJaggedMatrixToConsole(matrix);
//file.PrintMatrixToFile(gameMatrix);
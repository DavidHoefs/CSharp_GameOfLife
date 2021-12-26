using GameOfLifeConsole;
using System.Diagnostics;




int ModuloOperation(int a, int b)
{
    var floatA = (float)a;
    var floatB = (float)b;
    return (int)(floatA - floatB * Math.Floor(floatA / floatB));
}

List<(int, int)> QueueCells(Dictionary<(int,int),int> gameMatrix,int numCols,int numRows)
{
    
    List<(int, int)> cells = new();
    foreach (var cell in gameMatrix.Keys)
    {
        
        int x = cell.Item1;
        int y = cell.Item2; 
        List<int> xCellsToCheck = new();
        List<int> yCellsToCheck = new();

        xCellsToCheck.AddRange(new int[] { ModuloOperation(x - 1, numCols  ), x, ModuloOperation(x + 1, numCols) });
        yCellsToCheck.AddRange(new int[] { ModuloOperation(y - 1, numRows), y, ModuloOperation(y + 1, numRows) });

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
List<List<Tuple<int, int>>> CheckCells(int x, int y,Dictionary<(int,int),int> matrix,int numCols,int numRows)
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
            try
            {
                if (matrix.ContainsKey((xCoord,yCoord)) && (xCoord, yCoord) != (x, y))
                {
                    total++;

                }
            }
            catch 
            {
                continue;
                
            }

        }
    }

    List<Tuple<int, int>> alive = new();
    List<Tuple<int, int>> dead = new();
    int cell = 0;

    if (matrix.ContainsKey((x,y)))
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
Dictionary<(int, int), int> gameMatrix = new Dictionary<(int, int), int>(gameSize[0] * gameSize[1]);
gameMatrix = file.ReadMatrixFaster(inputFile);
Console.WriteLine();

//Console.WriteLine($"Before Start: ");
//file.PrintMatrixToConsole(gameMatrix);
Stopwatch sw = Stopwatch.StartNew();

while (iteration < 100)
{
    Test test = new();
   //var queuedCells = QueueCells(gameMatrix,gameSize[0],gameSize[1]);
    int counter = 0;
    foreach (var cell in QueueCells(gameMatrix,gameSize[0],gameSize[1]))
    {
        counter++;
        var x = cell.Item1;
        var y = cell.Item2;
        var output = CheckCells(x, y, gameMatrix,gameSize[0],gameSize[1]);
        int aliveCounter = 0;
        int deadCounter = 0;
        foreach (var item in output[0])
        {
            test.Alive = new (int, int)[output[0].Count];
            var coords = (Tuple<int, int>)item;
            int xAlive = coords.Item1;
            int yAlive = coords.Item2;
            test.Alive[aliveCounter] = (xAlive, yAlive);
            aliveCounter++;

            
        }
        foreach (var item in output[1])
        {
            test.Dead = new (int, int)[output[1].Count];
            var coords = (Tuple<int, int>)item;
            int xAlive = coords.Item1;
            int yAlive = coords.Item2;
            test.Dead[deadCounter] = (xAlive, yAlive);
            deadCounter++;
        }
       
    }
    if(test.Dead.Length > 0)
    {
        foreach (var item in test.Dead)
        {
            gameMatrix.Remove((item.Item1, item.Item2));
        }
    }
    if(test.Alive.Length > 0)
    {
        foreach (var item in test.Alive)
        {
            gameMatrix.TryAdd((item.Item1, item.Item2), 1);
        }
    }








    Console.WriteLine($"Iteration: {iteration}");
    iteration++;
    
}

sw.Stop();
Console.WriteLine($"Time : {sw.ElapsedMilliseconds}");
//Console.WriteLine("Final Iteration:");
//// print matrix
//file.PrintDictionaryToConsole(gameMatrix,gameSize[0],gameSize[1]);


struct Test
{
    public (int,int)[] Alive;
    public (int,int)[] Dead;
}
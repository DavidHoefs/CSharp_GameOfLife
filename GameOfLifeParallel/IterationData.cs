internal class IterationData
{
    public List<Tuple<int, int>> Alive { get; set; } = new List<Tuple<int, int>>();
    public List<Tuple<int, int>> Dead { get; set; } = new List<Tuple<int, int>>();
}
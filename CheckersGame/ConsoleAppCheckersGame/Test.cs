using DAL;

namespace ConsoleAppCheckersGame;

public class Test
{
    private readonly IMovementsLogRepository _logsRepo;

    public void foo()
    { 
        var Logs = _logsRepo.GetLogsByGameId(40);
        var myLastMove = Logs.Count < 2 ? Logs.Last() : Logs.ToList()[Logs.Count - 2];
        Console.WriteLine(myLastMove.WhoMoved);
        Console.WriteLine(myLastMove.MovementFromX);
        Console.WriteLine(myLastMove.MovementFromY);
    }
}
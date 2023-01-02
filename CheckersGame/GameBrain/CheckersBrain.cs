using ProjectDomain;

namespace GameBrain;

public class CheckersBrain
{
    public List<EBoardPiece> checkers = new List<EBoardPiece>()
    {
        EBoardPiece.BlackSquareBlackChecker,
        EBoardPiece.BlackSquareWhiteChecker,
        EBoardPiece.BlackSquareBlackKing,
        EBoardPiece.BlackSquareWhiteKing
    };

    public List<EBoardPiece> blackCheckers = new List<EBoardPiece>()
    {
        EBoardPiece.BlackSquareBlackChecker,
        EBoardPiece.BlackSquareBlackKing,
    };

    public List<EBoardPiece> whiteCheckers = new List<EBoardPiece>()
    {
        EBoardPiece.BlackSquareWhiteChecker,
        EBoardPiece.BlackSquareWhiteKing,
    };
    
    private readonly CheckersState _state;

    public CheckersBrain(CheckersOption options, CheckersGameState? state)
    {
        if (state == null)
        {
            _state = new CheckersState();
            InitializeNewGame(options);
        }
        else
        {
            _state = System.Text.Json.JsonSerializer.Deserialize<CheckersState>(state.SerializedGameState)!;
        }
    }

    private void InitializeNewGame(CheckersOption options)
    {
        var boardWidth = options.Width;
        var boardHeight = options.Height;

        _state.GameBoard = new EBoardPiece[boardWidth][];

        for (int i = 0; i < boardWidth; i++)
        {
            _state.GameBoard[i] = new EBoardPiece[boardHeight];
        }

        var count = 0;

        var opponentsRows = new[] { 0, 1, 2 };
        var myRows = new[] { boardWidth - 3, boardWidth - 2, boardWidth - 1 };

        for (var i = 0; i < boardWidth; i++)
        {
            for (var j = 0; j < boardHeight; j++)
            {
                if (count % 2 == 0)
                {
                    _state.GameBoard[i][j] = EBoardPiece.WhiteSquare;
                    count++;
                }
                else
                {
                    if (opponentsRows.Contains(j))
                    {
                        _state.GameBoard[i][j] = EBoardPiece.BlackSquareBlackChecker;
                        count++;
                    }
                    else if (myRows.Contains(j))
                    {
                        _state.GameBoard[i][j] = EBoardPiece.BlackSquareWhiteChecker;
                        count++;
                    }
                    else
                    {
                        _state.GameBoard[i][j] = EBoardPiece.BlackSquare;
                        count++;
                    }
                }
            }
            count++;
        }
    }

    public EBoardPiece[][] GetBoard()
    {
        var jsonStr = System.Text.Json.JsonSerializer.Serialize(_state.GameBoard);
        return System.Text.Json.JsonSerializer.Deserialize<EBoardPiece[][]>(jsonStr)!;
    }

    public string GetSerializedGameState()
    {
        return System.Text.Json.JsonSerializer.Serialize<CheckersState>(_state);
    }

    public MovementLog? MakeMove(int xTo, int yTo, int xFrom, int yFrom)
    {
        //impossible to make move to the white square
        if (_state.GameBoard[xTo][yTo] != EBoardPiece.WhiteSquare)
        {
            if (CheckerIsKing(xFrom, yFrom))
            {
                RunLogicForKing(xTo, yTo, xFrom, yFrom);
            }
            else
            {
                RunLogicForRegularChecker( xTo,  yTo,  xFrom, yFrom);
            }
            return new MovementLog()
            {
                MovementFromX = xFrom,
                MovementFromY = yFrom,
                MovementToX = xTo,
                MovementToY = yTo,
                WhoMoved = GetWhoMovedLastForLog()
            };
        }
        return null;
    }

    private void RunLogicForRegularChecker(int xTo, int yTo, int xFrom, int yFrom)
    {
        //change checker from regular to king
        if (CheckerWillBecomeKing(xTo, yTo))
        {
            _state.GameBoard[xTo][yTo] = _state.NextMoveByBlack
                ? EBoardPiece.BlackSquareBlackKing
                : EBoardPiece.BlackSquareWhiteKing;
        }
        else
        {
            _state.GameBoard[xTo][yTo] = _state.NextMoveByBlack
                ? EBoardPiece.BlackSquareBlackChecker
                : EBoardPiece.BlackSquareWhiteChecker;
        }
        //check if checker wants to eat opponents checker 
        if (Math.Abs(xTo - xFrom) == 2)
        {
            //replace eaten checker by black square
            _state.GameBoard[(xFrom + xTo) / 2][(yFrom + yTo) / 2] = EBoardPiece.BlackSquare;

            // if checker cannot eat another checker after previous eating - swap sides.
            if (!CheckerCanEat(xTo, yTo))
            {
                _state.NextMoveByBlack = !_state.NextMoveByBlack;
                _state.CheckerCanEatAgain = false;
            }
            else
            {
                _state.CheckerCanEatAgain = true;
            }
        }
        else
        {
            _state.NextMoveByBlack = !_state.NextMoveByBlack;
        }
        //replace by black square position, from which checker has moved
        _state.GameBoard[xFrom][yFrom] = EBoardPiece.BlackSquare;
    }

    private void RunLogicForKing(int xTo, int yTo, int xFrom, int yFrom)
    {
        _state.GameBoard[xTo][yTo] = _state.NextMoveByBlack
            ? EBoardPiece.BlackSquareBlackKing
            : EBoardPiece.BlackSquareWhiteKing;

        var target = CheckerThatKingEats(xFrom, yFrom, xTo, yTo);
        if (target != null) {
            _state.GameBoard[target.X][target.Y] = EBoardPiece.BlackSquare;
            if (KingCanEat(xTo, yTo) == null)
            {
                _state.NextMoveByBlack = !_state.NextMoveByBlack;
                _state.CheckerCanEatAgain = false;
            }
            else _state.CheckerCanEatAgain = true;
        }
        else _state.NextMoveByBlack = !_state.NextMoveByBlack;
        
        _state.GameBoard[xFrom][yFrom] = EBoardPiece.BlackSquare;
    }

    public MovementLog? MakeMoveByAi()
    {
        var checkersToMatch = NextMoveByBlack() ? blackCheckers : whiteCheckers;
        var candidatesToMove = new List<PossibleMoves>();
        for (int y = 0; y < _state.GameBoard.Length; y++)
        {
            for (int x = 0; x < _state.GameBoard[0].Length; x++)
            {
                if (!checkersToMatch.Contains(_state.GameBoard[x][y])) continue;
                if (CheckerIsKing(x, y))
                {
                    var possibleMoves = FindPossibleMovesForKing(x, y);
                    if (possibleMoves.AllPossibleMoves.Any()) candidatesToMove.Add(possibleMoves);
                }
                else
                {
                    if (NextMoveByBlack() && blackCheckers.Contains(_state.GameBoard[x][y]))
                    {
                        var possibleMoves = FindPossibleMovesForBlack(x, y);
                        if (possibleMoves.AllPossibleMoves.Any()) candidatesToMove.Add(possibleMoves);
                    }
                    if (!NextMoveByBlack() && whiteCheckers.Contains(_state.GameBoard[x][y]))
                    {
                        var possibleMoves = FindPossibleMovesForWhite(x, y);
                        if (possibleMoves.AllPossibleMoves.Any()) candidatesToMove.Add(possibleMoves);
                    }
                }
            }
        }

        foreach (var possibleMove in candidatesToMove)
        {
            var coordFrom = possibleMove.CheckerToMove;
            var moves = possibleMove.AllPossibleMoves;
            if (CheckerIsKing(coordFrom.X, coordFrom.Y))
            {
                if (KingCanEat(coordFrom.X, coordFrom.Y) == null) continue;

                foreach (var move in moves.Where(move => CheckerThatKingEats(
                             coordFrom.X, coordFrom.Y, move.X, move.Y) != null))
                {
                    RunLogicForKing(move.X, move.Y, coordFrom.X, coordFrom.Y);
                    return new MovementLog
                    {
                        MovementFromX = coordFrom.X,
                        MovementFromY = coordFrom.Y,
                        MovementToX = move.X,
                        MovementToY = move.Y,
                        WhoMoved = GetWhoMovedLastForLog()
                    };
                }
            } else
            {
                if (!CheckerCanEat(coordFrom.X, coordFrom.Y)) continue;

                foreach (var move in moves.Where(move => Math.Abs(coordFrom.X - move.X) == 2))
                {
                    RunLogicForRegularChecker(move.X, move.Y, coordFrom.X, coordFrom.Y);
                    return new MovementLog
                    {
                        MovementFromX = coordFrom.X,
                        MovementFromY = coordFrom.Y,
                        MovementToX = move.X,
                        MovementToY = move.Y,
                        WhoMoved = GetWhoMovedLastForLog()
                    };
                }
            }
        }
        //if checkers cannot eat just make a random move
        var random = new Random();
        if (candidatesToMove.Count > 0)
        {
            var move = candidatesToMove[random.Next(0, candidatesToMove.Count)];
            var from = move.CheckerToMove;
            var to = move.AllPossibleMoves[random.Next(0, move.AllPossibleMoves.Count)];
            
            if (CheckerIsKing(from.X, from.Y))
            {
                RunLogicForKing(to.X, to.Y, from.X, from.Y);
            }
            else
            {
                RunLogicForRegularChecker( to.X, to.Y, from.X, from.Y);
            }

            return new MovementLog()
            {
                MovementFromX = from.X,
                MovementFromY = from.Y,
                MovementToX = to.X,
                MovementToY = to.Y,
                WhoMoved = GetWhoMovedLastForLog()
            };
        }
        return null;
    }

    public bool GameOver()
    {
        return CountCheckersOnBoard(blackCheckers) == 0 || CountCheckersOnBoard(whiteCheckers) == 0;
    }

    private bool CheckerIsKing(int xFrom, int yFrom)
    {
        return _state.GameBoard[xFrom][yFrom] == EBoardPiece.BlackSquareBlackKing ||
               _state.GameBoard[xFrom][yFrom] == EBoardPiece.BlackSquareWhiteKing;
    }

    public bool NextMoveByBlack() => _state.NextMoveByBlack;

    public PossibleMoves FindPossibleMovesForWhite(int x, int y)
    {
        var coordsToMove = new List<Coordinate>()
        {
            new(x - 1, y - 1),
            new(x + 1, y - 1),
        };

        var coordsToEat = new List<Coordinate>()
        {
            new(x - 2, y - 2),
            new(x + 2, y - 2),
            new(x + 2, y + 2),
            new(x - 2, y + 2),
        };
        var currentChecker = new Coordinate(x, y);
        
        var allPossibleMoves = FindMoves(coordsToMove, coordsToEat, x, y, findMovesForWhite: true);

        return new PossibleMoves(currentChecker, allPossibleMoves);
    }

    public PossibleMoves FindPossibleMovesForBlack(int x, int y)
    {
        var coordsToMove = new List<Coordinate>()
        {
            new(x + 1, y + 1),
            new(x - 1, y + 1),
        };

        var coordsToEat = new List<Coordinate>()
        {
            new(x - 2, y - 2),
            new(x + 2, y - 2),
            new(x + 2, y + 2),
            new(x - 2, y + 2),
        };
        var currentChecker = new Coordinate(x, y);
        var allPossibleMoves = FindMoves(coordsToMove, coordsToEat, x, y, findMovesForWhite: false);

        return new PossibleMoves(currentChecker, allPossibleMoves);
    }

    private List<Coordinate> FindMoves(
        List<Coordinate> coordsToMove, List<Coordinate> coordsToEat, int x, int y, bool findMovesForWhite)
    {
        var enemyCheckers = findMovesForWhite ? blackCheckers : whiteCheckers;

        var result = new List<Coordinate>();

        if (!_state.CheckerCanEatAgain)
        {
            foreach (var coord in coordsToMove)
            {
                try
                {
                    if (_state.GameBoard[coord.X][coord.Y] == EBoardPiece.BlackSquare)
                        result.Add(new Coordinate(coord.X, coord.Y));
                }
                catch (Exception) { /*ignored*/ }
            }
        }
        
        foreach (var coord in coordsToEat)
        {
            try
            {
                if (_state.GameBoard[coord.X][coord.Y] == EBoardPiece.BlackSquare
                    && enemyCheckers.Contains(GetBoard()[(coord.X + x) / 2][(coord.Y + y) / 2]))
                {
                    result.Add(new Coordinate(coord.X, coord.Y));
                }
            }
            catch (Exception) { /*ignored*/ }
        }
        return result;
    }

    public PossibleMoves FindPossibleMovesForKing(int x, int y)
    {
        var coords = new List<Coordinate>();
        if (_state.CheckerCanEatAgain)
        {
            Coordinate target = KingCanEat(x, y)!;
            Coordinate coord;

            if (target.X < x && target.Y < y) coord = new Coordinate(x - 2, y - 2);
            else if (target.X > x && target.Y < y) coord = new Coordinate(x + 2, y - 2);
            else if (target.X < x && target.Y > y) coord = new Coordinate(x - 2, y + 2);
            else coord = new Coordinate(x + 2, y + 2);
            
            coords.Add(coord);
        }
        else
        {
            var topRightDiagonal = GetKingTargetsOnSpecificDiagonal(x, y, topRight: true);
            var topLeftDiagonal = GetKingTargetsOnSpecificDiagonal(x, y, topLeft: true);
            var bottomRightDiagonal = GetKingTargetsOnSpecificDiagonal(x, y, bottomRight: true);
            var bottomLeftDiagonal = GetKingTargetsOnSpecificDiagonal(x, y, bottomLeft: true);

            coords = topRightDiagonal
                .Concat(topLeftDiagonal)
                .Concat(bottomRightDiagonal)
                .Concat(bottomLeftDiagonal)
                .ToList();
        }
        return new PossibleMoves(new Coordinate(x, y), coords);
    }

    private Coordinate? KingCanEat(int x, int y)
    {
        var moves = new List<List<Coordinate>>
        {
            GetKingTargetsOnSpecificDiagonal(x, y, topRight: true),
            GetKingTargetsOnSpecificDiagonal(x, y, topLeft: true),
            GetKingTargetsOnSpecificDiagonal(x, y, bottomRight: true),
            GetKingTargetsOnSpecificDiagonal(x, y, bottomLeft: true)
        };

        foreach (var coordinateList in moves)
        {
            if (!coordinateList.Any()) continue;
            var endCoord = coordinateList.Last();
            var target = CheckerThatKingEats(x, y, endCoord.X, endCoord.Y);
            return target;
            //if (target != null) return true;
        }

        return null;
        // return false;
    }

    private bool CheckerCanEat(int x, int y)
    {
        var checkersToEat = NextMoveByBlack() ? whiteCheckers : blackCheckers;
        
        var coordsToEat = new List<Coordinate>()
        {
            new(x - 2, y - 2),
            new(x + 2, y - 2),
            new(x + 2, y + 2),
            new(x - 2, y + 2),
        };
        foreach (var coord in coordsToEat)
        {
            try
            {
                if (_state.GameBoard[coord.X][coord.Y] == EBoardPiece.BlackSquare
                    && checkersToEat.Contains(GetBoard()[(coord.X + x) / 2][(coord.Y + y) / 2]))
                {
                    return true;
                }
            }
            catch (Exception) { /*ignored*/ }
        }
        return false;
    }

    private Coordinate? CheckerThatKingEats(int xFrom, int yFrom, int xTo, int yTo)
    {

        var xis = GetXisWhereKingCanGo(xFrom, xTo);
        var yis = GetYisWhereKingCanGo(yFrom, yTo);
        
        var coords = ConvertListsToCoordinateList(xis, yis);

        foreach (var coord in coords)
        {
            if (checkers.Contains(GetBoard()[coord.X][coord.Y]))
            {
                return new Coordinate(coord.X, coord.Y);
            }
        }
        return null;
    }

    private bool CheckerWillBecomeKing(int x, int y)
    {
        if (!NextMoveByBlack()
            && y == 0
            && _state.GameBoard[x][y] == EBoardPiece.BlackSquare) return true;

        return NextMoveByBlack() && y == _state.GameBoard.Length - 1 &&
               _state.GameBoard[x][y] == EBoardPiece.BlackSquare;
    }
    
    private List<int> GetXisWhereKingCanGo(int xFrom, int xTo)
    {
        List<int> xis = new List<int>();
        if (xFrom > xTo)
        {
            for (int i = xFrom - 1; i > xTo; i--)
            {
                xis.Add(i);
            }
        }
        else
        {
            for (int i = xFrom + 1; i < xTo; i++)
            {
                xis.Add(i);
            }
        }
        return xis;
    }
    
    private List<int> GetYisWhereKingCanGo(int yFrom, int yTo)
    {
        List<int> yis = new List<int>();
        if (yFrom > yTo)
        {
            for (int i = yFrom - 1; i > yTo; i--)
            {
                yis.Add(i);
            }
        }
        else
        {
            for (int i = yFrom + 1; i < yTo; i++)
            {
                yis.Add(i);
            } 
        }
        return yis;
    }

    public List<Coordinate> GetKingTargetsOnSpecificDiagonal(int initialX, int initialY,
        bool topRight=false, bool topLeft=false, bool bottomRight=false, bool bottomLeft=false)
    {
        List<Coordinate> coordinates = new List<Coordinate>();
        
        int tmpX = initialX;
        int tmpY = initialY;

        int valueX = 0;
        int valueY = 0;
        if (bottomRight)
        {
            valueX = tmpX + 1;
            valueY = tmpY + 1;
        }
        else if (bottomLeft)
        {
            valueX = tmpX - 1;
            valueY = tmpY + 1;
        }
        else if (topRight)
        {
            valueX = tmpX + 1;
            valueY = tmpY - 1;
        }
        else if (topLeft)
        {
            valueX = tmpX - 1;
            valueY = tmpY - 1;
        }

        int count = 0;
        while (!CoordinatesOutsideOfBoard(valueX, valueY))
        {
            if (_state.GameBoard[valueX][valueY] == EBoardPiece.BlackSquare)
            {
                coordinates.Add(new Coordinate(valueX, valueY));
            }
            if (NextMoveByBlack() && blackCheckers.Contains(GetBoard()[valueX][valueY])
                || !NextMoveByBlack() && whiteCheckers.Contains(GetBoard()[valueX][valueY]))
            {
                break;
            }
            if (NextMoveByBlack() && whiteCheckers.Contains(GetBoard()[valueX][valueY]) 
                || !NextMoveByBlack() && blackCheckers.Contains(GetBoard()[valueX][valueY]))
            {
                count++;
                if (count == 2) break;
                //coordinates.Add(new Coordinate(valueX, valueY));
            }
            if (bottomRight)
            {
                valueX ++;
                valueY++;
            }
            else if (bottomLeft)
            {
                valueX--;
                valueY++;
            }
            else if (topRight)
            {
                valueX++;
                valueY--;
            }
            else if (topLeft)
            {
                valueX--;
                valueY--;
            }
        }
        return coordinates;
    }

    private List<Coordinate> ConvertListsToCoordinateList(List<int> xis, List<int> yis)
    {
        List<Coordinate> result = new List<Coordinate>();
        for (int i = 0; i < xis.Count; i++)
        {
            var coord = new Coordinate(xis[i], yis[i]);
            result.Add(coord);
        }
        return result;
    }

    private bool CoordinatesOutsideOfBoard(int x, int y)
    {
        return x > _state.GameBoard[0].Length - 1 || x < 0 || y > _state.GameBoard.Length - 1 || y < 0;
    }
    
    private int CountCheckersOnBoard(List<EBoardPiece> checkersToMatch)
    {
        int count = 0;
        for (int y = 0; y < GetBoard()[0].Length; y++)
        {
            for (int x = 0; x < GetBoard().Length; x++)
            {
                if (checkersToMatch.Contains(GetBoard()[x][y])) count++;
            }
        }
        return count;
    }
    
    private string GetWhoMovedLastForLog()
    {
        if (_state.CheckerCanEatAgain) return !NextMoveByBlack() ? "White" : "Black";
        return NextMoveByBlack() ? "White" : "Black";
    }

    public List<Coordinate> CheckersUserCanPick()
    {
        var res = new List<Coordinate>();
        
        var cols = GetBoard()[0].Length;
        var rows = GetBoard().Length;

        for (var y = 0; y < rows; y++)
        {
            for (var x = 0; x < cols; x++)
            {
                if (NextMoveByBlack() && blackCheckers.Contains(GetBoard()[x][y]))
                {
                    var possibleMoves = GetBoard()[x][y] == EBoardPiece.BlackSquareBlackKing 
                        ? FindPossibleMovesForKing(x, y) 
                        : FindPossibleMovesForBlack(x, y);
                    
                    if (possibleMoves.AllPossibleMoves.Any())
                    {
                        res.Add(new Coordinate(x, y));
                    } 
                } else if (!NextMoveByBlack() && whiteCheckers.Contains(GetBoard()[x][y]))
                {
                    var possibleMoves = GetBoard()[x][y] == EBoardPiece.BlackSquareBlackKing 
                        ? FindPossibleMovesForKing(x, y) 
                        : FindPossibleMovesForWhite(x, y);
                    
                    if (possibleMoves.AllPossibleMoves.Any())
                    {
                        res.Add(new Coordinate(x, y));
                    } 
                }
            }
        }
        return res;
    }
}
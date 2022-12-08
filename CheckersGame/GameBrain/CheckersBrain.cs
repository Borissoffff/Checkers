using ProjectDomain;

namespace GameBrain;

public class CheckersBrain
{
    public List<EBoardPiece> Checkers = new List<EBoardPiece>()
    {
        EBoardPiece.BlackSquareBlackChecker,
        EBoardPiece.BlackSquareWhiteChecker,
        EBoardPiece.BlackSquareBlackKing,
        EBoardPiece.BlackSquareWhiteKing
    };
    
    public List<EBoardPiece> BlackCheckers = new List<EBoardPiece>()
    {
        EBoardPiece.BlackSquareBlackChecker,
        EBoardPiece.BlackSquareBlackKing,
    };
    
    public List<EBoardPiece> WhiteCheckers = new List<EBoardPiece>()
    {
        EBoardPiece.BlackSquareWhiteChecker,
        EBoardPiece.BlackSquareWhiteKing,
    };

    public bool CheckerCanEatAgain = false;

    public Coordinate? EatenChecker = null;
    
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

        _state.GameBoard = new EBoardPiece?[boardWidth][];

        for (int i = 0; i < boardWidth; i++)
        {
            _state.GameBoard[i] = new EBoardPiece?[boardHeight];
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

    public void MakeMove(int xTo, int yTo, int xFrom, int yFrom)
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
        }
    }

    private void RunLogicForRegularChecker(int xTo, int yTo, int xFrom, int yFrom)
    {
        //change checker from regular to king
        if (CheckerWillBecomeKing(xTo, yTo)/* || CheckerIsKing(xFrom, yFrom)*/)
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
            EatenChecker = new Coordinate((xFrom + xTo) / 2, (yFrom + yTo) / 2);
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
            if (!KingCanEat(xTo, yTo))
            {
                _state.NextMoveByBlack = !_state.NextMoveByBlack;
                _state.CheckerCanEatAgain = false;
            }
            else _state.CheckerCanEatAgain = true;
        }
        else _state.NextMoveByBlack = !_state.NextMoveByBlack;
        
        _state.GameBoard[xFrom][yFrom] = EBoardPiece.BlackSquare;
    }

    public bool GameOver()
    {
        return CountCheckersOnBoard(BlackCheckers) == 0 || CountCheckersOnBoard(WhiteCheckers) == 0;
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
        var enemyCheckers = findMovesForWhite ? BlackCheckers : WhiteCheckers;

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
            var moves = new List<List<Coordinate>>
            {
                GetKingTargetsOnSpecificDiagonal(x, y, topRight: true),
                GetKingTargetsOnSpecificDiagonal(x, y, topLeft: true),
                GetKingTargetsOnSpecificDiagonal(x, y, bottomRight: true),
                GetKingTargetsOnSpecificDiagonal(x, y, bottomLeft: true)
            };
            foreach (var coordList in moves)
            {
                if (coordList.Count < 2) continue;
                var coordFrom = coordList.First();
                var coordTo = coordList.Last();
                if (CheckerThatKingEats(coordFrom.X, coordFrom.Y, coordTo.X, coordTo.Y) != null)
                {
                    coordList.ForEach(coord => coords.Add(coord));
                }
            }
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

        /*//bottom-right diagonal
        int tmpX = x;
        int tmpY = y;
        bool flag = false;
        int count = 0;
        while (!CoordinatesOutsideOfBoard(tmpX + 1, tmpY + 1))
        {
            if (_state.GameBoard[tmpX + 1][tmpY + 1] == EBoardPiece.BlackSquare)
            {
                coordinates.Add(new Coordinate(tmpX + 1, tmpY + 1));
            }
            if (NextMoveByBlack() && RedCheckers.Contains(GetBoard()[tmpX + 1][tmpY + 1]))
            {
                break;
            }
            if (NextMoveByBlack() && WhiteCheckers.Contains(GetBoard()[tmpX + 1][tmpY + 1]) 
                || !NextMoveByBlack() && RedCheckers.Contains(GetBoard()[tmpX + 1][tmpY + 1]))
            {
                count++;
                if (count == 2) break;
                coordinates.Add(new Coordinate(tmpX + 1, tmpY + 1));
            }
            tmpX++;
            tmpY++;
        }
        //bottom-left diagonal
        tmpX = x;
        tmpY = y;
        while (!CoordinatesOutsideOfBoard(tmpX - 1, tmpY + 1))
        {
            if (_state.GameBoard[tmpX - 1][tmpY + 1] == EBoardPiece.BlackSquare)
            {
                coordinates.Add(new Coordinate(tmpX - 1, tmpY + 1));
            }
            tmpX--;
            tmpY++;
        }
        //top-right diagonal
        tmpX = x;
        tmpY = y;
        while (!CoordinatesOutsideOfBoard(tmpX + 1, tmpY - 1))
        {
            if (_state.GameBoard[tmpX + 1][tmpY - 1] == EBoardPiece.BlackSquare)
            {
                coordinates.Add(new Coordinate(tmpX + 1, tmpY - 1));
            }
            tmpX++;
            tmpY--;
        }
        //top-left diagonal
        tmpX = x;
        tmpY = y;
        while (!CoordinatesOutsideOfBoard(tmpX - 1, tmpY - 1))
        {
           // if (y == 0 || tmpX - 1 <= -1) break;
            if (_state.GameBoard[tmpX - 1][tmpY - 1] == EBoardPiece.BlackSquare)
            {
                coordinates.Add(new Coordinate(tmpX - 1, tmpY - 1));
            }
            tmpX--;
            tmpY--;
        }
        return new PossibleMoves(new Coordinate(x, y), coordinates);*/
    }

    private bool KingCanEat(int x, int y)
    {
       // var pieceToMove = NextMoveByBlack() ? EBoardPiece.BlackSquareBlackKing : EBoardPiece.BlackSquareWhiteKing;
        var piecesToEat = NextMoveByBlack() ? WhiteCheckers : BlackCheckers;
        var pieceWhereToMove = EBoardPiece.BlackSquare;
        

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
            var endCoord = coordinateList.LastOrDefault();
            var target = CheckerThatKingEats(x, y, endCoord.X, endCoord.Y);
            if (target != null) return true;
            /*for (int i = 0; i < coordinateList.Count - 1; i++)
            {
                var initialCoord = coordinateList[i];
                var nextCoord = coordinateList[i + 1];
                if (i == 0 && Math.Abs(initialCoord.X - x) == 2)
                {
                    return true;
                }
                if (Math.Abs(initialCoord.X - nextCoord.X) == 2)
                {
                    return true;
                }
                /*var coordToEat = coordinateList[i + 1];
                var coordToStep = coordinateList[i + 2];
                if (/*GetBoard()[coord.X][coord.Y] == pieceToMove &&#2#
                    piecesToEat.Contains(GetBoard()[coordToEat.X][coordToEat.Y]) &&
                    GetBoard()[coordToStep.X][coordToStep.Y] == pieceWhereToMove) return true;#1#
            }*/
        }
        return false;
    }

    private bool CheckerCanEat(int x, int y)
    {
        var checkersToEat = NextMoveByBlack() ? WhiteCheckers : BlackCheckers;
        
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
            if (Checkers.Contains(GetBoard()[coord.X][coord.Y]))
            {
                return new Coordinate(coord.X, coord.Y);
            };
        }
        return null;

        /*return coords.Where(
            coord => Checkers.Contains(GetBoard()[coord.X][coord.Y]))
            .ToList();*/
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
        
        bool flag = false;
        int count = 0;
        while (!CoordinatesOutsideOfBoard(valueX, valueY))
        {
            if (_state.GameBoard[valueX][valueY] == EBoardPiece.BlackSquare)
            {
                coordinates.Add(new Coordinate(valueX, valueY));
            }
            if (NextMoveByBlack() && BlackCheckers.Contains(GetBoard()[valueX][valueY])
                || !NextMoveByBlack() && WhiteCheckers.Contains(GetBoard()[valueX][valueY]))
            {
                break;
            }
            if (NextMoveByBlack() && WhiteCheckers.Contains(GetBoard()[valueX][valueY]) 
                || !NextMoveByBlack() && BlackCheckers.Contains(GetBoard()[valueX][valueY]))
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

    private int CountBlackCheckersOnBoard()
    {
        int count = 0;
        for (int y = 0; y < GetBoard().Length; y++)
        {
            for (int x = 0; x < GetBoard()[0].Length; x++)
            {
                if (BlackCheckers.Contains(GetBoard()[x][y])) count++;
            }
        }
        return count;
    }
    private int CountCheckersOnBoard(List<EBoardPiece> checkersToMatch)
    {
        int count = 0;
        for (int y = 0; y < GetBoard().Length; y++)
        {
            for (int x = 0; x < GetBoard()[0].Length; x++)
            {
                if (checkersToMatch.Contains(GetBoard()[x][y])) count++;
            }
        }
        return count;
    }
}
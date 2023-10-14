using System.Text;
using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Semprg_Codingame;

/*
 1. Blunder starts from the place indicated by the @ symbol on the map and heads SOUTH.

 2. Blunder finishes his journey and dies when he reaches the suicide booth marked $.

 3. Obstacles that Blunder may encounter are represented by # or X.

 4. When Blunder encounters an obstacle, he changes direction using the following priorities: SOUTH, EAST, NORTH and WEST. So he first tries to go SOUTH, if he cannot, then he will go EAST, if he still cannot, then he will go NORTH, and finally if he still cannot, then he will go WEST.

 5. Along the way, Blunder may come across path modifiers that will instantaneously make him change direction. The S modifier will make him turn SOUTH from then on, E, to the EAST, N to the NORTH and W to the WEST.

 6. The circuit inverters (I on map) produce a magnetic field which will reverse the direction priorities that Blunder should choose when encountering an obstacle. Priorities will become WEST, NORTH, EAST, SOUTH. If Blunder returns to an inverter I, then priorities are reset to their original state (SOUTH, EAST, NORTH, WEST).

 7. Blunder can also find a few beers along his path (B on the map) that will give him strength and put him in “Breaker” mode. Breaker mode allows Blunder to destroy and automatically pass through the obstacles represented by the character X (only the obstacles X). When an obstacle is destroyed, it remains so permanently and Blunder maintains his course of direction. If Blunder is in Breaker mode and passes over a beer again, then he immediately goes out of Breaker mode. The beers remain in place after Blunder has passed.

 8. 2 teleporters T may be present in the city. If Blunder passes over a teleporter, then he is automatically teleported to the position of the other teleporter and he retains his direction and Breaker mode properties.

 9. Finally, the space characters are blank areas on the map (no special behavior other than those specified above).
 */

class Solution
{
    private static int _width;
    private static int _height;
    private static char[,] _map;

    private static (Int2 tp1, Int2 tp2) _teleporters = default;

    private static void DrawMap(Int2 blunderPos)
    {
        for (int y = 0; y < _height; y++)
        {
            for (int x = 0; x < _width; x++)
            {
                var c = _map[x, y];
                if (blunderPos.X == x && blunderPos.Y == y)
                    Console.Error.Write("&");
                else
                    Console.Error.Write($"{c}");

            }

            Console.Error.WriteLine();
        }
    }

    static void Main(string[] args)
    {
        string[] inputs = Console.ReadLine().Split(' ');
        _height = int.Parse(inputs[0]);
        _width = int.Parse(inputs[1]);
        _map = new char[_width, _height];

        var currentUnion = new BlunderMoveUnion(BlunderMove.South, new(3));
        Int2 currentPosition = default;

        bool assignedFirstTeleporter = false;

        for (int y = 0; y < _height; y++)
        {
            string row = Console.ReadLine();
            for (int x = 0; x < _width; x++)
            {
                var c = row[x];
                _map[x, y] = c;

                switch (c)
                {
                    case '@': //Start position
                        currentPosition = new Int2(x, y);
                        break;
                    case 'T':
                        _teleporters = assignedFirstTeleporter
                            ? (_teleporters.tp1, new Int2(x, y))
                            : (new Int2(x, y), _teleporters.tp2);
                        assignedFirstTeleporter = true;
                        break;
                }
            }
        }

        var resultBuilder = new StringBuilder();
        
        var historyI = 0;
        var positionHistory = new List<Int2>();
        var brokenWallsHistory = new List<List<Int2>>();
        var moveUnionHistory = new List<BlunderMoveUnion>();

        while (true)
        {
            var brokenWallsThisIteration = new List<Int2>();
            //Add all walls from last iteration
            if (historyI > 0)
                brokenWallsThisIteration.AddRange(brokenWallsHistory[historyI - 1]);

            DrawMap(currentPosition);
            var nextMove = GetNextMove(currentPosition, currentUnion);
            Console.Error.WriteLine(
                $"Next move: {nextMove.Move.Name}, isB: {nextMove.Attributes.Contains(BlunderMoveAttribute.IsBreakerMode)}, isI: {nextMove.Attributes.Contains(BlunderMoveAttribute.IsInvertedMode)}");

            if (nextMove.Move == BlunderMove.Stop)
                break;

            if (nextMove.Attributes.Contains(BlunderMoveAttribute.TeleportTo1))
            {
                currentPosition = _teleporters.tp1;
                nextMove.Attributes.Remove(BlunderMoveAttribute.TeleportTo1);
            }
            else if (nextMove.Attributes.Contains(BlunderMoveAttribute.TeleportTo2))
            {
                currentPosition = _teleporters.tp2;
                nextMove.Attributes.Remove(BlunderMoveAttribute.TeleportTo2);
            }

            if (currentUnion.Attributes.Contains(BlunderMoveAttribute.IsBreakerMode))
            {
                //Remove X from map
                if (_map[currentPosition.X, currentPosition.Y] == 'X')
                {
                    _map[currentPosition.X, currentPosition.Y] = ' ';
                    brokenWallsThisIteration.Add(currentPosition);
                }
            }

            resultBuilder.AppendLine(nextMove.Move.Name);
            brokenWallsThisIteration.ForEach(w => Console.Error.WriteLine($"bw: x{w.X} y{w.Y}"));

            if (IsBlunderLooping(currentPosition, brokenWallsThisIteration, nextMove, positionHistory, brokenWallsHistory, moveUnionHistory))
            {
                resultBuilder
                    .Clear()
                    .AppendLine("LOOP");

                break;
            }

            currentUnion = nextMove;

            //Save state
            historyI++;
            positionHistory.Add(currentPosition);
            brokenWallsHistory.Add(brokenWallsThisIteration);
            moveUnionHistory.Add(currentUnion);

            currentPosition += currentUnion.Move.Direction;
        }

        Console.WriteLine(resultBuilder.ToString());
    }

    private static bool IsBlunderLooping(
        Int2 currentPosition,
        List<Int2> currentBrokenWalls,
        BlunderMoveUnion nextMoveUnion,
        List<Int2> positionHistory, //Without current
        List<List<Int2>> brokenWallsHistory, //Without current
        List<BlunderMoveUnion> moveUnionHistory //Without current
        )
    {
        //There is a loop if blunder is in the same position and has the same broken walls as before
        for (int hI = 0; hI < positionHistory.Count; hI++)
        {
            if (currentPosition != positionHistory[hI]) 
                continue;

            //We are at the same position as before, check if broken walls are the same

            //Check if move union is the same
            var moveUnionAtHistory = moveUnionHistory[hI];
            if (
                nextMoveUnion.Move != moveUnionAtHistory.Move
                || !nextMoveUnion.Attributes.SequenceEqual(moveUnionAtHistory.Attributes)
                )
            {
                //We have a different move
                return false;
            }

            var brokenWallsAtHistory = brokenWallsHistory[hI];
            if (currentBrokenWalls.SequenceEqual(brokenWallsAtHistory))
            {
                //We have the same broken walls as before

                //Write everything
                Console.Error.WriteLine($"Current Pos: {currentPosition.X} {currentPosition.Y}");
                Console.Error.WriteLine($"History Pos: {positionHistory[hI].X} {positionHistory[hI].Y}");
                
                Console.Error.WriteLine($"Current Move: {nextMoveUnion.Move.Name}");
                Console.Error.WriteLine($"History Move: {moveUnionAtHistory.Move.Name}");

                Console.Error.WriteLine($"Current Attributes: {string.Join(", ", nextMoveUnion.Attributes)}");
                Console.Error.WriteLine($"History Attributes: {string.Join(", ", moveUnionAtHistory.Attributes)}");

                return true;
            }
        }

        return false;
    }

    private static BlunderMoveUnion GetNextMove(Int2 currentPosition, BlunderMoveUnion currentUnion, int triedDirections = 0, bool collectedBeerNow = false)
    {
        var moveAttributes = currentUnion.Attributes;
        switch (_map[currentPosition.X, currentPosition.Y])
        {
            case 'B':
                if (collectedBeerNow)
                    break;

                collectedBeerNow = true;
                if (moveAttributes.Contains(BlunderMoveAttribute.IsBreakerMode))
                    moveAttributes.Remove(BlunderMoveAttribute.IsBreakerMode);
                else
                    moveAttributes.Add(BlunderMoveAttribute.IsBreakerMode);
                break;
            case 'I':
                if (moveAttributes.Contains(BlunderMoveAttribute.IsInvertedMode))
                    moveAttributes.Remove(BlunderMoveAttribute.IsInvertedMode);
                else
                    moveAttributes.Add(BlunderMoveAttribute.IsInvertedMode);
                break;
            case 'T':
                if (currentPosition == _teleporters.tp1)
                    moveAttributes.Add(BlunderMoveAttribute.TeleportTo2);
                else
                    moveAttributes.Add(BlunderMoveAttribute.TeleportTo1);
                break;
        }

        var nextPositionInDir = currentPosition + currentUnion.Move.Direction;
        var moveToMake = (_map[currentPosition.X, currentPosition.Y], _map[nextPositionInDir.X, nextPositionInDir.Y]) switch
        {
            ('S', _) => BlunderMove.South,
            ('E', _) => BlunderMove.East,
            ('N', _) => BlunderMove.North,
            ('W', _) => BlunderMove.West,
            ('$', _) => BlunderMove.Stop,

            (_, '#') => GetNextMove(
                currentPosition,
                currentUnion with
                {
                    Move = moveAttributes.Contains(BlunderMoveAttribute.IsInvertedMode)
                    ? BlunderMove.DirectionsInOrderInverted[triedDirections]
                    : BlunderMove.DirectionsInOrder[triedDirections]
                },
                triedDirections + 1, collectedBeerNow).Move,

            (_, 'X') => moveAttributes.Contains(BlunderMoveAttribute.IsBreakerMode)
                ? currentUnion.Move
                : GetNextMove(
                    currentPosition,
                    currentUnion with
                    {
                        Move = moveAttributes.Contains(BlunderMoveAttribute.IsInvertedMode)
                            ? BlunderMove.DirectionsInOrderInverted[triedDirections]
                            : BlunderMove.DirectionsInOrder[triedDirections]
                    }, triedDirections + 1, collectedBeerNow).Move,

            _ => currentUnion.Move //Continue in same direction
        };

        return new BlunderMoveUnion(moveToMake, moveAttributes);
    }


    //return nextMove;
}

public readonly record struct BlunderMoveUnion(BlunderMove Move, List<BlunderMoveAttribute> Attributes);

public readonly struct BlunderMove
{
    public readonly string Name;
    public readonly Int2 Direction = default;

    public static bool operator ==(BlunderMove a, BlunderMove b)
        => a.Name == b.Name && a.Direction == b.Direction;

    public static bool operator !=(BlunderMove a, BlunderMove b) => !(a == b);

    private BlunderMove(string name, Int2 direction)
    {
        Name = name;
        Direction = direction;
    }

    private BlunderMove(string name)
    {
        Name = name;
    }

    public static readonly BlunderMove South = new("SOUTH", new Int2(0, 1)); //Down
    public static readonly BlunderMove East = new("EAST", new Int2(1, 0)); //Right
    public static readonly BlunderMove North = new("NORTH", new Int2(0, -1)); //Up
    public static readonly BlunderMove West = new("WEST", new Int2(-1, 0)); //Left

    public static readonly Dictionary<int, BlunderMove> DirectionsInOrder = new()
        {
            {0, South},
            {1, East},
            {2, North},
            {3, West}
        };

    public static readonly Dictionary<int, BlunderMove> DirectionsInOrderInverted = new()
        {
            {0, West},
            {1, North},
            {2, East},
            {3, South}
        };


    public static readonly BlunderMove Stop = new("S");
}

public enum BlunderMoveAttribute
{
    IsBreakerMode,
    IsInvertedMode,
    TeleportTo1,
    TeleportTo2
}

public readonly record struct Int2(int X, int Y)
{
    public override int GetHashCode()
        => HashCode.Combine(X, Y);

    public static Int2 operator +(Int2 a, Int2 b)
        => new Int2(a.X + b.X, a.Y + b.Y);
}
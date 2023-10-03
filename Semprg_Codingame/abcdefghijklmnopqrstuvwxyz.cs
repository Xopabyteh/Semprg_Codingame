//https://www.codingame.com/ide/puzzle/abcdefghijklmnopqrstuvwxyz

using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

class Solution
{
    private const string ALPHABET = "abcdefghijklmnopqrstuvwxyz";
    static void Main(string[] args)
    {
        int sideLength = int.Parse(Console.ReadLine());
        var inputSquare = new string[sideLength];


        for (int i = 0; i < sideLength; i++)
        {
            string line = Console.ReadLine();
            inputSquare[i] = line;

            Console.Error.WriteLine(line);
        }


        var dfsNodes = new Stack<Int2>(sideLength * sideLength);

        //Add start points ('a') to dfs
        for (int y = 0; y < sideLength; y++)
        {
            for (int x = 0; x < sideLength; x++)
            {
                if (inputSquare[y][x] == 'a')
                {
                    dfsNodes.Push(new Int2(x, y));
                }
            }
        }

        //Find a-z in order
        var snakePositions = new Int2[ALPHABET.Length];
        var letterIndex = 0; //The letter we are on
        var lastCrossLetterIndex = 0; //When was the last time we had multiple options (a cross)
        while (true)
        {
            //Look at neighbours of a point
            //If are the next letter of the alphabet, add to dfs
            //If we're at the end of the alphabet, we're done

            var current = dfsNodes.Pop();
            if (letterIndex == ALPHABET.Length - 1)
            {
                snakePositions[letterIndex] = current;
                break;
            }

            Console.Error.WriteLine($"{current} - {ALPHABET[letterIndex]} - lI: {letterIndex}");
            var searchForChar = ALPHABET[letterIndex + 1];

            //These are neighbours that we can move to
            var neighbours = GetNeighbours(searchForChar, inputSquare, current);

            //If we have multiple options, we need to remember where we are
            if (neighbours.Count > 1)
            {
                lastCrossLetterIndex = letterIndex;
            }

            //If we have no options, we need to go back to the last cross
            if (neighbours.Count == 0)
            {
                letterIndex = lastCrossLetterIndex;
                continue;
            }

            //Add options to dfs
            foreach (var neighbour in neighbours)
            {
                dfsNodes.Push(neighbour);
            }

            snakePositions[letterIndex] = current;
            letterIndex++;

    
            //Go to next neighbor
            continue;
        }

        //Now that we have the snake
        //Set letters that are not part of the snake to '-'

        for (int y = 0; y < sideLength; y++)
        {
            for (int x = 0; x < sideLength; x++)
            {
                var current = new Int2(x, y);
                if (snakePositions.Contains(current))
                    continue;

                inputSquare[y] = inputSquare[y].Remove(x, 1).Insert(x, "-");
            }
        }

        //Print result
        foreach (var line in inputSquare)
        {
            Console.WriteLine(line);
        }


    }

    /// <returns>Neighbours that have searchForChar as value</returns>
    private static List<Int2> GetNeighbours(char searchForChar, string[] inputSquare, Int2 whosNeighbours)
    {
        var up = whosNeighbours with {Y = whosNeighbours.Y + 1};
        var down = whosNeighbours with { Y = whosNeighbours.Y - 1};
        var left = whosNeighbours with { X = whosNeighbours.X - 1};
        var right = whosNeighbours with { X = whosNeighbours.X + 1};

        var neighbours = new List<Int2>(4);
        if (up.Y < inputSquare.Length && inputSquare[up.Y][up.X] == searchForChar)
        {
            neighbours.Add(up);
        }

        if (down.Y >= 0 && inputSquare[down.Y][down.X] == searchForChar)
        {
            neighbours.Add(down);
        }
         
        if (left.X >= 0 && inputSquare[left.Y][left.X] == searchForChar)
        {
            neighbours.Add(left);
        }

        if (right.X < inputSquare.Length && inputSquare[right.Y][right.X] == searchForChar)
        {
            neighbours.Add(right);
        }

        return neighbours;
    }

    private readonly record struct Int2(int X, int Y);
}
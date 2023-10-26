using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Xml;

namespace Scrabble;

class Solution
{
    static void Main(string[] args)
    {
        int numOfWords = int.Parse(Console.ReadLine()!);
        var existingWords = new string[numOfWords];
        for (int i = 0; i < numOfWords; i++)
        {
            string word = Console.ReadLine()!;
            existingWords[i] = word;
        }

        string handLetters = Console.ReadLine()!;
        
        //Acts only as a template
        //Key: letter, value: how many times it has occurred in the hand
        var handLettersDict = new Dictionary<char, int>();
        foreach (var c in handLetters)
        {
            if (handLettersDict.ContainsKey(c))
            {
                handLettersDict[c]++;
            }
            else
            {
                handLettersDict[c] = 1;
            }
        }

        //Word from the existing words
        //Which contains any sub array of the letters
        //With the highest score

        //Key: word, value: score
        var matchingWords = new Dictionary<string, int>(13);

        foreach (var word in existingWords)
        {
            if (!IsWordMadeOfLetters(word, handLettersDict))
                continue;

            var score = CalculateWordScore(word);
            matchingWords[word] = score;
        }

        //Get best word
        var bestWord = matchingWords.MaxBy(x => x.Value).Key;
        
        Console.WriteLine(bestWord);
    }

    private static bool IsWordMadeOfLetters(string word, Dictionary<char, int> handLetters)
    {
        var handLettersDictCopy = new Dictionary<char, int>(handLetters);

        //Check that it is made from the letters
        foreach (var wordC in word)
        {
            if (handLettersDictCopy.TryGetValue(wordC, out var occurrenceAmount))
            {
                //We have this letter
                if (occurrenceAmount > 0)
                {
                    //We have enough of this letter
                    handLettersDictCopy[wordC]--;
                }
                else
                {
                    //We don't have enough of this letter
                    return false;
                }
            }
            else
            {
                //We don't have this letter
                return false;
            }
        }

        return true;
    }

    //Key: character, Value: its score
    private static readonly Dictionary<char, int> LetterScores = new()
    {
        {'e', 1},
        {'a', 1},
        {'i', 1},
        {'o', 1},
        {'n', 1},
        {'r', 1},
        {'t', 1},
        {'l', 1},
        {'s', 1},
        {'u', 1},
        {'d', 2},
        {'g', 2},
        {'b', 3},
        {'c', 3},
        {'m', 3},
        {'p', 3},
        {'f', 4},
        {'h', 4},
        {'v', 4},
        {'w', 4},
        {'y', 4},
        {'k', 5},
        {'j', 8},
        {'x', 8},
        {'q', 10},
        {'z', 10}
    };
    private static int CalculateWordScore(string word)
    {
        var sum = 0;
        foreach (var c in word)
        {
            sum += LetterScores[c];
        }

        return sum;
    }
}
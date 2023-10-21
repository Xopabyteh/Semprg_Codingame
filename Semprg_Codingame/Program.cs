using System.Data;using Kata;
var flCalc = new FluentCalculator();

var expression = flCalc.One.Plus.Two.Times.Eight.Result();

Console.WriteLine(expression);
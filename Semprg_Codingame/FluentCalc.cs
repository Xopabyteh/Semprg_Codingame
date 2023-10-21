using System.Data;
using System.Text;
using System.Linq;
using System;

#nullable enable
namespace Kata;

public class FluentCalculator
{
    public FluentValue Zero => NewFluentComposition("0");
    public FluentValue One => NewFluentComposition("1");
    public FluentValue Two => NewFluentComposition("2");
    public FluentValue Three => NewFluentComposition("3");
    public FluentValue Four => NewFluentComposition("4");
    public FluentValue Five => NewFluentComposition("5");
    public FluentValue Six => NewFluentComposition("6");
    public FluentValue Seven => NewFluentComposition("7");
    public FluentValue Eight => NewFluentComposition("8");
    public FluentValue Nine => NewFluentComposition("9");
    public FluentValue Ten => NewFluentComposition("01");//Reverse order, due to reversing the sequence when composing it

    private FluentValue NewFluentComposition(string serializationSymbol)
    {
        return new FluentValue(null, serializationSymbol);
    }
}

public class FluentValue : Fluent
{

    public FluentValue(Fluent? parent, string serializationString) : base(parent, serializationString)
    { }

    public FluentOperation Plus => new FluentOperation(this, "+");
    public FluentOperation Minus => new FluentOperation(this, "-");
    public FluentOperation Times => new FluentOperation(this, "*");
    public FluentOperation DividedBy => new FluentOperation(this, "/");

    public static implicit operator double(FluentValue instance)
    {
        //Read composition
        var compositionBuilder = new StringBuilder();
        Fluent? iterator = instance;
        while (iterator is not null)
        {
            compositionBuilder.Append(iterator.SerializationString);
            iterator = iterator.Parent;
        }

        string? composition = compositionBuilder.ToString();
        //We have to reverse the composition, since the numbers are fed in backwards, due to reading it from the top parent
        composition = string.Join(String.Empty, composition.Reverse());
        var computedComposition = Convert.ToDouble(new DataTable().Compute(composition, ""));
        return computedComposition;
    }


    public double Result()
        => (double)this;
}
public class FluentOperation : Fluent
{

    public FluentOperation(Fluent? parent, string serializationString) : base(parent, serializationString)
    {
        //NOOP
    }

    public FluentValue Zero => CreateSecondComponent("0");
    public FluentValue One => CreateSecondComponent("1");
    public FluentValue Two => CreateSecondComponent("2");
    public FluentValue Three => CreateSecondComponent("3");
    public FluentValue Four => CreateSecondComponent("4");
    public FluentValue Five => CreateSecondComponent("5");
    public FluentValue Six => CreateSecondComponent("6");
    public FluentValue Seven => CreateSecondComponent("7");
    public FluentValue Eight => CreateSecondComponent("8");
    public FluentValue Nine => CreateSecondComponent("9");
    public FluentValue Ten => CreateSecondComponent("01");//Reverse order, due to reversing the sequence when composing it

    private FluentValue CreateSecondComponent(string serializationSymbol)
    {
        return new(this, serializationSymbol);
    }
}

public abstract class Fluent
{
    public Fluent? Parent;
    public string SerializationString { get; }

    protected Fluent(Fluent? parent, string serializationString)
    {
        Parent = parent;
        SerializationString = serializationString;
    }
}
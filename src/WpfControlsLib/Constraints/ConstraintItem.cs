using System;

public class ConstraintItem
{
    public ValueTuple<string, string, string> Elements { get; set; }

    public int Amount { get; set; }

    public string ObjectType { get; set; }

    public string Output
    {
        get
        {
            if (string.IsNullOrEmpty(this.Elements.Item3))
            {
                return "Type: " + this.ObjectType + Environment.NewLine + this.Elements.Item2 + Environment.NewLine + "Max amount: " + this.Amount;
            }

            return "Type: " + this.ObjectType + Environment.NewLine + this.Elements.Item2 + " -> " + this.Elements.Item3 + Environment.NewLine + "Max amount: " + this.Amount;
        }

        set
        {
        }
    }
}
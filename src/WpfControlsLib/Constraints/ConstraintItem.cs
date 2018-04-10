using System;

/// <summary>
/// Constraints item class for creating constraints item view (an element in Constraints panel).
/// </summary>
public class ConstraintItem
{
    private AttributeTypeEnum attributeType;
    private string output;

    public enum AttributeTypeEnum
    {
        Boolean, Int, String
    }

    public AttributeTypeEnum AttributeTypeFlag
    {
        get
        {
            return this.attributeType;
        }

        set
        {
            this.attributeType = value;
        }
    }

    public (string, string) ElementTypes { get; set; }

    public string ObjectType { get; set; }

    public string AttributeName { get; set; }

    public object Value { get; set; }

    public string Output
    {
        get
        {
            return this.GetOutput();
        }
    }

        private string GetOutput()
    {
        if (this.AttributeTypeFlag == AttributeTypeEnum.Boolean)
        {
            return String.Format("Type: {0}{1}Element: {2}{1}Attribute: {3}{1}Value: {4}",
                this.ObjectType, Environment.NewLine, this.ElementTypes.Item1, this.AttributeName, this.Value.ToString());
        }

        if (this.AttributeTypeFlag == AttributeTypeEnum.Int)
        {
            var value = (ValueTuple<int, int>)this.Value;
            if (!string.IsNullOrEmpty(this.AttributeName))
            {
                return String.Format("Type: {0}{1}Element: {2}{1}Attribute: {3}{1}Values range {4} - {5}",
                    this.ObjectType, Environment.NewLine, this.ElementTypes.Item1,
                    this.AttributeName, value.Item1, value.Item2);
            }

            if (string.IsNullOrEmpty(this.ElementTypes.Item2))
            {
                return String.Format("Type: {0}{1}Element: {2}{1}Amount range: {3} - {4}", 
                    this.ObjectType, Environment.NewLine, this.ElementTypes.Item1
                    + value.Item1, value.Item2);
            }

            return String.Format("Type: {0}{1}{2} -> {3}{1}Amount range: {4} - {5}",
                this.ObjectType, Environment.NewLine, this.ElementTypes.Item1,
                this.ElementTypes.Item2, value.Item1, value.Item2);
        }

        return String.Format("Type: {0}{1}Element: {2}{1}Attribute: {3}{1}Value: {4}",
            this.ObjectType, Environment.NewLine, this.ElementTypes.Item1, this.AttributeName, this.Value);
    }
}
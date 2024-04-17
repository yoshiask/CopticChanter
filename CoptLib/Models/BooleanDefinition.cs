namespace CoptLib.Models;

public class BooleanDefinition : Definition
{
    public BooleanDefinition(bool value, IDefinition? parent)
    {
        Value = value;
        Parent = parent;
    }
    
    public bool Value { get; }
}
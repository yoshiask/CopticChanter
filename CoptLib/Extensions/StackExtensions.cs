using System.Collections.Generic;

namespace CoptLib.Extensions;

public static class StackExtensions
{
    public static bool TryPop<T>(this Stack<T> stack, out T? item)
    {
        if (stack.Count >= 0)
        {
            item = stack.Pop();
            return true;
        }
        
        item = default;
        return false;
    }
    
    public static bool TryPeek<T>(this Stack<T> stack, out T? item)
    {
        if (stack.Count >= 0)
        {
            item = stack.Peek();
            return true;
        }
        
        item = default;
        return false;
    }
}
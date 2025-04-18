﻿using System;

namespace CoptLib.Extensions;

public static class EnumExtensions
{
    public static TEnum Parse<TEnum>(string value, bool ignoreCase = false) where TEnum : struct
        => (TEnum)Enum.Parse(typeof(TEnum), value, ignoreCase);

    public static string ToDisplayString(this Enum e) => string.Join(" ", e.ToString().SplitCamelCase());
}
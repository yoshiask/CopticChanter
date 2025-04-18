﻿using CoptLib.Models;
using CoptLib.Models.Text;
using System;
using CoptLib.IO;

namespace CoptLib.Scripting.Commands;

public class TimestampCmd : TextCommandBase
{
    private readonly string _timePart;
    
    public TimestampCmd(string cmd, InlineCommand inline, IDefinition[] parameters)
        : base(cmd, inline, parameters)
    {
        _timePart = Parameters[0].ToString();
    }

    public TimeSpan TimeOffset { get; private set; }
    
    protected override IDefinition? ExecuteInternal(ILoadContext? context)
    {
        if (TimeSpan.TryParse(_timePart, out var timeOffset))
            TimeOffset = timeOffset;
            
        return default;
    }
}
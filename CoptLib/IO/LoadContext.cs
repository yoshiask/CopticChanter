using CoptLib.Models;

namespace CoptLib.IO;

/// <summary>
/// A load context that loads all items as they are added.
/// </summary>
public class LoadContext : LoadContextBase
{
    public LoadContext()
    {
        AddDefaultDefinitions();
    }

    private void AddDefaultDefinitions()
    {
        foreach (var knownRole in RoleInfo.KnownRoles)
            AddDefinition(knownRole, null);

        AddDefinition(new Variable
        {
            Key = "PopeName",
            Label = "Name of current Pope",
            DefaultValue = string.Empty,
            Configurable = true,
        }, null);
        AddDefinition(new Variable
        {
            Key = "IsPopePresent",
            Label = "Is Pope present?",
            DefaultValue = false,
            Configurable = true,
        }, null);
    }
}
using System;
namespace DSharpPlus.SlashCommands;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class SlashCommandPermissionsAttribute(Permissions permissions) : Attribute
{
    public Permissions Permissions { get; } = permissions;
}

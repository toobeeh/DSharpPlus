using System;

namespace DSharpPlus.SlashCommands;

/// <summary>
/// Specifies a locale for a slash command name. The longest name is the name that counts toward character limits.
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Parameter, AllowMultiple = true)]
public sealed class NameLocalizationAttribute(Localization locale, string name) : Attribute
{
    public string Locale { get; } = LocaleHelper.LocaleToStrings[locale];

    public string Name { get; } = name;
}

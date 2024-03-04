using System;

namespace DSharpPlus.SlashCommands;

/// <summary>
/// Specifies a locale for a slash command description. The longest description is the one that counts toward character limits.
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Parameter, AllowMultiple = true)]
public sealed class DescriptionLocalizationAttribute(Localization locale, string description) : Attribute
{
    public string Locale { get; } = LocaleHelper.LocaleToStrings[locale];

    public string Description { get; } = description;
}

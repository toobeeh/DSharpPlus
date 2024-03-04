using System;

namespace DSharpPlus.SlashCommands;

/// <summary>
/// Sets a IChoiceProvider for a command options. ChoiceProviders can be used to provide
/// DiscordApplicationCommandOptionChoice from external sources such as a database.
/// </summary>
/// <remarks>
/// Adds a choice provider to this command.
/// </remarks>
/// <param name="providerType">The type of the provider.</param>
[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = true)]
public sealed class ChoiceProviderAttribute(Type providerType) : Attribute
{
    /// <summary>
    /// The type of the provider.
    /// </summary>
    public Type ProviderType { get; } = providerType;
}

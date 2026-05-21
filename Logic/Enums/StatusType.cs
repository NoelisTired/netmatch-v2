namespace Logic.Enums;

/// <summary>
/// Levensloop van een offerte. Een nieuwe offerte start als <see cref="Concept"/>
/// (FR-08 autosave werkt op concepten); <see cref="Definitief"/> is verzendklaar.
/// </summary>
public enum StatusType
{
    Concept,
    Definitief
}

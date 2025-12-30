namespace Core.Services.TelegramBot.State.Inspiration;

/// <summary>
/// Represents the active edit session for a user.
/// Stores which inspiration is being edited and which field
/// the next incoming text message should update.
/// </summary>
public sealed class InspirationEditContext
{
    /// <summary>
    /// Identifier of the inspiration being edited.
    /// </summary>
    public string InspirationId
    {
        get;
        init;
    }

    /// <summary>
    /// Specifies which field of the inspiration is being edited.
    /// </summary>
    public EditField Field
    {
        get;
        init;
    }
}

/// <summary>
/// Editable fields of an inspiration.
/// 
/// Determines how the incoming user message will be interpreted
/// and which update operation will be executed.
/// </summary>
public enum EditField
{
    /// <summary>
    /// Main inspiration text/content.
    /// </summary>
    Content,

    /// <summary>
    /// Comma-separated list of tags.
    /// </summary>
    Tags,

    /// <summary>
    /// Short label or category.
    /// </summary>
    Label
}

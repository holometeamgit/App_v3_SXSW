/// <summary>
/// Blind View
/// </summary>
public interface IBlindView {

    /// <summary>
    /// Show View
    /// </summary>
    /// <param name="objects"></param>
    void Show(params object[] objects);

    /// <summary>
    /// Hide View
    /// </summary>
    void Hide();
}

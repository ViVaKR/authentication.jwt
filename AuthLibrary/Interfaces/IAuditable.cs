namespace AuthLibrary.Interfaces;

/// <summary>
/// CreatedAt - 탄생의 흔적
/// UpdatedAt - 변화의 흔적
/// </summary>
public interface IAuditable
{
    DateTime CreatedAt { get; set; }
    DateTime? UpdatedAt { get; set; }
}

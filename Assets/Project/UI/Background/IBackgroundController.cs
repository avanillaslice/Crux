namespace Project.UI.Background
{
    public interface IBackgroundController
    {
        void InitiateScrolling();
        void CheckAndAdd();
        float Duration { get; }
    }
}

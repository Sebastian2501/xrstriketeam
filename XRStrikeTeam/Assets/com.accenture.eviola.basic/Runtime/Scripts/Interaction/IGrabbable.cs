namespace Accenture.eviola.Interaction
{
    public interface IGrabbable
    {
        bool IsBeingGrabbed();
        InteractionEvent OnGrabStateChanged { get; }
    }
}
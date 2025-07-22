using Unity.Netcode.Components;

public class ClientNetworkAnimatior : NetworkAnimator
{
    protected override bool OnIsServerAuthoritative()
    {
        return false;
    }
}

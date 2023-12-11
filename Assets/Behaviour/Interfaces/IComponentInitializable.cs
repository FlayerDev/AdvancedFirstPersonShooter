using Mirror;

interface IComponentInitializable
{
    [ClientRpc]
    public void Init();
}
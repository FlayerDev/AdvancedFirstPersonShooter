using Mirror;
/// <summary>
/// When player is replaced for the connection Awake and Start have already ran without authority
/// Init is implemented to initialize the components MonoBehaviours again after authority has been given
/// </summary>
interface IComponentInitializable
{
    public void Init();
}
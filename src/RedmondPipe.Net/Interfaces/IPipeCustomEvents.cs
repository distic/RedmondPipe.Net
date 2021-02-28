namespace RedmondPipe.Interfaces
{
    internal interface IPipeCustomEvents
    {
        void OnMessageReceived(string message, int size);
    }
}

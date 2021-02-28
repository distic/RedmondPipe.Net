using System.Runtime.InteropServices;

namespace RedmondPipe.Interop
{
    internal class NativeFunctions
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool PeekNamedPipe(SafeHandle handle, 
            byte[] buffer, uint nBufferSize, ref uint bytesRead,
            ref uint bytesAvail, ref uint BytesLeftThisMessage);
    }
}
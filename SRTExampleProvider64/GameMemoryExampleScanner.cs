using ProcessMemory;
using System;
using System.Diagnostics;
using SRTExampleProvider64.Structs.GameStructs;
using System.Linq;

namespace SRTExampleProvider64
{
    internal class GameMemoryExampleScanner : IDisposable
    {
        /// <summary>
        /// VARIABLES
        /// </summary>
        private ProcessMemoryHandler memoryAccess;
        private GameMemoryExample gameMemoryValues;
        public bool HasScanned;
        public bool ProcessRunning => memoryAccess != null && memoryAccess.ProcessRunning;
        public int ProcessExitCode => (memoryAccess != null) ? memoryAccess.ProcessExitCode : 0;

        /// <summary>
        /// POINTER ADDRESS VARIABLES
        /// </summary>
        private long pointerAddressExample;

        /// <summary>
        /// POINTER VARIABLES
        /// </summary>
        private long BaseAddress { get; set; }
        private MultilevelPointer PointerExample { get; set; }

        /// <summary>
        /// CLASS CONTRUCTOR
        /// </summary>
        /// <param name="proc"></param>
        internal GameMemoryExampleScanner(Process process = null)
        {
            gameMemoryValues = new GameMemoryExample();
            if (process != null)
                Initialize(process);
        }

        /// <summary>
        /// INITIALIZE GAME MEMORY SCANNER
        /// </summary>
        internal void Initialize(Process process)
        {
            SelectPointerAddresses(GameHashes.DetectVersion(process.MainModule.FileName));
            int pid = GetProcessId(process).Value;
            memoryAccess = new ProcessMemoryHandler(pid, false);

            if (ProcessRunning)
            {
                BaseAddress = NativeWrappers.GetProcessBaseAddress(pid, PInvoke.ListModules.LIST_MODULES_64BIT).ToInt64(); // Bypass .NET's managed solution for getting this and attempt to get this info ourselves via PInvoke since some users are getting 299 PARTIAL COPY when they seemingly shouldn't.
                PointerExample = new MultilevelPointer(memoryAccess, (IntPtr)(BaseAddress + pointerAddressExample), 0x428L, 0x18L);
            }
        }

        /// <summary>
        /// GETS POINTER ADDRESS OFFSETS FOR SPECIFIC GAME VERSION LOADED
        /// </summary>
        private void SelectPointerAddresses(GameVersion version)
        {
            if (version == GameVersion.GameName_Region_ReleaseData_Patch)
            {
                pointerAddressExample = 0x04D020D0;
            }
            else if (version == GameVersion.UNKNOWN){
                pointerAddressExample = 0x04D020D0;
            } 
        }

        /// <summary>
        /// UPDATES POINTER ADDRESSES
        /// </summary>
        internal void UpdatePointers()
        {
            PointerExample.UpdatePointers();
        }

        /// <summary>
        /// UPDATES GAME MEMORY VALUES FROM POINTERS
        /// </summary>
        internal unsafe IGameMemoryExample Refresh()
        {
            bool success;
            // Example With MultiLevelPointer
            // gameMemoryValues._money = PointerExample.DerefInt(0x20);
            fixed (int* p = &gameMemoryValues._money)
                success = PointerExample.TryDerefInt(0x20, p);

            fixed (int* p = &gameMemoryValues._kudos)
                success = PointerExample.TryDerefInt(0x4C0, p);

            fixed (int* p = &gameMemoryValues._liberty)
                success = PointerExample.TryDerefInt(0x36C, p);

            fixed (int* p = &gameMemoryValues._utility)
                success = PointerExample.TryDerefInt(0x370, p);

            fixed (int* p = &gameMemoryValues._morality)
                success = PointerExample.TryDerefInt(0x374, p);

            HasScanned = true;
            return gameMemoryValues;
        }

        private int? GetProcessId(Process process) => process?.Id;

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    if (memoryAccess != null)
                        memoryAccess.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~REmake1Memory() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
#endregion
    }
}

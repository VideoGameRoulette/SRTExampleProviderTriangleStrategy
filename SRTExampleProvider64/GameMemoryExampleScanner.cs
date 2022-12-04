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
        private long pointerAddressPropsManager;
        private long pointerAddressGameManager;
        private long pointerAddressPauseManager;

        /// <summary>
        /// POINTER VARIABLES
        /// </summary>
        private long BaseAddress { get; set; }
        private MultilevelPointer PointerPropsManager { get; set; }
        private MultilevelPointer PointerGameManager { get; set; }
        private MultilevelPointer PointerPauseManager { get; set; }

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
                PointerGameManager = new MultilevelPointer(memoryAccess, (IntPtr)(BaseAddress + pointerAddressGameManager));
                PointerPropsManager = new MultilevelPointer(memoryAccess, (IntPtr)(BaseAddress + pointerAddressPropsManager), 0x428L, 0x18L);
                PointerPauseManager = new MultilevelPointer(memoryAccess, (IntPtr)(BaseAddress + pointerAddressPauseManager), 0x198L, 0x10L);
            }
        }

        /// <summary>
        /// GETS POINTER ADDRESS OFFSETS FOR SPECIFIC GAME VERSION LOADED
        /// </summary>
        private void SelectPointerAddresses(GameVersion version)
        {
            if (version == GameVersion.GameName_Region_ReleaseData_Patch || version == GameVersion.UNKNOWN)
            {
                pointerAddressGameManager = 0x04A4AD58;
                pointerAddressPropsManager = 0x04D020D0;
                pointerAddressPauseManager = 0x04D05910;
            }
        }

        /// <summary>
        /// UPDATES POINTER ADDRESSES
        /// </summary>
        internal void UpdatePointers()
        {
            PointerPropsManager.UpdatePointers();
            PointerGameManager.UpdatePointers();
            PointerPauseManager.UpdatePointers();
        }

        /// <summary>
        /// UPDATES GAME MEMORY VALUES FROM POINTERS
        /// </summary>
        internal unsafe IGameMemoryExample Refresh()
        {
            bool success;
            
            gameMemoryValues._isGameplay = PointerGameManager.DerefByte(0x0);

            fixed (byte* p = &gameMemoryValues._isPaused)
                success = PointerPauseManager.TryDerefByte(0x0A, p);

            fixed (int* p = &gameMemoryValues._money)
                success = PointerPropsManager.TryDerefInt(0x20, p);

            fixed (int* p = &gameMemoryValues._kudos)
                success = PointerPropsManager.TryDerefInt(0x4C0, p);

            fixed (int* p = &gameMemoryValues._liberty)
                success = PointerPropsManager.TryDerefInt(0x36C, p);

            fixed (int* p = &gameMemoryValues._utility)
                success = PointerPropsManager.TryDerefInt(0x370, p);

            fixed (int* p = &gameMemoryValues._morality)
                success = PointerPropsManager.TryDerefInt(0x374, p);

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

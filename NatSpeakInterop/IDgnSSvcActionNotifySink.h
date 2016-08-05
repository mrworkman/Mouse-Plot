#pragma once

using namespace System;

namespace Dragon {
   namespace ComInterfaces {
      using namespace System::Runtime::InteropServices;

      [ComVisible(true)]
      [Guid("dd108202-6205-11cf-ae61-0000e8a28647")]
      [InterfaceType(ComInterfaceType::InterfaceIsIUnknown)]
      public interface class IDgnSSvcActionNotifySink {
         void PlaybackDone(DWORD);
         void PlaybackAborted(DWORD, HRESULT);
         void ExecutionDone(DWORD);
         void ExecutionStatus(DWORD, DWORD);
         void ExecutionAborted(DWORD, HRESULT, DWORD);
      };
   }
}

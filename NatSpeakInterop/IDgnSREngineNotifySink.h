#pragma once

using namespace System;

namespace Dragon {
   namespace ComInterfaces {
      using namespace System::Runtime::InteropServices;

      [ComVisible(true)]
      [Guid("dd108001-6205-11cf-ae61-0000e8a28647")]
      [InterfaceType(ComInterfaceType::InterfaceIsIUnknown)]
      public interface class IDgnSREngineNotifySink {
         void AttribChanged2(DWORD);
         void Paused(QWORD);
         void MimicDone(DWORD, LPUNKNOWN);
         void ErrorHappened(LPUNKNOWN);
         void Progress(int, const char*);
      };
   }
}

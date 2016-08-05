#pragma once

namespace Dragon {
   namespace ComInterfaces {
      using namespace System::Runtime::InteropServices;

      [ComVisible(true)]
      [Guid("dd108010-6205-11cf-ae61-0000e8a28647")]
      [InterfaceType(ComInterfaceType::InterfaceIsIUnknown)]
      public interface class IDgnGetSinkFlags {
         void SinkFlagsGet(DWORD*);
      };
   }
}
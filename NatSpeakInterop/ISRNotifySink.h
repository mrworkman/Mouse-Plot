#pragma once

using namespace System;

namespace Dragon {
   namespace ComInterfaces {
      using namespace System::Runtime::InteropServices;

      [ComVisible(true)]
      [Guid("090CD9B0-DA1A-11CD-B3CA-00AA0047BA4F")]
      [InterfaceType(ComInterfaceType::InterfaceIsIUnknown)]
      public interface class ISRNotifySink {
         void AttribChanged(DWORD);
         void Interference(QWORD, QWORD, DWORD);
         void Sound(QWORD, QWORD);
         void UtteranceBegin(QWORD);
         void UtteranceEnd(QWORD, QWORD);
         void VUMeter(QWORD, WORD);
      };
   }
}

// Project Renfrew
// Copyright(C) 2016  Stephen Workman (workman.stephen@gmail.com)
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.If not, see<http://www.gnu.org/licenses/>.
//

#pragma once

#include <Windows.h>
#include <vcclr.h>

typedef unsigned __int64 QWORD, *PQWORD;

#include "ComHelper.h"
#include "sinfo.h"

using namespace System;
using namespace System::Collections::Generic;
using namespace System::Diagnostics;
using namespace System::Runtime::InteropServices;
using namespace System::Runtime::InteropServices::ComTypes;

#include "IDgnAppSupport.h"
#include "IDgnDictate.h"
#include "IDgnGetSinkFlags.h"
#include "IDgnSpeechServices.h"
#include "IDgnSrEngineControl.h"
#include "IDgnSrEngineNotifySink.h"
#include "IDgnSrSpeaker.h"
#include "IDgnSSvcActionNotifySink.h"
#include "IDgnSSvcAppTrackingNotifySink.h"
#include "IDgnSSvcInterpreter.h"
#include "IDgnSSvcOutputEvent.h"
#include "IDgnSSvcTracking.h"
#include "IDgnSrGramCommon.h"
#include "ISpchServices.h"
#include "ISrCentral.h"
#include "ISrGramCommon.h"
#include "ISrGramNotifySink.h"
#include "ISrNotifySink.h"
#include "ISrResBasic.h"
#include "ISrResGraph.h"
#include "ISrSpeaker.h"

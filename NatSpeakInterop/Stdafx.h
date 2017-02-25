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

typedef unsigned __int64 QWORD, *PQWORD;

#include  "ComHelper.h"

typedef struct {} PSRMODEINFOW;
typedef struct {} SRGRMFMT;
typedef struct {} SDATA;
typedef struct {} *PSRPHRASEW;

#include "IDgnAppSupport.h"
#include "IDgnDictate.h"
#include "IDgnGetSinkFlags.h"
#include "IDgnSpeechServices.h"
#include "IDgnSrEngineControl.h"
#include "IDgnSrEngineNotifySink.h"
#include "IDgnSrSpeaker.h"
#include "IDgnSSvcActionNotifySink.h"
#include "IDgnSSvcInterpreter.h"
#include "IDgnSSvcOutputEvent.h"
#include "ISpchServices.h"
#include "ISrCentral.h"
#include "ISrGramNotifySink.h"
#include "ISrNotifySink.h"
#include "ISrSpeaker.h"

#include "SrNotifySink.h"
#include "SSvcActionNotifySink.h"
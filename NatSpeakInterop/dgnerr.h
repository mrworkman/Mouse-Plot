// Project Renfrew
// Copyright(C) 2019 Stephen Workman (workman.stephen@gmail.com)
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

#define DGNERR(i) (0x80041000 + (i))

// Some of Dragon's custom COM error codes.
#define DGNERR_UNKNOWNWORD			          DGNERR(1)
#define DGNERR_INVALIDFORM			          DGNERR(2)
#define DGNERR_WAVEDEVICEMISSING	          DGNERR(3)
#define DGNERR_WAVEDEVICEERROR		       DGNERR(4)
#define DGNERR_TERMINATING			          DGNERR(5)
#define DGNERR_MICNOTPAUSED			       DGNERR(6)
#define DGNERR_ENGINENOTPAUSED		       DGNERR(7)
#define DGNERR_INVALIDDIRECTORY		       DGNERR(8)
#define DGNERR_ONLYONETRACKER		          DGNERR(9)
#define DGNERR_INVALIDMODE			          DGNERR(10)
#define DGNERR_ALREADYACTIVE		          DGNERR(11)
#define DGNERR_MODENOTACTIVE		          DGNERR(12)
#define DGNERR_TRAININGFAILED		          DGNERR(13)
#define DGNERR_OUTOFDISK			          DGNERR(14)
#define DGNERR_INVALIDTOPICNAME		       DGNERR(15)
#define DGNERR_TOPICALREADYEXISTS	       DGNERR(16)
#define DGNERR_TOPICDOESNOTEXIST	          DGNERR(17)
#define DGNERR_TOPICALREADYOPEN		       DGNERR(18)
#define DGNERR_TOPICNOTOPEN			       DGNERR(19)
#define DGNERR_TOPICINUSE			          DGNERR(20)
#define DGNERR_INVALIDSPEAKER		          DGNERR(21)
#define DGNERR_LMBUILDACTIVE		          DGNERR(22)
#define DGNERR_LMBUILDINACTIVE		       DGNERR(23)
#define DGNERR_LMBUILDABORTED		          DGNERR(24)
#define DGNERR_NOTASELECTGRAMMAR	          DGNERR(25)
#define DGNERR_DOESNOTMATCHGRAMMAR	       DGNERR(26)
#define DGNERR_OBJECTISLOCKED		          DGNERR(27)
#define DGNERR_CANTLOCK				          DGNERR(28)
#define DGNERR_LMWORDSMISSING		          DGNERR(29)
#define DGNERR_TRANSCRIBING_ON_WITHOUT_OFF DGNERR(30)
#define DGNERR_TRANSCRIBING_OFF_WITHOUT_ON DGNERR(31)
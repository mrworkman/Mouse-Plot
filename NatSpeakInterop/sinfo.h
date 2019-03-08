// Project Renfrew
// Copyright(C) 2017 Stephen Workman (workman.stephen@gmail.com)
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

#define LANG_LEN 0x40
#define SRMI_NAMELEN 0x106

#define ISRNOTEFIN_RECOGNIZED      0x01
#define ISRNOTEFIN_THISGRAMMAR     0x02
#define ISRNOTEFIN_FROMTHISGRAMMAR 0x04

// Related to IDgnSSvcOutputEvent (PlayString, in particular)
#define HOOK_F_SHIFT             0x001 // Add SHIFT to the first character in the string
#define HOOK_F_ALT               0x002 // Add ALT to the first character in the string
#define HOOK_F_CTRL              0x004 // Add CTRL to the first character in the string
#define HOOK_F_RIGHTSHIFT        0x008 // Add RIGHT SHIFT to the first character in the string
#define HOOK_F_RIGHTALT          0x010 // Add RIGHT ALT to the first character in the string
#define HOOK_F_RIGHTCTRL         0x020 // Add RIGHT CTRL to the first character in the string
#define HOOK_F_EXTENDED          0x040 // Use the extended keyboard version of the first character in the string
#define HOOK_F_DEFERTERMINATION  0x100 // Defer termination of PlayString until the event queue is emptied
#define HOOK_F_SYSTEMKEYS        0x200 // Use kbd_event()/mouse_event() instead of JournalPlayback

typedef enum {
   SRGRMFMT_CFG                 = 0x0000,
   SRGRMFMT_LIMITEDDOMAIN       = 0x0001,
   SRGRMFMT_DICTATION           = 0x0002,
   SRGRMFMT_CFGNATIVE           = 0x8000,
   SRGRMFMT_LIMITEDDOMAINNATIVE = 0x8001,
   SRGRMFMT_DICTATIONNATIVE     = 0x8002,
   SRGRMFMT_DRAGONNATIVE1       = 0x8101,
   SRGRMFMT_DRAGONNATIVE2       = 0x8102,
   SRGRMFMT_DRAGONNATIVE3       = 0x8103
} SRGRMFMT, *PSRGRMFMT;

typedef enum {
   CHARSET_TEXT,
   CHARSET_IPAPHONETIC,
   CHARSET_ENGINEPHONETIC
} VOICECHARSET;

typedef enum _VOICEPARTOFSPEECH {
   VPS_UNKNOWN,
   VPS_NOUN,
   VPS_VERB,
   VPS_ADVERB,
   VPS_ADJECTIVE,
   VPS_PROPERNOUN,
   VPS_PRONOUN,
   VPS_CONJUNCTION,
   VPS_CARDINAL,
   VPS_ORDINAL,
   VPS_DETERMINER,
   VPS_QUANTIFIER,
   VPS_PUNCTUATION,
   VPS_CONTRACTION,
   VPS_INTERJECTION,
   VPS_ABBREVIATION,
   VPS_PREPOSITION
} VOICEPARTOFSPEECH;

typedef struct {
   PVOID pData;
   DWORD dwSize;
} SDATA, *PSDATA;

#pragma warning (disable: 4200)
typedef struct {
   DWORD dwSize;
   DWORD dwWordNum;
   WCHAR szWord[0];
} SRWORDW, *PSRWORDW;

typedef struct {
   DWORD dwSize;
   BYTE  abWords[0];
} SRPHRASEW, *PSRPHRASEW;
#pragma warning (default: 4200)

typedef struct {
   LANGID LanguageID;
   WCHAR  szDialect[LANG_LEN];
} LANGUAGEW, FAR *PLANGUAGEW;

typedef struct {
   GUID      gEngineID;
   WCHAR     szMfgName[SRMI_NAMELEN];
   WCHAR     szProductName[SRMI_NAMELEN];
   GUID      gModeID;
   WCHAR     szModeName[SRMI_NAMELEN];
   LANGUAGEW language;
   DWORD     dwSequencing;
   DWORD     dwMaxWordsVocab;
   DWORD     dwMaxWordsState;
   DWORD     dwGrammars;
   DWORD     dwFeatures;
   DWORD     dwInterfaces;
   DWORD     dwEngineFeatures;
} SRMODEINFOW, *PSRMODEINFOW;

typedef struct {
   DWORD dwNextPhonemeNode;
   DWORD dwUpAlternatePhonemeNode;
   DWORD dwDownAlternatePhonemeNode;
   DWORD dwPreviousPhonemeNode;
   DWORD dwWordNode;
   QWORD qwStartTime;
   QWORD qwEndTime;
   DWORD dwPhonemeScore;
   WORD  wVolume;
   WORD  wPitch;
} SRRESPHONEMENODE, *PSRRESPHONEMENODE;

typedef struct {
   DWORD dwNextWordNode;
   DWORD dwUpAlternateWordNode;
   DWORD dwDownAlternateWordNode;
   DWORD dwPreviousWordNode;
   DWORD dwPhonemeNode;
   QWORD qwStartTime;
   QWORD qwEndTime;
   DWORD dwWordScore;
   WORD  wVolume;
   WORD  wPitch;
   VOICEPARTOFSPEECH
           pos;
   DWORD dwCFGParse;
   DWORD dwCue;
} SRRESWORDNODE, *PSRRESWORDNODE;

typedef struct {
   DWORD    message;
   DWORD    paramL;
   DWORD    paramH;
} HOOK_EVENTMSG;

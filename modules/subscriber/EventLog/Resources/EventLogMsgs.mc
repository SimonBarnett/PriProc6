; // EventLogMsgs.mc
; // ********************************************************

; // Use the following commands to build this file:

; //   mc -s EventLogMsgs.mc
; //   rc EventLogMsgs.rc
; //   link /DLL /SUBSYSTEM:WINDOWS /NOENTRY /MACHINE:x86 EventLogMsgs.Res 
; // ********************************************************

; // - Event categories -
; // Categories must be numbered consecutively starting at 1.
; // ********************************************************

MessageIdTypedef = WORD

LanguageNames = (English=0x409:MSG00409)

MessageId=0x1
SymbolicName=SYS_CATEGORY
Language=English
System
.

MessageId=0x2
SymbolicName=APP_CATEGORY
Language=English
Application
.

MessageId=0x3
SymbolicName=WEB_CATEGORY
Language=English
Web
.

; // - Event messages -
; // *********************************

MessageId = 1000
SymbolicName = SYS_MESSAGE_ID_1000
Language=English
System Message> %1
.

MessageId = 1001
SymbolicName = APP_MESSAGE_ID_1000
Language=English
Application Message> %1
.

MessageId = 1002
SymbolicName = WEB_MESSAGE_ID_1000
Language=English
Web message> %1
.


; // - Event log display name -
; // ********************************************************


MessageId = 5001
Severity = Success
Facility = Application
SymbolicName = EVENT_LOG_DISPLAY_NAME_MSGID
Language=English
PriPROC6 Event Log
.




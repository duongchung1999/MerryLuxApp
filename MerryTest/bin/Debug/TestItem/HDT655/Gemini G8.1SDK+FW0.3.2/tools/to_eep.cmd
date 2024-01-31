: # to_eep wrapper for bash and cmd interpreted
:<<"::CMDLITERAL"
@ECHO OFF
GOTO :CMDSCRIPT
::CMDLITERAL

CURPATH=$0;
HERE=$(dirname "${CURPATH}");

case "$(uname)" in
    "Darwin")
        _to_eep_bin=$HERE/../bin/mac/to_eep;;
    *"MINGW"*)
        _to_eep_bin=$HERE/../bin/windows/to_eep;;
    *)
        _to_eep_bin=$HERE/../bin/linux/to_eep;;
esac

"${_to_eep_bin}" "$@"
exit $?

:CMDSCRIPT
SET HERE=%~dp0
"%HERE%\..\bin\windows\to_eep.exe" %*

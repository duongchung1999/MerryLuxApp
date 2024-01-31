: # flashtool wrapper for bash and cmd interpreted
:<<"::CMDLITERAL"
@ECHO OFF
GOTO :CMDSCRIPT
::CMDLITERAL

CURPATH=$0;
HERE=$(dirname "${CURPATH}");

case "$(uname)" in
    "Darwin")
        _flashtool_bin=$HERE/../bin/mac/flashtool;;
    *"MINGW"*)
        _flashtool_bin=$HERE/../bin/windows/flashtool;;
    *)
        _flashtool_bin=$HERE/../bin/linux/flashtool;;
esac

"${_flashtool_bin}" "$@"
exit $?

:CMDSCRIPT
SET HERE=%~dp0
"%HERE%\..\bin\windows\flashtool.exe" %*

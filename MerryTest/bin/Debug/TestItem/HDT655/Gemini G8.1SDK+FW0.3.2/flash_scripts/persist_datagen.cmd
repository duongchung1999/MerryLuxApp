: # persist_datagen wrapper for bash and cmd interpreted
:<<"::CMDLITERAL"
@ECHO OFF
GOTO :CMDSCRIPT
::CMDLITERAL

CURPATH=$0;
HERE=$(dirname "${CURPATH}");

case "$(uname)" in
    "Darwin")
        persist_datagen_bin=$HERE/../bin/mac/persist_datagen;;
    *"MINGW"*)
        persist_datagen_bin=$HERE/../bin/windows/persist_datagen;;
    *)
        persist_datagen_bin=$HERE/../bin/linux/persist_datagen;;
esac

"$persist_datagen_bin" "$@"
exit $?

:CMDSCRIPT
SET HERE=%~dp0
"%HERE%\..\bin\windows\persist_datagen.exe" %*

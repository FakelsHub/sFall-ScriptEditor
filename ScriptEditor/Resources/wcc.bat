@echo off
set fscript=%1
set pfile=%2
set def=%3

echo ---------------------------------------------------------
echo Open Watcom preprocessing script:
echo    %1
echo ---------------------------------------------------------
wcc386.exe %fscript% /pc /fo=%pfile% %def% 

if not exist %pfile% goto FAILED

echo Created preprocessing file for compiling:
echo    %~dp2%~n1_[wcc].ssl 

echo [Done] Preprocessing script successfully completed.
goto DONE

:FAILED
echo [Error] Preprocessing script failed...
Exit 1

:DONE


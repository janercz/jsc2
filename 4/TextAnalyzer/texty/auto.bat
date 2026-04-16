@echo off
setlocal
echo Preparing sample text data...
for %%F in (*.txt) do (
    echo Found text file: %%F
)
echo Done.
endlocal

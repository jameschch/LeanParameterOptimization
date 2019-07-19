rem Bypass "Terminate Batch Job" prompt.
if "%~1"=="-FIXED_CTRL_C" (
   REM Remove the -FIXED_CTRL_C parameter
   SHIFT
) ELSE (
   REM Run the batch with <NUL and -FIXED_CTRL_C
   CALL <NUL %0 -FIXED_CTRL_C %*
   GOTO :EOF
)

dotnet build Jtc.Optimization.Api\Jtc.Optimization.Api.csproj -c Debug
START "Jtc.Optimization.Api" "cmd.exe" "/C dotnet run --project Jtc.Optimization.Api\Jtc.Optimization.Api.csproj  --no-build -c Debug"
"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe"  --purge-memory-button --auto-open-devtools-for-tabs --remote-debugging-port=9222 --new-window "http://localhost:61222/chart"

FOR /L %%A IN (1,1,10) DO (
	rem dotnet build Jtc.Optimization.BlazorClient\Jtc.Optimization.BlazorClient.csproj -c Debug
	dotnet run --project Jtc.Optimization.BlazorClient\Jtc.Optimization.BlazorClient.csproj -c Debug  
)


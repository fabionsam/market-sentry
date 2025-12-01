@echo off
setlocal

:: --- CONFIGURAÇÕES ---
set "NOME_PROJETO_UI=MarketSentry.UI"
set "NOME_PROJETO_API=MarketSentry.API"
set "PASTA_FINAL=Instalador_MarketSentry"

echo ==========================================
echo      GERADOR DE BUILD - MARKET SENTRY
echo ==========================================
echo.

:: 1. Limpeza
if exist "%PASTA_FINAL%" (
    echo [1/6] Limpando build anterior...
    rmdir /s /q "%PASTA_FINAL%"
)

:: 2. Build do Angular (Frontend)
echo [2/6] Compilando o Frontend (Angular)...
cd %NOME_PROJETO_UI%
call npm install
call ng build --configuration production
if %errorlevel% neq 0 (
    echo [ERRO] Falha no build do Angular.
    pause
    exit /b %errorlevel%
)
cd ..

:: 3. Publish do .NET (Backend + Worker)
echo [3/6] Compilando o Backend (.NET)...
dotnet publish %NOME_PROJETO_API% -c Release -r win-x64 --self-contained -p:PublishSingleFile=true -o "%PASTA_FINAL%"
if %errorlevel% neq 0 (
    echo [ERRO] Falha no publish do .NET.
    pause
    exit /b %errorlevel%
)

:: 4. Copiar arquivos do Angular para wwwroot
echo [4/6] Integrando Frontend ao Backend...
:: Cria a pasta wwwroot se o .NET não tiver criado
if not exist "%PASTA_FINAL%\wwwroot" mkdir "%PASTA_FINAL%\wwwroot"

:: Copia o conteúdo da dist do Angular para a wwwroot
:: ATENÇÃO: O caminho 'dist\%NOME_PROJETO_UI%\browser' depende da versão do Angular.
:: Se der erro aqui, verifique se sua pasta dist tem a subpasta 'browser'.
xcopy "%NOME_PROJETO_UI%\dist\%NOME_PROJETO_UI%\browser\*.*" "%PASTA_FINAL%\wwwroot" /E /H /C /I /Y > nul

:: 5. Copiar Banco de Dados (Se existir na raiz, para não perder dados)
if exist "MarketSentry.API\market_sentry.db" (
    echo [5/6] Copiando banco de dados de desenvolvimento...
    copy "MarketSentry.API\market_sentry.db" "%PASTA_FINAL%\" > nul
) else (
    echo [5/6] Banco de dados não encontrado, será criado na primeira execução.
)

:: 6. Criar o arquivo de Início Fácil
echo [6/6] Criando atalho de inicializacao...
(
echo @echo off
echo echo Iniciando Market Sentry...
echo echo O navegador abrira em instantes.
echo start http://localhost:5000
echo start MarketSentry.API.exe
) > "%PASTA_FINAL%\Iniciar_Sistema.bat"

echo.
echo ==========================================
echo    SUCESSO! PASTA FINAL CRIADA:
echo    %CD%\%PASTA_FINAL%
echo ==========================================
echo.
pause
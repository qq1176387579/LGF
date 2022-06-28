@SET EXCEL_FOLDER=.\
@SET JSON_FOLDER=..\Assets\Resources\AllJsonDatas
@SET EXE=.\ExcelToJsonTool\ExcelToJson.exe

@CALL %EXE% --excel %EXCEL_FOLDER% --json %JSON_FOLDER%
::pause
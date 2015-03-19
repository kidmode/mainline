java -jar .\Localization\csv_ripper.jar "LocalizationSpreadsheet.csv"

XCOPY /E /Y  ".\output" "..\Resources\Localization"

RMDIR /S /Q .\output

PAUSE
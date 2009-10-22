ndbconsole create MySql "User=root;Database=test_user" YourDll.dll
ndbconsole alter MySql "User=root;Database=test_user" YourDll.dll
ndbconsole drop MySql "User=root;Database=test_user" YourDll.dll
ndbconsole generate MySql "User=root;Database=test_user" "c:\temp"
ndbconsole check MySql "User=root;Database=test_user" YourDll.dll
ndbconsole importexcel MySql "User=root;Database=test_user" "ExcelConnectionString" YourDll.dll

pause
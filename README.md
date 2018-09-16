# DataController
### Library designed to communicate with an ms sql database, using provided credentials.

Controller handles simple commands between client and database with framework for encryption. SqlInjection prevention methods have been implemented and the majority of possible errors have been handled. 
The controller is accompanied with a settings file, allowing storage of connection details such as server name, ip and log in information. This data can be changed during run time assuming the controller 
is succesfully disposed of and re-initalised when doing so to avoid major errors.

The DataCommand file has been provided with the base commands for any database connection, it is intended that this class is extended for your commands and for the implmentation of encrpytion you may wish 
to use, simply call the SecurityManager routines to do so. 

Validation methods are also present allowing for simple checks such if string is Email, Postcode, Phone, ValidPassword or other RegEx and length checks.

To use this library please download the .dll file and add as a reference in your project. (DataController.dll)

Please note that testing is still in progress for this library and some aspects may not work at this time, it is advised that you wait before attempting to use this library however feel free to test it yourself.

## Database commands
```
InitaliseDatabase(string server, string database, string username, string password, bool trustedConnection = false, int timeoutDelay = 15)

ConnectToDatabase()

GenerateNewRow(string tableName, string[] newData, string[] columnsTargetted = null)

FetchDatabaseData(string tableName,  string queryTerm, string queryItem, string[] requestedColumns = null)

UpdateRowData(string tableName, string queryTerm, string queryItem, string[] columnsToUpdate, string[] newColumnData)

DeleteRowData(string tableName, string queryTerm, string queryItem)
```

## Encrpytion commands
```
string GenerateNewSALT(int len = 32)        

EncryptPassword(string pass, string salt)
        
ComparePassword(string encryptedPassword, string passToCheck, string salt)

EncryptData(string data, string encryptionPass, bool isSearchTerm = false)

DecrpytData(string encrpytedData, string encryptionPass, bool isSearchTerm = false)

EncryptFile(string[] fileContents, string encryptionPassword, string salt)

DecryptFile(string[] encryptedContents, string encrpytionPassword)
            
```

## Validation commands
```
ValidateColumns(string tableName, string[] columns, bool ignoreIfNull = false)

ValidateColumn(string tableName, string column, bool ignoreIfNull = false)

ValidateNewEntry(string tableName, int newColumns)

ValidateName(string toValidate, int minLength = 4, int maxLength = 20)

ValidateUsername(string toValidate, int minLength = 3, int maxLength = 32)

ValidatePassword(string toValidate, int minLength = 8, int maxLength = 32)

ValidateEmail(string toValidate, int minLength = 5, int maxLength = 32)

ValidatePhone(string toValidate, int minLength = 8, int maxLength = 32)

ValidateAddress(string toValidate, int minLength = 8, int maxLength = 32)

ValidatePostCode(string toValidate, int minLength = 8, int maxLength = 32)

```

When extended database commands I suggest viewing both DataController.cs and DataCommands.cs.

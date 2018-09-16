# DataController
## Library designed to communicate with an ms sql database, using provided credentials.

Controller handles simple commands between client and database with framework for encryption. SqlInjection prevention methods have been implemented and the majority of possible errors have been handled. 
The controller is accompanied with a settings file, allowing storage of connection details such as server name, ip and log in information. This data can be changed during run time assuming the controller 
is succesfully disposed of and re-initalised when doing so to avoid major errors.

The DataCommand file has been provided with the base commands for any database connection, it is intended that this class is extended for your commands and for the implmentation of encrpytion you may wish 
to use, simply call the SecurityManager routines to do so. 

Validation methods are also present allowing for simple checks such if string is Email, Postcode, Phone, ValidPassword or other RegEx and length checks.

To use this library please download the .dll file and add as a reference in your project. (DataController.dll)

Please note that testing is still in progress for this library and some aspects may not work at this time, it is advised that you wait before attempting to use this library however feel free to test it yourself.
import sqlite3
# Make sure that the WebSecurity.db file is in the same folder. Or if using an IDE like Google Colab, upload the file.
conn = sqlite3.connect('WebSecurity.db') # Connects to the db file in the same folder and returns a connection object
cursor = conn.cursor() # Uses the db connection object to get a cursor, allowing us to traverse the database

def SusceptibleToSQLi():

    expectedInput = 'CA' # This input is the two letter state code and it an expected input from users of our WebApp
    maliciousInput = "CA' UNION SELECT ingredients from SecretRecipes;--" # This input is designed to perform SQL injection. Basically it takes the expected input and adds additional SQL code to get information from another table in our DB.

    state = expectedInput
    state = maliciousInput # &lt;---- UNCOMMENT LINE TO PERFORM SQLi ATTACK using malicious input
    sql = "SELECT BrandName FROM Competition WHERE State='" + state + "'"

    # This loop simply prints out the content of the SQL query results
    for row in cursor.execute(sql):
        print(row[0])

def NotSusceptibleToSQLi():
    expectedInput = 'CA' # This input is the two letter state code and it an expected input from users of our WebApp
    maliciousInput = "CA' UNION SELECT ingredients from SecretRecipes;--" # This input is designed to perform SQL injection. Basically it takes the expected input and adds additional SQL code to get information from another table in our DB.

    state = expectedInput
    #state = maliciousInput # &lt;---- UNCOMMENT LINE TO PERFORM SQLi ATTACK using malicious input
    parameterizedSQL = "SELECT BrandName FROM Competition WHERE State=?" # This is an example of a parameterized SQL query. The state is not added using "string interpolation".

    print('\n~~~Results with Simple Placeholders~~~')
    for row in cursor.execute(parameterizedSQL, (state,)): # Here we run our parameterized query. The syntax with the comma is required in python because the library is expecting a list for parameters. For 2 or more, just separate the items with commas.
        print(row[0])


    # We can also use Named placeholders
    parameterizedSQL = "SELECT BrandName FROM Competition WHERE State=:state" # This is an example of a parameterized SQL query with named placeholders.
    print('\n~~~Results with Named Placeholders~~~')
    for row in cursor.execute(parameterizedSQL, {'state': state,}): # Here we run our parameterized query. The syntax with the comma is required in python because the library is expecting a list for parameters. For 2 or more, just separate the items with commas.
        print(row[0])

def FixSQLi():
    expectedDescription = '%' + 'origin%' 
    expectedCaneSugar = 1 
    maliciousCaneSugar = "1 UNION SELECT ingredients from SecretRecipes;--" 

    # CHANGE FOR MALICIOUS/EXPECTED
    sugar = expectedCaneSugar
    #sugar = maliciousCaneSugar 
    
    # NORMAL SQL
    # sql = "SELECT BrandName from Competition WHERE Description LIKE '" + expectedDescription + "' and CaneSugar=" + str(sugar)
    # for row in cursor.execute(sql):
    #     print(row[0])

    #PARAMETERIZED SQL
    parameterizedSQL = "SELECT BrandName from Competition WHERE Description LIKE ? and CaneSugar=?"
    for row in cursor.execute(parameterizedSQL, (expectedDescription, str(sugar))):
        print(row[0])

FixSQLi()


#SusceptibleToSQLi()
# NotSusceptibleToSQLi()

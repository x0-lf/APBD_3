using Microsoft.Data.SqlClient;

namespace APBD_3.Data;

public interface IDbConnectionFactory
{
    /*We can change this to IDbConnection -> Other Different Databases
        this allows us for the
        support multiple DBs in the future:
        - Like PostgreSQL/MySQL
    */
    SqlConnection CreateConnection(); 

}
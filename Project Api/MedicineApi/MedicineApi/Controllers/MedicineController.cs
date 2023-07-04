using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Tracing;

namespace MedicineApi.Controllers
{
    public class MedicineController : ApiController
    {

        string connectionString = @"Data Source=DESKTOP-CEUJHK7;Initial Catalog=MedicineDB;Persist Security Info=True;User ID=sa;Password=123;";
        string newdb;

        private void Log(string message)
        {
            // You can replace this implementation with your desired logging implementation
            // For example, using Serilog:
            Console.WriteLine(message);
        }
        [HttpGet]
        public HttpResponseMessage Login(string email, string password)
        {
            try
            {
                // Step 1: Fetch the list of user databases from the master database
                var connectionString = @"Data Source=DESKTOP-CEUJHK7;Initial Catalog=master;Persist Security Info=True;User ID=sa;Password=123;";
                var userDatabases = new List<string>();
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (var command = new SqlCommand("SELECT name FROM sys.databases", connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                userDatabases.Add(reader.GetString(0));
                            }
                        }
                    }
                }

                // Step 2: Iterate over each user database and try to authenticate the user
                foreach (var userDatabase in userDatabases)
                {
                    newdb = $@"Data Source=DESKTOP-CEUJHK7;Initial Catalog={userDatabase};User ID=sa;Password=123;";
                    var userDb = new SqlConnection(newdb);
                    userDb.Open();
                    try
                    {
                        using (var command = new SqlCommand("SELECT Email, Password FROM Appuserss WHERE Email = @Email AND Password = @Password", userDb))
                        {
                            command.Parameters.AddWithValue("@Email", email);
                            command.Parameters.AddWithValue("@Password", password);
                            using (var reader = command.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    var user1Db = userDatabase;
                                    // Authentication succeeded, return a successful response
                                    return Request.CreateResponse(HttpStatusCode.OK, new { userDdb = user1Db });
                                }
                            }
                        }
                    }
                    catch (SqlException)
                    {
                        // Skip to the next database
                        continue;
                    }
                }

                // Step 4: Return a failed response if the user was not authenticated in any database
                return Request.CreateResponse(HttpStatusCode.Unauthorized, "Invalid username or password");
            }
            catch (Exception ex)
            {
                Log($"An error occurred while logging in the user at {DateTime.Now}: {ex}");
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "An error occurred while logging in the user", ex);
            }
        }
        [HttpPost]
        public HttpResponseMessage AddMedicine(Medicine med)
        {
            string connectionString = @"Data Source=DESKTOP-CEUJHK7;Initial Catalog=MedicineDB;Persist Security Info=True;User ID=sa;Password=123;";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Check if medicine already exists
                string selectQuery = "SELECT COUNT(*) FROM Medicine WHERE Name = @Name AND Type = @Type";
                using (SqlCommand command = new SqlCommand(selectQuery, connection))
                {
                    command.Parameters.AddWithValue("@Name", med.Name);
                    command.Parameters.AddWithValue("@Type", med.Type);
                    int count = Convert.ToInt32(command.ExecuteScalar());

                    if (count > 0)
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest, "Medicine already exists");
                    }
                }

                // Insert new medicine into database
                string insertQuery = "INSERT INTO Medicine (Name, Type, Status) VALUES (@Name, @Type, @Status); SELECT SCOPE_IDENTITY()";
                using (SqlCommand command = new SqlCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@Name", med.Name);
                    command.Parameters.AddWithValue("@Type", med.Type);
                    command.Parameters.AddWithValue("@Status", 1);
                    int insertedId = Convert.ToInt32(command.ExecuteScalar());
                    med.id = insertedId;
                }
            }

            return Request.CreateResponse(HttpStatusCode.OK, "Medicine added successfully");
        }

        public class Medicine
        {
            public int id { get; set; }
            public string Name { get; set; }
            public string Type { get; set; }
        }
        [HttpGet]
        public HttpResponseMessage GetMedicine()
        {
            string connectionString = @"Data Source=DESKTOP-CEUJHK7;Initial Catalog=MedicineDB;Persist Security Info=True;User ID=sa;Password=123;";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Select all medicines from database
                string selectQuery = "SELECT Name FROM Medicine WHERE Status = 1   ";
                using (SqlCommand command = new SqlCommand(selectQuery, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        List<string> medicineNames = new List<string>();
                        while (reader.Read())
                        {
                            medicineNames.Add(reader.GetString(0));
                        }

                        return Request.CreateResponse(HttpStatusCode.OK, medicineNames);
                    }
                }
            }
        }


        [HttpGet]
        public HttpResponseMessage GetDMedicine(string disease)
        {
            string connectionString = @"Data Source=DESKTOP-CEUJHK7;Initial Catalog=MedicineDB;Persist Security Info=True;User ID=sa;Password=123;";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Select medicines that are not contraindicated for the given disease
                string selectQuery = @"
            SELECT M.Name
            FROM Medicine M
            WHERE M.id NOT IN (
                SELECT DC.id
                FROM DiseaseContraindication DC
                INNER JOIN Disease D ON DC.Did = D.Did
                WHERE D.DiseaseName = @Disease
            )";
                using (SqlCommand command = new SqlCommand(selectQuery, connection))
                {
                    command.Parameters.AddWithValue("@Disease", disease);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        List<string> medicineNames = new List<string>();
                        while (reader.Read())
                        {
                            medicineNames.Add(reader.GetString(0));
                        }

                        return Request.CreateResponse(HttpStatusCode.OK, medicineNames);
                    }
                }
            }
        }


        [HttpPut]
        public HttpResponseMessage BanMedicine(string medicineName)
        {
            string connectionString = @"Data Source=DESKTOP-CEUJHK7;Initial Catalog=MedicineDB;Persist Security Info=True;User ID=sa;Password=123;";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Update the status of the medicine to "0"
                string updateQuery = "UPDATE Medicine SET Status = 0 WHERE Name = @MedicineName";
                using (SqlCommand command = new SqlCommand(updateQuery, connection))
                {
                    command.Parameters.AddWithValue("@MedicineName", medicineName);
                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, "Medicine banned successfully");
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound, "Medicine not found");
                    }
                }
            }
        }
        public class Contraindication
        {
            public string MedicineName { get; set; }
            public string DiseaseName { get; set; }
            public string Comment { get; set; }
        }

        [HttpPost]
        public HttpResponseMessage AddDiseaseContraindication(Contraindication contra)
        {
            try
            {
                // Establish database connection
                string connectionString = @"Data Source=DESKTOP-CEUJHK7;Initial Catalog=MedicineDB;Persist Security Info=True;User ID=sa;Password=123;";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Check if the disease exists in the database
                    int diseaseId;
                    string selectDiseaseQuery = "SELECT Did FROM Disease WHERE DiseaseName = @DiseaseName";
                    using (SqlCommand selectDiseaseCommand = new SqlCommand(selectDiseaseQuery, connection))
                    {
                        selectDiseaseCommand.Parameters.AddWithValue("@DiseaseName", contra.DiseaseName);
                        object result = selectDiseaseCommand.ExecuteScalar();
                        if (result == null)
                        {
                            // Disease doesn't exist, insert it into the database
                            string insertDiseaseQuery = "INSERT INTO Disease (DiseaseName) VALUES (@DiseaseName); SELECT SCOPE_IDENTITY()";
                            using (SqlCommand insertDiseaseCommand = new SqlCommand(insertDiseaseQuery, connection))
                            {
                                insertDiseaseCommand.Parameters.AddWithValue("@DiseaseName", contra.DiseaseName);
                                diseaseId = Convert.ToInt32(insertDiseaseCommand.ExecuteScalar());
                            }
                        }
                        else
                        {
                            // Disease exists, get its ID
                            diseaseId = Convert.ToInt32(result);
                        }
                    }

                    // Get the medicine ID
                    int medicineId;
                    string selectMedicineQuery = "SELECT id FROM Medicine WHERE Name = @MedicineName";
                    using (SqlCommand selectMedicineCommand = new SqlCommand(selectMedicineQuery, connection))
                    {
                        selectMedicineCommand.Parameters.AddWithValue("@MedicineName", contra.MedicineName);
                        object result = selectMedicineCommand.ExecuteScalar();
                        if (result == null)
                        {
                            // Medicine not found, handle error
                            return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Medicine not found");
                        }
                        else
                        {
                            // Medicine found, get its ID
                            medicineId = Convert.ToInt32(result);
                        }
                    }

                    // Insert the data into the DiseaseContraindication table
                    string insertQuery = "INSERT INTO DiseaseContraindication (id, Did, Comment) VALUES (@MedicineId, @DiseaseId, @Comment)";
                    using (SqlCommand insertCommand = new SqlCommand(insertQuery, connection))
                    {
                        insertCommand.Parameters.AddWithValue("@MedicineId", medicineId);
                        insertCommand.Parameters.AddWithValue("@DiseaseId", diseaseId);
                        insertCommand.Parameters.AddWithValue("@Comment", contra.Comment);
                        insertCommand.ExecuteNonQuery();
                    }

                    // Return success response
                    return Request.CreateResponse(HttpStatusCode.OK, "Disease contraindication added successfully");
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "An error occurred while adding disease contraindication: " + ex.Message);
            }
        }

        public class Contraindication2
        {
            public string MedicineName { get; set; }
            public string MedicineName2 { get; set; }
            public string Comment { get; set; }
        }

        [HttpPost]
        public HttpResponseMessage AddMedicineContraindication(Contraindication2 contra)
        {
            try
            {
                // Establish database connection
                string connectionString = @"Data Source=DESKTOP-CEUJHK7;Initial Catalog=MedicineDB;Persist Security Info=True;User ID=sa;Password=123;";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                   
                    // Get the medicine ID
                    int medicineId;
                    string selectMedicineQuery = "SELECT id FROM Medicine WHERE Name = @MedicineName";
                    using (SqlCommand selectMedicineCommand = new SqlCommand(selectMedicineQuery, connection))
                    {
                        selectMedicineCommand.Parameters.AddWithValue("@MedicineName", contra.MedicineName);
                        object result = selectMedicineCommand.ExecuteScalar();
                        if (result == null)
                        {
                            // Medicine not found, handle error
                            return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Medicine not found");
                        }
                        else
                        {
                            // Medicine found, get its ID
                            medicineId = Convert.ToInt32(result);
                        }
                    }
                    int medicineId2;
                    string selectMedicineQuery2 = "SELECT id FROM Medicine WHERE Name = @MedicineName2";
                    using (SqlCommand selectMedicineCommand = new SqlCommand(selectMedicineQuery2, connection))
                    {
                        selectMedicineCommand.Parameters.AddWithValue("@MedicineName2", contra.MedicineName2);
                        object result = selectMedicineCommand.ExecuteScalar();
                        if (result == null)
                        {
                            // Medicine not found, handle error
                            return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Medicine not found");
                        }
                        else
                        {
                            // Medicine found, get its ID
                            medicineId2 = Convert.ToInt32(result);
                        }
                    }


                    // Insert the data into the DiseaseContraindication table
                    string insertQuery = "INSERT INTO MedicineContraindication (id, C_id, Comment) VALUES (@MedicineId, @MedicineId2, @Comment)";
                    using (SqlCommand insertCommand = new SqlCommand(insertQuery, connection))
                    {
                        insertCommand.Parameters.AddWithValue("@MedicineId", medicineId);
                        insertCommand.Parameters.AddWithValue("@MedicineId2",medicineId2 );
                        insertCommand.Parameters.AddWithValue("@Comment", contra.Comment);
                        insertCommand.ExecuteNonQuery();
                    }

                    // Return success response
                    return Request.CreateResponse(HttpStatusCode.OK, "Medicine contraindication added successfully");
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "An error occurred while add Medicine contraindication: " + ex.Message);
            }
        }

        [HttpGet]
        public HttpResponseMessage GetAllMedicineContraindications()
        {
            try
            {
                // Establish database connection
                string connectionString = @"Data Source=DESKTOP-CEUJHK7;Initial Catalog=MedicineDB;Persist Security Info=True;User ID=sa;Password=123;";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Retrieve all medicines and their contraindications
                    string query = "SELECT m.Name AS MedicineName, mc.Comment, cm.Name AS ContraindicationName " +
                                   "FROM MedicineContraindication mc " +
                                   "JOIN Medicine m ON m.id = mc.id " +
                                   "JOIN Medicine cm ON cm.id = mc.C_id";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            // Create a list to hold the retrieved data
                            List<object> data = new List<object>();

                            while (reader.Read())
                            {
                                // Get the data from the reader
                                string medicineName = reader["MedicineName"].ToString();
                                string contraindicationName = reader["ContraindicationName"].ToString();
                                string comment = reader["Comment"].ToString();

                                // Create an object to hold the retrieved data
                                var entry = new
                                {
                                    MedicineName = medicineName,
                                    ContraindicationName = contraindicationName,
                                    Comment = comment
                                };

                                // Add the object to the list
                                data.Add(entry);
                            }

                            // Return success response with the retrieved data
                            return Request.CreateResponse(HttpStatusCode.OK, data);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "An error occurred while retrieving medicine contraindications: " + ex.Message);
            }
        }
        [HttpGet]
        public HttpResponseMessage GetAllMedicineToDiseaseContraindications()
        {
            try
            {
                // Establish database connection
                string connectionString = @"Data Source=DESKTOP-CEUJHK7;Initial Catalog=MedicineDB;Persist Security Info=True;User ID=sa;Password=123;";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Retrieve all medicines and their disease contraindications
                    string selectQuery = "SELECT Medicine.Name AS MedicineName, Disease.DiseaseName, DiseaseContraindication.Comment " +
                                  "FROM DiseaseContraindication " +
                                  "JOIN Medicine ON Medicine.id = DiseaseContraindication.id " +
                                  "JOIN Disease ON Disease.Did = DiseaseContraindication.Did";

                    using (SqlCommand command = new SqlCommand(selectQuery, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            // Create a list to hold the retrieved data
                            List<object> data = new List<object>();

                            while (reader.Read())
                            {
                                // Get the data from the reader
                                string medicineName = reader["MedicineName"].ToString();
                                string contraindicationName = reader["DiseaseName"].ToString();
                                string comment = reader["Comment"].ToString();

                                // Create an object to hold the retrieved data
                                var entry = new
                                {
                                    MedicineName = medicineName,
                                    ContraindicationName = contraindicationName,
                                    Comment = comment
                                };

                                // Add the object to the list
                                data.Add(entry);
                            }

                            // Return success response with the retrieved data
                            return Request.CreateResponse(HttpStatusCode.OK, data);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "An error occurred while retrieving medicine to disease contraindications: " + ex.Message);
            }
        }
        [HttpGet]
        public HttpResponseMessage GetContraindicationComment(string disease, string medicine)
        {
            try
            {
                // Establish database connection
                string connectionString = @"Data Source=DESKTOP-CEUJHK7;Initial Catalog=MedicineDB;Persist Security Info=True;User ID=sa;Password=123;";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Retrieve the comment for the given disease and medicine
                    string selectQuery = "SELECT DiseaseContraindication.Comment " +
                                         "FROM DiseaseContraindication " +
                                         "JOIN Medicine ON Medicine.id = DiseaseContraindication.id " +
                                         "JOIN Disease ON Disease.Did = DiseaseContraindication.Did " +
                                         "WHERE Disease.DiseaseName = @DiseaseName AND Medicine.Name = @MedicineName";

                    using (SqlCommand command = new SqlCommand(selectQuery, connection))
                    {
                        // Set the parameter values
                        command.Parameters.AddWithValue("@DiseaseName", disease);
                        command.Parameters.AddWithValue("@MedicineName", medicine);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Get the comment from the reader
                                string comment = reader["Comment"].ToString();

                                // Return success response with the comment
                                return Request.CreateResponse(HttpStatusCode.OK, comment);
                            }
                            else
                            {
                                // Return a not found response if no matching record is found
                                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No contraindication found for the given disease and medicine.");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "An error occurred while retrieving contraindication comment: " + ex.Message);
            }
        }

        [HttpGet]
        public HttpResponseMessage GetMedicineContraindicationsByName(string medicineName)
        {
            try
            {
                // Establish database connection
                string connectionString = @"Data Source=DESKTOP-CEUJHK7;Initial Catalog=MedicineDB;Persist Security Info=True;User ID=sa;Password=123;";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Retrieve contraindications for the specified medicine
                    string query = "SELECT m.Name AS MedicineName, mc.Comment, cm.Name AS ContraindicationName " +
                                   "FROM MedicineContraindication mc " +
                                   "JOIN Medicine m ON m.id = mc.id " +
                                   "JOIN Medicine cm ON cm.id = mc.C_id " +
                                   "WHERE m.Name = @MedicineName"; // Filter by the provided medicineName parameter

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        // Add the parameter for the medicine name
                        command.Parameters.AddWithValue("@MedicineName", medicineName);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            // Create a list to hold the retrieved data
                            List<object> data = new List<object>();

                            while (reader.Read())
                            {
                                // Get the data from the reader
                                string contraindicationName = reader["ContraindicationName"].ToString();
                                string comment = reader["Comment"].ToString();

                                // Create an object to hold the retrieved data
                                var entry = new
                                {
                                    MedicineName = medicineName,
                                    ContraindicationName = contraindicationName,
                                    Comment = comment
                                };

                                // Add the object to the list
                                data.Add(entry);
                            }

                            // Return success response with the retrieved data
                            return Request.CreateResponse(HttpStatusCode.OK, data);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "An error occurred while retrieving medicine contraindications: " + ex.Message);
            }
        }


    }
}


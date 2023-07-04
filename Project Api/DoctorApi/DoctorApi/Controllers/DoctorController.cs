using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Tracing;

namespace DoctorApi.Controllers
{
    public class DoctorController : ApiController
    {
        public class UserSignupParameters
        {
            public string Name { get; set; }
            public string PhoneNumber { get; set; }
            public Int64 CNIC { get; set; }
            public string Email { get; set; }
            public string Address { get; set; }
            public string Specialization { get; set; }
            public string Password { get; set; }
            public string Gender { get; set; }
        }

        string connectionString = @"Data Source=DESKTOP-CEUJHK7;Initial Catalog=master;Persist Security Info=True;User ID=sa;Password=123;";
        string newdb;
        [HttpPost]
        public HttpResponseMessage Signup(UserSignupParameters parameters)
        {
            try
            {
                // Step 1: Create a new user account in your ASP.NET application
                // ...

                // Step 2: Check if the user database already exists
                string dbName = parameters.Name + "Dr_" + DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
                string userDbConnectionString = $@"Data Source=DESKTOP-CEUJHK7;Initial Catalog=master;Persist Security Info=True;User ID=sa;Password=123;";

                using (var conn = new SqlConnection(userDbConnectionString))
                {
                    conn.Open();
                    var cmd = new SqlCommand($"SELECT COUNT(*) FROM sys.databases WHERE name = '{dbName}'", conn);
                    var result = (int)cmd.ExecuteScalar();

                    if (result > 0)
                    {
                        // The database already exists, handle the error or return an appropriate message
                        //  ...
                        Log($"User database already exists for {dbName} at {DateTimeOffset.UtcNow}");
                        return Request.CreateResponse(HttpStatusCode.BadRequest, $"User database already exists for {dbName} at {DateTimeOffset.UtcNow}");
                    }
                    else
                        try
                        {
                            cmd = new SqlCommand($"CREATE DATABASE {dbName}", conn);
                            cmd.ExecuteNonQuery();
                        }
                        catch (Exception ex)
                        {
                            // Log or print the exception details
                            Console.WriteLine("An error occurred while creating the database: " + ex.Message);
                            return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "An error occurred while creating the database", ex);
                        }
                        finally
                        {
                            conn.Close();
                        }
                    string DbConnectionString = $@"Data Source=DESKTOP-CEUJHK7;Initial Catalog={dbName};Persist Security Info=True;User ID=sa;Password=123;";
                    using (var con = new SqlConnection(DbConnectionString))
                    {

                        con.Open();

                        // Step 4: Use ADO.NET to execute the SQL script to create the schema of the new database based on the template database
                        var sqlScript = System.IO.File.ReadAllText(@"D:\Semester 7th\Rx for Pakisatn With EHR\DatabaseBackup\DoctorDb.sql");
                        cmd = new SqlCommand(sqlScript, con);
                        cmd.ExecuteNonQuery();

                        // Step 5: Create a new user object and set its properties
                        cmd = new SqlCommand("INSERT INTO Appuser(Name, PhoneNumber, Cnic, Email, Address,Specialization,Password, Gender) VALUES (@Name, @PhoneNumber, @Cnic, @Email, @Address,@Specialization, @Password, @Gender)", con);
                        cmd.Parameters.AddWithValue("@Name", parameters.Name);
                        cmd.Parameters.AddWithValue("@PhoneNumber", parameters.PhoneNumber);
                        cmd.Parameters.AddWithValue("@CNIC", parameters.CNIC);
                        cmd.Parameters.AddWithValue("@Email", parameters.Email);
                        cmd.Parameters.AddWithValue("@Address", parameters.Address);
                        cmd.Parameters.AddWithValue("@Specialization", parameters.Specialization);
                        cmd.Parameters.AddWithValue("@Password", parameters.Password);
                        cmd.Parameters.AddWithValue("@Gender", parameters.Gender);

                        cmd.ExecuteNonQuery();

                        // Update the database connection string in your ASP.NET application

                        Log($"User created successfully for {dbName} at {DateTimeOffset.UtcNow}");
                        return Request.CreateResponse(HttpStatusCode.OK, "User created successfully.");
                    }
                }
            }
            catch (Exception ex)
            {
                Log($"An error occurred while signing up the user at {DateTimeOffset.UtcNow}: {ex}");
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "An error occurred while signing up the user", ex);
            }
        }

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
                        using (var command = new SqlCommand("SELECT Email, Password,Cnic FROM Appuser WHERE Email = @Email AND Password = @Password", userDb))
                        {
                            command.Parameters.AddWithValue("@Email", email);
                            command.Parameters.AddWithValue("@Password", password);
                            using (var reader = command.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    var user1Db = userDatabase;
                                    // Authentication succeeded, return a successful response
                                    return Request.CreateResponse(HttpStatusCode.OK, new { userDb = user1Db });
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

        [HttpGet]
        public HttpResponseMessage DocDash(string doctordb)
        {
            string connectionString = $@"Data Source=DESKTOP-CEUJHK7;Initial Catalog={doctordb};User ID=sa;Password=123;";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT Name,PhoneNumber,Address ,Specialization FROM Appuser ";
                    SqlCommand command = new SqlCommand(query, connection);

                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        string name = reader.GetString(0);
                        string Specialization = reader.GetString(3);
                        string Address = reader.GetString(2);
                        string phone = reader.GetString(1);

                        var doctorInfo = new { Name = name, Address = Address, Specialization = Specialization, Phone = phone };
                        return Request.CreateResponse(HttpStatusCode.OK, doctorInfo);
                    }
                    else
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No doctor found.");
                    }
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        public HttpResponseMessage GetHistory(string doctordb, int cnic)

        {
            string connectionString = $@"Data Source=DESKTOP-CEUJHK7;Initial Catalog={doctordb};User ID=sa;Password=123;";
            try
            {
                List<HistoryItem> history = new List<HistoryItem>();

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = @"SELECT  patientMedicine.Medicine, Prescription.Routine, Prescription.Days
                                 FROM Prescription
                                          INNER JOIN patientMedicine ON Prescription.Mid = patientMedicine.Mid
                                 WHERE Prescription.Aid = @cnic";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@cnic", cnic);

                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {

                        string medicine = reader.GetString(0);
                        string routine = reader.GetString(1);
                        string days = reader.GetString(2);

                        HistoryItem item = new HistoryItem { Medicine = medicine, Routine = routine, Days = days };
                        history.Add(item);
                    }
                }

                return Request.CreateResponse(HttpStatusCode.OK, history);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        private class HistoryItem
        {

            public string Medicine { get; set; }
            public string Routine { get; set; }
            public string Days { get; set; }
        }



        [HttpGet]
        public HttpResponseMessage GetPatient(string doctordb, Int64 cnic)

        {
            string connectionString = $@"Data Source=DESKTOP-CEUJHK7;Initial Catalog={doctordb};User ID=sa;Password=123;";
            try
            {
                List<Patientdata> history = new List<Patientdata>();

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = @"SELECT appointment.Aid ,appointment.Date, appointment.Time, patient.PatientName, patient.PatientAddress, patient.PhoneNumber
                                 FROM patient
                                 INNER JOIN appointment ON patient.Cnic = appointment.Cnic
                                 WHERE patient.Cnic = @cnic";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@cnic", cnic);

                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {

                        int Aid = reader.GetInt32(0);
                        DateTime date = reader.GetDateTime(1);
                        TimeSpan time = reader.GetTimeSpan(2);



                        string PatientName = reader.GetString(3);
                        string PatientAddress = reader.GetString(4);
                        Int64 PhoneNumber = reader.GetInt64(5);

                        Patientdata patient = new Patientdata { Aid = Aid, Date = date, Time = time, PatientName = PatientName, PatientAddress = PatientAddress, PhoneNumber = PhoneNumber };
                        history.Add(patient);
                    }
                }

                return Request.CreateResponse(HttpStatusCode.OK, history);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        private class Patientdata
        {
            public int Aid { get; set; }
            public DateTime Date { get; set; }
            public TimeSpan Time { get; set; }
            public string PatientName { get; set; }
            public string PatientAddress { get; set; }
            public Int64 PhoneNumber { get; set; }
        }




        [HttpGet]


        public HttpResponseMessage GetPendingAppointments(string doctordb)
        {
            try
            {
                // Get the current date and time
                DateTime now = DateTime.Now.Date;

                // Define a SQL query to retrieve all pending appointments from the current date and time onwards
                string query = "SELECT p.PatientName, p.Cnic, p.PatientAddress, a.Aid, a.Date, a.Time " +
                               "FROM patient p " +
                               "INNER JOIN appointment a ON p.Cnic = a.Cnic " +
                               "LEFT JOIN prescription pr ON a.Aid = pr.Aid " +
                               "WHERE a.Date = @date AND pr.Aid IS NULL " +
                               "ORDER BY a.Date";

                // Create a SQL connection and command object
                using (SqlConnection connection = new SqlConnection($@"Data Source=DESKTOP-CEUJHK7;Initial Catalog={doctordb};User ID=sa;Password=123;"))
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    // Add parameters to the command object
                    command.Parameters.AddWithValue("@date", now.Date);
                    command.Parameters.AddWithValue("@time", now.TimeOfDay);

                    // Open the connection
                    connection.Open();

                    // Execute the query and retrieve the results
                    List<object[]> results = new List<object[]>();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // Extract the data from the reader
                            string name = reader.GetString(0);
                            string cnic = reader.GetInt64(1).ToString();
                            string address = reader.GetString(2);
                            int aid = reader.GetInt32(3);
                            DateTime date = reader.GetDateTime(4);
                            TimeSpan time = reader.GetTimeSpan(5);

                            // Add the data to the results list
                            results.Add(new object[] { name, cnic, address, aid, date, time });
                        }
                    }

                    // Close the connection
                    connection.Close();

                    // Create a JSON response message with the results
                    var response = Request.CreateResponse(HttpStatusCode.OK, results);
                    response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                    return response;
                }
            }
            catch (Exception ex)
            {
                // If an error occurs, create an error response message
                var response = Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
                return response;
            }
        }

        [HttpGet]
        public HttpResponseMessage GetPendingAppointmentsV(string doctordb)
        {
            try
            {
                // Get the current date and time
                DateTime now = DateTime.Now.Date;

                // Define a SQL query to retrieve all pending appointments from the current date and time onwards
                string query = "SELECT p.PatientName, p.Cnic, p.PatientAddress, a.Aid, a.Date, a.Time " +
                               "FROM patient p " +
                               "INNER JOIN appointment a ON p.Cnic = a.Cnic " +
                               "LEFT JOIN Vital v ON a.Aid = v.Aid " +
                               "LEFT JOIN DiseasesH d ON a.Aid = d.Aid " +
                               "WHERE a.Date = @date AND v.Aid IS NULL AND d.Aid IS NULL " +
                               "ORDER BY a.Date";

                // Create a SQL connection and command object
                using (SqlConnection connection = new SqlConnection($@"Data Source=DESKTOP-CEUJHK7;Initial Catalog={doctordb};User ID=sa;Password=123;"))
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    // Add parameters to the command object
                    command.Parameters.AddWithValue("@date", now.Date);
                    command.Parameters.AddWithValue("@time", now.TimeOfDay);

                    // Open the connection
                    connection.Open();

                    // Execute the query and retrieve the results
                    List<object[]> results = new List<object[]>();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // Extract the data from the reader
                            string name = reader.GetString(0);
                            string cnic = reader.GetInt64(1).ToString();
                            string address = reader.GetString(2);
                            int aid = reader.GetInt32(3);
                            DateTime date = reader.GetDateTime(4);
                            TimeSpan time = reader.GetTimeSpan(5);

                            // Add the data to the results list
                            results.Add(new object[] { name, cnic, address, aid, date, time });
                        }
                    }

                    // Close the connection
                    connection.Close();

                    // Create a JSON response message with the results
                    var response = Request.CreateResponse(HttpStatusCode.OK, results);
                    response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                    return response;
                }
            }
            catch (Exception ex)
            {
                // If an error occurs, create an error response message
                var response = Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
                return response;
            }
        }

        [HttpGet]
        public HttpResponseMessage SearchPatients(string doctordb, string searchStr)
        {
            try
            {
                // Create a connection to the database
                string connectionString = $@"Data Source=DESKTOP-CEUJHK7;Initial Catalog={doctordb};User ID=sa;Password=123;";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Create a command to query the database for patients that match the search string
                    SqlCommand command = new SqlCommand("SELECT  Cnic,PatientName,PatientAddress FROM patient WHERE PatientName LIKE @SearchStr", connection);
                    command.Parameters.AddWithValue("@SearchStr", searchStr + "%");

                    // Execute the command and read the results
                    SqlDataReader reader = command.ExecuteReader();
                    List<object> matchingPatients = new List<object>();
                    while (reader.Read())
                    {
                        var patient = new
                        {
                            Cnic = reader.GetValue(0).ToString(),
                            PatientName = reader.GetString(1),
                            PatientAddress = reader.GetString(2)
                        };
                        matchingPatients.Add(patient);
                    }

                    var response = Request.CreateResponse(HttpStatusCode.OK, matchingPatients);
                    return response;
                }
            }
            catch (Exception ex)
            {
                var response = Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
                return response;
            }
        }

        [HttpGet]
        public HttpResponseMessage GetPatientDetails(string doctordb, string Cnic)
        {
            try
            {
                // Create a connection to the database
                string connectionString = $@"Data Source=DESKTOP-CEUJHK7;Initial Catalog={doctordb};User ID=sa;Password=123;";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Create a command to query the database for the patient's details
                    SqlCommand command = new SqlCommand("SELECT Name, CNIC, Address FROM Patients WHERE Cnic = @Cnic", connection);
                    command.Parameters.AddWithValue("@Cnic", Cnic);

                    // Execute the command and read the results
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        var patient = new
                        {
                            Name = reader.GetString(0),
                            CNIC = reader.GetString(1),
                            Address = reader.GetString(2)
                        };

                        // Create a response message containing the patient's details
                        var response = Request.CreateResponse(HttpStatusCode.OK, patient);
                        return response;
                    }
                    else
                    {
                        // If no matching patient is found, return a not found response message
                        var response = Request.CreateResponse(HttpStatusCode.NotFound, "Patient not found.");
                        return response;
                    }
                }
            }
            catch (Exception ex)
            {
                var response = Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
                return response;
            }
        }


        public class AppointmentInput
        {
            public string PatientName { get; set; }
            public string PatientAddress { get; set; }
            public Int64 PPhoneNumber { get; set; }
            public Int64 Cnic { get; set; }

            public DateTime Date { get; set; }
            public TimeSpan Time { get; set; }
        }


        [HttpPost]
        public HttpResponseMessage CreateAppointment(string doctordb, AppointmentInput input)
        {
            try
            {
                if (input == null)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Input is null");
                }

                // Create a connection to the database
                string connectionString = $@"Data Source=DESKTOP-CEUJHK7;Initial Catalog={doctordb};User ID=sa;Password=123;";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Check if patient exists
                    SqlCommand checkPatientCommand = new SqlCommand("SELECT Cnic, PatientName, PatientAddress FROM patient WHERE Cnic = @Cnic", connection);
                    checkPatientCommand.Parameters.AddWithValue("@Cnic", input.Cnic);
                    SqlDataReader reader = checkPatientCommand.ExecuteReader();

                    string patientName = null;
                    string Patientaddress = null;
                    Int64 cnic = 0;

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            cnic = (Int64)reader["Cnic"];
                            patientName = (string)reader["PatientName"];
                            Patientaddress = (string)reader["PatientAddress"];
                        }
                    }

                    reader.Close();

                    if (cnic == 0)
                    {
                        // Patient not found, create a new patient
                        SqlCommand createPatientCommand = new SqlCommand("INSERT INTO patient (Cnic, PatientName, PatientAddress, PhoneNumber) VALUES (@Cnic, @PatientName, @PatientAddress, @PhoneNumber)", connection);
                        createPatientCommand.Parameters.AddWithValue("@Cnic", input.Cnic);
                        createPatientCommand.Parameters.AddWithValue("@PatientName", input.PatientName);
                        createPatientCommand.Parameters.AddWithValue("@PatientAddress", input.PatientAddress);
                        createPatientCommand.Parameters.AddWithValue("@PhoneNumber", input.PPhoneNumber);
                        createPatientCommand.ExecuteNonQuery();

                        // Set the patient information
                        cnic = input.Cnic;
                        patientName = input.PatientName;
                        Patientaddress = input.PatientAddress;
                    }

                    // Create new appointment
                    SqlCommand createAppointmentCommand = new SqlCommand("INSERT INTO appointment (Cnic, Date, Time) VALUES (@Cnic, @Date, @Time); SELECT SCOPE_IDENTITY();", connection);
                    createAppointmentCommand.Parameters.AddWithValue("@Cnic", cnic);
                    createAppointmentCommand.Parameters.AddWithValue("@Date", input.Date);
                    createAppointmentCommand.Parameters.AddWithValue("@Time", input.Time);
                    int appointmentId = Convert.ToInt32(createAppointmentCommand.ExecuteScalar());

                    var appointment = new
                    {
                        Cnic = cnic,
                        PatientName = patientName,
                        PatientAddress = Patientaddress,
                        Date = input.Date,
                        Time = input.Time
                    };

                    return Request.CreateResponse(HttpStatusCode.Created, appointment);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        public HttpResponseMessage GetPatients()
        {
            try
            {
                var connectionString = @"Data Source=DESKTOP-CEUJHK7;Initial Catalog=master;Persist Security Info=True;User ID=sa;Password=123;";
                var patientData = new List<Dictionary<string, object>>();

                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (var command = new SqlCommand("SELECT name FROM sys.databases WHERE name NOT IN ('master', 'tempdb', 'model', 'msdb')", connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var dbName = reader.GetString(0);

                                using (var patientDb = new SqlConnection($@"Data Source=DESKTOP-CEUJHK7;Initial Catalog={dbName};User ID=sa;Password=123;"))
                                {
                                    patientDb.Open();

                                    if (patientDb.GetSchema("Tables", new string[] { null, null, "Appusers" }).Rows.Count > 0)
                                    {
                                        using (var patientCommand = new SqlCommand("SELECT Cnic, Name, Address, PhoneNumber FROM Appusers", patientDb))
                                        {
                                            using (var patientReader = patientCommand.ExecuteReader())
                                            {
                                                while (patientReader.Read())
                                                {
                                                    var patient = new Dictionary<string, object>();
                                                    patient["Cnic"] = patientReader.GetInt64(0);
                                                    patient["Name"] = patientReader.GetString(1);
                                                    patient["Address"] = patientReader.GetString(2);
                                                    patient["PhoneNumber"] = patientReader.GetString(3);

                                                    patientData.Add(patient);
                                                }
                                            }
                                        }

                                    }
                                }
                            }
                        }
                    }
                }

                // Return a successful response with a list of patient data
                return Request.CreateResponse(HttpStatusCode.OK, patientData);
            }
            catch (Exception ex)
            {
                Log($"An error occurred while fetching the patient data at {DateTime.Now}: {ex}");
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "An error occurred while fetching the patient data", ex);
            }
        }


        [HttpPost]
        public HttpResponseMessage AddDiseases(string doctordb, int appointmentId, string disease)
        {
            try
            {
                if (string.IsNullOrEmpty(disease))
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Disease is null or empty");
                }

                // Create a connection to the database
                string connectionString = $@"Data Source=DESKTOP-CEUJHK7;Initial Catalog={doctordb};User ID=sa;Password=123;";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Check if appointment exists
                    SqlCommand checkAppointmentCommand = new SqlCommand("SELECT Aid FROM appointment WHERE Aid = @Aid", connection);
                    checkAppointmentCommand.Parameters.AddWithValue("@Aid", appointmentId);
                    int aid = (int)checkAppointmentCommand.ExecuteScalar();

                    if (aid == 0)
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest, "Appointment not found");
                    }

                    // Check if the disease already exists for the appointment
                    SqlCommand checkDiseaseCommand = new SqlCommand("SELECT Disease FROM DiseasesH WHERE Aid = @Aid AND Disease = @Disease", connection);
                    checkDiseaseCommand.Parameters.AddWithValue("@Aid", appointmentId);
                    checkDiseaseCommand.Parameters.AddWithValue("@Disease", disease);
                    string existingDisease = checkDiseaseCommand.ExecuteScalar() as string;

                    if (!string.IsNullOrEmpty(existingDisease))
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest, "Disease already exists for this appointment");
                    }

                    // Create new disease for the appointment
                    SqlCommand createDiseaseCommand = new SqlCommand("INSERT INTO DiseasesH (Aid, Disease) VALUES (@Aid, @Disease)", connection);
                    createDiseaseCommand.Parameters.AddWithValue("@Aid", appointmentId);
                    createDiseaseCommand.Parameters.AddWithValue("@Disease", disease);
                    createDiseaseCommand.ExecuteNonQuery();

                    return Request.CreateResponse(HttpStatusCode.Created, "Disease added successfully");
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
        [HttpGet]
        public HttpResponseMessage GetDiseases(string doctordb, int appointmentId)
        {
            try
            {
                // Create a connection to the database
                string connectionString = $@"Data Source=DESKTOP-CEUJHK7;Initial Catalog={doctordb};User ID=sa;Password=123;";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Check if appointment exists
                    SqlCommand checkAppointmentCommand = new SqlCommand("SELECT Aid FROM appointment WHERE Aid = @Aid", connection);
                    checkAppointmentCommand.Parameters.AddWithValue("@Aid", appointmentId);
                    int aid = (int)checkAppointmentCommand.ExecuteScalar();

                    if (aid == 0)
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest, "Appointment not found");
                    }

                    // Retrieve diseases for the appointment
                    SqlCommand getDiseasesCommand = new SqlCommand("SELECT Disease FROM DiseasesH WHERE Aid = @Aid", connection);
                    getDiseasesCommand.Parameters.AddWithValue("@Aid", appointmentId);

                    using (SqlDataReader reader = getDiseasesCommand.ExecuteReader())
                    {
                        List<string> diseases = new List<string>();
                        while (reader.Read())
                        {
                            string disease = reader.GetString(0);
                            diseases.Add(disease);
                        }

                        return Request.CreateResponse(HttpStatusCode.OK, diseases);
                    }
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        //[HttpPost]
        //public HttpResponseMessage AddDiseases(string doctordb, int appointmentId, List<string> diseases)
        //{
        //    try
        //    {
        //        if (diseases == null || !diseases.Any())
        //        {
        //            return Request.CreateResponse(HttpStatusCode.BadRequest, "Diseases list is null or empty");
        //        }

        //        // Create a connection to the database
        //        string connectionString = $@"Data Source=DESKTOP-CEUJHK7;Initial Catalog={doctordb};User ID=sa;Password=123;";
        //        using (SqlConnection connection = new SqlConnection(connectionString))
        //        {
        //            connection.Open();

        //            // Check if appointment exists
        //            SqlCommand checkAppointmentCommand = new SqlCommand("SELECT Aid FROM appointment WHERE Aid = @Aid", connection);
        //            checkAppointmentCommand.Parameters.AddWithValue("@Aid", appointmentId);
        //            int aid = (int)checkAppointmentCommand.ExecuteScalar();

        //            if (aid == 0)
        //            {
        //                return Request.CreateResponse(HttpStatusCode.BadRequest, "Appointment not found");
        //            }

        //            // Create new diseases for the appointment
        //            foreach (var disease in diseases)
        //            {
        //                SqlCommand createDiseaseCommand = new SqlCommand("INSERT INTO DiseasesH (Aid, Disease) VALUES (@Aid, @Disease)", connection);
        //                createDiseaseCommand.Parameters.AddWithValue("@Aid", appointmentId);
        //                createDiseaseCommand.Parameters.AddWithValue("@Disease", disease);
        //                createDiseaseCommand.ExecuteNonQuery();
        //            }

        //            return Request.CreateResponse(HttpStatusCode.Created, "Diseases added successfully");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
        //    }
        //}

        public class VitalsInput
        {
            public int High_BP { get; set; }
            public int Low_BP { get; set; }
            public int SugarLevel { get; set; }
            public int Temperature { get; set; }
        }

        [HttpPost]
        public HttpResponseMessage AddVitals(string doctordb, int? appointmentId, VitalsInput vitals)
        {
            try
            {
                if (appointmentId == null)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid appointmentId");
                }

                // Create a connection to the database
                string connectionString = $@"Data Source=DESKTOP-CEUJHK7;Initial Catalog={doctordb};User ID=sa;Password=123;";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Check if appointment exists
                    SqlCommand checkAppointmentCommand = new SqlCommand("SELECT Aid FROM appointment WHERE Aid = @Aid", connection);
                    checkAppointmentCommand.Parameters.AddWithValue("@Aid", appointmentId);
                    int aid = (int)checkAppointmentCommand.ExecuteScalar();

                    if (aid == 0)
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest, "Appointment not found");
                    }

                    //SqlCommand checkVitalsCommand = new SqlCommand("SELECT Aid FROM Vital WHERE Aid = @Aid", connection);
                    //checkVitalsCommand.Parameters.AddWithValue("@Aid", appointmentId);
                    //int existingVitals = (int)checkVitalsCommand.ExecuteScalar();

                    //if (existingVitals > 0)
                    //{
                    //    return Request.CreateResponse(HttpStatusCode.BadRequest, "Vitals already added for this appointment");
                    //}

                    // Create new vitals for the appointment
                    SqlCommand createVitalsCommand = new SqlCommand("INSERT INTO Vital (Aid, High_BP, Low_BP, Temperature, SugarLevel) VALUES (@Aid, @Hbp, @Lpb, @Sugar, @Temperature)", connection);
                    createVitalsCommand.Parameters.AddWithValue("@Aid", appointmentId);
                    createVitalsCommand.Parameters.AddWithValue("@Hbp", vitals.High_BP);
                    createVitalsCommand.Parameters.AddWithValue("@Lpb", vitals.Low_BP);
                    createVitalsCommand.Parameters.AddWithValue("@Sugar", vitals.Temperature);
                    createVitalsCommand.Parameters.AddWithValue("@Temperature", vitals.SugarLevel);
                    createVitalsCommand.ExecuteNonQuery();

                    return Request.CreateResponse(HttpStatusCode.Created, "Vitals added successfully");
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }



        [HttpGet]
        public HttpResponseMessage GetVitals(string doctordb, int appointmentId)
        {
            try
            {
                // Create a connection to the database
                string connectionString = $@"Data Source=DESKTOP-CEUJHK7;Initial Catalog={doctordb};User ID=sa;Password=123;";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Check if appointment exists
                    SqlCommand checkAppointmentCommand = new SqlCommand("SELECT Aid FROM appointment WHERE Aid = @Aid", connection);
                    checkAppointmentCommand.Parameters.AddWithValue("@Aid", appointmentId);
                    int aid = (int)checkAppointmentCommand.ExecuteScalar();

                    if (aid == 0)
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest, "Appointment not found");
                    }

                    // Get the vitals for the appointment
                    SqlCommand getVitalsCommand = new SqlCommand("SELECT High_BP, Low_BP, Temperature, SugarLevel FROM Vital WHERE Aid = @Aid", connection);
                    getVitalsCommand.Parameters.AddWithValue("@Aid", appointmentId);
                    SqlDataReader reader = getVitalsCommand.ExecuteReader();

                    if (reader.HasRows)
                    {
                        reader.Read();
                        int highBP = reader.GetInt32(0);
                        int lowBP = reader.GetInt32(1);
                        int temperature = reader.GetInt32(2);
                        int sugarLevel = reader.GetInt32(3);

                        VitalsInput vitals = new VitalsInput()
                        {
                            High_BP = highBP,
                            Low_BP = lowBP,
                            Temperature = temperature,
                            SugarLevel = sugarLevel
                        };

                        return Request.CreateResponse(HttpStatusCode.OK, vitals);
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound, "Vitals not found");
                    }
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }


        public class MedicationRequest
        {
            public string DbName { get; set; }
            public int Aid { get; set; }
            public int MedicineId { get; set; }
            public string Routine { get; set; }
            public string Frequency { get; set; }
            public string PatientCnic { get; set; }
            public DateTime AppointmentDate { get; set; }
            public string AppointmentTime { get; set; }
        }

        //[HttpPost]

        //public HttpResponseMessage Prescription(MedicationRequest prescriptionRequest)
        //{
        //    try
        //    {
        //        string connectionString = ConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;
        //        using (SqlConnection connection = new SqlConnection(connectionString))
        //        {
        //            connection.Open();

        //            // Insert prescription details into the prescription table
        //            string insertPrescriptionQuery = "INSERT INTO prescription (Aid, mid, routine, frequency) VALUES (@Aid, @mid, @routine, @frequency)";
        //            using (SqlCommand command = new SqlCommand(insertPrescriptionQuery, connection))
        //            {
        //                command.Parameters.AddWithValue("@Aid", prescriptionRequest.Aid);
        //                command.Parameters.AddWithValue("@mid", prescriptionRequest.MedicineId);
        //                command.Parameters.AddWithValue("@routine", prescriptionRequest.Routine);
        //                command.Parameters.AddWithValue("@frequency", prescriptionRequest.Frequency);
        //                int rowsAffected = command.ExecuteNonQuery();
        //            }

        //            // Get the patient's CNIC from the appointment table
        //            string getPatientCnicQuery = "SELECT cnic FROM appointment WHERE Aid = @Aid";
        //            using (SqlCommand command = new SqlCommand(getPatientCnicQuery, connection))
        //            {
        //                command.Parameters.AddWithValue("@Aid", prescriptionRequest.Aid);
        //                using (SqlDataReader reader = command.ExecuteReader())
        //                {
        //                    if (reader.Read())
        //                    {
        //                        string patientCnic = reader["cnic"].ToString();

        //                        // Insert prescription details into the patient's prescription table
        //                        string insertPatientPrescriptionQuery = "INSERT INTO patient_prescription (pid, mid, routine, frequency) VALUES (@pid, @mid, @routine, @frequency)";
        //                        using (SqlCommand patientCommand = new SqlCommand(insertPatientPrescriptionQuery, connection))
        //                        {
        //                            patientCommand.Parameters.AddWithValue("@pid", patientCnic);
        //                            patientCommand.Parameters.AddWithValue("@mid", prescriptionRequest.MedicineId);
        //                            patientCommand.Parameters.AddWithValue("@routine", prescriptionRequest.Routine);
        //                            patientCommand.Parameters.AddWithValue("@frequency", prescriptionRequest.Frequency);
        //                            int patientRowsAffected = patientCommand.ExecuteNonQuery();
        //                        }

        //                        // Insert prescription details into the pharmacy's prescription table
        //                        string insertPharmacyPrescriptionQuery = "INSERT INTO pharmacy_prescription (pid, mid, routine, frequency) VALUES (@pid, @mid, @routine, @frequency)";
        //                        using (SqlCommand pharmacyCommand = new SqlCommand(insertPharmacyPrescriptionQuery, connection))
        //                        {
        //                            pharmacyCommand.Parameters.AddWithValue("@pid", prescriptionRequest.PharmacyId);
        //                            pharmacyCommand.Parameters.AddWithValue("@mid", prescriptionRequest.MedicineId);
        //                            pharmacyCommand.Parameters.AddWithValue("@routine", prescriptionRequest.Routine);
        //                            pharmacyCommand.Parameters.AddWithValue("@frequency", prescriptionRequest.Frequency);
        //                            int pharmacyRowsAffected = pharmacyCommand.ExecuteNonQuery();
        //                        }
        //                    }
        //                }
        //            }

        //            return Request.CreateResponse(HttpStatusCode.OK, "Prescription saved successfully.");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
        //    }
        //}

        //[HttpPost]
        //public HttpResponseMessage GetAppointment(string doctordb, Appointment appointment)
        //{
        //    try
        //    {
        //        var connectionString = $@"Data Source=DESKTOP-CEUJHK7;Initial Catalog={doctordb};User ID=sa;Password=123;";

        //        using (var connection = new SqlConnection(connectionString))
        //        {
        //            connection.Open();

        //            // Insert patient data into the Patient table
        //            using (var patientCommand = new SqlCommand("INSERT INTO Patient (Cnic, PatientName, PatientAddress, PhoneNumber) VALUES (CAST(@Cnic AS BIGINT), @PatientName, @PatientAddress, @PhoneNumber)", connection))
        //            {
        //                patientCommand.Parameters.AddWithValue("@Cnic", appointment.Cnic.ToString());
        //                patientCommand.Parameters.AddWithValue("@PatientName", appointment.PatientName);
        //                patientCommand.Parameters.AddWithValue("@PatientAddress", appointment.PatientAddress);
        //                patientCommand.Parameters.AddWithValue("@PhoneNumber", appointment.PhoneNumber.ToString());
        //                patientCommand.ExecuteNonQuery();
        //            }

        //            using (var appointmentCommand = new SqlCommand("INSERT INTO Appointment (Cnic, Date, Time) VALUES (CAST(@Cnic AS BIGINT), @Date, @Time)", connection))
        //            {
        //                appointmentCommand.Parameters.AddWithValue("@Cnic", appointment.Cnic.ToString());
        //                appointmentCommand.Parameters.AddWithValue("@Date", appointment.Date);
        //                appointmentCommand.Parameters.AddWithValue("@Time", appointment.Time);
        //                appointmentCommand.ExecuteNonQuery();
        //            }
        //        }

        //        // Return a successful response with the appointment object
        //        return Request.CreateResponse(HttpStatusCode.OK, appointment);
        //    }
        //    catch (Exception ex)
        //    {
        //        Log($"An error occurred while creating the appointment at {DateTime.Now}: {ex}");
        //        return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "An error occurred while creating the appointment", ex);
        //    }
        //}

        //public class Appointment
        //{
        //    public Int64 Cnic { get; set; }
        //    public string PatientName { get; set; }
        //    public string PatientAddress { get; set; }
        //    public Int64 PhoneNumber { get; set; }
        //    public DateTime Date { get; set; }
        //    public TimeSpan Time { get; set; }
        //}


        [HttpGet]
        public HttpResponseMessage GetPatientData(string doctordb, int appointmentId)
        {
            try
            {
                var connectionString = $@"Data Source=DESKTOP-CEUJHK7;Initial Catalog={doctordb};User ID=sa;Password=123;";

                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Retrieve patient data based on appointment ID
                    using (var command = new SqlCommand("SELECT p.PatientName, p.PatientAddress, p.PhoneNumber,p.Cnic FROM patient p JOIN appointment a ON p.Cnic = a.Cnic WHERE a.Aid = @AppointmentId", connection))
                    {
                        command.Parameters.AddWithValue("@AppointmentId", appointmentId);

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Create a new patient object and populate it with the retrieved data
                                var patient = new Patient
                                {
                                    PatientName = reader.GetString(reader.GetOrdinal("PatientName")),
                                    PatientAddress = reader.GetString(reader.GetOrdinal("PatientAddress")),
                                    PhoneNumber = reader.GetInt64(reader.GetOrdinal("PhoneNumber")),
                                    Cnic = reader.GetInt64(reader.GetOrdinal("Cnic"))
                                };

                                // Return a successful response with the patient object
                                return Request.CreateResponse(HttpStatusCode.OK, patient);
                            }
                            else
                            {
                                // If no data was retrieved, return a not found response
                                return Request.CreateErrorResponse(HttpStatusCode.NotFound, $"No patient data was found for appointment ID {appointmentId}");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log($"An error occurred while retrieving patient data at {DateTime.Now}: {ex}");
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "An error occurred while retrieving patient data", ex);
            }
        }

        public class Patient
        {
            public string PatientName { get; set; }
            public string PatientAddress { get; set; }
            public Int64 PhoneNumber { get; set; }
            public Int64 Cnic { get; set; }
        }


        [HttpPost]
        public HttpResponseMessage AddPrescription(string doctordb, Prescription input)
        {
            try
            {
                if (input == null)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Input is null");
                }

                var connectionString = $@"Data Source=DESKTOP-CEUJHK7;Initial Catalog={doctordb};User ID=sa;Password=123;";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Check if the prescription already exists for the given appointment
                    using (var checkPrescriptionCommand = new SqlCommand("SELECT COUNT(*) FROM Prescription WHERE Aid = @Aid", connection))
                    {
                        checkPrescriptionCommand.Parameters.AddWithValue("@Aid", input.Aid);

                        int existingPrescriptions = Convert.ToInt32(checkPrescriptionCommand.ExecuteScalar());

                        if (existingPrescriptions > 0)
                        {
                            return Request.CreateResponse(HttpStatusCode.Conflict, "Prescription already exists for the appointment");
                        }
                    }

                    // Insert prescription data into the PharmacyPrescription table
                    foreach (var medicine in input.Medicines)
                    {
                        // Check if the medicine exists in the patientMedicine table
                        using (var checkMedicineCommand = new SqlCommand("SELECT COUNT(*) FROM patientMedicine WHERE Medicine = @MedicineName", connection))
                        {
                            checkMedicineCommand.Parameters.AddWithValue("@MedicineName", medicine.MedicineName);

                            int existingMedicine = Convert.ToInt32(checkMedicineCommand.ExecuteScalar());

                            // Insert medicine data into the patientMedicine table if it doesn't exist
                            if (existingMedicine == 0)
                            {
                                using (var medicineCommand = new SqlCommand("INSERT INTO patientMedicine (Medicine) VALUES (@MedicineName)", connection))
                                {
                                    medicineCommand.Parameters.AddWithValue("@MedicineName", medicine.MedicineName);

                                    medicineCommand.ExecuteNonQuery();
                                }
                            }
                        }

                        // Get the medicine ID from the patientMedicine table
                        using (var getMedicineIdCommand = new SqlCommand("SELECT Mid FROM patientMedicine WHERE Medicine = @MedicineName", connection))
                        {
                            getMedicineIdCommand.Parameters.AddWithValue("@MedicineName", medicine.MedicineName);

                            int medicineId = Convert.ToInt32(getMedicineIdCommand.ExecuteScalar());

                            // Insert prescription data into the Prescription table
                            using (var prescriptionCommand = new SqlCommand("INSERT INTO Prescription (Aid, Mid, Routine, Days) VALUES (@Aid, @Mid, @Routine, @Days)", connection))
                            {
                                prescriptionCommand.Parameters.AddWithValue("@Aid", input.Aid);
                                prescriptionCommand.Parameters.AddWithValue("@Mid", medicineId);
                                prescriptionCommand.Parameters.AddWithValue("@Routine", medicine.Routine);
                                prescriptionCommand.Parameters.AddWithValue("@Days", medicine.Days);

                                prescriptionCommand.ExecuteNonQuery();
                            }
                        }
                    }
                }

                return Request.CreateResponse(HttpStatusCode.Created, "Prescription added successfully");
            }
            catch (Exception ex)
            {
                Log($"An error occurred while adding the prescription at {DateTime.Now}: {ex}");
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "An error occurred while adding the prescription", ex);
            }
        }

        public class Prescription
        {


            public string Aid { get; set; }

            public List<MedicineInput> Medicines { get; set; }
        }

        public class MedicineInput
        {
            public string MedicineName { get; set; }
            public string Routine { get; set; }
            public int Days { get; set; }
        }



        [HttpGet]
        public HttpResponseMessage GetPharmacy()
        {
            try
            {
                var connectionString = @"Data Source=DESKTOP-CEUJHK7;Initial Catalog=master;Persist Security Info=True;User ID=sa;Password=123;";
                var patientData = new List<Dictionary<string, object>>();

                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (var command = new SqlCommand("SELECT name FROM sys.databases WHERE name NOT IN ('master', 'tempdb', 'model', 'msdb')", connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var dbName = reader.GetString(0);

                                using (var patientDb = new SqlConnection($@"Data Source=DESKTOP-CEUJHK7;Initial Catalog={dbName};User ID=sa;Password=123;"))
                                {
                                    patientDb.Open();

                                    if (patientDb.GetSchema("Tables", new string[] { null, null, "Appusersss" }).Rows.Count > 0)
                                    {
                                        using (var patientCommand = new SqlCommand("SELECT PharamacyName, Licence FROM Appusersss", patientDb))
                                        {
                                            using (var patientReader = patientCommand.ExecuteReader())
                                            {
                                                while (patientReader.Read())
                                                {
                                                    var Pharmacy = new Dictionary<string, object>();
                                                    Pharmacy["PharamacyName"] = patientReader.GetString(0);
                                                    Pharmacy["Licence"] = patientReader.GetString(1);


                                                    patientData.Add(Pharmacy);
                                                }
                                            }
                                        }

                                    }
                                }
                            }
                        }
                    }
                }

                // Return a successful response with a list of patient data
                return Request.CreateResponse(HttpStatusCode.OK, patientData);
            }
            catch (Exception ex)
            {
                Log($"An error occurred while fetching the patient data at {DateTime.Now}: {ex}");
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "An error occurred while fetching the patient data", ex);
            }
        }



        [HttpGet]
        public HttpResponseMessage GetDoctors()
        {
            try
            {
                var connectionString = @"Data Source=DESKTOP-CEUJHK7;Initial Catalog=master;Persist Security Info=True;User ID=sa;Password=123;";
                var patientData = new List<Dictionary<string, object>>();

                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (var command = new SqlCommand("SELECT name FROM sys.databases WHERE name NOT IN ('master', 'tempdb', 'model', 'msdb')", connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var dbName = reader.GetString(0);

                                using (var patientDb = new SqlConnection($@"Data Source=DESKTOP-CEUJHK7;Initial Catalog={dbName};User ID=sa;Password=123;"))
                                {
                                    patientDb.Open();

                                    if (patientDb.GetSchema("Tables", new string[] { null, null, "Appuser" }).Rows.Count > 0)
                                    {
                                        using (var patientCommand = new SqlCommand("SELECT Cnic, Name, Address, PhoneNumber FROM Appuser", patientDb))
                                        {
                                            using (var patientReader = patientCommand.ExecuteReader())
                                            {
                                                while (patientReader.Read())
                                                {
                                                    var patient = new Dictionary<string, object>();
                                                    patient["Cnic"] = patientReader.GetInt64(0);
                                                    patient["Name"] = patientReader.GetString(1);
                                                    patient["Address"] = patientReader.GetString(2);
                                                    patient["PhoneNumber"] = patientReader.GetString(3);

                                                    patientData.Add(patient);
                                                }
                                            }
                                        }

                                    }
                                }
                            }
                        }
                    }
                }

                // Return a successful response with a list of patient data
                return Request.CreateResponse(HttpStatusCode.OK, patientData);
            }
            catch (Exception ex)
            {
                Log($"An error occurred while fetching the patient data at {DateTime.Now}: {ex}");
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "An error occurred while fetching the patient data", ex);
            }
        }
        public class RequestInput
        {
            public string Name { get; set; }
            public string Address { get; set; }
            public string DrName { get; set; }
            public string DrAddress { get; set; }
            public string PName { get; set; }
            public string PAddress { get; set; }
            public string Comment { get; set; }

        }
        [HttpPost]
        public HttpResponseMessage ProcessRequest(RequestInput input)
        {
            try
            {
                // Step 1: Fetch the list of user databases from the master database
                var connectionString = @"Data Source=DESKTOP-CEUJHK7;Initial Catalog=master;Persist Security Info=True;User ID=sa;Password=123;";
                var userDatabases = new List<string>();
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (var command = new SqlCommand("SELECT name FROM sys.databases WHERE name NOT IN ('master', 'tempdb', 'model', 'msdb')", connection))
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
                    var connectionStringForUser = $@"Data Source=DESKTOP-CEUJHK7;Initial Catalog={userDatabase};User ID=sa;Password=123;";
                    using (var connectionForUser = new SqlConnection(connectionStringForUser))
                    {
                        connectionForUser.Open();
                        try
                        {
                            using (var command = new SqlCommand("SELECT Name, Address FROM Appuser WHERE Name = @Name AND Address = @Address", connectionForUser))
                            {
                                command.Parameters.AddWithValue("@Name", input.Name);
                                command.Parameters.AddWithValue("@Address", input.Address);

                                using (var reader = command.ExecuteReader())
                                {
                                    if (reader.Read())
                                    {
                                        // Name and Address matched, add appointment in the current database
                                        using (var createAppointmentCommand = new SqlCommand("INSERT INTO DrRequest (DrName, DrAddress, PName, PAddress, Request, status) VALUES (@DrName, @DrAddress, @PName, @PAddress, @Comment, @Status)", connectionForUser))
                                        {
                                            createAppointmentCommand.Parameters.AddWithValue("@DrName", input.DrName);
                                            createAppointmentCommand.Parameters.AddWithValue("@DrAddress", input.DrAddress);
                                            createAppointmentCommand.Parameters.AddWithValue("@PName", input.PName);
                                            createAppointmentCommand.Parameters.AddWithValue("@PAddress", input.PAddress);
                                            createAppointmentCommand.Parameters.AddWithValue("@Comment", input.Comment);
                                            createAppointmentCommand.Parameters.AddWithValue("@Status", "0");

                                            reader.Close(); // Explicitly close the DataReader object before executing the new command
                                            createAppointmentCommand.ExecuteNonQuery();

                                            // Return a successful response
                                            var request = new
                                            {
                                                DrName = input.DrName,
                                                DrAddress = input.DrAddress,
                                                PName = input.PName,
                                                PAddress = input.PAddress,
                                                Comment = input.Comment
                                            };
                                            return Request.CreateResponse(HttpStatusCode.Created, request);
                                        }
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
                }

                // Step 3: Return a failed response if the user was not authenticated in any database
                return Request.CreateResponse(HttpStatusCode.Unauthorized, "Invalid Name or Address");
            }
            catch (Exception ex)
            {
                // Log and return an error response
                Log($"An error occurred while processing the request at {DateTime.Now}: {ex}");
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "An error occurred while processing the request", ex);
            }
        }
        [HttpGet]
        public HttpResponseMessage ShowRequest(string doctorDb)
        {
            try
            {
                var connectionStringForDoctor = $@"Data Source=DESKTOP-CEUJHK7;Initial Catalog={doctorDb};User ID=sa;Password=123;";
                using (var connectionForDoctor = new SqlConnection(connectionStringForDoctor))
                {
                    connectionForDoctor.Open();
                    using (var command = new SqlCommand("SELECT DRid,DrName, DrAddress, PName, PAddress, Request FROM DrRequest WHERE status= 0", connectionForDoctor))
                    {
                        var requests = new List<object>();
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var request = new
                                {
                                    DRid = reader.GetInt32(0),
                                    DrName = reader.GetString(1),
                                    DrAddress = reader.GetString(2),
                                    PName = reader.GetString(3),
                                    PAddress = reader.GetString(4),
                                    Request = reader.GetString(5)

                                };
                                requests.Add(request);
                            }
                        }

                        return Request.CreateResponse(HttpStatusCode.OK, requests);
                    }
                }
            }
            catch (Exception ex)
            {
                // Log and return an error response
                Log($"An error occurred while retrieving requests from the {doctorDb} database at {DateTime.Now}: {ex}");
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "An error occurred while retrieving requests", ex);
            }
        }

        [HttpPut]
        public HttpResponseMessage UpdateRequestStatus(string doctorDb, int DRid)
        {
            try
            {
                var connectionStringForDoctor = $@"Data Source=DESKTOP-CEUJHK7;Initial Catalog={doctorDb};User ID=sa;Password=123;";
                using (var connectionForDoctor = new SqlConnection(connectionStringForDoctor))
                {
                    connectionForDoctor.Open();
                    using (var command = new SqlCommand("UPDATE DrRequest SET status = 1 WHERE DRid = @DRid", connectionForDoctor))
                    {
                        command.Parameters.AddWithValue("@DRid", DRid);
                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            return Request.CreateResponse(HttpStatusCode.OK, "Request status updated successfully");
                        }
                        else
                        {
                            return Request.CreateErrorResponse(HttpStatusCode.NotFound, $"Request with DRid '{DRid}' not found");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log and return an error response
                Log($"An error occurred while updating the request status in the {doctorDb} database at {DateTime.Now}: {ex}");
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "An error occurred while updating the request status", ex);
            }
        }

        [HttpGet]
        public HttpResponseMessage GetResponseData(string doctordb, string name)
        {
            string connectionString = $@"Data Source=DESKTOP-CEUJHK7;Initial Catalog={doctordb};User ID=sa;Password=123;";
            try
            {
                List<Patientdata> history = new List<Patientdata>();

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = @"SELECT appointment.Aid, appointment.Date, appointment.Time, patient.PatientName, patient.PatientAddress, patient.PhoneNumber
                             FROM patient
                             INNER JOIN appointment ON patient.Cnic = appointment.Cnic
                             WHERE patient.PatientName = @name";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@name", name);

                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        int Aid = reader.GetInt32(0);
                        DateTime date = reader.GetDateTime(1);
                        TimeSpan time = reader.GetTimeSpan(2);
                        string PatientName = reader.GetString(3);
                        string PatientAddress = reader.GetString(4);
                        Int64 PhoneNumber = reader.GetInt64(5);

                        Patientdata patient = new Patientdata
                        {
                            Aid = Aid,
                            Date = date,
                            Time = time,
                            PatientName = PatientName,
                            PatientAddress = PatientAddress,
                            PhoneNumber = PhoneNumber
                        };
                        history.Add(patient);
                    }
                }

                return Request.CreateResponse(HttpStatusCode.OK, history);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        public HttpResponseMessage RxData(string doctordb, int Aid)
        {
            try
            {
                var connectionStringForDoctor = $@"Data Source=DESKTOP-CEUJHK7;Initial Catalog={doctordb};User ID=sa;Password=123;";
                using (var connectionForDoctor = new SqlConnection(connectionStringForDoctor))
                {
                    connectionForDoctor.Open();

                    // Retrieve CComplaint from Ccompalint table
                    var cComplaint = string.Empty;
                    using (var cComplaintCommand = new SqlCommand("SELECT Disease FROM DiseasesH WHERE Aid = @Aid", connectionForDoctor))
                    {
                        cComplaintCommand.Parameters.AddWithValue("@Aid", Aid);
                        using (var cComplaintReader = cComplaintCommand.ExecuteReader())
                        {
                            if (cComplaintReader.Read())
                            {
                                cComplaint = cComplaintReader.GetString(0);
                            }
                        }
                    }

                    // Retrieve prescription data from Priscription and patientMedicine tables
                    var prescriptionData = new List<string>();
                    using (var prescriptionCommand = new SqlCommand("SELECT M.Medicine FROM Prescription P JOIN patientMedicine M ON P.Mid = M.Mid WHERE P.Aid = @Aid", connectionForDoctor))
                    {
                        prescriptionCommand.Parameters.AddWithValue("@Aid", Aid);
                        using (var prescriptionReader = prescriptionCommand.ExecuteReader())
                        {
                            while (prescriptionReader.Read())
                            {
                                var medicine = prescriptionReader.GetString(0);
                                prescriptionData.Add(medicine);
                            }
                        }
                    }

                    // Retrieve vitals data from Vitals table
                    var vitalsData = new
                    {
                        High_BP = 0,
                        Low_BP = 0,
                        Temperature = 0,
                        SugarLevel = 0
                    };
                    using (var vitalsCommand = new SqlCommand("SELECT High_BP, Low_BP, Temperature, SugarLevel FROM Vital WHERE Aid = @Aid", connectionForDoctor))
                    {
                        vitalsCommand.Parameters.AddWithValue("@Aid", Aid);
                        using (var vitalsReader = vitalsCommand.ExecuteReader())
                        {
                            if (vitalsReader.Read())
                            {
                                vitalsData = new
                                {
                                    High_BP = vitalsReader.GetInt32(0),
                                    Low_BP = vitalsReader.GetInt32(1),
                                    Temperature = vitalsReader.GetInt32(2),
                                    SugarLevel = vitalsReader.GetInt32(3)
                                };
                            }
                        }
                    }

                    var responseData = new
                    {
                        CComplaint = cComplaint,
                        PrescriptionData = prescriptionData,
                        VitalsData = vitalsData
                    };

                    var responseContent = new StringContent(JsonConvert.SerializeObject(responseData), Encoding.UTF8, "application/json");
                    return new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = responseContent
                    };
                }
            }
            catch (Exception ex)
            {
                // Log and return an error response
                Log($"An error occurred while retrieving RxData from the {doctordb} database for Aid: {Aid} at {DateTime.Now}: {ex}");
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "An error occurred while retrieving RxData", ex);
            }
        }

        public class RxDataResponse
        {
            public string Dname { get; set; }
            public string PName { get; set; }
            public string CComplaint { get; set; }
            public string HighBP { get; set; }
            public string LowBP { get; set; }
            public string Temperature { get; set; }
            public string Sugar { get; set; }
            public List<string> Medicines { get; set; }
        }

        [HttpGet]
        public HttpResponseMessage ShowRxData(string doctordb)
        {
            try
            {
                var connectionString = $@"Data Source=DESKTOP-CEUJHK7;Initial Catalog={doctordb};User ID=sa;Password=123;";

                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    var responseData = new List<RxDataResponse>();

                    using (var command = new SqlCommand("SELECT R.Drname, R.PName, R.CComplaint, R.HighBP, R.LowBP, R.Temperature, R.Sugar, Rx.MNme " +
                                                        "FROM DrResponse R " +
                                                        "JOIN ResponseRx Rx ON R.Rid = Rx.Rid ", connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var response = responseData.FirstOrDefault(r =>
                                    r.Dname == reader.GetString(0) &&
                                    r.PName == reader.GetString(1) &&
                                    r.CComplaint == reader.GetString(2) &&
                                    r.HighBP == reader.GetString(3) &&
                                    r.LowBP == reader.GetString(4) &&
                                    r.Temperature == reader.GetString(5) &&
                                    r.Sugar == reader.GetString(6)
                                );

                                if (response == null)
                                {
                                    response = new RxDataResponse
                                    {
                                        Dname = reader.GetString(0),
                                        PName = reader.GetString(1),
                                        CComplaint = reader.GetString(2),
                                        HighBP = reader.GetString(3),
                                        LowBP = reader.GetString(4),
                                        Temperature = reader.GetString(5),
                                        Sugar = reader.GetString(6),
                                        Medicines = new List<string>()
                                    };

                                    responseData.Add(response);
                                }

                                var medicine = reader.GetString(7);
                                response.Medicines.Add(medicine);
                            }
                        }
                    }

                    var responseContent = new StringContent(JsonConvert.SerializeObject(responseData), Encoding.UTF8, "application/json");
                    return new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = responseContent
                    };
                }
            }
            catch (Exception ex)
            {
                // Log and return an error response
                Log($"An error occurred while retrieving ShowRxData at {DateTime.Now}: {ex}");
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "An error occurred while retrieving ShowRxData", ex);
            }
        }

        // Define the request body object
        public class Medicine
        {
            public string MNme { get; set; }
            // Add any additional properties for the medicine
        }

        public class RxDataObject
        {
            public string Name { get; set; }

            public string Dname { get; set; }
            public string PName { get; set; }
            public string CComplaint { get; set; }
            public int HighBP { get; set; }
            public int LowBP { get; set; }
            public int Temperature { get; set; }
            public int Sugar { get; set; }
            public List<Medicine> Medicines { get; set; }
        }

        [HttpPost]
        public HttpResponseMessage SendResponse(RxDataObject rxData)
        {
            try
            {
                var masterConnectionString = @"Data Source=DESKTOP-CEUJHK7;Initial Catalog=master;Persist Security Info=True;User ID=sa;Password=123;";

                // Step 1: Fetch the list of user databases from the master database
                var userDatabases = new List<string>();
                using (var masterConnection = new SqlConnection(masterConnectionString))
                {
                    masterConnection.Open();
                    using (var command = new SqlCommand("SELECT name FROM sys.databases WHERE name NOT IN ('master', 'tempdb', 'model', 'msdb')", masterConnection))
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
                    var connectionString = $@"Data Source=DESKTOP-CEUJHK7;Initial Catalog={userDatabase};User ID=sa;Password=123;";

                    using (var connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        // Check user authentication in the current database
                        using (var authCommand = new SqlCommand("SELECT Name FROM Appuser WHERE Name = @Name ", connection))
                        {
                            authCommand.Parameters.AddWithValue("@Name", rxData.Name);

                            using (var reader = authCommand.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    // Name matched, insert the response in the current database
                                    var Dname = rxData.Dname;
                                    var PName = rxData.PName;
                                    var CComplaint = rxData.CComplaint;
                                    var HighBP = rxData.HighBP;
                                    var LowBP = rxData.LowBP;
                                    var Temperature = rxData.Temperature;
                                    var Sugar = rxData.Sugar;

                                    if (string.IsNullOrEmpty(Dname) || string.IsNullOrEmpty(PName) || string.IsNullOrEmpty(CComplaint) ||
                                        HighBP == 0 || LowBP == 0 || Temperature == 0 || Sugar == 0 || rxData.Medicines == null || rxData.Medicines.Count == 0)
                                    {
                                        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid request body");
                                    }

                                    // Insert data into DrResponse table
                                    using (var insertCommand = new SqlCommand("INSERT INTO DrResponse (Drname, PName, CComplaint, HighBP, LowBP, Temperature, Sugar) " +
                                                                            "VALUES (@Dname, @PName, @CComplaint, @HighBP, @LowBP, @Temperature, @Sugar); " +
                                                                            "SELECT SCOPE_IDENTITY();", connection))
                                    {
                                        insertCommand.Parameters.AddWithValue("@Dname", Dname);
                                        insertCommand.Parameters.AddWithValue("@PName", PName);
                                        insertCommand.Parameters.AddWithValue("@CComplaint", CComplaint);
                                        insertCommand.Parameters.AddWithValue("@HighBP", HighBP);
                                        insertCommand.Parameters.AddWithValue("@LowBP", LowBP);
                                        insertCommand.Parameters.AddWithValue("@Temperature", Temperature);
                                        insertCommand.Parameters.AddWithValue("@Sugar", Sugar);
                                        reader.Close();
                                        var rid = insertCommand.ExecuteScalar();

                                        // Close the reader before executing the new command
                                        // reader.Close();

                                        // Insert data into ResponseRx table for each medicine
                                        foreach (var medicine in rxData.Medicines)
                                        {
                                            using (var responseCommand = new SqlCommand("INSERT INTO ResponseRx (Rid, MNme) " +
                                                                                        "VALUES (@Rid, @MNme)", connection))
                                            {
                                                responseCommand.Parameters.AddWithValue("@Rid", rid);
                                                responseCommand.Parameters.AddWithValue("@MNme", medicine.MNme);

                                                responseCommand.ExecuteNonQuery();
                                            }
                                        }

                                        return new HttpResponseMessage(HttpStatusCode.OK);
                                    }
                                }
                            }
                        }
                    }
                }

                // Step 3: Return a failed response if the user was not authenticated in any database
                return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Invalid Name or Address");
            }
            catch (Exception ex)
            {
                // Log and return an error
                Log($"An error occurred while sending response at {DateTime.Now}: {ex}");
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "An error occurred while sending response", ex);
            }
        }


        [HttpGet]
        public HttpResponseMessage ShowReceived(string doctorDb)
        {
            try
            {
                var connectionStringForDoctor = $@"Data Source=DESKTOP-CEUJHK7;Initial Catalog={doctorDb};User ID=sa;Password=123;";
                using (var connectionForDoctor = new SqlConnection(connectionStringForDoctor))
                {
                    connectionForDoctor.Open();
                    using (var command = new SqlCommand("SELECT Rid,DrName, PName,CComplaint FROM DrResponse ", connectionForDoctor))
                    {
                        var requests = new List<object>();
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var request = new
                                {
                                    Rid = reader.GetInt32(0),
                                    DrName = reader.GetString(1),
                                    PName = reader.GetString(2),
                                    CComplaint = reader.GetString(3),

                                };
                                requests.Add(request);
                            }
                        }

                        return Request.CreateResponse(HttpStatusCode.OK, requests);
                    }
                }
            }
            catch (Exception ex)
            {
                // Log and return an error response
                Log($"An error occurred while retrieving requests from the {doctorDb} database at {DateTime.Now}: {ex}");
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "An error occurred while retrieving requests", ex);
            }
        }





        [HttpGet]
        public HttpResponseMessage ShowRecievedData(string doctordb, int Rid)
        {
            try
            {
                var connectionString = $@"Data Source=DESKTOP-CEUJHK7;Initial Catalog={doctordb};User ID=sa;Password=123;";

                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    var responseData = new List<RxDatanew>();

                    using (var command = new SqlCommand("SELECT R.CComplaint, R.HighBP, R.LowBP, R.Temperature, R.Sugar, Rx.MNme " +
                                                        "FROM DrResponse R " +
                                                        "JOIN ResponseRx Rx ON R.Rid = @Rid", connection))
                    {
                        command.Parameters.AddWithValue("@Rid", Rid);
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var response = responseData.FirstOrDefault(r =>
                                    r.CComplaint == reader.GetString(0) &&
                                    r.HighBP == reader.GetString(1) &&
                                    r.LowBP == reader.GetString(2) &&
                                    r.Temperature == reader.GetString(3) &&
                                    r.Sugar == reader.GetString(4)
                                );

                                if (response == null)
                                {
                                    response = new RxDatanew
                                    {
                                        CComplaint = reader.GetString(0),
                                        HighBP = reader.GetString(1),
                                        LowBP = reader.GetString(2),
                                        Temperature = reader.GetString(3),
                                        Sugar = reader.GetString(4),
                                        Medicines = new List<Medicin>()
                                    };

                                    responseData.Add(response);
                                }

                                var medicine = new Medicin
                                {
                                    MNme = reader.GetString(5)
                                };
                                response.Medicines.Add(medicine);
                            }
                        }
                    }

                    var responseContent = new StringContent(JsonConvert.SerializeObject(responseData), Encoding.UTF8, "application/json");
                    return new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = responseContent
                    };
                }
            }
            catch (Exception ex)
            {
                // Log and return an error response
                Log($"An error occurred while retrieving ShowRxData at {DateTime.Now}: {ex}");
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "An error occurred while retrieving ShowRxData", ex);
            }
        }

        public class RxDatanew
        {
            public string CComplaint { get; set; }
            public string HighBP { get; set; }
            public string LowBP { get; set; }
            public string Temperature { get; set; }
            public string Sugar { get; set; }
            public List<Medicin> Medicines { get; set; }
        }

        public class Medicin
        {
            public string MNme { get; set; }
        }

    }
}

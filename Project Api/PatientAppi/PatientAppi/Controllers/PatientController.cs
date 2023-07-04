using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace PatientAppi.Controllers
{
    public class PatientController : ApiController
    {
        string connectionString = @"Data Source=DESKTOP-CEUJHK7;Initial Catalog=master;Persist Security Info=True;User ID=sa;Password=123;";
        string newdb;
        public class SignupRequest
        {
            public string Name { get; set; }
            public string PhoneNumber { get; set; }
            public int Cnic { get; set; }
            public string Email { get; set; }
            public string Address { get; set; }
            public string Password { get; set; }
            public string Gender { get; set; }
        }

        [HttpPost]
        public HttpResponseMessage Signup(SignupRequest signupRequest)
        {
            try
            {
                // Step 1: Create a new user account in your ASP.NET application
                // ...

                // Step 2: Check if the user database already exists
                string dbName = signupRequest.Name + "Patient_" + DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
                string userDbConnectionString = @"Data Source=DESKTOP-CEUJHK7;Initial Catalog=master;Persist Security Info=True;User ID=sa;Password=123;";

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
                    {
                        // Step 3: Create a new database based on your template database
                        cmd = new SqlCommand($"CREATE DATABASE {dbName}", conn);
                        cmd.ExecuteNonQuery();
                        conn.Close();
                    }
                    string DbConnectionString = $@"Data Source=DESKTOP-CEUJHK7;Initial Catalog={dbName};Persist Security Info=True;User ID=sa;Password=123;";
                    using (var con = new SqlConnection(DbConnectionString))
                    {
                        con.Open();

                        // Step 4: Use ADO.NET to execute the SQL script to create the schema of the new database based on the template database
                        var sqlScript = System.IO.File.ReadAllText(@"D:\Semester 7th\Rx for Pakisatn With EHR\DatabaseBackup\PatientDb.sql");
                        cmd = new SqlCommand(sqlScript, con);
                        cmd.ExecuteNonQuery();

                        // Step 5: Create a new user object and set its properties
                        cmd = new SqlCommand("INSERT INTO Appusers(Name, PhoneNumber, Cnic, Email, Address,Password, Gender) VALUES (@Name, @PhoneNumber, @Cnic, @Email, @Address, @Password, @Gender)", con);
                        cmd.Parameters.AddWithValue("@Name", signupRequest.Name);
                        cmd.Parameters.AddWithValue("@PhoneNumber", signupRequest.PhoneNumber);
                        cmd.Parameters.AddWithValue("@Cnic", signupRequest.Cnic);
                        cmd.Parameters.AddWithValue("@Email", signupRequest.Email);
                        cmd.Parameters.AddWithValue("@Address", signupRequest.Address);
                        cmd.Parameters.AddWithValue("@Password", signupRequest.Password);
                        cmd.Parameters.AddWithValue("@Gender", signupRequest.Gender);

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
                    var userDb1 = new SqlConnection(newdb);
                    userDb1.Open();
                    try
                    {
                        using (var command = new SqlCommand("SELECT Email, Password,Cnic FROM Appusers WHERE Email = @Email AND Password = @Password", userDb1))
                        {
                            command.Parameters.AddWithValue("@Email", email);
                            command.Parameters.AddWithValue("@Password", password);
                            using (var reader = command.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    var userDb11 = userDatabase;
                                    // Authentication succeeded, return a successful response
                                    return Request.CreateResponse(HttpStatusCode.OK, new { userDb = userDb11 });
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
        public HttpResponseMessage GetHistory(string PatientDB)
{
    try
    {
        var today = DateTime.Today;
        var appointments = new List<object>();

        using (SqlConnection connection = new SqlConnection($@"Data Source=DESKTOP-CEUJHK7;Initial Catalog={PatientDB};User ID=sa;Password=123;"))
        {
            string query = "SELECT D_id, DoctorName, PhoneNumber, Date, Time " +
               "FROM Appointment " +
               "WHERE Date < @Today " +
               "ORDER BY Date DESC";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Today", DateTime.Today.ToString("yyyy-MM-dd"));


                    connection.Open();

            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                            var appointment = new
                            {
                                D_id = reader.GetInt32(0),
                                DoctorName = reader.GetString(1),
                                PhoneNumber = reader.GetString(2),
                                Date = reader.GetDateTime(3),
                                Time = reader.GetTimeSpan(4).ToString(@"hh\:mm\:ss")
                            };

                            appointments.Add(appointment);
                }
            }
        }

        var response = Request.CreateResponse(HttpStatusCode.OK, appointments);
        return response;
    }
    catch (Exception ex)
    {
        var errorResponse = Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Error occurred while fetching history.");
        return errorResponse;
    }
}

        [HttpGet]
        public HttpResponseMessage GetAppointment(string PatientDB)
        {
            try
            {
                var today = DateTime.Today;
                var appointments = new List<object>();

                using (SqlConnection connection = new SqlConnection($@"Data Source=DESKTOP-CEUJHK7;Initial Catalog={PatientDB};User ID=sa;Password=123;"))
                {
                    string query = "SELECT D_id, DoctorName, PhoneNumber, Date, Time " +
                       "FROM Appointment " +
                       "WHERE Date >= @Today " +
                       "ORDER BY Date DESC";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Today", DateTime.Today.ToString("yyyy-MM-dd"));


                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var appointment = new
                            {
                                D_id = reader.GetInt32(0),
                                DoctorName = reader.GetString(1),
                                PhoneNumber = reader.GetString(2),
                                Date = reader.GetDateTime(3),
                                Time = reader.GetTimeSpan(4).ToString(@"hh\:mm\:ss")
                            };

                            appointments.Add(appointment);
                        }
                    }
                }

                var response = Request.CreateResponse(HttpStatusCode.OK, appointments);
                return response;
            }
            catch (Exception ex)
            {
                var errorResponse = Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Error occurred while fetching history.");
                return errorResponse;
            }
        }

        [HttpGet]
        public HttpResponseMessage Getmedicine(string PatientDB, int did)
        {
            string connectionString = $@"Data Source=DESKTOP-CEUJHK7;Initial Catalog={PatientDB};User ID=sa;Password=123;";
            try
            {
                List<HistoryItem> history = new List<HistoryItem>();
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = @"
                SELECT MedicineName.MedicineName, Prescription.Routine, Prescription.Days 
                FROM Appointment 
                INNER JOIN Prescription ON Appointment.D_id = Prescription.D_id 
                INNER JOIN MedicineName ON Prescription.M_id = MedicineName.M_id 
                WHERE Appointment.D_id = @did
            ";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@did", did);
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        string medicine = reader.GetString(0);
                        string routine = reader.GetString(1);
                        string days = reader.GetString(2);
                        HistoryItem item = new HistoryItem
                        {
                            Medicine = medicine,
                            Routine = routine,
                            Days = days
                        };
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

        public class AppointmentInput
        {

            public Int64 Cnic { get; set; }
            public string DoctorName { get; set; }
            public string PhoneNumber { get; set; }
            public DateTime Date { get; set; }
            public TimeSpan Time { get; set; }
        }

        [HttpPost]
        public HttpResponseMessage NewAppointment(AppointmentInput input)
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
                    var connectionStringForUser = $@"Data Source=DESKTOP-CEUJHK7;Initial Catalog={userDatabase};User ID=sa;Password=123;";
                    using (var connectionForUser = new SqlConnection(connectionStringForUser))
                    {
                        connectionForUser.Open();
                        try
                        {
                            using (var command = new SqlCommand("SELECT Cnic FROM Appusers WHERE Cnic = @Cnic", connectionForUser))
                            {
                                command.Parameters.AddWithValue("@Cnic", input.Cnic);
                                using (var reader = command.ExecuteReader())
                                {
                                    if (reader.Read())
                                    {
                                        // CNIC matched, add appointment in the current database
                                        using (var createAppointmentCommand = new SqlCommand("INSERT INTO Appointment ( DoctorName, PhoneNumber, Date, Time) VALUES (@DoctorName, @PhoneNumber, @Date, @Time)", connectionForUser))
                                        {
                                            createAppointmentCommand.Parameters.AddWithValue("@DoctorName", input.DoctorName);
                                            createAppointmentCommand.Parameters.AddWithValue("@PhoneNumber", input.PhoneNumber);
                                            createAppointmentCommand.Parameters.AddWithValue("@Date", input.Date);
                                            createAppointmentCommand.Parameters.AddWithValue("@Time", input.Time);
                                            reader.Close(); // Explicitly close the DataReader object before executing the new command
                                            createAppointmentCommand.ExecuteNonQuery();

                                            // Return a successful response
                                            var appointment = new
                                            {
                                                DoctorName = input.DoctorName,
                                                PhoneNumber = input.PhoneNumber,
                                                Date = input.Date,
                                                Time = input.Time
                                            };
                                            return Request.CreateResponse(HttpStatusCode.Created, appointment);
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

                // Step 4: Return a failed response if the user was not authenticated in any database
                return Request.CreateResponse(HttpStatusCode.Unauthorized, "Invalid CNIC");
            }
            catch (Exception ex)
            {
                // Log and return an error response
                Log($"An error occurred while creating the appointment at {DateTime.Now}: {ex}");
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "An error occurred while creating the appointment", ex);
            }
        }

        [HttpPost]
        public HttpResponseMessage AddPrescription(PrescriptionInput input)
        {
            try
            {
                if (input == null)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Input is null");
                }

                // Step 1: Fetch the list of user databases from the master database
                var connectionString1 = @"Data Source=DESKTOP-CEUJHK7;Initial Catalog=master;Persist Security Info=True;User ID=sa;Password=123;";
                var userDatabases = new List<string>();
                using (var connection = new SqlConnection(connectionString1))
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

                // Step 2: Iterate over each user database and try to find the patient by CNIC
                string patientDb = null;
                foreach (var userDatabase in userDatabases)
                {
                    var newdb = $@"Data Source=DESKTOP-CEUJHK7;Initial Catalog={userDatabase};User ID=sa;Password=123;";
                    using (var userDbConnection = new SqlConnection(newdb))
                    {
                        userDbConnection.Open();
                        try
                        {
                            using (var command = new SqlCommand("SELECT Cnic FROM Appusers WHERE Cnic = @Cnic", userDbConnection))
                            {
                                command.Parameters.AddWithValue("@Cnic", input.Cnic);
                                using (var reader = command.ExecuteReader())
                                {
                                    if (reader.Read())
                                    {
                                        patientDb = userDatabase;
                                        break;
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

                if (patientDb == null)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Patient not found");
                }

                var connectionString = $@"Data Source=DESKTOP-CEUJHK7;Initial Catalog={patientDb};User ID=sa;Password=123;";
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Check if the appointment exists
                    using (var checkAppointmentCommand = new SqlCommand("SELECT D_id FROM Appointment WHERE DoctorName = @DrName AND Date = @Date", connection))
                    {
                        checkAppointmentCommand.Parameters.AddWithValue("@DrName", input.DrName);
                        checkAppointmentCommand.Parameters.AddWithValue("@Date", input.Date);

                        // Declare a variable to store the Aid
                        int Aid = 0;

                        using (var reader = checkAppointmentCommand.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Get the Aid value
                                Aid = reader.GetInt32(0);
                            }
                        }

                        if (Aid == 0)
                        {
                            return Request.CreateResponse(HttpStatusCode.NotFound, "Appointment not found");
                        }

                        // Store the appointmentId variable for later use

                        // ...

                        // Check if the prescription already exists for the given appointment

                        // ...

                        // Insert prescription data into the PharmacyPrescription table
                        foreach (var medicine in input.Medicines)
                        {
                            // Check if the medicine exists in the MedicineName table
                            using (var checkMedicineCommand = new SqlCommand("SELECT COUNT(*) FROM MedicineName WHERE MedicineName = @MedicineName", connection))
                            {
                                checkMedicineCommand.Parameters.AddWithValue("@MedicineName", medicine.MedicineName);

                                int existingMedicine = Convert.ToInt32(checkMedicineCommand.ExecuteScalar());

                                // Insert medicine data into the MedicineName table if it doesn't exist
                                if (existingMedicine == 0)
                                {
                                    using (var medicineCommand = new SqlCommand("INSERT INTO MedicineName (MedicineName) VALUES (@MedicineName)", connection))
                                    {
                                        medicineCommand.Parameters.AddWithValue("@MedicineName", medicine.MedicineName);

                                        medicineCommand.ExecuteNonQuery();
                                    }
                                }
                            }

                            // Get the medicine ID from the MedicineName table
                            using (var getMedicineIdCommand = new SqlCommand("SELECT M_id FROM MedicineName WHERE MedicineName = @MedicineName", connection))
                            {
                                getMedicineIdCommand.Parameters.AddWithValue("@MedicineName", medicine.MedicineName);

                                int medicineId = Convert.ToInt32(getMedicineIdCommand.ExecuteScalar());

                                // Insert prescription data into the PharmacyPrescription table
                                using (var prescriptionCommand = new SqlCommand("INSERT INTO Prescription (M_id, D_id, Routine, Days) VALUES (@Mid, @Aid, @Routine, @Days)", connection))
                                {
                                    prescriptionCommand.Parameters.AddWithValue("@Aid", Aid);
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
            }
            catch (Exception ex)
            {
                Log($"An error occurred while adding the prescription at {DateTime.Now}: {ex}");
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "An error occurred while adding the prescription", ex);
            }
        }

        public class PrescriptionInput
        {

            public DateTime Date { get; set; }
         
            public string DrName { get; set; }
          //  public string Mid { get; set; }
            public Int64 Cnic { get; set; }
           
            public List<MedicineInput> Medicines { get; set; }
        }

        public class MedicineInput
        {
            public string MedicineName { get; set; }
            public string Routine { get; set; }
            public int Days { get; set; }
        }

    }
}

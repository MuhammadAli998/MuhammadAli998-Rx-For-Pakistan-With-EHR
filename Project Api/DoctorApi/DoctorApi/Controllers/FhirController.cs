using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DoctorApi.Controllers
{
    public class FhirController : ApiController
    {
        public class Appointment
        {
            public string PatientName { get; set; }
            public string PatientAddress { get; set; }
            public Int64 PPhoneNumber { get; set; }
            public Int64 Cnic { get; set; }
            public Int64 PhoneNumber { get; set; }
            public string DoctorName { get; set; }
            public DateTime Date { get; set; }
            public TimeSpan Time { get; set; }
        }


        [HttpPost]

        public HttpResponseMessage Posttoanotherapi(Appointment appointment, string doctordb)
        {
            try
            {
                // Serialize the input object into a JSON string
                string json = JsonConvert.SerializeObject(appointment);

                // Set the URL and create a new request for CreateAppointment API
                string createAppointmentUrl = $"http://192.168.43.35/DoctorApi/api/Doctor/CreateAppointment?doctordb={doctordb}";
                var createAppointmentRequest = WebRequest.Create(createAppointmentUrl);
                createAppointmentRequest.Method = "POST";
                createAppointmentRequest.ContentType = "application/json";

                // Write the JSON string to the request body
                using (var streamWriter = new StreamWriter(createAppointmentRequest.GetRequestStream()))
                {
                    streamWriter.Write(json);
                    streamWriter.Flush();
                    streamWriter.Close();
                }

                // Get the response from CreateAppointment API and read the response stream
                var createAppointmentResponse = createAppointmentRequest.GetResponse();
                var createAppointmentResponseStream = createAppointmentResponse.GetResponseStream();
                var createAppointmentResponseStreamReader = new StreamReader(createAppointmentResponseStream);
                var createAppointmentResponseString = createAppointmentResponseStreamReader.ReadToEnd();
                createAppointmentResponseStreamReader.Close();
                createAppointmentResponseStream.Close();

                // Set the URL and create a new request for newAppointment API
                string newAppointmentUrl = "http://192.168.43.35/PatientAppi/api/Patient/NewAppointment";
                var newAppointmentRequest = WebRequest.Create(newAppointmentUrl);
                newAppointmentRequest.Method = "POST";
                newAppointmentRequest.ContentType = "application/json";

                // Write the JSON string to the request body
                using (var streamWriter = new StreamWriter(newAppointmentRequest.GetRequestStream()))
                {
                    streamWriter.Write(json);
                    streamWriter.Flush();
                    streamWriter.Close();
                }

                // Get the response from newAppointment API and read the response stream
                try
                {
                    using (var newAppointmentResponse = newAppointmentRequest.GetResponse())
                    {
                        using (var newAppointmentResponseStream = newAppointmentResponse.GetResponseStream())
                        {
                            using (var newAppointmentResponseStreamReader = new StreamReader(newAppointmentResponseStream))
                            {
                                var newAppointmentResponseString = newAppointmentResponseStreamReader.ReadToEnd();
                                return Request.CreateResponse(HttpStatusCode.OK, newAppointmentResponseString);
                            }
                        }
                    }
                }
                catch (WebException ex)
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }


        public class RX
        {
            public string Aid { get; set; }

            public string DrName { get; set; }
            public string PharmacyName { get; set; }
            public string License { get; set; }
            public Int64 Cnic { get; set; }
            public string PatientName { get; set; }
            public string PPhoneNumber { get; set; }
            public string PhoneNumber { get; set; }
            public DateTime Date { get; set; }
            public TimeSpan Time { get; set; }

            public List<MedicineInput> Medicines { get; set; }
        }

        public class MedicineInput
        {

            public string MedicineName { get; set; }
            public string Routine { get; set; }
            public int Days { get; set; }
        }

        [HttpPost]

        public HttpResponseMessage SendRx(RX appointment, string doctordb)
        {
            try
            {
                // Serialize the input object into a JSON string
                string json = JsonConvert.SerializeObject(appointment);

                // Set the URL and create a new request for CreateAppointment API

                string createRXUrl = $"http://192.168.43.35/DoctorApi/api/Doctor/AddPrescription?doctordb={doctordb}";
                var createRXRequest = WebRequest.Create(createRXUrl);
                createRXRequest.Method = "POST";
                createRXRequest.ContentType = "application/json";

                // Write the JSON string to the request body
                using (var streamWriter = new StreamWriter(createRXRequest.GetRequestStream()))
                {
                    streamWriter.Write(json);
                    streamWriter.Flush();
                    streamWriter.Close();
                }

                // Get the response from CreateAppointment API and read the response stream
                var createRXResponse = createRXRequest.GetResponse();
                var createRXResponseStream = createRXResponse.GetResponseStream();
                var createRXResponseStreamReader = new StreamReader(createRXResponseStream);
                var createRXResponseString = createRXResponseStreamReader.ReadToEnd();
                createRXResponseStreamReader.Close();
                createRXResponseStream.Close();


                //string createRXUrl2 = $"http://192.168.43.35/PatientAppi/api/Patient/AddPrescription";
                //var createRXRequest2 = WebRequest.Create(createRXUrl2);
                //createRXRequest2.Method = "POST";
                //createRXRequest2.ContentType = "application/json";

                //// Write the JSON string to the request body
                //using (var streamWriter = new StreamWriter(createRXRequest2.GetRequestStream()))
                //{
                //    streamWriter.Write(json);
                //    streamWriter.Flush();
                //    streamWriter.Close();
                //}

                //// Get the response from CreateAppointment API and read the response stream
                //var createRxResponse2 = createRXRequest2.GetResponse();
                //var createRxResponseStream2 = createRxResponse2.GetResponseStream();
                //var createRXResponseStreamReader2 = new StreamReader(createRxResponseStream2);
                //var createRXResponseString2 = createRXResponseStreamReader2.ReadToEnd();
                //createRXResponseStreamReader2.Close();
                //createRxResponseStream2.Close();

                // Set the URL and create a new request for newAppointment API
                string newRXUrl3 = "http://192.168.43.35/PharmacyApi/api/Pharmacy/AddPrescription";
                var newRXRequest3 = WebRequest.Create(newRXUrl3);
                newRXRequest3.Method = "POST";
                newRXRequest3.ContentType = "application/json";

                // Write the JSON string to the request body
                using (var streamWriter = new StreamWriter(newRXRequest3.GetRequestStream()))
                {
                    streamWriter.Write(json);
                    streamWriter.Flush();
                    streamWriter.Close();
                }

                // Get the response from newAppointment API and read the response stream
                try
                {
                    using (var newRXResponse3 = newRXRequest3.GetResponse())
                    {
                        using (var newRXResponseStream3 = newRXResponse3.GetResponseStream())
                        {
                            using (var newRXReResponseStreamReader3 = new StreamReader(newRXResponseStream3))
                            {
                                var newRXResponseStreamString3 = newRXReResponseStreamReader3.ReadToEnd();
                                return Request.CreateResponse(HttpStatusCode.OK, newRXResponseStreamString3);
                            }
                        }
                    }
                }
                catch (WebException ex)
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }




    }
}

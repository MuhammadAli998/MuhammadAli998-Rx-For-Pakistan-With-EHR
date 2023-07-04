import React, {useState,useEffect} from 'react';
import {
  View,
  Text,
  Image,
  ScrollView,
  TouchableOpacity,
  TextInput,
  StyleSheet,
  Platform,
} from 'react-native';


//import DatePicker from 'react-native-date-picker';
import { Alert } from 'react-native';
import { SelectList } from 'react-native-dropdown-select-list'
import { useRoute } from '@react-navigation/native';
import DateTimePicker from '@react-native-community/datetimepicker';
import DateTimePickerModal from 'react-native-modal-datetime-picker';


//import DatePicker from 'react-native-date-picker';




const Apointment = ({ navigation }) => {

  const route = useRoute();
  const { doctordb } = route.params;
 // const [user,setuser]=useState("");
  const [patients, setPatients] = useState([]);
  const [selectedPatient, setSelectedPatient] = useState(null);
  const [Address, setAddress] = useState("");
  const [doctors, setDoctors] = useState([]);
  const [selectedDoctor, setSelectedDoctor] = useState(null);
  const [DAddress, setDAddress] = useState("");
  //const [PhoneNumber, setPhoneNumber] = useState("");
  const [DoctorName, setDoctorName] = useState('');
  const [DPhoneNumber, setDPhoneNumber] = useState('');
  //const [Date, setDate] = useState("");
  //const [Time, setTime] = useState("");

  
  const [text, setText] = useState('');

  const handleTextChange = (inputText) => {
    setText(inputText);
  };
 
  
  useEffect(() => {
    fetch(`http://${ip}/DoctorApi/api/Doctor/GetPatients`)
      .then((response) => response.json())
      .then((data) => {
        const patients = data.map((patient) => ({
          label: `${patient.Name} - ${patient.Cnic}`,
          value: patient.Name,
          cnic: patient.Cnic,
          address: patient.Address,
          phoneNumber: patient.PhoneNumber,
        }));
        setPatients(patients);
        setSelectedPatient(patients[0]?.value);
       
        setAddress(patients[0]?.address);
       
      })
      .catch((error) => console.log(error));
  }, []);

  const handlePatientSelection = (patientValue) => {
    const selectedPatientData = patients.find((patient) => patient.value === patientValue);
    setSelectedPatient(selectedPatientData?.value);
   
    setAddress(selectedPatientData?.address);
    
  };
  useEffect(() => {
    fetch(`http://${ip}/DoctorApi/api/Doctor/GetDoctors`)
      .then((response) => response.json())
      .then((data) => {
        const doctorsData = data.map((doctor) => ({
          label: `${doctor.Name} - ${doctor.Cnic}`,
          value: doctor.Name,
          cnic: doctor.Cnic,
          address: doctor.Address,
          phoneNumber: doctor.PhoneNumber,
        }));
        setDoctors(doctorsData);
        setSelectedDoctor(doctorsData[0]?.value);
        setDAddress(doctorsData[0]?.address);
      })
      .catch((error) => console.log(error));
  }, []);

  const handleDoctorSelection = (doctorValue) => {
    const selectedDoctorData = doctors.find((doctor) => doctor.value === doctorValue);
    setSelectedDoctor(selectedDoctorData?.value);
    setDAddress(selectedDoctorData?.address);
  };
  const handleSubmit = () => {
    // Prepare the request input data
    const requestData = {
      Name: selectedDoctor,
      Address: DAddress,
      DrName: DoctorName,
      DrAddress: DPhoneNumber,
      PName: selectedPatient,
      PAddress: Address,
      Comment: text,
    };
  
    // Make the API request
    fetch(`http://${ip}/DoctorApi/api/Doctor/ProcessRequest`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(requestData),
    })
      .then((response) => {
        if (response.ok) {
          // Successful response
          return response.json();
        } else {
          // Handle error response
          throw new Error('Request failed');
        }
      })
      .then((data) => {
        // Handle the response data
        console.log('Send Successfully:',requestData );
        // Show success alert
        alert('Request sent successfully');
      })
      .catch((error) => {
        // Handle any errors
        console.error('An error occurred:',requestData);
        // Show error alert
        
        alert('An error occurred while sending the request');
      });
  };
  
 useEffect(() => {
    const apiUrl = `http://${ip}/DoctorApi/api/Doctor/DocDash?doctordb=${doctordb}`;

    fetch(apiUrl, {
      method: 'GET',
      headers: {
        Accept: 'application/json',
        'Content-Type': 'application/json',
      },
    })
      .then((response) => {
        if (response.ok) {
          return response.json();
        } else {
          throw new Error('Network response was not ok.');
        }
      })
      .then((data) => {
        console.log(data);
        setDoctorName(data.Name);
        setDPhoneNumber(data.Address);
      })
      .catch((error) => {
        console.error('Error:', error);
      });
  },[doctordb]);

  

  
  return (
    <ScrollView style={styles.container}>
      {/*Flex 1 screen ka laya  */}
      <View style={styles.flex1}>
        {/*Doctor  image  */}
        <Image style={styles.imagess} source={require('../image/Appointment.png')} />
      </View>
      
      {/*touchable button for login */}
      
        
      
      <View>
      <SelectList
        setSelected={(patient) => handlePatientSelection(patient)}
        data={patients}
        save="value"
        label="Select Patient"
        placeholder="Select Patient"
        boxStyles={styles.input}
      />
        </View>
        <View>
        <SelectList
        setSelected={(doctor) =>handleDoctorSelection(doctor)}
        data={doctors}
        save="value"
        label="Select Doctor"
        placeholder="Select Doctor"
        boxStyles={styles.input}
      />
        </View>
        <View>
        <TextInput
        multiline
        numberOfLines={4}
        value={text}
        onChangeText={handleTextChange}
        placeholder="Comment"
        style={styles.textInput}
      />
        </View>
     
      
      <View   style={styles.loginb}>
        <TouchableOpacity
          style={styles.button1}
          onPress={handleSubmit}
        >
          <Text style={styles.Text2}>Save</Text>
          
        </TouchableOpacity>
      </View>
    </ScrollView>
  );
};
const styles = StyleSheet.create({
  container: {
    backgroundColor: '#A4E5EE',
  },
  flex1: {
    padding: 30,
    alignItems: 'center',
    backgroundColor: '#398AA4',
  },
  flex2: {
    padding: 30,
    alignItems: 'center',
    backgroundColor: '#8cdbe6',
  },
  imagess: {
    height: 220,
    width: 220,
    alignItems: 'center',
  },
  Textheader: {
    paddingTop: 12,
    fontSize: 25,
    color: '#000000',
    textAlign: 'center',
  },
  Text2: {
    padding: 10,
    fontSize: 25,
    color: '#000000',
    textAlign: 'left',
    fontWeight: 'bold',
  },
  Text3: {
    padding: 1,
    fontSize: 15,
    color: 'blue',
    textAlign: 'right',

    textDecorationLine: 'underline',
    textAlign: 'center',
  },
  input: {
    height: 55,
    margin: 15,
    borderWidth: 2,
    borderColor:'#000000', 
    width: "90%",
      borderRadius: 25, 
      padding: 10,
      
  },
  checkboxWrapper: {
    flexDirection: 'row',
    alignItems: 'center',
    paddingVertical: 9,
    color: '#000000',
    fontWeight: 'bold',
  },

  button: {
    // flex: 1,
    // flexDirection: 'row',
    // justifyContent: 'space-between',
    // alignItems: 'flex-start',
    backgroundColor: '#17725C',
    padding: 20,
    borderRadius: 10,
    margin:20,
    //marginTop: 30,
   // fontSize: 18,
    width: '52%',
  },
  button1: {
    // flex: 1,
    // flexDirection: 'row',
    // justifyContent: 'space-between',
    // alignItems: 'flex-start',
    backgroundColor: '#efff',
    padding: 15,
    borderRadius: 25,
    margin: 1,
    marginTop: 30,
   // fontSize: 18,
    width: '33%',
  },
  link1: {
    fontSize: 15,
    color: '#ff0000',
    textAlign: 'right',
    margin: 8,
  },
  Bview:{
   // borderWidth: 1,
    flexDirection: 'row',
    justifyContent:'space-evenly',
    // flexWrap: 'nowrap',
    alignItems:'stretch',
    margin:12,
    

  },
  loginb:{
    // borderWidth: 1,
     flexDirection: 'row',
     justifyContent:'flex-end',
     // flexWrap: 'nowrap',
     //alignItems:'right',
     margin:12,
 
   },
   textInput: {
    borderWidth: 3,
    borderColor: 'black',
    borderRadius: 45,
    padding: 10,
    fontSize: 12,
    height: 120,
    width: "90%",
    margin: 15,
  },
});

export default Apointment;

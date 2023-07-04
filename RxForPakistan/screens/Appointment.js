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
  const [user,setuser]=useState("");
  const [patients, setPatients] = useState([]);
  const [selectedPatient, setSelectedPatient] = useState(null);
  const [Cnic, setCnic] = useState("");
  //const [cnic, setcnic] = useState("");
  const [Address, setAddress] = useState("");
  const [PhoneNumber, setPhoneNumber] = useState("");
  const [DoctorName, setDoctorName] = useState('');
  const [DPhoneNumber, setDPhoneNumber] = useState('');
  //const [Date, setDate] = useState("");
  //const [Time, setTime] = useState("");

  

  const [date, setDate] = useState(new Date());
  const [showDatePicker, setShowDatePicker] = useState(false);


  const [isTimePickerVisible, setTimePickerVisibility] = useState(false);
  const [selectedTime, setSelectedTime] = useState('');

  const showTimePicker = () => {
    setTimePickerVisibility(true);
  };

  const hideTimePicker = () => {
    setTimePickerVisibility(false);
  };

  const handleTimeConfirm = (time) => {
    hideTimePicker();
    const formattedTime = time.toLocaleTimeString('en-US', { hour12: false, hour: '2-digit', minute: '2-digit', second: '2-digit' });
    setSelectedTime(formattedTime);
  };

  

  const handleDateChange = (event, selectedDate) => {
    const currentDate = selectedDate || date;
    setShowDatePicker(Platform.OS === 'ios');
    setDate(currentDate);
  };

  const showPicker = () => {
    setShowDatePicker(true);
  };
  
  useEffect(() => {
    fetch(`http://${ip}/DoctorApi/api/Doctor/GetPatients/`)
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
        setCnic(patients[0]?.cnic);
        setAddress(patients[0]?.address);
        setPhoneNumber(patients[0]?.phoneNumber);
      })
      .catch((error) => console.log(error));
  }, []);

  const handlePatientSelection = (patientValue) => {
    const selectedPatientData = patients.find((patient) => patient.value === patientValue);
    setSelectedPatient(selectedPatientData?.value);
    setCnic(selectedPatientData?.cnic);
    setAddress(selectedPatientData?.address);
    setPhoneNumber(selectedPatientData?.phoneNumber);
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
        setDPhoneNumber(data.Phone);
      })
      .catch((error) => {
        console.error('Error:', error);
      });
  },[doctordb]);

  

  const handleSubmit = () => {
    // Extracting only the date part from the selected date
    const selectedDate = date.toISOString().split('T')[0];
    // Set the time to "00:00:00" if no time is selected
    const selectedTime = selectedTime ? selectedTime : '00:00:00';
  
    const timeComponents = selectedTime.split(':');
    const timeSpan = new Date();
    timeSpan.setHours(parseInt(timeComponents[0]));
    timeSpan.setMinutes(parseInt(timeComponents[1]));
    timeSpan.setSeconds(parseInt(timeComponents[2]));
  
    const appointment = {
      PatientName: selectedPatient,
      PatientAddress: Address,
      PhoneNumber: DPhoneNumber,
      PPhoneNumber: PhoneNumber,
      Cnic: Cnic,
      DoctorName: DoctorName,
      Date: selectedDate,
      Time: timeSpan.toISOString(),
    };
    
    fetch(`http://${ip}/DoctorApi/api/Fhir/Posttoanotherapi?doctordb=${doctordb}`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(appointment),
    })
      .then((response) => {
        if (response.ok) {
          console.log(appointment);
          Alert.alert('Appointment Created Successfully');
        } else {
          console.log(appointment);
          response.text().then((errorMessage) => {
            Alert.alert('Error', errorMessage);
          });
        }
      })
      .catch((error) => {
        console.log(appointment);
        Alert.alert('Error', error.message);
      });
  };
  

  return (
    <ScrollView style={styles.container}>
      {/*Flex 1 screen ka laya  */}
      <View style={styles.flex1}>
        {/*Doctor  image  */}
        <Image style={styles.imagess} source={require('../image/Appointment.png')} />
      </View>
      
      {/*touchable button for login */}
      <View >
        
      <View>
      <View>
      <SelectList
        setSelected={(patient) => handlePatientSelection(patient)}
        data={patients}
        save="value"
        label="Select Patient"
        placeholder="Select Patient"
        boxStyles={styles.input}
      />

           <View >
      <TextInput
        style={styles.input}
        placeholder="Select Date"
        onFocus={showPicker}
        
        value={date.toDateString()}
      />
      {showDatePicker && (
        <DateTimePicker
          testID="datePicker"
          value={date}
          mode="date"
          display="default"
          onChange={handleDateChange}
        />
      )}
    </View>
    <TouchableOpacity onPress={showTimePicker}>
        <TextInput
          style={styles.input}
          placeholder="Select Time"
          editable={false}
          value={selectedTime}
        />
      </TouchableOpacity>
      <DateTimePickerModal
        isVisible={isTimePickerVisible}
        mode="time"
        onConfirm={handleTimeConfirm}
        onCancel={hideTimePicker}
      />


      </View>
     
    
      
      </View>
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
    margin: 20,
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
});
export default Apointment;

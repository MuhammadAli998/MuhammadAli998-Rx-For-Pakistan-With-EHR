import React, {useState,useEffect} from 'react';
import {
  View,
  Text,
  Image,
  ScrollView,
  TouchableOpacity,
  TextInput,
  StyleSheet,
} from 'react-native';
import CheckBox from '@react-native-community/checkbox';
import { DataTable } from 'react-native-paper';
import { useRoute } from '@react-navigation/native';
const Appointment = ({navigation}) => {
  const route = useRoute();
 const {PatientDB } = route.params;
  const [historyData, setHistoryData] = useState([]);
  const [appointments, setAppointments] = useState([]);
 
  useEffect(() => {
    fetch( `http://${ip}/PatientAppi/api/Patient/GetHistory?PatientDB=${PatientDB}`)
      .then(response => response.json())
      .then(data => {
        setHistoryData(data);
      })
      .catch(error => console.log(error));
      
    fetch(`http://${ip}/PatientAppi/api/Patient/GetAppointment?PatientDB=${PatientDB}`)
      .then(response => response.json())
      .then(data => {
        setAppointments(data);
      })
      .catch(error => console.log(error));
  }, []);
  const handleLogout = () => {
    // Clear session data
    //clearSessionData();
  
    // Navigate to login screen
    navigation.reset({
      index: 0,
      routes: [{ name: 'Login' }],
    });
  }
  return (
    <ScrollView style={styles.container}>
      <View style={styles.flex1}>
      <Image style={styles.imagess} source={require('../image/Patient.png')} />
        <Text style={styles.Textheader}>Be Healthy Be Happy</Text>
      </View>

      <View>
        <Text style={styles.Textheader}>WelCome!</Text>
        <View  >
          <Text style={styles.Text2}>History</Text>
          <View style={styles.flex2}>
          <DataTable>
            <DataTable.Header style={styles.tableHeader}>
              <DataTable.Title>Sr#</DataTable.Title>
              <DataTable.Title>Date</DataTable.Title>
              <DataTable.Title>Time</DataTable.Title>
              <DataTable.Title>Dr.Name</DataTable.Title>
              <DataTable.Title>Phone#</DataTable.Title>
            </DataTable.Header>
            {historyData.map((historyItem, index) => (
              <TouchableOpacity
                key={index}
                onPress={() => navigation.navigate('PatientMedicine', { doctorId: historyItem.D_id, PatientDB: 'PatientDB1' })}

              >
                <DataTable.Row>
                  <DataTable.Cell>{index + 1}</DataTable.Cell>
                  <DataTable.Cell>
                    {new Date(historyItem.Date).toLocaleDateString()}
                  </DataTable.Cell>
                  <DataTable.Cell>
                    {historyItem.Time ? new Date(`2000-01-01T${historyItem.Time}`).toLocaleTimeString() : '-'}
                  </DataTable.Cell>
                  <DataTable.Cell>{historyItem.DoctorName}</DataTable.Cell>
                  <DataTable.Cell>{historyItem.PhoneNumber}</DataTable.Cell>
                </DataTable.Row>
              </TouchableOpacity>
            ))}
          </DataTable>
          </View>
        </View>
      </View>

      <View>
       
          <Text style={styles.Text2}>Upcoming Appointment</Text>
          <View style={styles.flex2}>
          <DataTable>
            <DataTable.Header style={styles.tableHeader}>
              <DataTable.Title>Sr#</DataTable.Title>
              <DataTable.Title>Date</DataTable.Title>
              <DataTable.Title>Time</DataTable.Title>
              <DataTable.Title>Dr.Name</DataTable.Title>
              <DataTable.Title>Phone#</DataTable.Title>
            </DataTable.Header>
            {appointments.map((appointment, index) => (
              <DataTable.Row key={index}>
                <DataTable.Cell>{index + 1}</DataTable.Cell>
                <DataTable.Cell>
                  {new Date(appointment.Date).toLocaleDateString()}
                </DataTable.Cell>
                <DataTable.Cell>
                  {appointment.Time ? new Date(`2000-01-01T${appointment.Time}`).toLocaleTimeString() : '-'}
                </DataTable.Cell>
                <DataTable.Cell>{appointment.DoctorName}</DataTable.Cell>
                <DataTable.Cell>{appointment.PhoneNumber}</DataTable.Cell>
              </DataTable.Row>
            ))}
          </DataTable>
        </View>
      </View>

      <View   style={styles.loginb}>
        <TouchableOpacity
          style={styles.button1}
          onPress={handleLogout}
        >
           <Image style={styles.image1}  source={require('../image/logout.png')} />
          
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
    width: 320,
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
    color: '#000',
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
    margin: 8,
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
    borderRadius: 120,
    margin: 10,
    marginTop: 10,
   // fontSize: 18,
    width: '30%',
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
   image1: {
    height: 70,
    width: 70,
    alignItems: 'center',
  }
});
export default Appointment;

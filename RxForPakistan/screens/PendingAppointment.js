import React, { useState, useEffect } from 'react';
import {
  View,
  Text,
  Image,
  ScrollView,
  TouchableOpacity,
  TextInput,
  StyleSheet,
  TouchableWithoutFeedback,
  Alert,
} from 'react-native';
import CheckBox from '@react-native-community/checkbox';
import { DataTable } from 'react-native-paper';
import { useRoute } from '@react-navigation/native';



const Appointment = ({ navigation }) => {
  const route = useRoute();
  const { doctordb } = route.params;
  
  const [appointments, setAppointments] = useState([]);
  
  useEffect(() => {
    fetch(`http://${ip}/DoctorApi/api/Doctor/GetPendingAppointments?doctordb=${doctordb}`
    )
      .then(response => response.json())
      .then(data => {
        setAppointments(data);
        console.log(data);
      })
      .catch(error => console.log(error));
    }, []);

  return (
    <ScrollView style={styles.container}>
      <View style={styles.flex1}>
      <Image style={styles.imagess} source={require('../image/Appointment.png')} />
      </View>

     
      <View>
        <View>
          <Text style={styles.Text2}>Upcoming Appointment</Text>
          <DataTable>
            <DataTable.Header style={styles.tableHeader}>
              <DataTable.Title>Sr#</DataTable.Title>
              <DataTable.Title>Patient Name</DataTable.Title>
              <DataTable.Title>Cnic</DataTable.Title>
              <DataTable.Title>Address</DataTable.Title>
              <DataTable.Title>Date</DataTable.Title>
              <DataTable.Title>Time</DataTable.Title>
            </DataTable.Header>
            {appointments.map((appointment, index) => (
              <TouchableOpacity
              key={index}
              onPress={() =>navigation.navigate('Rx', { Aid: appointment[3],doctordb:doctordb,Date: appointment[4],Time: appointment[5] })}
                
            >
                <DataTable.Row>
        <DataTable.Cell>{index + 1}</DataTable.Cell>
        <DataTable.Cell>{appointment[0]}</DataTable.Cell>
        <DataTable.Cell>{appointment[1]}</DataTable.Cell>
        <DataTable.Cell>{appointment[2]}</DataTable.Cell>
        <DataTable.Cell>
          {new Date(appointment[4]).toLocaleDateString()}
        </DataTable.Cell>
        <DataTable.Cell>
          {appointment[5]
            ? new Date(`2000-01-01T${appointment[5]}`).toLocaleTimeString()
            : '-'}
        </DataTable.Cell>
      </DataTable.Row>
              </TouchableOpacity>
            ))}
          </DataTable>
        </View>
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
    backgroundColor: '#17725C',
    padding: 15,
    borderRadius: 15,
    margin: 10,
    marginTop: 60,
   // fontSize: 18,
    width: '45%',
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
export default Appointment;

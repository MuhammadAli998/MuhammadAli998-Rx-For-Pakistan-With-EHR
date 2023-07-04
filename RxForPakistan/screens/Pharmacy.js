import React, { useState, useEffect } from 'react';
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

const Appointment = ({ navigation }) => {
  const route = useRoute();
  const { Pharmacydb } = route.params;
  const [historyData, setHistoryData] = useState([]);
  const [appointments, setAppointments] = useState([]);
  const [name, setName] = useState("");
  const [Oname, setOName] = useState("");

  useEffect(() => {
    fetch(`http://${ip}/PharmacyApi/api/Pharmacy/GetPrescription?PharmacyDB=${Pharmacydb}`)
      .then(response => response.json())
      .then(data => {
        setHistoryData(data);
        console.log(data);
      })
      .catch(error => console.log(error));

    fetch(`http://${ip}/PharmacyApi/api/Pharmacy/GetoldPrescription?PharmacyDB=${Pharmacydb}`)
      .then(response => response.json())
      .then(data => {
        setAppointments(data);
        console.log(data);
      })
      .catch(error => console.log(error));
  }, []);

  const getbyName = () => {
    const apiUrl = `http://${ip}/PharmacyApi/api/Pharmacy/GetByNamePrescription?PharmacyDB=${Pharmacydb}&name=${name}`;

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
        setHistoryData(data);
      })
      .catch((error) => {
        console.error('Error:', error);
      });
  };
  
  const getbyNameold = () => {
    const apiUrl = `http://${ip}/PharmacyApi/api/Pharmacy/GetOldNamePrescription?PharmacyDB=${Pharmacydb}&name=${Oname}`;

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
        setAppointments(data);
      })
      .catch((error) => {
        console.error('Error:', error);
      });
  };

  return (
    <ScrollView style={styles.container}>
       <View style={styles.flex1}>
        {/*Doctor  image  */}
        <Image style={styles.imagess} source={require('../image/Pharmacy.png')} />
      </View>

      <View>
        <Text style={styles.Textheader}>Welcome!</Text>

        <View style={styles.row}>
          <TextInput
            style={styles.input}
            value={name}
            onChangeText={(text) => setName(text)}
            placeholder="Search by Name"
            keyboardType="Text"
          />
          <TouchableOpacity
            style={styles.rolebutton}
            onPress={getbyName}
          >
            <Text style={styles.Text3}>Search</Text>
          </TouchableOpacity>
        </View>

        <View style={styles.flex2}>
          <Text style={styles.Text2}>New Prescriptions</Text>

          <DataTable>
            <DataTable.Header style={styles.tableHeader}>
              <DataTable.Title>Sr#</DataTable.Title>
              <DataTable.Title>Date</DataTable.Title>
              <DataTable.Title>Time</DataTable.Title>
              <DataTable.Title>Patient Name</DataTable.Title>
              <DataTable.Title>Phone#</DataTable.Title>
              <DataTable.Title>Address#</DataTable.Title>
            </DataTable.Header>
                  {historyData.map((historyItem, index) => (
              <TouchableOpacity
                key={index}
                onPress={() => navigation.navigate('PharmacyPrescription', { Cnic: historyItem.Pid, Pharmacydb: Pharmacydb,highbp:historyItem.HighBP,lowbp:historyItem.LowBP,temperature:historyItem.Temperature,sugar:historyItem.Sugar,ccomplaint:historyItem.Disease, })}
              >
                <DataTable.Row>
                  <DataTable.Cell>{index + 1}</DataTable.Cell>
                  <DataTable.Cell>
                    {new Date(historyItem.Date).toLocaleDateString()}
                  </DataTable.Cell>
                  <DataTable.Cell>
                    {historyItem.Time ? new Date(`2000-01-01T${historyItem.Time}`).toLocaleTimeString() : '-'}
                  </DataTable.Cell>
                  <DataTable.Cell>{historyItem.PatientName}</DataTable.Cell>
                  <DataTable.Cell>{historyItem.PhoneNumber}</DataTable.Cell>
                  <DataTable.Cell>{historyItem.PatientAddress}</DataTable.Cell>
                </DataTable.Row>
              </TouchableOpacity>
            ))}
          </DataTable>
        </View>
      </View>

      <View>
        <Text style={styles.Text2}>History (Old Prescriptions)</Text>
        <View style={styles.row}>
          <TextInput
            style={styles.input}
            value={Oname}
            onChangeText={(text) => setOName(text)}
            placeholder="Search by Name"
            keyboardType="Text"
          />
          <TouchableOpacity
            style={styles.rolebutton}
            onPress={getbyNameold}
          >
            <Text style={styles.Text3}>Search</Text>
          </TouchableOpacity>
        </View>

        <View style={styles.flex2}>
          <DataTable>
            <DataTable.Header style={styles.tableHeader}>
              <DataTable.Title>Sr#</DataTable.Title>
              <DataTable.Title>Date</DataTable.Title>
              <DataTable.Title>Time</DataTable.Title>
              <DataTable.Title>Patinet Name</DataTable.Title>
              <DataTable.Title>Patinet Address</DataTable.Title>
              <DataTable.Title>Phone#</DataTable.Title>
            </DataTable.Header>
            {appointments.map((appointment, index) => (
              <TouchableOpacity
                key={index}
                onPress={() => navigation.navigate('PharmacyPrescription', { Cnic: appointment.Pid, Pharmacydb: Pharmacydb,highbp:appointment.HighBP,lowbp:appointment.LowBP,temperature:appointment.Temperature,sugar:appointment.Sugar,ccomplaint:appointment.Disease, })}
              >
                <DataTable.Row>
                  <DataTable.Cell>{index + 1}</DataTable.Cell>
                  <DataTable.Cell>
                    {new Date(appointment.Date).toLocaleDateString()}
                  </DataTable.Cell>
                  <DataTable.Cell>
                    {appointment.Time ? new Date(`2000-01-01T${appointment.Time}`).toLocaleTimeString() : '-'}
                  </DataTable.Cell>
                  <DataTable.Cell>{appointment.PatientName}</DataTable.Cell>
                  <DataTable.Cell>{appointment.PatientAddress}</DataTable.Cell>
                  <DataTable.Cell>{appointment.PhoneNumber}</DataTable.Cell>
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
    height:55,
    margin:15,
    borderWidth: 2,
    borderColor:'#000000', 
    width: "60%",
      borderRadius: 40, 
      padding:20,
      
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
  imagess: {
    height: 220,
    width: 220,
    alignItems: 'center',
  },
  loginb:{
    // borderWidth: 1,
     flexDirection: 'row',
     justifyContent:'flex-end',
     // flexWrap: 'nowrap',
     //alignItems:'right',
     margin:12,
 
   },
   rolebutton:{
    alignItems: "center",
backgroundColor: "#efff",
padding:1,
 borderRadius:25, 
 margin: 1,
 fontSize:10,
 width: "30%",
  },

  Text3: {
    padding:6,
    fontSize:20,
    color:'#000000',
    textAlign:"center",
    fontWeight: 'bold',
    borderRadius: 20,
  
},
row:{
  flexDirection:'row',
  alignItems: 'center',
  justifyContent: 'center',
},
});
export default Appointment;

import React,{useState,useEffect} from 'react';
import { View, Text, Image, ScrollView,TouchableOpacity, TextInput,SafeAreaView,StyleSheet} from 'react-native';
import CheckBox from '@react-native-community/checkbox';
import DatePicker from 'react-native-datepicker';
import { SelectList } from 'react-native-dropdown-select-list'
import { DataTable } from 'react-native-paper';
import { useRoute } from '@react-navigation/native';


import RadioForm,{RadioButton, RadioButtonInput, RadioButtonLabel} from 'react-native-simple-radio-button';

const Pharmacy = ({navigation}) => {
  const route = useRoute();
  const [cnic, setcnic] = useState('');
  
  
  const [medicines, setMedicines] = useState([]);
  const { doctordb,name,DrName } = route.params;
  const [historyData, setHistoryData] = useState([]);
  useEffect(() => {
    gethistory();
  }, []);

  const gethistory = ()  => {
    const apiUrl =`http://${ip}/DoctorApi/api/Doctor/GetResponseData?doctordb=${doctordb}&name=${name}`;
    
  
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
       
      ;
      //setData(data);
    })
      .catch((error) => {
        console.error('Error:', error);
      });
  };



    return(
    <ScrollView style={styles.container}>
      <View style={styles.flex1}>
      <Image style={styles.imagess}
           source={require("../image/history.png")}
       />
      </View>
     
      <ScrollView>
      <View style={styles.flex2}>
          <DataTable>
            <DataTable.Header style={styles.tableHeader}>
              <DataTable.Title>Sr#</DataTable.Title>
              <DataTable.Title>Patient Name</DataTable.Title>
              <DataTable.Title>Patient Address#</DataTable.Title>
              <DataTable.Title>Phone Number</DataTable.Title>
              <DataTable.Title>Date</DataTable.Title>
              <DataTable.Title>Time</DataTable.Title>
              
            </DataTable.Header>
            {historyData.map((historyItem, index) => (
              <TouchableOpacity
                key={index}
                onPress={() => navigation.navigate('ResponseRx', { Aid: historyItem.Aid,doctordb: doctordb,DrName:DrName,name:name })}

              >
                <DataTable.Row>
                  <DataTable.Cell>{index + 1}</DataTable.Cell>
                  
                  <DataTable.Cell>{historyItem.PatientName}</DataTable.Cell>
                  <DataTable.Cell>{historyItem.PatientAddress}</DataTable.Cell>
                  <DataTable.Cell>{historyItem.PhoneNumber}</DataTable.Cell>

                  <DataTable.Cell>
                    {new Date(historyItem.Date).toLocaleDateString()}
                  </DataTable.Cell>
                  <DataTable.Cell>
                    {historyItem.Time ? new Date(`2000-01-01T${historyItem.Time}`).toLocaleTimeString() : '-'}
                  </DataTable.Cell>
                </DataTable.Row>
              </TouchableOpacity>
            ))}
          </DataTable>
          </View>
        
    </ScrollView>

                     
    </ScrollView>




    )}
    const styles = StyleSheet.create({
        container: {
         
           
        backgroundColor:'#A4E5EE',
        
        },
        Heading:{
            padding:1,
            fontSize:25,
            color:'#000000',
           // textAlign:"center",
            fontWeight: 'bold',
            margin: 40,
        },
        input: {
            height:55,
            margin:15,
            borderWidth: 2,
            borderColor:'#000000', 
            width: "62%",
              borderRadius: 40, 
              padding:20,
              
          },
          /*radio button */
         text:{
           
            padding:5,
            fontSize:20,
            color:'#000000',
           
            fontWeight: 'bold',
            margin: 1,
              
             
              
          },
          Text2: {
            padding:8,
            fontSize:25,
            color:'#0fffff',
            textAlign:"center",
            fontWeight: 'bold',
            borderRadius: 15,
          
        },
        Text3: {
          padding:8,
          fontSize:20,
          color:'#0000FF',
          textAlign:"right",
          margin:5,
          textDecorationLine: 'underline',
          textAlign:"center",
        
      },
        button:{
          alignItems: "center",
      backgroundColor: "#27727A",
      padding:12,
       borderRadius:35, 
       margin: 50,
       fontSize:20,
       width: "53%",
        },
        flex1: {
          padding:25,
        //  alignItems:"center",
          backgroundColor: '#398AA4'
      },
      imagess: {
        height:200,
        width:230,
        alignItems:"center",
      
    },
    row:{
      flexDirection:'row',
      alignItems: 'center',
      justifyContent: 'center',
    }, rolebutton:{
      alignItems: "center",
  backgroundColor: "#efff",
  padding:2,
   borderRadius:25, 
   margin: 4,
   fontSize:20,
   width: "30%",
    },flex2: {
      padding: 30,
      alignItems: 'center',
      backgroundColor: '#8cdbe6',
    }
          
    })
export default Pharmacy;
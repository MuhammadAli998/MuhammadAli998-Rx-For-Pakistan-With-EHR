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
 
  
  
  
  const { doctordb } = route.params;
 

  const [historyData, setHistoryData] = useState([]);
  const [checkedItems, setCheckedItems] = useState([]);



  
  useEffect(() => {
    // Call the API to fetch the request history data
    fetch(`http://${ip}/DoctorAPi/api/Doctor/ShowRequest?doctorDb=${doctordb}`
    )
      .then(response => response.json())
      .then(data => {
        setHistoryData(data);
      })
      .catch(error => {
        console.log('An error occurred while fetching request history:', error);
      });
  }, []);
  const handleCheck = (doctordb, DRid) => {
    // Call the update request status API here
    fetch(`http://${ip}/DoctorAPi/api/Doctor/UpdateRequestStatus?doctorDb=${doctordb}&DRid=${DRid}`, {
      method: 'PUT',
    })
      .then(response => {
        if (response.ok) {
          // Update the state or perform any necessary actions
          console.log('Request status updated successfully');
        } else {
          throw new Error('Failed to update request status');
        }
      })
      .catch(error => {
        console.error(error);
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
              <DataTable.Title>Doctor Name</DataTable.Title>
              <DataTable.Title>Doctor Address#</DataTable.Title>
              <DataTable.Title>Patient Name</DataTable.Title>
              <DataTable.Title>Patient Address</DataTable.Title>
              <DataTable.Title>Message</DataTable.Title>
              <DataTable.Title>Check</DataTable.Title>
            </DataTable.Header>
            {historyData.map((historyItem, index) => (
              <TouchableOpacity
                key={index}
               onPress={() => navigation.navigate('Response', { DrName: historyItem.DrName,name: historyItem.PName,doctordb: doctordb })}

              >
                <DataTable.Row>
                  <DataTable.Cell>{index + 1}</DataTable.Cell>
                  
                  <DataTable.Cell>{historyItem.DrName}</DataTable.Cell>
                  <DataTable.Cell>{historyItem.DrAddress}</DataTable.Cell>
                  <DataTable.Cell>{historyItem.PName}</DataTable.Cell>
                  <DataTable.Cell>{historyItem.PAddress}</DataTable.Cell>
                  <DataTable.Cell>{historyItem.Request}</DataTable.Cell>
                  <DataTable.Cell>
                  <CheckBox
                      value={checkedItems.includes(historyItem.DRid)}
                      onValueChange={() => {
                        const updatedCheckedItems = [...checkedItems];
                        if (updatedCheckedItems.includes(historyItem.DRid)) {
                          // Remove DRid if already checked
                          const index = updatedCheckedItems.indexOf(historyItem.DRid);
                          updatedCheckedItems.splice(index, 1);
                        } else {
                          // Add DRid if not checked
                          updatedCheckedItems.push(historyItem.DRid);
                        }
                        setCheckedItems(updatedCheckedItems);
                        handleCheck(doctordb, historyItem.DRid); // Call the function when checked
                      }}
                    />
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
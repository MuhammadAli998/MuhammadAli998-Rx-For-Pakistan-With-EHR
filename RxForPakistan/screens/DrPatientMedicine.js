import React, { useState, useEffect } from 'react';
import { View, Text, Image, ScrollView, TouchableOpacity, TextInput, SafeAreaView, StyleSheet } from 'react-native';
//import CheckBox from '@react-native-community/checkbox';
//import { SelectList } from 'react-native-dropdown-select-list';
import { DataTable } from 'react-native-paper';
import { useRoute } from '@react-navigation/native';
//import RadioForm, { RadioButton, RadioButtonInput, RadioButtonLabel } from 'react-native-simple-radio-button';

const Pharmacy = ({ navigation }) => {
  const route = useRoute();

  const [medicines, setMedicines] = useState([]);
  const { Aid, doctordb } = route.params;
console.log(Aid);
console.log(doctordb);

  useEffect(() => {
    const fetchData = async () => {
      try {
        const response = await fetch(`http://${ip}/DoctorApi/api/Doctor/GetHistory?doctordb=${doctordb}&cnic=${Aid}`);
        const data = await response.json();
        setMedicines(data);
      } catch (error) {
        console.error(error);
      }
    };

    fetchData();
  }, []);

  return (
    <ScrollView style={styles.container}>
      <View style={styles.flex1}>
        <Image style={styles.imagess} source={require('../image/history.png')} />
      </View>

      <ScrollView>
        {<DataTable>
          <DataTable.Header style={styles.tableHeader}>
            <DataTable.Title>Sr#</DataTable.Title>
            <DataTable.Title>Medicine</DataTable.Title>
            <DataTable.Title>Frequency</DataTable.Title>
            <DataTable.Title>Dosage</DataTable.Title>
          </DataTable.Header>
          {medicines.map((medicine, index) => (
            <DataTable.Row key={index}>
              <DataTable.Cell>{index + 1}</DataTable.Cell>
              <DataTable.Cell>{medicine.Medicine}</DataTable.Cell>
              <DataTable.Cell>{medicine.Routine}</DataTable.Cell>
              <DataTable.Cell>{medicine.Days}</DataTable.Cell>
            </DataTable.Row>
          ))}
        </DataTable>
}
      </ScrollView>

      <View></View>
    </ScrollView>
  );
};

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
    },
          
    })
export default Pharmacy;
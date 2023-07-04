import React,{useState} from 'react';
import { View, Text, Image,Button, ScrollView,TouchableOpacity, TextInput,SafeAreaView,StyleSheet} from 'react-native';
import { Alert } from 'react-native';
//import ip from './screens/config';
//import { Picker } from '@react-native-picker/picker';
import { SelectList } from 'react-native-dropdown-select-list'
import RadioForm,{RadioButton, RadioButtonInput, RadioButtonLabel} from 'react-native-simple-radio-button';

const Signup = ({navigation}) => {
    
    const [Name, setName] = useState('');
 
   
    
    const [selectedMedicine, setSelectedMedicine] = useState('');
    const medicine = [
      {key:'1', value:'Tablete'},
      {key:'2', value:'Injection'},
      {key:'3', value:'Capsule'},
      {key:'4', value:'Drip'},
      {key:'5', value:'Syrup'},
     
  ]

  const addMedicine = async () => {
    try {
      const response = await fetch(`http://${ip}/MedicineApi/api/Medicine/AddMedicine`, {
        method: 'POST',
        headers: {
          Accept: 'application/json',
          'Content-Type': 'application/json'
        },
        body: JSON.stringify({
          Name: Name,
          Type: selectedMedicine
        })
      });
      
      if (response.status === 200) {
        const responseData = await response.json();
        console.log(responseData);
        // show success message to user
        Alert.alert('Successfully Add ', responseData.message);
      } 
      else{
      if (response.status === 400) {
        const responseData = await response.json();
        console.log(responseData);
        // show success message to user
        Alert.alert('Already Added ', responseData.message);
      }
       else {
        // show error message to user
        Alert.alert('Error', 'Failed to add medicine');
      }
    }
    } catch (error) {
      console.error(error);
      // show error message to user
      Alert.alert('Error', 'Failed to add medicine');
    }
  }
  

    
    return(
        <ScrollView style={styles.container}>


<View style={styles.flex1}>
        <Image style={styles.imagess} source={require('../image/medicine.png')} />
      </View>
      <Text style={styles.signup}>Add Medicines</Text>
      <View>
        {/*text box for 1st name*/}
        <TextInput
          style={styles.input}
          placeholder="  Medicne Name"
          keyboardType="Text"
          value={Name} 
          onChangeText={(text) => setName(text)} 
        />
        {/*text box for 2nd name*/}
        <SelectList
          setSelected={(val) => setSelectedMedicine(val)}
          data={medicine}
          save="value"
          label="Type"
          placeholder="Type"
          boxStyles={styles.input}
        />
      </View>
      {/*radio button's view for gender and sign up as*/}
     
      
      <View  style={styles.view} >
     
        <TouchableOpacity
          style={styles.button}
          onPress={addMedicine}
        >
           <Text style={styles.Text2}>Add</Text>
        </TouchableOpacity>
      </View>
      
    </ScrollView>
      );
}
const styles = StyleSheet.create({
    container: {
     
       
    backgroundColor:'#A4E5EE',
    
    },
    signup:{
        padding:10,
        fontSize:25,
        color:'#000000',
        textAlign:"center",
        fontWeight: 'bold',
        margin: 10,
    },
    input: {
        height:55,
        margin:20,
        borderWidth: 2,
        borderColor:'#000000', 
        width: "90%",
          borderRadius: 25, 
          padding:1,
          
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
        padding:10,
        fontSize:20,
        color:'#000000',
        textAlign:"center",
        fontWeight: 'bold'
      
    }, 
    Text4: {
        padding:10,
        fontSize:12,
        color:'#000000',
        textAlign:"center",
        fontWeight: 'bold'
      
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
  padding:15,
   borderRadius:25, 
   margin:60,
   fontSize:20,
   width: "60%",
   
    },
    rolebutton:{
        alignItems: "center",
    backgroundColor: "#efff",
    padding:2,
     borderRadius:25, 
     margin: 4,
     fontSize:20,
     width: "30%",
      },
      flex1: {
        padding: 50,
        alignItems: 'center',
        backgroundColor: '#398AA4',
      },
row:{
    flexDirection:'row',
    alignItems: 'center',
    justifyContent: 'center',
  },
  imagess: {
    height: 240,
    width: 240,
    alignItems: 'center',
  },view:{
    flexDirection: 'row',
     alignItems: 'center',
      justifyContent: 'flex-start',
  }
      
})
export default Signup;
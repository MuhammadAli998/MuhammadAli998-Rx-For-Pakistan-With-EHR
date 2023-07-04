import React,{useState,useEffect} from 'react';
import { View, Text, Image,Button, ScrollView,TouchableOpacity, TextInput,SafeAreaView,StyleSheet} from 'react-native';
import { Alert } from 'react-native';
//import { Picker } from '@react-native-picker/picker';
import { SelectList } from 'react-native-dropdown-select-list'
import RadioForm,{RadioButton, RadioButtonInput, RadioButtonLabel} from 'react-native-simple-radio-button';

const Signup = ({navigation}) => {
    
    const [disease, setdisease] = useState('');
    const [comment, setcomment] = useState('');
    const [medicineList, setMedicineList] = useState([]);

    
    const [selectedMedicine, setSelectedMedicine] = useState('');
    useEffect(() => {
     
      fetch( `http://${ip}/MedicineApi/api/Medicine/GetMedicine`, {
        method: 'GET'
      })
        .then(response => response.json())
        .then(data => setMedicineList(data))
        .catch(error => console.error(error))
    }, [])
    const banMedicine = () => {
      // Create a JSON object with the data to send
      const data = {
        MedicineName: selectedMedicine,
        DiseaseName: disease,
        Comment: comment
      };
     
    
      // Make the HTTP POST request
      fetch( `http://${ip}/MedicineApi/api/Medicine/AddDiseaseContraindication`, {
        method: 'POST',
        headers: {
          Accept: 'application/json',
          'Content-Type': 'application/json'
        },
        body: JSON.stringify(data)
      })
        .then(response => {
          if (response.ok) {
            // Success, show an alert message and navigate back to the previous screen
            Alert.alert('Success', 'Disease contraindication added successfully', [
              {
                text: 'OK',
                
              }
            ]);
          } else {
            // Error, show an alert message with the error text
            response.text().then(errorMessage => {
              Alert.alert('Error', errorMessage);
            });
          }
        })
        .catch(error => {
          // Network error, show an alert message with the error text
          Alert.alert('Error', error.message);
        });
    };
    
    
    
    return(
        <ScrollView style={styles.container}>


<View style={styles.flex1}>
        <Image style={styles.imagess} source={require('../image/banned.png')} />
      </View>
      <Text style={styles.signup}>Add Medicinesc To Disease Contraindications </Text>
      <View>
       
        {/*text box for 2nd name*/}
        <SelectList
          setSelected={(val) => setSelectedMedicine(val)}
          data={medicineList}
          save="value"
          label="Medicine"
          placeholder="Medicine"
          boxStyles={styles.input}
        />
        <TextInput
          style={styles.input}
          placeholder="Disease"
          keyboardType="Text"
          value={disease} 
          onChangeText={(text) => setdisease(text)} 
        />
        <TextInput
          style={styles.input}
          placeholder="Comment"
          keyboardType="Text"
          value={comment} 
          onChangeText={(text) => setcomment(text)} 
        />
         
      </View>
      {/*radio button's view for gender and sign up as*/}
     
      
      <View  style={styles.view} >
     
        <TouchableOpacity
          style={styles.button}
         onPress={banMedicine}
        >
          <Image style={styles.image1}  source={require('../image/Ban.png')} />
           <Text style={styles.Text2}>Ban Medicine</Text>
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
        height:45,
        margin:8,
        borderWidth: 2,
        borderColor:'#000000', 
        width: "90%",
          borderRadius: 35, 
          padding:3,
          
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
  backgroundColor: "#efff",
  padding:2,
   borderRadius:20, 
   margin:14,
   fontSize:20,
   width: "90%",
   
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
    height: 195,
    width: 360,
    alignItems: 'center',
  },view:{
    flexDirection: 'row',
     alignItems: 'center',
      justifyContent: 'flex-start',
  },image1: {
    height: 45,
    width: 45,
    alignItems: 'center',
  }
      
})
export default Signup;
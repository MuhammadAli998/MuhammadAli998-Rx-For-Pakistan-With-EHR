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
//import DatePicker from 'react-native-date-picker';
import { Alert } from 'react-native';
import { SelectList } from 'react-native-dropdown-select-list'
import { useRoute } from '@react-navigation/native';

//import DatePicker from 'react-native-date-picker';




const Appointment = ({ navigation }) => {

  const route = useRoute();
  const { doctordb,Aid } = route.params;
  
  const [HighBP, setHighBP] = useState('');
  const [LowBP, setLowBP] = useState('');
  const [Temperature, setTemperature] = useState("");
  const [Sugar, setSugar] = useState("");

  const addVitals = async () => {
    try {
      const response = await fetch(`http://192.168.43.35/DoctorApi/api/Doctor/AddVitals?doctordb=${doctordb}&appointmentId=${Aid}`, {
        method: 'POST',
        headers: {
          Accept: 'application/json',
          'Content-Type': 'application/json'
        },
        body: JSON.stringify({
            High_BP: HighBP,
            Low_BP: LowBP,
            SugarLevel: Sugar,
            Temperature: Temperature
        })
      });
  
      if (response.ok) {
        const responseData = await response.json();
        console.log(responseData);
        Alert.alert('SuccessFully Add Vitals', responseData.message);
      } else if (response.status===400) {
        const responseData = await response.json();
        console.log(responseData);
        Alert.alert('Already Added', responseData.message);
      } else {
        const responseData = await response.json();
        console.log(responseData);
        Alert.alert('Error', 'Failed to send the data');
      }
    } catch (error) {
      console.error(error);
      Alert.alert('Error', 'Failed to add vitals');
    }
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
      <TextInput
        style={styles.input2}
        value={HighBP} 
          onChangeText={(text) => setHighBP(text)}
        
        placeholder="High Bp(mmhg)"
        keyboardType="Text"
      />
     {/* <Text>Low BP</Text>*/}


      </View>
     
    
      
      </View>
      </View>
      <View   style={styles.loginb}>
        <TouchableOpacity
          style={styles.button1}
         onPress={addVitals}
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
   input2: {
    height:45,
    margin: 10,
    borderWidth: 2,
    borderColor: '#000000',
    width: '70%',
    borderRadius: 35,
    padding: 10,
    paddingTop: 15,
  
  }
});
export default Appointment;

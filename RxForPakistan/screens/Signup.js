import React,{useState} from 'react';
import { View, Text, Image,Button, ScrollView,TouchableOpacity, TextInput,SafeAreaView,StyleSheet} from 'react-native';
import { Alert } from 'react-native';
import RadioForm,{RadioButton, RadioButtonInput, RadioButtonLabel} from 'react-native-simple-radio-button';

const Signup = ({navigation}) => {
    const gender=[
        {label:"Male" , value:0     },
         {label:"Female" , value:1  },
]
    const [Name, setName] = useState('');
 
    const [Cnic, setCnic] = useState('');
    const [Email, setEmail] = useState('');
    const [Address, setAddress] = useState('');
    const [PhoneNumber, setPhoneNumber] = useState('');
    const [Password, setPassword] = useState('');
    const [ConfirmPassword, setConfirmPassword] = useState('');
    const [Gender, setGenderValue] = useState(0);
    const [license, setLicense] = useState('');
    const [specialization, setSpecialization] = useState('');
    const [role, setRole] = useState('patient');

    
    const handlePatientSignup = async () => {
      // Create the request body for patient signup
      let response = await fetch(`http://${ip}/PatientAppi/api/Patient/Signup`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({
          Name: Name,
          PhoneNumber: PhoneNumber,
          Cnic: Cnic,
          Email: Email,
          Address: Address,
          Password: Password,
          Gender: Gender,
        }),
      });
      
      const data = await response.json();
      if (response.status === 200) {
        Alert.alert('Patient signup successful!');
      } else {
        console.log(response);
        Alert.alert('Patient signup failed.');
      }
    };
    
    const handleDoctorSignup = async () => {
      // Create the request body for doctor signup
      let response = await fetch(`http://${ip}/DoctorApi/api/Doctor/Signup`, {
  method: 'POST',
  headers: {
    'Content-Type': 'application/json',
  },
  body: JSON.stringify({
    Name: Name,
    CNIC: Cnic,
    PhoneNumber: PhoneNumber,
    Email: Email,
    Address: Address,
    Specialization: specialization,
    Password: Password,
    Gender: Gender,
  }),
});

const data = await response.json();

if (response.status === 200) {
  Alert.alert('Signup successful!');
} else {
  console.log(response); // log the full response object
  Alert.alert('Signup failed.');
}
    
    }
    

    const handlePharmacySignup = async () => {
      // Create the request body for pharmacy signup
      const requestBody = {
        Name: Name,
        PhoneNumber: PhoneNumber,
        Licence: license,
        Email: Email,
        Address: Address,
        Password: Password,
        Gender: Gender,
      };
      
      try {
        const response = await fetch(`http://${ip}/PharmacyAPi/api/Pharmacy/Signup`, {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json',
          },
          body: JSON.stringify(requestBody),
        });
    
        const responseData = await response.json();
    
        if (response.ok) {
          // Signup successful
          Alert.alert('Pharmacy signup successful');
        } else {
          // Signup failed
          Alert.alert('Pharmacy signup failed', responseData);
        }
      } catch (error) {
        console.error('An error occurred while signing up the pharmacy', error);
      }
    

    
    }
    
    const handleSignup = async () => {
      if (role === 'patient') {
        handlePatientSignup();
      } else if (role === 'doctor') {
        handleDoctorSignup();
      } else if (role === 'pharmacy') {
        handlePharmacySignup();
      }
      /*eNumber('');
  setPassword('');
  setConfirmPassword('');
  setGenderValue(0);
  setLicense('');
  setSpecialization('');*/
    };

    function handleRoleChange(value) {
        setRole(value);
      }
      function renderDoctorFields() {
        if (role === 'doctor') {
          return (
            <>
              <TextInput
                style={styles.input}
                placeholder="Specialization"
                value={specialization}
                onChangeText={setSpecialization}
              />
            </>
          );
        } else if (role === 'pharmacy') {
          return (
            <>
              <TextInput
                style={styles.input}
                placeholder="License Number"
                value={license}
                onChangeText={setLicense}
              />
            </>
          );
        } else {
          return null;
        }
      }

    return(
        <ScrollView style={styles.container}>
      <Text style={styles.signup}>Sign up to create account</Text>
      <View>
        {/*text box for 1st name*/}
        <TextInput
          style={styles.input}
          placeholder="Enter Name/Pharmacy Name"
          keyboardType="Text"
          value={Name} 
          onChangeText={(text) => setName(text)} 
        />
        {/*text box for 2nd name*/}
        <TextInput
          style={styles.input}
          value={Cnic}
          placeholder="Enter CNIC/left empty for Pharmacy"
          keyboardType="Text"
          
          onChangeText={(text) => setCnic(text)}
        />
        <TextInput
          style={styles.input}
          value={Email}
          placeholder="Enter your Email"
          keyboardType="Text"
          onChangeText={(text )=> setEmail(text)}
        />
        <TextInput
          style={styles.input}
          value={PhoneNumber}
          placeholder="Phone Number"
          keyboardType="Text"
          onChangeText={(text )=> setPhoneNumber(text)}
        />

<TextInput
          style={styles.input}
          value={Address}
          placeholder="Address"
          keyboardType="Text"
          onChangeText={(text )=> setAddress(text)}
        />
        {/*text box for password*/}
        <TextInput
          style={styles.input}
          value={Password}
          secureTextEntry={true}
          placeholder="Password"
          keyboardType="Text"
          onChangeText={(text )=> setPassword(text)} 
        />
        {/*text box for confirm password*/}
        <TextInput
          style={styles.input}
          value={ConfirmPassword}
          secureTextEntry={true}
          placeholder=" Confirm Password"
          keyboardType="Text"
          onChangeText={(text )=> setConfirmPassword(text)}
        />
      </View>
      {/*radio button's view for gender and sign up as*/}
      <View>
        <Text style={styles.text}>Gender</Text>
        <RadioForm   
          radio_props={gender} 
          initial={0}
          formHorizontal={true}
          labelHorizontal={true}
          buttonColor={'#2196f3'}
          animation={true}
          buttonSize={15}
          buttonOuterSize={25}
          buttonStyle={{}}
          buttonWrapStyle={{marginLeft: 10}}
          value={Gender}
          onPress={(value) => setGenderValue(value)}
        />
      </View>
      <View>
        <Text style={styles.text}>Select Your Role</Text>
        <View   style={styles.row}>
        <TouchableOpacity   
        style={styles.rolebutton}  
         onPress={() => handleRoleChange('patient')}  >
        <Text style={styles.Text4}>As a Patient</Text> 
          </TouchableOpacity>
          <TouchableOpacity   
        style={styles.rolebutton}  
         onPress={() => handleRoleChange('doctor')}  >
        <Text style={styles.Text4}>As a Doctor</Text> 
          </TouchableOpacity><TouchableOpacity   
        style={styles.rolebutton}  
         onPress={() => handleRoleChange('pharmacy')}  >
        <Text style={styles.Text4}>As a Pharmacist</Text> 
          </TouchableOpacity>
          </View>
      </View>
      <View>
      {renderDoctorFields()}
        <TouchableOpacity
          style={styles.button}
          onPress={handleSignup}
        >
           <Text style={styles.Text2}>Signup</Text>
        </TouchableOpacity>
      </View>
      <View>       
        <TouchableOpacity onPress={() =>navigation.navigate('Login')}>
          <Text style={styles.Text2}>
            Already have an account?
            <Text style={styles.Text3}> Login</Text>
          </Text> 
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
        height:40,
        margin:5,
        borderWidth: 1,
        borderColor:'#000000', 
        width: "95%",
          borderRadius: 30, 
          padding:10,
          
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
   margin: 20,
   fontSize:20,
   width: "87%",
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
row:{
    flexDirection:'row',
    alignItems: 'center',
    justifyContent: 'center',
  }
      
})
export default Signup;
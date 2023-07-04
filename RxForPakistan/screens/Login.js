import React, { useState,useEffect} from 'react';
import {
  View,
  Text,
  Image,
  ScrollView,
  TouchableOpacity,
  TextInput,
  StyleSheet,
} from 'react-native';

import { Alert } from 'react-native';
import CheckBox from '@react-native-community/checkbox';
import axios from 'axios'; // import axios library
import AsyncStorage from '@react-native-async-storage/async-storage';
const Login = ({ navigation }) => {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');

  const [rememberMe, setRememberMe] = useState(false);
  useEffect(() => {
    // Store rememberMe state in local storage
    AsyncStorage.setItem('rememberMe', JSON.stringify(rememberMe));
  }, [rememberMe]);
  const handleLogin = async () => {
    const apis = [
      {
        apiUrl: `http://${ip}/DoctorApi/api/Doctor/Login`,
        param: 'doctordb',
        screen: 'DoctorDash',
      },
      {
        apiUrl: `http://${ip}/PatientAppi/api/Patient/Login`,
        param: 'PatientDB',
        screen: 'Patient',
      },
      {
        apiUrl: `http://${ip}/MedicineApi/api/Medicine/Login`,
       
        param: 'Medicinedb',
        screen: 'DrugInspector',
      },
      {
        apiUrl: `http://${ip}/PharmacyApi/api/Pharmacy/Login`,
        param: 'Pharmacydb',
        screen: 'Pharmacy',
      }
    ]
    for (let i = 0; i < apis.length; i++) {
      const { apiUrl, param, screen } = apis[i];
      const url = `${apiUrl}?email=${email}&password=${password}`;
    
      try {
        const response = await fetch(url, {
          method: 'GET',
          headers: {
            Accept: 'application/json',
            'Content-Type': 'application/json',
          },
        });
    if (response.ok) {
      const data = await response.json();
      console.log(data);
      const userDb = data.userDb;
      console.log(userDb);
      navigation.navigate(screen, { [param]: userDb });
      return;
    }
  } catch (error) {
    console.error('Error:', error);
  }
}

Alert.alert('Login failed', 'Invalid email or password');
};

  return (
    <ScrollView style={styles.container}>
      <View style={styles.flex1}>
        <Image style={styles.imagess} source={require('../image/login.png')} />
      </View>
      <View>
        <Text style={styles.Textheader}>Let's Start</Text>
        <Text style={styles.Text2}>Welcome to Rx For Pakistan With EHR</Text>
      </View>
      <View>
        <TextInput
          style={styles.input}
          value={email}
          onChangeText={setEmail}
          placeholder="Email"
          keyboardType="defult"
        />
        <TextInput
          style={styles.input}
          value={password}
          secureTextEntry={true}
          placeholder="Password"
          onChangeText={setPassword}
          keyboardType="default"
        />
      </View>
      <View style={styles.checkboxWrapper}>
        <CheckBox  value={rememberMe}
        onValueChange={setRememberMe} />
        <Text style={{ fontSize: 18 }}>Remember me</Text>
      </View>
      <View>
        <TouchableOpacity style={styles.button} onPress={handleLogin}>
          <Text style={styles.Text2}>Login</Text>
        </TouchableOpacity>
      </View>
      <View>
        <TouchableOpacity>
          <Text style={styles.link1}>Forgot Password?</Text>
        </TouchableOpacity>

        <TouchableOpacity  onPress={() => navigation.navigate('Signup')}>
          <Text style={styles.Text2}>
            Don't have any account?{''}
            <Text style={styles.Text3}> Signup</Text>

          </Text>
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
  imagess: {
    height: 220,
    width: 220,
    alignItems: 'center',
  },
  Textheader: {
    fontSize: 46,
    color: '#000000',
    textAlign: 'center',
  },
  Text2: {
    padding: 10,
    fontSize: 20,
    color: '#000000',
    textAlign: 'center',
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
    borderColor: '#000000',
    width: '90%',
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
    alignItems: 'center',
    backgroundColor: '#27727A',
    padding: 10,
    borderRadius: 25,
    margin: 7,
    fontSize: 34,
    width: '92%',
  },
  link1: {
    fontSize: 15,
    color: '#ff0000',
    textAlign: 'right',
    margin: 8,
  },
});
export default Login;

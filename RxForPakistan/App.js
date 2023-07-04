import React from 'react';
import 'react-native-gesture-handler';
import { View, Text, Image, ScrollView, TextInput,StyleSheet } from 'react-native';
import { createStackNavigator } from '@react-navigation/stack'; 
import { NavigationContainer } from '@react-navigation/native';
import Login from './screens/Login';
import Signup from './screens/Signup';
import DoctorReg from './screens/DoctorReg';
import PharmacyReg from './screens/PharmacyReg';
import Disease from './screens/Disease';
import DoctorDash from './screens/DoctorDash';
import Rx from './screens/Rx';
import Patienthis  from './screens/Patienthis';
import Appointment  from './screens/Appointment';
import Patient  from './screens/Patient';
import Test  from './screens/Test';
import DrugInspector from './screens/DrugInspector';
import PendingAppointment from './screens/PendingAppointment';
import AddMedicine from './screens/AddMedicine';
import BanMedicine from './screens/BanMedicine';
import MedicineToDisease from './screens/MedicineToDisease';
import MedicineToMedicine from './screens/MedicineToMedicine';
import PatientMedicine  from './screens/PatientMedicine';
import Pharmacy  from './screens/Pharmacy';
import PharmacyPrescription  from './screens/PharmacyPrescription';
import DrPatientMedicine from './screens/DrPatientMedicine';
import PAppointments from './screens/PAppointments';
import Vitals from './screens/Vitals';
import DeReceieved from './screens/DeReceieved';
import DrRequest from './screens/DrRequest';
import Response from './screens/Response';
import ResponseRx from './screens/ResponseRx';
import ShowResponse from './screens/ShowResponse';
import ShowResponseData from './screens/ShowResponseData';
global.ip='192.168.43.35';
const Stack = createStackNavigator();
const App = () => {
  return (
   /*<View>
    
    <ResponseRx/>
  </View>*/
   <NavigationContainer> 
      <Stack.Navigator headerShown='false'>



        <Stack.Screen
          name="Login"
          component={Login}
          options={{ headerShown: false }} 
        /> 
        <Stack.Screen
          name="Signup"
          component={Signup}
          options={{ headerShown: false }} 
        />
        
         <Stack.Screen
          name="DoctorReg"
          component={DoctorReg}
          options={{ headerShown: false }} 
        />
        <Stack.Screen
          name="PharmacyReg"
          component={PharmacyReg}
          options={{ headerShown: false }} 
        />
       
        <Stack.Screen
          name="DoctorDash"
          component={DoctorDash}
          options={{ headerShown: false }} 
        />
         <Stack.Screen
          name="Rx"
          component={Rx}
          options={{ headerShown: false }} 
        />
        <Stack.Screen
          name="Patienthis"
          component={Patienthis}
          options={{ headerShown: false }} 
        />
        <Stack.Screen
          name="Appointment"
          component={Appointment}
          options={{ headerShown: false }} 
        />
         <Stack.Screen
          name="Patient"
          component={Patient}
          options={{ headerShown: false }} 
        />
         <Stack.Screen
          name="DrugInspector"
          component={DrugInspector}
          options={{ headerShown: false }} 
        />
        <Stack.Screen
          name="AddMedicine"
          component={AddMedicine}
          options={{ headerShown: false }} 

        />
        <Stack.Screen
          name="BanMedicine"
          component={BanMedicine}
          options={{ headerShown: false }} 
        />
        <Stack.Screen
          name="MedicineToDisease"
          component={MedicineToDisease}
          options={{ headerShown: false }} 
        />
        <Stack.Screen
          name="MedicineToMedicine"
          component={MedicineToMedicine}
          options={{ headerShown: false }} 
        />
        <Stack.Screen
          name="PatientMedicine"
          component={PatientMedicine}
          options={{ headerShown: false }} 
        />
         <Stack.Screen
          name="PendingAppointment"
          component={PendingAppointment}
          options={{ headerShown: false }} 
        />
        <Stack.Screen
          name="Pharmacy"
          component={Pharmacy}
          options={{ headerShown: false }} 
        />
        <Stack.Screen
          name="PharmacyPrescription"
          component={PharmacyPrescription}
          options={{ headerShown: false }} 
        />
        <Stack.Screen
          name="DrPatientMedicine"
          component={DrPatientMedicine}
          options={{ headerShown: false }} 
        />
        <Stack.Screen
          name="Vitals"
          component={Vitals}
          options={{ headerShown: false }} 
        />
        <Stack.Screen
          name="PAppointments"
          component={PAppointments}
          options={{ headerShown: false }} 
        />
        <Stack.Screen
          name="Disease"
          component={Disease}
          options={{ headerShown: false }} 
        />
        <Stack.Screen
          name="DrRequest"
          component={DrRequest}
          options={{ headerShown: false }} 
        />
        <Stack.Screen
          name="DeReceieved"
          component={DeReceieved}
          options={{ headerShown: false }} 
        />
        <Stack.Screen
          name="Response"
          component={Response}
          options={{ headerShown: false }} 
        />
        <Stack.Screen
          name="ResponseRx"
          component={ResponseRx}
          options={{ headerShown: false }} 
        />
       <Stack.Screen
          name="ShowResponse"
          component={ShowResponse}
          options={{ headerShown: false }} 
        />
        <Stack.Screen
          name="ShowResponseData"
          component={ShowResponseData}
          options={{ headerShown: false }} 
        />
      
      </Stack.Navigator>
      
    </NavigationContainer>

  );
}


const styles = StyleSheet.create({
  container: {
    
    flex:3,
    
    backgroundColor:'#A4E5EE',

  },
  bigBlue: {
    color: 'blue',
    fontWeight: 'bold',
    fontSize: 30,
  }
});
export default App;
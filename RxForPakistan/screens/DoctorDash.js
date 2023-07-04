import React, {useEffect,useState} from 'react';
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
import { createBottomTabNavigator } from '@react-navigation/bottom-tabs';
import { NavigationContainer } from '@react-navigation/native';
import { useRoute } from '@react-navigation/native';





const Login = ({navigation}) => {
  const route = useRoute();
  //for check box
  const [labelValue, setLabelValue] = useState('');
  const [labelValue1, setLabelValue1] = useState('');
  const [labelValue2, setLabelValue2] = useState('');
  const { doctordb } = route.params;
  useEffect(() => {
    const apiUrl = `http://${ip}/DoctorApi/api/Doctor/DocDash?doctordb=${doctordb}`;

    
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
      setLabelValue(data.Name);
      
      setLabelValue2(data.Specialization);
      //setData(data);
    })
      .catch((error) => {
        console.error('Error:', error);
      });
  },[doctordb]);
  
  useEffect(() => {
    const apiUrl = `http://${ip}/DoctorApi/api/Doctor/DocDash?doctordb=${doctordb}`;

    
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
      setLabelValue(data.Name);
      
      setLabelValue2(data.Specialization);
      //setData(data);
    })
      .catch((error) => {
        console.error('Error:', error);
      });
  },[doctordb]);
  
  const handleLogout = () => {
    // Clear session data
    //clearSessionData();
  
    // Navigate to login screen
    navigation.reset({
      index: 0,
      routes: [{ name: 'Login' }],
    });
  };

  return (
    <ScrollView style={styles.container}>
      {/*Flex 1 screen ka laya  */}
      <View style={styles.flex1}>
        {/*Doctor  image  */}
        <Image style={styles.imagess} source={require('../image/Doctor.png')} />
      </View>
      <View>
        {/*Top header  */}
        <Text style={styles.Textheader}>Wellcome Dr.{labelValue}</Text>
        {/*line 2  */}
      
</View>

<View>
        {/*Top header  }
        <Text style={styles.Textheader}>Specialization:{labelValue2}</Text>
        {/*line 2  */}
      
</View>
      {/*touchable button for login */}
      <View style={styles.flex2}>
      <View style={styles.row}>
        <TouchableOpacity
          style={styles.button}
          // onPress={onPress}
          onPress={() => navigation.navigate('Appointment',{doctordb:doctordb})}
        >
                    <Image style={styles.image1}  source={require('../image/Appointment.png')} />

          <Text style={styles.Text4}>New Appointment</Text>
          
        </TouchableOpacity>

        <TouchableOpacity
          style={styles.button}
          onPress={() => navigation.navigate('Patienthis',{doctordb:doctordb})}
        >
                    <Image style={styles.image1}  source={require('../image/history.png')} />

          <Text style={styles.Text4}>Patient's History</Text>
        </TouchableOpacity>
          </View>  
          <View style={styles.row}>  
        <TouchableOpacity
          style={styles.button}
          onPress={() => navigation.navigate('PendingAppointment',{doctordb:doctordb})}
        >
                    <Image style={styles.image2}  source={require('../image/RX.png')} />

          <Text style={styles.Text4}>Rx</Text>
        </TouchableOpacity>

        <TouchableOpacity
          style={styles.button}
          
          onPress={() => navigation.navigate('PAppointments',{doctordb:doctordb})}
        >
          <Image style={styles.image2}  source={require('../image/pending.png')} />
          <Text style={styles.Text4}>Add Vitals</Text>
        </TouchableOpacity>
      </View>
      {/*
      <View style={styles.row}>  
        <TouchableOpacity
          style={styles.button}
          onPress={() => navigation.navigate('DrRequest',{doctordb:doctordb})}
        >
          {
           //         <Image style={styles.image2}  source={require('../image/RX.png')} />
          }
          <Text style={styles.Text4}>Send Request </Text>
        </TouchableOpacity>

        <TouchableOpacity
          style={styles.button}
          
          onPress={() => navigation.navigate('DeReceieved',{doctordb:doctordb})}
          
        >
          {
          //<Image style={styles.image2}  source={require('../image/pending.png')} />
}
          <Text style={styles.Text4}>Receieved Request </Text>
        </TouchableOpacity>
      </View>
      
     
      <View style={styles.row}>
        <TouchableOpacity
          style={styles.button}
          // onPress={onPress}
          onPress={() => navigation.navigate('ShowResponse',{doctordb:doctordb})}
        >
                   { 
                  // <Image style={styles.image1}  source={require('../image/Appointment.png')} />
                   }

          <Text style={styles.Text4}>Receieved Response</Text>
          
        </TouchableOpacity>

        
          </View>
                  */}  
      </View>
      {/*
      <View   style={styles.loginb}>
        <TouchableOpacity
          style={styles.button1}
          onPress={handleLogout}
        >
           <Image style={styles.image1}  source={require('../image/logout.png')} />
          
        </TouchableOpacity>
      </View>
                  */}
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
    fontSize: 18,
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

  button:{
    alignItems: "center",
backgroundColor: "#efff",
padding:25,
 borderRadius:40, 
 margin: 5,
 fontSize:10,
 width: "50%",
  },
  button1: {
    // flex: 1,
    // flexDirection: 'row',
    // justifyContent: 'space-between',
    // alignItems: 'flex-start',
    backgroundColor: '#efff',
    padding: 15,
    borderRadius: 120,
    margin: 10,
    marginTop: 10,
   // fontSize: 18,
    width: '30%',
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
 
   }
   ,image1: {
    height: 70,
    width: 70,
    alignItems: 'center',
  }
  ,image2: {
    height: 85,
    width: 67,
    alignItems: 'center',
  },
  row:{
    flexDirection:'row',
    alignItems: 'center',
    justifyContent: 'center',
  },
  Text4: {
    padding:10,
    fontSize:12,
    color:'#000000',
    textAlign:"center",
    fontWeight: 'bold'
  
}
});
export default Login;

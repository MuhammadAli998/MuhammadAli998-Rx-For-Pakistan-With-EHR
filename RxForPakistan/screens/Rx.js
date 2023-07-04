import React, {useState,useEffect} from 'react';
import {
  View,
  Text,
  Image,
  ScrollView,
  TouchableOpacity,
  TextInput,
  StyleSheet,
  Alert,
} from 'react-native';
//import { Feather, Entypo } from "@expo/vector-icons";
import CheckBox from '@react-native-community/checkbox';
//import {Dropdown} from 'react-native-material-dropdown'
import { SelectList } from 'react-native-dropdown-select-list'
import { DataTable } from 'react-native-paper';
import { useRoute } from '@react-navigation/native';

const Rx = ({navigation}) => {
  const route = useRoute();
  const { Aid,Date,Time,doctordb } = route.params;
   
 

 const [appointments, setAppointments] = useState([]);
  const [highbp, sethighbp] = useState(false);
  const [lowbp, setlowbp] = useState(false);
  const [sugar, setsugar] = useState(false);
  const [temperature, settemperature] = useState(false);
  const [patientname, setpatientname] = useState(false);
  const [patientaddress, setpatientaddress] = useState(false);
  const [pphonenumber, setpphonenumber] = useState(false);
  const [cnic, setcnic] = useState(false);
  const [ccomplaint, setccomplaint] = useState(false);
  
  const [HighBP, setHighBP] = useState("00");
  const [LowBP, setLowBP] = useState("00");
  const [Sugar, setSugar] = useState("00");
  const [Temperature, setTemperature] = useState("00");

  const [DoctorName, setDoctorName] = useState("");
  const [PhoneNumber, setPhoneNumber] = useState("");

  const [PatientName, setPatientName] = useState("");
  const [PatientAddress, setPatientAddress] = useState("");
  const [PPhoneNumber, setPPhoneNumber] = useState("");
  const [Cnic, setCnic] = useState("");

  const [medicineList, setMedicineList] = useState([]);
  const [selectedMedicine, setSelectedMedicine] = useState('');


  const [pharmacies, setPharmacies] = useState([]);
const [selectedPharmacy, setSelectedPharmacy] = useState(null);
const [pharmacyName, setPharmacyName] = useState("");
const [pharmacyLicense, setPharmacyLicense] = useState("");

 
const [selected, setSelected] = useState("");
const [deleteIndex, setDeleteIndex] = useState(-1);
const [medicationList, setMedicationList] = useState([]);
const [selectDays, setSelectDays] = useState("");
const [selectDosage, setSelectDosage] = useState("");
const [selectType, setSelectType] = useState("");

const frequency  = [
  {key:'1', value:'1+1+1'},
  {key:'2', value:'1+1+0'},
  {key:'3', value:'1+0+0'},
  {key:'4', value:'0+0+1'},
  {key:'5', value:'0+1+1'},
  {key:'6', value:'1+0+1'},
  {key:'7', value:'0+1+0'},
]
const Days  = [
  {key:'1', value:'1'},
  {key:'2', value:'3'},
  {key:'3', value:'5'},
  {key:'4', value:'7'},
  {key:'5', value:'14'},
  {key:'6', value:'20'},
  {key:'7', value:'30'},
]
const Dosage = [
  {key:'1', value:'500mg'},
  {key:'2', value:'250mg '},
  {key:'3', value:'2.5 g'},
  {key:'4', value:'50mcg'},
  {key:'5', value:'100 IU'},
  
]
const Type = [
  {key:'1', value:'Tablet'},
  {key:'2', value:'Injection '},
  {key:'3', value:'Drip'},
  {key:'4', value:'Syrup'},
  
  
]
const [diseases, setDiseases] = useState([]);
const [medicineCData, setMedicineCData] = useState([]);
useEffect(() => {
  
 
  fetch(`http://${ip}/DoctorApi/api/Doctor/GetVitals?doctordb=${doctordb}&appointmentId=${Aid}`)
    .then(response => {
      if (response.ok) {
        return response.json();
      }
      throw new Error('Network response was not ok.');
    })
    .then(data => {
      // Update the state variables with the fetched data
      setHighBP(data.High_BP);
      setLowBP(data.Low_BP);
      setSugar(data.SugarLevel);
      setTemperature(data.Temperature);
    })
    .catch(error => {
      console.error('Error fetching vitals data:', error);
      // Handle the error condition, for example by showing an error message to the user
    });

  fetch(`http://${ip}/DoctorApi/api/Doctor/GetPatientData?doctordb=${doctordb}&appointmentId=${Aid}`)
    .then(response => {
      if (response.ok) {
        return response.json();
      }
      throw new Error('Network response was not ok.');
    })
    .then(patient => {
      // Update the state variables with the fetched patient data
      setPatientName(patient.PatientName);
      setPatientAddress(patient.PatientAddress);
      setPPhoneNumber(patient.PhoneNumber);
      setCnic(patient.Cnic);
    })
    .catch(error => {
      console.error('Error fetching patient data:', error);
      // Handle the error condition, for example by showing an error message to the user
    });

  fetch(`http://${ip}/DoctorApi/api/Doctor/DocDash?doctordb=${doctordb}`)
    .then(response => {
      if (response.ok) {
        return response.json();
      }
      throw new Error('Network response was not ok.');
    })
    .then(Data => {
      // Update the state variables with the fetched patient data
      setDoctorName(Data.Name);
      setPhoneNumber(Data.Phone);

      console.log(DoctorName);
      console.log(PhoneNumber);
    })
    .catch(error => {
      console.error('Error fetching patient data:', error);
      // Handle the error condition, for example by showing an error message to the user
    });
    fetch(`http://${ip}/DoctorApi/api/Doctor/GetDiseases?doctordb=${doctordb}&appointmentId=${Aid}`)
    .then(response => {
      if (response.ok) {
        return response.json();
      }
      throw new Error('Network response was not ok.');
    })
    .then(data => {
      // Update the state variable with the fetched diseases data
      setDiseases(data);
      console.log(data);
    })
    .catch(error => {
      console.error('Error fetching diseases:', error);
      // Handle the error condition, for example by showing an error message to the user
    });
    
  fetch(`http://${ip}/DoctorApi/api/Doctor/GetPharmacy`)
    .then((response) => response.json())
    .then((data) => {
      const pharmacies = data.map((pharmacy) => ({
        label: pharmacy.PharamacyName,
        value: pharmacy.PharamacyName,
        PharamacyName: pharmacy.PharamacyName,
        Licence: pharmacy.Licence,
      }));
      setPharmacies(pharmacies);
      setSelectedPharmacy(pharmacies[0]?.value);
      setPharmacyName(pharmacies[0]?.PharamacyName);
      setPharmacyLicense(pharmacies[0]?.Licence);
    })
    .catch((error) => console.log(error));
}, []);


const sendRx = async () => {
  const data = {
    Aid: Aid,
    DrName: DoctorName,
    PharmacyName:pharmacyName ,
    License: pharmacyLicense,
    Cnic: Cnic,
    PatientName: PatientName,
    PPhoneNumber: PPhoneNumber,
    PhoneNumber: PhoneNumber,
    Date: Date,
    Time: Time,
    Medicines: medicationList.map((medication) => ({
     MedicineName: medication.medicine,
     Routine: medication.frequency,
     Days: medication.dosage,
    })),
  };

  try {
    const response = await fetch(`http://${ip}/DoctorApi/api/Fhir/SendRx?doctordb=${doctordb}`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(data),
    });

    if (response.ok) {
      const result = await response.json();
      // Handle the API response
      console.log(result);
      // Redirect to the patient history screen or show a success message
      Alert.alert('Rx Sent Successfully');
      console.log(data);
    } else {
      console.log(data);
      throw new Error('Request failed with status ' + response.status);
      
    }
  } catch (error) {
    // Handle the error condition
    console.log(data);
    console.error('Error:', error);
    // Show an error message to the user
    Alert.alert('Error', 'An error occurred while sending the prescription.');
  }
};

useEffect(() => {
  //if (diseases === '') {
    // Call the first API to get all medicines
    fetch(`http://${ip}/MedicineApi/api/Medicine/GetMedicine`, {
      method: 'GET'
    })
      .then(response => response.json())
      .then(data => setMedicineList(data))
      .catch(error => console.error(error));
  })
  /*} else {
    // Call the second API to get medicines based on the disease
    fetch(`http://${ip}/MedicineApi/api/Medicine/GetDMedicine?disease=${diseases}`, {
      method: 'GET'
    })
      .then(response => response.json())
      .then(data => setMedicineList(data))
      .catch(error => console.error(error));
  }
}, [diseases]);*/
//handel function for medicine to set in table
const handleAddMedication = async () => {
  if (selectedMedicine === '' || selected === '' || selectDosage === '') {
    // Validation: Make sure all fields are selected
    Alert.alert('Error', 'Please select a medicine, frequency, and dosage');
    return;
  }

  // Retrieve contraindications for the selected medicine
  try {
    const response = await fetch(`http://${ip}/MedicineApi/api/Medicine/GetMedicineContraindicationsByName?medicineName=${selectedMedicine}`);
    if (response.ok) {
      const contraindications = await response.json();

      // Check for contraindications with existing medications
      let hasContraindication = false;
      medicationList.forEach((medication) => {
        if (contraindications.includes(medication.medicine)) {
          hasContraindication = true;
        }
      });

      if (hasContraindication) {
        // Display an alert if contraindication is found
        Alert.alert('Contraindication', 'The selected medicine has a contraindication with one or more of the existing medications');
        return;
      }
    } else {
      // Handle API error response
      Alert.alert('Error', 'Failed to retrieve contraindications');
      return;
    }
  } catch (error) {
    // Handle fetch error
    Alert.alert('Error', 'Failed to retrieve contraindications');
    return;
  }

  const newMedication = {
    medicine: selectedMedicine,
    frequency: selected,
    dosage: selectDosage,
  };

  setMedicationList((prevList) => [...prevList, newMedication]);

  // Clear the selected values
  setSelectedMedicine('');
  setSelected('');
  setSelectDosage('');
};



  const handlePharmacySelection = (pharmacyValue) => {
    const selectedPharmacyData = pharmacies.find((pharmacy) => pharmacy.value === pharmacyValue);
    setSelectedPharmacy(selectedPharmacyData?.value);
    setPharmacyName(selectedPharmacyData?.PharamacyName);
    setPharmacyLicense(selectedPharmacyData?.Licence);
    console.log(pharmacyName);
  };
  
return(   
<ScrollView style={styles.container}>
{/*Search by cnic */}

{/*patient info */}

<Text  style={styles.Textheader}>Patient Details</Text>
<View style={styles.row}>
<Text  style={styles.Text1}>Patient Name:{PatientName}</Text>
<CheckBox  value={patientname}
        onValueChange={setpatientname} />
        </View>
<View style={styles.row}>
  
<Text  style={styles.Text1}>Patient Contact:{PPhoneNumber}</Text>
<CheckBox 
 value={pphonenumber}
        onValueChange={setpphonenumber}
         />
        </View>
<View style={styles.row}>
<Text  style={styles.Text1}>Patient Address:{PatientAddress}</Text>
<CheckBox  value={patientaddress}
        onValueChange={setpatientaddress} /></View>
<View style={styles.row}>
<Text  style={styles.Text1}>Patient Cnic:{Cnic}</Text>
<CheckBox  value={cnic}
        onValueChange={setcnic} /></View>

<View >
<Text  style={styles.Textheader}>Patient Vitals</Text>

       
</View>

<View style={styles.row}>
  <Text style={styles.Text1}> Blood Pressure:{HighBP}/{LowBP} mm Hg</Text>
  <CheckBox  value={highbp}
        onValueChange={sethighbp} /></View>
  <View style={styles.row}>

  <Text style={styles.Text1}>Temperature:{Temperature} °F</Text>
  <CheckBox  value={temperature}
        onValueChange={settemperature} />
        </View>
  <View style={styles.row}>
  <Text style={styles.Text1}>Sugar Level:{Sugar} mg/dl</Text>
  <CheckBox 
           value={sugar}
        onValueChange={setsugar} />
        </View>


<View>
  <Text style={styles.Textheader}>RX</Text>
  <View>
    <SelectList
      setSelected={(val) => setSelectedMedicine(val)}
      data={medicineList}
      save="value"
      label="Medicine"
      placeholder="Medicine"
      boxStyles={styles.input}
    />
    <SelectList
      setSelected={(val) => setSelected(val)}
      data={frequency}
      save="value"
      label="Frequency"
      placeholder="Frequency"
      boxStyles={styles.input}
    />
    <SelectList
      setSelected={(val) => setSelectDosage(val)}
      data={Days}
      save="value"
      label="Days"
      placeholder="Days"
      boxStyles={styles.input}
    />
    <>{/*
    <SelectList
      setSelected={(val) => setSelectDays(val)}
      data={Dosage}
      save="value"
      label="Dosage"
      placeholder="Dosage"
      boxStyles={styles.input}
    />
    <SelectList
      setSelected={(val) => setSelectType(val)}
      data={Type}
      save="value"
      label="Medicine Type"
      placeholder="Medicine Type"
      boxStyles={styles.input}
    />
*/}</>
  </View>
  <View style={styles.MultipleSelectList}>
    <TouchableOpacity style={styles.button} onPress={handleAddMedication}>
      
      <Text style={styles.Text2}>ADD</Text>
    </TouchableOpacity>
  </View>
</View>

<View style={{ marginTop: 30 }}>
<DataTable style={styles.container}>
  <DataTable.Header style={styles.tableHeader}>
    <DataTable.Title>Sr#</DataTable.Title>
    <DataTable.Title>Medicine</DataTable.Title>
    <DataTable.Title>Frequency</DataTable.Title>
    <DataTable.Title>Days</DataTable.Title>
  </DataTable.Header>
  {medicationList.map((medication, index) => (
    <TouchableOpacity
      key={index}
      onPress={() => setDeleteIndex(index)}
      style={deleteIndex === index ? styles.highlightedRow : null}
    >
    <DataTable.Row>
      <DataTable.Cell>{index + 1}</DataTable.Cell>
      <DataTable.Cell>{medication.medicine}</DataTable.Cell>
      <DataTable.Cell>{medication.frequency}</DataTable.Cell>
      <DataTable.Cell>{medication.dosage}</DataTable.Cell>
    </DataTable.Row>
    </TouchableOpacity>
  ))}
</DataTable>
</View>
<View>
<SelectList
        setSelected={(pharmacy) => handlePharmacySelection(pharmacy)}
        data={pharmacies}
        save="value"
        label="Select Pharmacy"
        placeholder="Select Pharmacy"
        boxStyles={styles.input}
      />


</View>
<View>
<TouchableOpacity
          style={styles.button1}
         onPress={sendRx}
        >
          <Text style={styles.Text2}>Send</Text>
          
        </TouchableOpacity>
           
    </View>
</ScrollView>




);
}
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
      padding:10,
     // alignItems: 'center',
      backgroundColor: '#8cdbe6',
    },
    imagess: {
      height: 220,
      width: 220,
      alignItems: 'center',
    },
    Textheader: {
      paddingTop: 10,
      fontSize: 35,
      color: '#000000',
      textAlign: 'center',
    },
    Text2: {
      padding: 10,
      fontSize: 15,
      color: '#efffff',
      textAlign: 'left',
      fontWeight: 'bold',
    },
    Text1: {
        padding:5,
        fontSize: 20,
        color: '#000000',
       // textAlign: 'left',
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
      height:45,
      margin: 20,
      borderWidth: 2,
      borderColor: '#000000',
      width: '60%',
      borderRadius: 20,
      padding: 4,
    },
    input2: {
        height:35,
        margin: 20,
        borderWidth: 2,
        borderColor: '#000000',
        width: '40%',
        borderRadius: 25,
        padding: 7,
        paddingTop: 10,
      
      },
    checkboxWrapper: {
      flexDirection: 'row',
      alignItems: 'center',
      paddingVertical: 9,
      color: '#000000',
      fontWeight: 'bold',
    },
  
    button: {
      //flex: 1,
      //flexDirection: 'row',
      //justifyContent: 'flex-end',
       alignItems: 'center',
      backgroundColor: '#17725C',
      padding: 1,
      borderRadius: 15,
      margin:5,

      //marginTop: 30,
     // fontSize: 18,
      width: '35%',
    },
    button1: {
      // flex: 1,
      // flexDirection: 'row',
      // justifyContent: 'space-between',
      // alignItems: 'flex-start',
      backgroundColor: '#17725C',
      padding: 15,
      borderRadius: 15,
      margin: 10,
      marginTop: 60,
     // fontSize: 18,
      width: '45%',
    },
    link1: {
      fontSize: 15,
      color: '#ff0000',
      textAlign: 'right',
      margin: 8,
    },
    VitalView:{
     // borderWidth: 1,
      flexDirection: 'row',
      //justifyContent:'space-evenly',
      // flexWrap: 'nowrap',
     // alignItems:'stretch',
      margin:1,
      // paddingTop: 12,
      
  
    },
    MultipleSelectList:{
      // borderWidth: 1,
       flexDirection: 'row',
       justifyContent:'space-evenly',
       // flexWrap: 'nowrap',
      alignItems:'stretch',
       margin:1,
       padding:3,
       
   
     },
    loginb:{
      // borderWidth: 1,
       flexDirection: 'row',
       justifyContent:'flex-end',
       // flexWrap: 'nowrap',
       //alignItems:'right',
       margin:12,
   
     },
     multiline: {
      width:200,
      borderWidth:1,
      width: '97%',
      height:'20%',
      textAlignVertical: "top",
      //borderBottomColor:'red',
     // borderBottomWidth:1,
  },
  tableHeader: {
    backgroundColor: '#808080',
    borderRadius: 15,
    fontSize: 15,

  },
  row:{
    flexDirection:'row',
    alignItems: 'center',
    justifyContent: 'center',
  },
  rolebutton:{
    alignItems: "center",
backgroundColor: "#17725C",
padding:4,
 borderRadius:25, 
 justifyContent:'space-evenly',
 margin:4,
 fontSize:18,
 width: "30%",
  },
  row:{
    flexDirection:'row',
    alignItems: 'center',
    justifyContent: 'center',
  },
  });
export default Rx;
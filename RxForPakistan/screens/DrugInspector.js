import React,{useState} from 'react';
import { View, Text, Image,Button, ScrollView,TouchableOpacity, TextInput,SafeAreaView,StyleSheet} from 'react-native';
import { Alert } from 'react-native';
import RadioForm,{RadioButton, RadioButtonInput, RadioButtonLabel} from 'react-native-simple-radio-button';
const DrugInspector = ({navigation}) => {

return(

< ScrollView   style={styles.container}>

    
<View style={styles.flex1}>
        <Image style={styles.imagess} source={require('../image/DrugInspector.png')} />
      </View>


      <View>
        <Text style={styles.text}>Welcome</Text>
    </View>
<View style={styles.row}>


<TouchableOpacity style={styles.button }   
 onPress={() => navigation.navigate('AddMedicine')}
>
      <Image style={styles.image1}  source={require('../image/medicine.png')} />
      <Text   style={styles.Text2} >Add Medicine</Text>

    </TouchableOpacity>


<TouchableOpacity style={styles.button }   
onPress={() => navigation.navigate('BanMedicine')}
>
      <Image style={styles.image1}  source={require('../image/Ban.png')} />
      <Text   style={styles.Text2} >Ban Medicine</Text>
    </TouchableOpacity>
</View>
<View style={styles.row}>


<TouchableOpacity style={styles.button }   
onPress={() => navigation.navigate('MedicineToMedicine')}
>
      <Image style={styles.image1}  source={require('../image/MtoM.png')} />
      <Text   style={styles.Text4} >Medicine to Medicine </Text>
    </TouchableOpacity>


<TouchableOpacity style={styles.button }   
onPress={() => navigation.navigate('MedicineToDisease')}
>
      <Image style={styles.image1}  source={require('../image/MtoD.png')} />
      <Text   style={styles.Text4} >Medidine to Disease  </Text>
    </TouchableOpacity>
</View>
</ScrollView>



)





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
        fontSize:40,
        color:'#000000',
       
        fontWeight: 'bold',
        margin: 1,
        textAlign:"center",
                 
         
          
      },
      Text2: {
        padding:10,
        fontSize:16,
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
  padding:25,
   borderRadius:15, 
   margin: 10,
   fontSize:10,
   width: "40%",
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
  },  flex1: {
    padding: 10,
    alignItems: 'center',
    backgroundColor: '#398AA4',
  },imagess: {
    height: 220,
    width: 220,
    alignItems: 'center',
  },image1: {
    height: 65,
    width: 65,
    alignItems: 'center',
  }
      
})
export default DrugInspector;
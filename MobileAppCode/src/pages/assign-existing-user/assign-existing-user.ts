import { Component } from '@angular/core';
import { AlertController, IonicPage, NavController, NavParams, TabHighlight } from 'ionic-angular';
import { AuthProvider } from '../../providers/auth/auth';
import { LiveList } from '../../providers/custom-class/custom-class';

/**
 * Generated class for the AssignExistingUserPage page.
 *
 * See https://ionicframework.com/docs/components/#navigation for more info on
 * Ionic pages and navigation.
 */

@IonicPage()
@Component({
  selector: 'page-assign-existing-user',
  templateUrl: 'assign-existing-user.html',
})
export class AssignExistingUserPage {

  loc:any;
  listData:any=[];
  alllist:LiveList[]=[];

  pastListData:LiveList[]=[];
  pastAllList:LiveList[]=[];

  loginRes:any;
  search:any;
  searchPast:any;
  pvalue:any;
  checkdata:string;
  checkedArray :any = [];
  Event_Id:any;
  checknull:any;

  constructor(public navCtrl: NavController, public navParams: NavParams,public auth:AuthProvider,public alertCtrl: AlertController,) {
    this.Event_Id = navParams.data["Event_Id"];
    this.loginRes=JSON.parse(localStorage.getItem("LoginRes"));
    console.log("Assign response",this.loginRes);
    console.log("Assign EmailId",this.loginRes.EmailId);
  }

  ionViewDidLoad() {
    console.log('ionViewDidLoad AssignExistingUserPage');
    this.GetScannedUserList();
  }

  doRefresh(event) {  
    console.log('Pull Event',event); 
    if (event){
      this.GetScannedUserList();
      event.complete();
    }else{
      console.log('Pull Event Triggered!'); 
    }
     
  }  

  back(){
    this.navCtrl.pop();
  }

  GetScannedUserList()
  {
    this.listData=[];

    this.loc=localStorage.getItem("Organizer"); //this.Event_Id = navParams.data["Event_Id"];
    if(this.loc=="Organizer" ||this.loc=="Event" ){
      console.log("Organizer");
      this.auth.GetAssignedScannerList(this.Event_Id).subscribe(x=>
        {
          if (x.length === 0) {
            
            this.checknull = false;
          }else{
          // alert(JSON.stringify(x));
          this.listData=x;
          this.alllist=x;
          }
        })

        // this.auth.GetLiveEventsList(this.loginRes.EmailId,0).subscribe(x=>
        //   {
        //     // alert(JSON.stringify(x));
        //     this.pastListData=x;
        //     this.pastAllList=x;
        //   })
    }else{
      console.log("Event");
    }
  
  }

  // if(this.checkedArray.includes(obj1)) {
  //   this.checkedArray = this.checkedArray.filter((value)=>value!=obj1);

  // } else {
  //   this.checkedArray.push(obj1)
  // }

  // EventOption(){
  //   console.log("EventOption",this.pvalue);
  // }

  addCheckbox(event, checkbox : any) { 
    console.log("click",checkbox);
      this.listData.forEach(item => {
        if(checkbox.ScannerId==item.ScannerId){
          item.IsActive = event._value;
        }
        
      });
   
  }

  //Removes checkbox from array when you uncheck it
  // removeCheckedFromArray(checkbox : String) {
  //   return this.checkedArray.findIndex((category)=>{
  //     return category === checkbox;
  //   })
  // }

  //Empties array with checkedboxes
  // emptyCheckedArray() {
  //   this.checkedArray = [];
  // }

  getCheckedBoxes() {
    //Do whatever
    let env = this;
    console.log("final listdataaaa",this.listData);
    if(this.Event_Id && this.listData.length>0){
      var obj1 = {
        "EventId" : this.Event_Id
        };
        var obj2 = {
          "exuser" : this.listData
          };
        // var obj2 = this.checkedArray;
      var obj = Object.assign(obj1, obj2);
      this.assignExisting(obj);
    }else{
      env.presentAlert("Please select");
    }
    console.log("finalArray",obj);
  }

  assignExisting(obj){
    let env = this;
    this.auth.showLoading();
    console.log("assignExisting",obj);
    this.auth
    .assignExistingUser(obj)
    .then(
      res => {
          console.log("checkIn response",res);
          if (res["Code"] == 200) {
            env.presentAlert("User Assigned Successfully");
            console.log("checkIn response",res['Msg']);
          }else if (res["Code"] == 400) {
            env.presentAlert("User Already Exist");
            this.GetScannedUserList();
          }else{
            env.presentAlert("Something went wrong");
          }
          this.auth.dismissLoading();

        },
      error => {
        if (error == "Something went wrong") {
          env.presentAlert(error);
        } else if (error == "Something went wrong") {
          env.presentAlert(error);
        }
        else {
          env.presentAlert("Something went wrong");
        }
        this.auth.dismissLoading();

      }
    )
    .catch(e => {
      // In case Of Exception
      env.presentAlert("Something went wrong");
      this.auth.dismissLoading();
    });
  }
  presentAlert(error: any) {
    let alert = this.alertCtrl.create({
      subTitle: error,
      buttons: [{
        text: 'OK',
        role: 'OK',
        cssClass: 'cancel-btn',
        handler: data => {
          console.log('Cancel clicked');
          this.GetScannedUserList();
        }
      }],
      cssClass:"myalert1",
    });
    alert.present();
  }
}

import { LiveList, LocalData } from './../../providers/custom-class/custom-class';
import { Component } from '@angular/core';
import { AlertController, NavController, NavParams, Platform } from 'ionic-angular';
import { AuthProvider } from '../../providers/auth/auth';



@Component({
    selector: 'customer-list-details',
    templateUrl: 'customer-list-details.html'
  })
export class CustomerListDetails{
  // Variable Declared in it
  myForm: any;
  userData: any;
  submitt: boolean = false;
  Toast: any;
  type: boolean = true;
  LocalData:any;
  Event_Id:any;
  userid:any;
  UserName:any;
  checknull:any;
  ListData=[];
  loginUserId:any="0";
  loginRes:any;
  loc:any;
  // AllList:LiveList[]=[];

  constructor(
    public navCtrl: NavController,
    public auth: AuthProvider,
    public platform: Platform,
    public alertCtrl: AlertController,
    public navParams: NavParams,
    private alert: AlertController,
  ) {
    this.LocalData=localStorage.getItem("Organizer");
    this.Event_Id = navParams.data["Event_Id"];
    this.userid = navParams.data["UserId"];
    this.UserName = navParams.data["UserName"];
    console.log(this.Event_Id,this.userid);

  this.loc = localStorage.getItem("Organizer"); //this.Event_Id = navParams.data["Event_Id"];
  if (this.loc == "Organizer") {
    this.loginRes=JSON.parse(localStorage.getItem("LoginRes"));
  console.log("Home response organization",this.loginRes);
  console.log("loginUserId",this.loginRes.org_scanid);
  this.loginUserId =this.loginRes.org_scanid;
  } else {
    this.loginRes=JSON.parse(localStorage.getItem("LoginRes"));
  console.log("Home response event",this.loginRes);
  console.log("loginUserId",this.loginRes.Id);
  this.loginUserId =this.loginRes.Id;
    
  }

  }

  back(){
    this.navCtrl.pop();
  }

  ionViewDidLoad(){
   this.GetDetailLists();
  }

  
  GetDetailLists()
  {
 //this.Event_Id = navParams.data["Event_Id"];this.Event_Id,this.userid  EventId=1446&&userid=15658
      this.auth.GetEventsListUserDetails(this.Event_Id,this.userid).subscribe(x=>
        {
          if (x.length === 0) {
            // array empty or does not exist

            this.checknull=false;

    }else{

            this.ListData=x;
        }
          // this.ListData=(x);
          // this.allGroups=(x);
          console.log("GetEventsListUserDetails",this.ListData);
         
        }
        )
  }
  checkIn(ticket){
    let env = this;
    console.log("ticket.TicketmapId",ticket.TicketmapId,this.loginUserId);
    this.auth
    .CheckedIn(ticket.TicketmapId,this.loginUserId)
    .then(
      res => {
          console.log("checkIn response",JSON.stringify(res["Msg"]));
          if (res["Code"] == 200) {
            env.presentAlert("Successfully Checked In");
          }else {
            env.presentAlert("Something went wromg");
          }

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

      }
    )
    .catch(e => {
      // In case Of Exception
      env.presentAlert("Something went wrong");
    });
  }
 // Alert any Error occured
 presentAlert(error: any) {
  let alert = this.alert.create({
    subTitle: error,
    buttons: [{
      text: 'OK',
      role: 'OK',
      cssClass: 'cancel-btn',
      handler: data => {
        console.log('Cancel clicked');
        this.GetDetailLists();
      }
    },],
    cssClass:"myalert1",
  });
  alert.present();
}

}
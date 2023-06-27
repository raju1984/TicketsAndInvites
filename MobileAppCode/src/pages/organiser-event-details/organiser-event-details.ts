import { TicketSalesPage } from './../ticket-sales/ticket-sales';
import { TabsPage } from './../tabs/tabs';
import { AssignExistingUserPage } from './../assign-existing-user/assign-existing-user';
import { TotalTicketAvailableTotalBoughtPage } from './../total-ticket-available-total-bought/total-ticket-available-total-bought';
import { ScanUserListPage } from './../scan-user-list/scan-user-list';
import { Component } from '@angular/core';
import { AlertController, NavController, Platform, NavParams } from 'ionic-angular';
import { AuthProvider } from '../../providers/auth/auth';
import { AccessUserDashboard } from '../access-user-dashboard/access-user-dashboard';
import { ViewAllCustomer } from '../view-all-customer/view-all-customer';
import { SidebarComponent } from '../../components/sidebar/sidebar';
import { ScanTicketPage } from '../scan-ticket/scan-ticket';
import { HttpClient } from '@angular/common/http';
import { HomePage } from '../home/home';

@Component({
    selector: 'organiser-event-details',
    templateUrl: 'organiser-event-details.html'
  })
export class OrganizerEventDetails{
  // Variable Declared in it
  myForm: any;
  userData: any;
  submitt: boolean = false;
  Toast: any;
  type: boolean = true;
  Event_Id:any;
  loginRes:any;
  getEventDetails:any={};
  EventName:any;

  constructor(
    public navCtrl: NavController,
    public auth: AuthProvider,
    public platform: Platform,
    public alertCtrl: AlertController,
    public navParams: NavParams,
    private http:HttpClient,
  ) {
    this.Event_Id = navParams.data["Event_Id"];
    console.log("Event_Id",this.Event_Id);
    this.EventName = navParams.data["EventName"];

    this.loginRes=JSON.parse(localStorage.getItem("LoginRes"));
    console.log("organization response",this.loginRes);
    console.log("organization EmailId",this.loginRes.EmailId);
  }

  ionViewDidLoad(){
    this.GetEventsListDetail();
   
  }

  // reloadData(){
  //   this.GetEventsListDetail();
  // }
  viewAllCustomer() {
    this.navCtrl.push(ViewAllCustomer,{Event_Id:this.Event_Id});
    
  }
  scanUserDetails(){
    this.navCtrl.push(ScanUserListPage,{Event_Id:this.Event_Id});
  }
  EventActivity(){
    this.navCtrl.push(TotalTicketAvailableTotalBoughtPage,{Event_Id:this.Event_Id});
  }
  ticketSales(){
    this.navCtrl.push(TicketSalesPage,{Event_Id:this.Event_Id});
  }

  alert(){
    this.alertButton();
  }

  presentPrompt() {
    let alert = this.alertCtrl.create({
      title: 'Add Scan User',
      cssClass : 'alert-ui',
      inputs: [
        {
          name: 'Name',
          placeholder: 'Enter Scanner Name',
        },
        {
          name: 'Email',
          placeholder: 'Enter Email'
        },
        {
          name: 'Password',
          placeholder: 'Enter Password'
        },
      ],
      buttons: [
        {
          text: 'Cancel',
          role: 'cancel',
          cssClass : 'cancel-btn',
          handler: data => {
            console.log('Cancel clicked');
          }
        },
        {
          text: 'Add',
          cssClass : 'add-btn',
          handler: data => {
            if(data.Name && data.Email && data.Password){
              console.log("Event_Id",data);
              this.AddScanUser(data);

            }else{
              this.alertCtrl.create({title:"USSID",message:"Please fill all fields ",buttons: [{text:"Ok",role:"cancel"}]}).present();
            }
            // if (data.isValid(data.UserId, data.Name, data.Password)) {
            //   console.log("Event_Id",data);
            // } else {
            //   // invalid login
            //   return false;
            // }
          }
        }
      ]
    });
    alert.present();
  }

  alertButton() {
    let alert = this.alertCtrl.create({
      // title: 'Add Scan User',
      cssClass : 'alert-ui alert-option',
      
      buttons: [
        {
          text: 'Add New',
          role: 'add new',
          handler: data => {
           this.presentPrompt();
          }
        },
        {
          text: 'Assign Existing User',
          cssClass : 'assign-existing-user-btn',
          handler: data => {
            this.navCtrl.push(AssignExistingUserPage,{Event_Id:this.Event_Id});
            // if (User.isValid(data.username, data.password)) {
            //   // logged in!
            // } else {
            //   // invalid login
            //   return false;
            // }
          }
        }
      ]
    });
    alert.present();
  }

  support(){
    this.navCtrl.push(AccessUserDashboard,{Event_Id:this.Event_Id});
  }
  openScanner(){
     // this.navCtrl.push(TicketsPage,{event_id:325,barcodeValue:579264869});
    //  debugger;
    localStorage.setItem("EventId",this.Event_Id);
    //  this.navCtrl.push(TabsPage,{Event_Id:this.Event_Id});
     this.navCtrl.push(HomePage,{Event_Id:this.Event_Id});
  }

  GetEventsListDetail()
  {
    this.auth.showLoading();
 //this.Event_Id = navParams.data["Event_Id"];
      this.auth.GetEventsListDetails(this.Event_Id).subscribe(x=>
        {
          this.getEventDetails=(x);
          console.log("getEventDetails",this.getEventDetails);
          this.auth.dismissLoading();
         
        })
  
  }

  AddScanUser(data){
    try {
      this.http.post("http://151.106.41.94:88/api/ScanNPassAPI/AddScanUser",{
        // this.http.post("https://veetickets.com/api/ScanNPassAPI/AddScanUser",{
          Name:data.Name,
          email:data.Email,
          Password:data.Password,
          orgemail:this.loginRes.EmailId,    
          EventId:this.Event_Id
        }).subscribe(r=>{
          console.log(r);
              if(r){
                this.alertCtrl.create({title:"AddScanUser",message:"User Created Successfully",buttons: [{text:"Ok",role:"cancel"}]}).present();
        }else{
          // this.alertCtrl.create({title:"USSID",message: '<table class="table-ui">' +
          // '<tr><th>Ticket Codes</th><td>"'+this.CustomCode+'"</td></tr><tr><th>Event</th><td>-</td></tr><tr><th>Ticket Name</th><td>-</td></tr><tr><th>Price</th><td>-</td></tr><tr><th>Date</th><td>-</td></tr>' +
          // '</table>',buttons: [{text:"Ok",role:"cancel"}]}).present();
            this.alertCtrl.create({title:"AddScanUser",message:"Something went wrong. Please try again",buttons: [{text:"Ok",role:"cancel"}]}).present();
          }
        },e=>{
          console.log(e);
        })
      
    } catch (error) {
      this.alertCtrl.create({title:"AddScanUser",message:"Something went wrong. Please try again",buttons: [{text:"Ok",role:"cancel"}]}).present();
    }
    console.log(data);
   
  }

  doRefresh(event) {  
    console.log('Pull Event',event); 
    if (event){
      this.GetEventsListDetail();
      event.complete();
    }else{
      console.log('Pull Event Triggered!'); 
    }
     
  }  

}
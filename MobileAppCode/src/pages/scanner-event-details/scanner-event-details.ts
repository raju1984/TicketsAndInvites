import { ViewAllCustomer } from './../view-all-customer/view-all-customer';
import { TabsPage } from './../tabs/tabs';
import { ScanUserListSummaryPage } from './../scan-user-list-summary/scan-user-list-summary';
import { Component } from '@angular/core';
import { AlertController, IonicPage, NavController, NavParams } from 'ionic-angular';
import { AuthProvider } from '../../providers/auth/auth';
import { HomePage } from '../home/home';
import { Network } from '@ionic-native/network';

/**
 * Generated class for the ScannerEventDetailsPage page.
 *
 * See https://ionicframework.com/docs/components/#navigation for more info on
 * Ionic pages and navigation.
 */

@IonicPage()
@Component({
  selector: 'page-scanner-event-details',
  templateUrl: 'scanner-event-details.html',
})
export class ScannerEventDetailsPage {
  Event_Id:any;
  getEventDetails:any={};
  loginRes:any;
  EventName:any;
  constructor(public navCtrl: NavController, public navParams: NavParams,private network: Network,public auth:AuthProvider,public alertCtrl: AlertController) {
    this.Event_Id = navParams.data["Event_Id"];
    this.EventName = navParams.data["EventName"];
    console.log("Event_Id scanner",this.Event_Id);

    this.loginRes=JSON.parse(localStorage.getItem("LoginRes"));
    console.log("organization response",this.loginRes);
    console.log("organization EmailId",this.loginRes.Id);
  }

  ionViewDidLoad() {
    console.log('ionViewDidLoad ScannerEventDetailsPage');
    if (this.network.type !== 'none') {
      // alert("server db");
      this.GetScanEventsListDetail();
    } else {
      // alert("local db");
      console.log("no network");
    }
  }

  doRefresh(event) {  
    console.log('Pull Event',event); 
    if (event){
      if (this.network.type !== 'none') {
      this.GetScanEventsListDetail();
      }
      event.complete();
    }else{
      console.log('Pull Event Triggered!'); 
    }
     
  }  

  back(){
    this.navCtrl.pop();
  }

  scanUserDetails(){
    this.navCtrl.push(ScanUserListSummaryPage,{Event_Id:this.Event_Id});
  }
  openScanner(){
    localStorage.setItem("EventId",JSON.stringify(this.Event_Id));
    // console.log('ScannerEventDetailsPage this.Event_Id',this.Event_Id);
    // this.navCtrl.push(TabsPage,{Event_Id:this.Event_Id});
    this.navCtrl.push(HomePage,{Event_Id:this.Event_Id});
  }
  viewAllGuest(){
    localStorage.setItem("Event_Id",JSON.stringify(this.Event_Id));
    this.navCtrl.push(ViewAllCustomer,{Event_Id:this.Event_Id});
  }

  GetScanEventsListDetail()
  {
    this.auth.showLoading();
 //this.Event_Id = navParams.data["Event_Id"];
      this.auth.GetScannerEventsListDetails(this.loginRes.Id,this.Event_Id).subscribe(x=>
        {
          this.getEventDetails=(x);
          console.log("GetScannerEventsListDetails",this.getEventDetails);
          this.auth.dismissLoading();
         
        })
        this.auth.dismissLoading();
  }

}

import { OrganizerEventDetails } from './../organiser-event-details/organiser-event-details';
import { Component, OnInit } from '@angular/core';
import { AlertController, NavController, NavParams } from 'ionic-angular';
import { CustomerListDetails } from '../customer-list-details/customer-list-details';
import { UssdCodeDetailsPage } from '../ussd-code-details/ussd-code-details';
import { AuthProvider } from '../../providers/auth/auth';
import { WebChannelTicketListPage } from '../web-channel-ticket-list/web-channel-ticket-list';

@Component({
  selector: 'access-user-dashboard',
  templateUrl: 'access-user-dashboard.html'
})
export class AccessUserDashboard implements OnInit {
  i_tab = 't1';
  segmentSelect: string = "t1";
  listData: any = [];
  checknullWeb: any;
  checknullUssd: any;
  ListData: any = [];
  searchData: any = [];
  checkdata: string;
  checkedArray: any = [];
  search: any;
  Event_Id: any;
  transactionId: any;
  UssdListData: any = [];
  UssdsearchData: any = [];
  Ussdsearch: any;

  constructor(public navCtrl: NavController, public auth: AuthProvider, public alertCtrl: AlertController, public navParams: NavParams,) {
    this.Event_Id = navParams.data["Event_Id"];
    console.log("Event_Id 666", this.Event_Id);

  }

  ngOnInit() {

  }
  ionViewDidLoad() {
    this.GetWebLists();
    this.GetUssdLists();
  }

  doRefresh(event) {  
    console.log('Pull Event',event); 
    if (event){
      this.GetWebLists();
      this.GetUssdLists();
      event.complete();
    }else{
      console.log('Pull Event Triggered!'); 
    }
     
  }  

  back(){
    this.navCtrl.pop();
  }

  segmentChanged(ev: any) {
    console.log('Segment changed', ev);
    //   console.log("okk",ev.detail.value);
    this.i_tab = ev._value;
  }

  ticketPage(list) {
    this.navCtrl.push(WebChannelTicketListPage,{Name:list.Name,Qty:list.Qty,Paymentid:list.Paymentid,Userid:list.Userid,Event_Id:this.Event_Id});
  }
  // ussdDetailPage(_list) {
  //   console.log('UssdCodeDetailsPage', _list);
  //   // this.navCtrl.push(UssdCodeDetailsPage);
  // }

  GetWebLists() {
    this.auth.showLoading();
    //this.Event_Id = navParams.data["Event_Id"];this.Event_Id,this.userid  EventId=1446&&userid=15658
    this.auth.WebChannel(this.Event_Id).subscribe(x => {
      if (x.length === 0) {
        // array empty or does not exist
        this.auth.dismissLoading();
        this.checknullWeb = false;

      } else {

        this.ListData = x;
        this.searchData = x;
        this.auth.dismissLoading();
        this.checknullWeb = true;
      }
      // this.ListData=(x);
      // this.allGroups=(x);
      console.log("GetEventsListUserDetails", this.ListData);

    }
    )
  }

  Onsearch() {
    this.ListData = this.searchData.filter((notChunk) => {
      return (
        notChunk.Name.toLowerCase().indexOf(this.search.toLowerCase()) > -1 || notChunk.Email.toLowerCase().indexOf(this.search.toLowerCase()) > -1
      );
    });
  }

  addCheckbox(event, checkbox) {
    // this.transactionId=checkbox.TransactionId;
    console.log("click", checkbox);


    if (event._value) {
      // var obj1 = {
      //   "email": checkbox.Email,
      //   "name": checkbox.Name,
      //   "barcode": checkbox.Barcode,
      //   "tickusermapid": checkbox.Tickusermapid,
      //   "ticketid": checkbox.TicketId,
      //   "ticketcreatedat": checkbox.TicketCreatedAt,
      //   "TransactionId":checkbox.TransactionId,
      // };
      this.checkedArray.push(checkbox);
      console.log("check66ed", this.checkedArray);
    } else {
      console.log("uncheck66ed", checkbox);
      let index = this.removeCheckedFromArray(checkbox);
      this.checkedArray.splice(index, 1);
      console.log("unchecked", event._value);
    }
    // console.log("checked", event._value);
  }

  removeCheckedFromArray(checkbox: Object) {
    console.log("Remove checkbox", checkbox);
    return this.checkedArray.findIndex((category) => {
      // console.log("Remove category",category);
      // console.log("Remove checkbox",checkbox);
      return category === checkbox;
    })
  }

  resendT1() {
    //Do whatever
    let env = this;

    if (this.checkedArray.length > 0) {

      var obj2 = {
        "SendEmail": this.checkedArray
      };
      // var obj2 = this.checkedArray;
      var obj = Object.assign(obj2);
      this.t1resend(obj2);
      console.log("finalArraybody99", obj2);
      // this.assignExisting(obj);
    } else {
      env.presentAlert("Please select Items");
    }
    // console.log("finalArray",obj);
  }

  t1resend(obj) {
    let env = this;
    this.auth.showLoading();
    console.log("assignExisting", obj);
    this.auth
      .webSupportResend(obj)
      .then(
        res => {
          console.log("checkIn response", res);
          this.checkedArray = [];
          if (res["Code"] == 200) {
            env.presentAlert("Email has been sent successfully");
          } else if (res["Code"] == 400) {
            env.presentAlert("Sending failed");
            // this.GetScannedUserList();
          } else if (res["Code"] == 500) {
            env.presentAlert(res["Msg"]);
            // this.GetScannedUserList();
          } else {
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

  onStatusGet() {
    let env = this;

    if (this.checkedArray.length > 0 && this.checkedArray.length < 2) {

      // var obj2 = {
      //   "SendEmail": this.checkedArray
      // };
      var obj2 = this.checkedArray;
      // var obj = Object.assign(obj2);
      // this.t1resend(obj);
      console.log("this.checkedArray", this.checkedArray);
      console.log("this.checkedArrayTransactionId", this.checkedArray[0].TransactionId);
      this.getStatust1(this.checkedArray[0].TransactionId);
    } else {
      env.presentAlertStatus("Please select single item at once.");
    }
  }
  getStatust1(transactionId) {
    let env = this;
    this.auth.showLoading();
    console.log("assignExisting", transactionId);
    this.auth
      .GetOrgStatus(transactionId)
      .then(
        res => {
          console.log("checkIn response", res);
          this.checkedArray = [];
          if (res["Code"] == 200) {
            env.presentAlertRes(res["Msg"],res["Data"]);
          } else if (res["Code"] == 400) {
            env.presentAlertRes(res["Msg"],res["Data"]);
            // this.GetScannedUserList();
          } else if (res["Code"] == 402) {
            env.presentAlertRes(res["Msg"],res["Data"]);
            // this.GetScannedUserList();
          } else if (res["Code"] == 500) {
            env.presentAlertRes(res["Msg"],res["Data"]);
            // this.GetScannedUserList();
          } else {
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
          this.GetWebLists();
          this.GetUssdLists();
        }
      },],
      cssClass: "SimpleAlert",
    });
    alert.present();
  }

  presentAlertStatus(error: any) {
    let alert = this.alertCtrl.create({
      subTitle: error,
      buttons: [{
        text: 'OK',
        role: 'OK',
        cssClass: 'cancel-btn',
        handler: data => {
          console.log('Cancel clicked');
        }
      },],
      cssClass: "SimpleAlert",
    });
    alert.present();
  }


  presentAlertRes(msg: any,success: any) {
    let alert = this.alertCtrl.create({
      title: "", message: '<table class="table-ui">' +
        '<tr><th>Message:</th></tr><tr><td>' + msg + '</td></tr><tr><th>Status:</th></tr><tr><td>'+success+'</td></tr>' +
        '</table>',buttons: [{
          text: 'OK',
          role: 'OK',
          cssClass: 'cancel-btn',
          handler: data => {
            console.log('Cancel clicked');
            this.GetWebLists();
          }
        },],
        cssClass: "myalert",
    }).present();
  }

  GetUssdLists() {
    this.auth.showLoading();
    //this.Event_Id = navParams.data["Event_Id"];this.Event_Id,this.userid  EventId=1446&&userid=15658
    this.auth.UssdChannel(this.Event_Id).subscribe(x => {
      if (x.length === 0) {
        // array empty or does not exist
        this.auth.dismissLoading();
        this.checknullUssd = false;

      } else {

        this.UssdListData = x;
        this.UssdsearchData = x;
        this.auth.dismissLoading();
        this.checknullUssd = true;
      }
      // this.ListData=(x);
      // this.allGroups=(x);
      console.log("GetUssdLists", this.UssdListData);

    }
    )
  }

  OnsearchUssd() {
    this.UssdListData = this.UssdsearchData.filter((notChunk) => {
      return (
        notChunk.TicketCode.toLowerCase().indexOf(this.Ussdsearch.toLowerCase()) > -1 || notChunk.Mobile.toLowerCase().indexOf(this.Ussdsearch.toLowerCase()) > -1
      );
    });
  }

  UssdaddCheckbox(event, checkbox) {
    // this.transactionId=checkbox.TransactionId;
    console.log("click", checkbox);


    if (event._value) {
      // var obj1 = {
      //   "email": checkbox.Email,
      //   "name": checkbox.Name,
      //   "barcode": checkbox.Barcode,
      //   "tickusermapid": checkbox.Tickusermapid,
      //   "ticketid": checkbox.TicketId,
      //   "ticketcreatedat": checkbox.TicketCreatedAt,
      //   "TransactionId":checkbox.TransactionId,
      // };
      this.checkedArray.push(checkbox);
      console.log("check66edUssd", this.checkedArray);
    } else {
      console.log("uncheck66edUssd", checkbox);
      let index = this.UssdremoveCheckedFromArray(checkbox);
      this.checkedArray.splice(index, 1);
      console.log("uncheckedUssd", event._value);
    }
    // console.log("checked", event._value);
  }

  UssdremoveCheckedFromArray(checkbox: Object) {
    console.log("Ussd Remove checkbox", checkbox);
    return this.checkedArray.findIndex((category) => {
      // console.log("Remove category",category);
      // console.log("Remove checkbox",checkbox);
      return category === checkbox;
    })
  }

  t2(){
    let env = this;

    if (this.checkedArray.length > 0) {

      var obj2 = {
        "Ev_id": this.Event_Id,
        "SMS": this.checkedArray
      };
      // var obj2 = this.checkedArray;
      var obj = Object.assign(obj2);
      // this.t1resend(obj);
      console.log("Ussd finalArraybody", obj2);

      this.auth.showLoading();
      console.log("assignExisting", obj2);
      this.auth
        .UssdSupportResendSMS(obj2)
        .then(
          res => {
            console.log("checkIn response", res);
            this.checkedArray = [];
            if (res["Code"] == 200) {
              env.presentAlert("SMS has been send successfully");
            } else if (res["Code"] == 400) {
              env.presentAlert("SMS has not been send successfully");
              // this.GetScannedUserList();
            } else if (res["Code"] == 500) {
              env.presentAlert(res["Msg"]);
              // this.GetScannedUserList();
            } else {
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
      // this.assignExisting(obj);
    } else {
      env.presentAlert("Please select Items");
    }
  }

  //   public donutChartData = [{
  //   id: 1,
  //   label: 'water',
  //   value: 20,
  //   color: 'red',
  // }];
  // this.alertCtrl.create({title:"USSID",message: '<table class="table-ui">' +
  // '<tr><th>Ticket Codes</th><td>"'+this.CustomCode+'"</td></tr><tr><th>Event</th><td>-</td></tr><tr><th>Ticket Name</th><td>-</td></tr><tr><th>Price</th><td>-</td></tr><tr><th>Date</th><td>-</td></tr>' +
  // '</table>',buttons: [{text:"Ok",role:"cancel"}]}).present();

}
import { loginRq } from './../../providers/custom-class/custom-class';
import { ModalpopupPage } from './../modalpopup/modalpopup';

import { GlobalFunctionProvider } from './../../providers/global-function/global-function';
import { Component } from '@angular/core';
import { NavController, NavParams, ModalController, AlertController } from 'ionic-angular';
import { ScanTicketPage } from '../scan-ticket/scan-ticket';
import { AuthProvider } from '../../providers/auth/auth';
import { Eventsdata } from '../../providers/custom-class/custom-class';
import { ScannedTicketListPage } from '../scanned-ticket-list/scanned-ticket-list';
//import { SQLite, SQLiteObject } from '@ionic-native/sqlite';
import { Network } from '@ionic-native/network';
import { DataBaseProvider } from '../../providers/data-base/data-base';
import { TicketsPage } from '../tickets/tickets';
import { HttpClient } from '@angular/common/http';
import { OrganizerHome } from '../organiser-home/organiser-home';
import { OrganizerEventDetails } from '../organiser-event-details/organiser-event-details';
import { ScannerEventDetailsPage } from '../scanner-event-details/scanner-event-details';


@Component({
  selector: 'page-home',
  templateUrl: 'home.html'
})
export class HomePage {

  user_id: any;
  EventId:any;
  loginRes:any;
  Detail:any;
  EventList:Eventsdata[];
  prefix:any;
  Type:any;
  not=0;
  CustomCode="";//"STANDARD-AA50504";
  checkCode():void{
    console.log(this.CustomCode);
    // this.http.post("http://151.106.41.94:88/api/ScanNPassAPI/ValidateTicketUSSD",{
    this.http.post("https://veetickets.com/api/ScanNPassAPI/ValidateTicketUSSD",{
      TicketCode:this.CustomCode,
      scannerid:this.user_id,
      EventId:this.EventId
    }).subscribe(r=>{
      console.log(r);
      // debugger;
      // var ticketcode=r['TICKET_CODES'].split('-');
      // console.log("Home response",ticketcode);
      // console.log("r['TicketCode']",r['TicketCode']);
      // if(r['TicketCode'] && r['TicketCode'].replace("-","")==this.CustomCode){
        if(r){
        // var ticketcode=r['TicketCode'].split('-');
      // console.log("Home response",ticketcode);
      console.log("r['TicketCode']",r['TicketCode']);
      if(r['TicketCode']==this.CustomCode){
        if(r["IsCheckIn"]==false || r["IsCheckIn"]==null ){
          // if(r){
          // if(r['TicketCode']==this.CustomCode){
      // this.alertCtrl.create({title:"USSID",message:"Ticket is valid.",buttons: [{text:"Ok",role:"cancel"}]}).present();
      this.alertCtrl.create({title:"USSID",message: '<table class="table-ui">' +
      '<tr><th>Ticket Codes</th><td>"'+r['TICKET_CODES']+'"</td></tr><tr><th>Ticket Date</th><td>"'+r['TicketDate']+'"</td></tr><tr><th>Ticket Name</th><td>"'+r['TICKET_NAME']+'"</td></tr><tr><th>Amount</th><td>"'+r['ActualAmount']+'"</td></tr>' +
      '</table>',buttons: [{text:"Ok",role:"cancel"}]}).present();
          }else {
            // this.alertCtrl.create({title:"USSID",message:"Ticket already Scanned",buttons: [{text:"Ok",role:"cancel"}]}).present();
            this.alertCtrl.create({title:"USSID",message: '<p>Ticket already Scanned</p><table class="table-ui">' +
      '<tr><th>Ticket Codes</th><td>"'+r['TICKET_CODES']+'"</td></tr><tr><th>Ticket Date</th><td>"'+r['TicketDate']+'"</td></tr><tr><th>Ticket Name</th><td>"'+r['TICKET_NAME']+'"</td></tr><tr><th>Amount</th><td>"'+r['ActualAmount']+'"</td></tr>' +
      '</table>',buttons: [{text:"Ok",role:"cancel"}]}).present();
          }
      // this.showModal();
          }else{
            this.alertCtrl.create({title:"USSID",message: '<p>Ticket already Scanned</p><table class="table-ui">' +
            '<tr><th>Ticket Codes</th><td>"'+r['TICKET_CODES']+'"</td></tr><tr><th>Ticket Date</th><td>"'+r['TicketDate']+'"</td></tr><tr><th>Ticket Name</th><td>"'+r['TICKET_NAME']+'"</td></tr><tr><th>Amount</th><td>"'+r['ActualAmount']+'"</td></tr>' +
            '</table>',buttons: [{text:"Ok",role:"cancel"}]}).present();
            // this.alertCtrl.create({title:"USSID",message:"Invalid code. Please try again",buttons: [{text:"Ok",role:"cancel"}]}).present();
             // this.showModal();
          }
    }else{
      // this.alertCtrl.create({title:"USSID",message: '<table class="table-ui">' +
      // '<tr><th>Ticket Codes</th><td>"'+this.CustomCode+'"</td></tr><tr><th>EventDate</th><td>-</td></tr><tr><th>Ticket Name</th><td>-</td></tr><tr><th>Price</th><td>-</td></tr><tr><th>Date</th><td>-</td></tr>' +
      // '</table>',buttons: [{text:"Ok",role:"cancel"}]}).present();
        this.alertCtrl.create({title:"USSID",message:"Invalid code. Please try again",buttons: [{text:"Ok",role:"cancel"}]}).present();
        // this.showModal();
      }
    },e=>{
      console.log(e);
    })
  }

  constructor(public navCtrl: NavController,
    public navParams: NavParams,
    public auth:AuthProvider,
    public modal:ModalController,
    private network: Network,
    private http:HttpClient,
    private globalProvider:GlobalFunctionProvider,
    private database: DataBaseProvider,
    private alertCtrl:AlertController,
    ) {

    if(localStorage.getItem("Organizer")=="Organizer"){
      console.log("Organizer",localStorage.getItem("EventId"));
      this.EventId = localStorage.getItem("EventId");
      this.loginRes=JSON.parse(localStorage.getItem("LoginRes"));
      console.log("Home response organis",this.loginRes);
      // console.log("Home CompanyId",this.loginRes.CompanyId);
      this.user_id = this.loginRes.org_scanid;
      this.Type = "0";
      
     
    }else{
    // this.user_id = this.auth.authUser["Id"];
    // this.Type = this.auth.authUser["Type"];
    this.EventId = localStorage.getItem("EventId");
    this.loginRes=JSON.parse(localStorage.getItem("LoginRes"));
    console.log("this.EventId",this.EventId);
    console.log("this.loginRes",this.loginRes);
    this.user_id = this.loginRes.Id;
    this.Type = "0";
    }

    console.log("UserId org_scanid", this.user_id);
    console.log("Type", this.Type);

  }

  async showModal() {  
    // const modal = await this.modal.create({  
    //   component: ModalpopupPage  
    // });  
    // return await modal.present();  
  } 

  ionViewDidEnter(){
    if(this.EventList==[] || (this.EventList && this.EventList.length<=0) ){
      this.doRefresh('',0);
    }

  }

  ngOnInit()
  {
    if (this.network.type !== 'none') {
      this.GetEventListData();
    } else
    {
      this.ReadDataInSqlLight();
    }
    //this.ReadDataInSqlLight();

  //
  }

   ticket(data: any)
   {
    // this.navCtrl.push(TicketsPage,{event_id:325,barcodeValue:579264869});
    // debugger;
    this.navCtrl.push(ScanTicketPage, { Id: data.Id });
  }

  GetEventListData()
  {
    // debugger;
    this.auth.GetEventsList(this.user_id,this.Type,this.EventId).subscribe(x=>{
      console.log(x);

      if(x)
      {
        this.EventList=x;
        this.EventList.forEach(m=>{
          // alert(JSON.stringify(m));
			// debugger;
             m["imagePath"] = m["imagePath"];
        });

        // this.globalProvider.presentAlert(this.EventList,1,"ok");
      }
      else
      {
        this.EventList=[];
      }
    })
  }

  getItems(Event:any)
  {
    debugger;
    var val = Event.value;

    if(val.trim()!="")
    {
      this.prefix=val
      this.auth.GetEventSearchList(this.user_id, this.prefix,this.Type).subscribe(x=>{
        if(x)
        {
          this.EventList=x;
          this.EventList.forEach(m=>{
              m["imagePath"] = m["imagePath"];
          });
        }
        else
        {
          this.EventList=[];
        }
      })
    }
    else
    {
      this.prefix=val
      this.GetEventListData();
    }

  }


  listdata(data:any)
  {
    console.log("this.EventId",data);
    // this.globalProvider.presentAlert(data,1);
    debugger;
    this.navCtrl.push(ScannedTicketListPage, {
      Id: data.Id,
      Name: data.EventName,
    });

  }

  letstringlist:any;
  ReadDataInSqlLight()  {
    debugger;
    var db = null;
    this.EventList=[];
    this.database.getEventList().then(x=>{
      // debugger;
      var local_list=[];
      this.letstringlist=x;
      // console.log(x);
      // this.globalProvider.presentAlert(x,1);
      if(this.letstringlist.length>0)      {
        this.letstringlist.forEach(x => {
           //   this.updatedatalist.push({ "event_id": result.rows.item(m).Event_Id, "event_Name": result.rows.item(m).Event_Name, "city_Name": result.rows.item(m).City_Name })
              let Event_id=+ x["event_id"];
              local_list.push({
                Id:Event_id,
                cityName:x.city_Name,
                endDate:x.end_Date,
                EventName:x.event_Name,
                imagePath:"assets/imgs/dummy.jpg",
                startDate:x.start_Date,
                venue:x.event_Venue })
        });
        this.EventList=local_list;
      }

    })
   }

   doRefresh(refresher,flag=1)   {
    if (this.network.type !== 'none') {
      this.GetEventListData();
    } else     {
		// debugger;
      this.ReadDataInSqlLight();
    }
    if(flag)
      setTimeout(() => {
        refresher.complete();
      }, 2000);
    }

    back(){
      // this.navCtrl.pop();
      if(localStorage.getItem("Organizer")=="Organizer"){
        console.log("Organizer",localStorage.getItem("EventId"));
        // this.navCtrl.push(OrganizerEventDetails);
        this.navCtrl.pop();
        
       
      }else{
        // this.navCtrl.push(ScannerEventDetailsPage);
        this.navCtrl.pop();
      }
    }

}

import { Component } from '@angular/core';
import { NavController, NavParams } from 'ionic-angular';
import { scanedList } from '../../providers/custom-class/custom-class';
import { AuthProvider } from '../../providers/auth/auth';
import { Network } from '@ionic-native/network';
import { DataBaseProvider } from '../../providers/data-base/data-base';


@Component({
  selector: 'page-scanned-ticket-list',
  templateUrl: 'scanned-ticket-list.html',
})
export class ScannedTicketListPage {

  Event_id:any;
  lsitData:scanedList[]=[];
  Event_Name:any;
  constructor(public navCtrl: NavController,
     public navParams: NavParams,
     public auth:AuthProvider,
     private network: Network,
     private database: DataBaseProvider) {
      this.Event_id = navParams.data["Id"];
      this.Event_Name = navParams.data["Name"];
      // alert(this.Event_id+"  -  "+this.Event_Name);
  }

  ionViewDidLoad() {
	  // debugger;
    if (this.network.type !== 'none') {
      // alert("server db");
      this.GetScannedUserList();
    } else 
    {
      // alert("local db");
      this.GetScanListInLDb();
    }

   // this.GetScannedUserList();
  }

  GetScannedUserList()
  {
    this.lsitData=[];
    this.auth.GetScanedList(this.Event_id).subscribe(x=>
      {
        // alert(JSON.stringify(x));
        this.lsitData=x;
      })
  }

  listLdata:any;
  GetScanListInLDb() {
    this.database.getScanList(this.Event_id).then(x=>{
      this.listLdata=x;
      if(this.listLdata.length>0) {
        this.listLdata.forEach(x => {
          this.lsitData.push({
            Id : +x.ticket_Id,
            TicketName  :x.ticket_Name,
            Name :x.ticket_buyer_name,
            Eventname :x.event_Name,
            TicketbuyerEmail :x.email,
            CheckInStatus:x.isCheckIn,
            Startdate:x.start_Date,
            Venuename :x.event_Venue,
          })
          
        });
      }
    })
  }

  back(){
    this.navCtrl.pop();
  }

}

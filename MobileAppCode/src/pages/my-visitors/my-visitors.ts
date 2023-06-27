import { Component } from '@angular/core';
import { IonicPage, NavController, NavParams } from 'ionic-angular';
import { scanedList } from '../../providers/custom-class/custom-class';
import { Network } from '@ionic-native/network';
import { AuthProvider } from '../../providers/auth/auth';
import { DataBaseProvider } from '../../providers/data-base/data-base';

/**
 * Generated class for the MyVisitorsPage page.
 *
 * See https://ionicframework.com/docs/components/#navigation for more info on
 * Ionic pages and navigation.
 */

@IonicPage()
@Component({
  selector: 'page-my-visitors',
  templateUrl: 'my-visitors.html',
})
export class MyVisitorsPage {

  Event_id:any;
  lsitData:scanedList[]=[];
  Event_Name:any;
  ScannerId:any;
  constructor(public navCtrl: NavController,
     public navParams: NavParams,
     public auth:AuthProvider,
     private network: Network,
     private database: DataBaseProvider) {
      this.Event_id = navParams.data["Id"];
      this.Event_Name = navParams.data["Name"];
      this.ScannerId = navParams.data["ScannerId"];
      // alert(this.Event_id+"  -  "+this.Event_Name);ScannerId
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
    this.auth.ScannedTicketListByScanner(this.Event_id,this.ScannerId).subscribe(x=>
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

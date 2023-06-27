import { ScannerEventDetailsPage } from './../scanner-event-details/scanner-event-details';
import { TabsPage } from './../tabs/tabs';
import { OrganizerEventDetails } from './../organiser-event-details/organiser-event-details';
import { Component, OnInit } from '@angular/core';
import { NavController } from 'ionic-angular';
import { SidebarComponent } from '../../components/sidebar/sidebar';
import { LiveList, scanedList } from '../../providers/custom-class/custom-class';
import { AuthProvider } from '../../providers/auth/auth';
import { Network } from '@ionic-native/network';
import { LoadingController } from 'ionic-angular';
import { CustomLoading } from '../../providers/custom-class/custom-class';
@Component({
  selector: 'organiser-home',
  templateUrl: 'organiser-home.html',
})
export class OrganizerHome implements OnInit {
  i_tab = 't1';
  segmentSelect: string = "t1";
  loc: any;
  listData: LiveList[] = [];
  alllist: LiveList[] = [];

  pastListData: LiveList[] = [];
  pastAllList: LiveList[] = [];

  loginRes: any;
  search: any;
  searchPast: any;
  checknull: any;
  pastchecknull:any;
  constructor(public navCtrl: NavController, public auth: AuthProvider, private network: Network, private loadingCtrl: LoadingController) {
    // this.segmentSelect = "t1";
    this.loc = localStorage.getItem("Organizer");
    this.loginRes = JSON.parse(localStorage.getItem("LoginRes"));
    console.log("Home response", this.loginRes);
    console.log("Home EmailId", this.loginRes.EmailId);
  }

  ngOnInit() {
    if (!localStorage.getItem('foo')) {
      localStorage.setItem('foo', 'no reload')
      location.reload();
      // this.navCtrl.push(OrganizerHome); 
    } else {
      // localStorage.removeItem('foo') 
    }
  }

  ionViewDidLoad() {
    // debugger;
    if (this.network.type !== 'none') {
      // alert("server db");
      this.GetScannedUserList();
    } else {
      // alert("local db");
      console.log("no network");
    }
  }


  GetScannedUserList() {
    this.listData = [];
    this.auth.showLoading();
    this.loc = localStorage.getItem("Organizer"); //this.Event_Id = navParams.data["Event_Id"];
    // if(this.loc=="Organizer" ||this.loc=="Event" ){
    if (this.loc == "Organizer") {
      console.log("Organizer");
      this.auth.GetLiveEventsList(this.loginRes.EmailId, 0).subscribe(x => {
        this.auth.dismissLoading();
        // this.loading.dismissLoading();
        // alert(JSON.stringify(x));
        if (x.length === 0) {
          // array empty or does not exist
          this.checknull = false;

        } else {

          this.listData = x;
          this.alllist = x;
          this.checknull = true;
        }

      })

      this.auth.GetPastEventsList(this.loginRes.EmailId, 0).subscribe(x => {
        this.auth.dismissLoading();
        // alert(JSON.stringify(x));
        if (x.length === 0) {
          // array empty or does not exist

          this.pastchecknull = false;

        } else {

        this.pastListData = x;
        this.pastAllList = x;
        this.pastchecknull = true;
        }
        
      })

    } else {
      console.log("Event");
      // console.log("Organizer");
      this.auth.GetLiveEventsList(this.loginRes.EmailId, 1).subscribe(x => {
        this.auth.dismissLoading();
        // this.loading.dismissLoading();
        // alert(JSON.stringify(x));
        this.listData = x;
        this.alllist = x;
      })

      this.auth.GetPastEventsList(this.loginRes.EmailId, 1).subscribe(x => {
        this.auth.dismissLoading();
        // alert(JSON.stringify(x));
        this.pastListData = x;
        this.pastAllList = x;
      })
    }
    this.auth.dismissLoading();

  }


  Onsearch() {
    this.listData = this.alllist.filter((notChunk) => {
      return (
        notChunk.EventName.toLowerCase().indexOf(this.search.toLowerCase()) > -1
      );
    });
  }

  OnsearchPast() {
    // console.log(this.searchPast);
    this.pastListData = this.pastAllList.filter((notChunk) => {
      return (
        notChunk.EventName.toLowerCase().indexOf(this.searchPast.toLowerCase()) > -1
      );
    });
  }

  segmentChanged(ev: any) {
    console.log('Segment changed', ev);
    //   console.log("okk",ev.detail.value);
    this.i_tab = ev._value;
  }

  eventDetails(ev: any) {
    console.log('Segment changed', ev);
    this.loc = localStorage.getItem("Organizer"); //this.Event_Id = navParams.data["Event_Id"];
    if (this.loc == "Organizer") {
      console.log('Segment Organizer', ev.EventId);
      this.navCtrl.push(OrganizerEventDetails, { Event_Id: ev.EventId, EventName: ev.EventName });
    } else {
      console.log('Segment Organizer', ev.EventId);
      this.navCtrl.push(ScannerEventDetailsPage, { Event_Id: ev.EventId, EventName: ev.EventName });
    }

  }

  showLoading() {
    const loading = this.loadingCtrl.create({
      // message:"Loading",
      duration: 2000,
    });

    loading.present();
  }

  doRefresh(event) {  
    console.log('Pull Event',event); 
    if (event){
      if (this.network.type !== 'none') {
        // alert("server db");
        this.GetScannedUserList();
      } else {
        // alert("local db");
        console.log("no network");
      }
      event.complete();
    }else{
      console.log('Pull Event Triggered!'); 
    }
     
  }  

  //   public donutChartData = [{
  //   id: 1,
  //   label: 'water',
  //   value: 20,
  //   color: 'red',
  // }];

}
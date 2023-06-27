import { DateTime } from "ionic-angular";
import { LoadingController } from "ionic-angular";

export class loginRq {
  email: string;
  password: string;
}

export class Eventsdata{
  Id:number;
  cityName:string;
  endDate:any;
  EventName:string;
  imagePath:string
  startDate:any;
  venue:any;
}

export class Barcdoe{
  EventId:any;
  barcode:string;
  scannerid:any;
  
}


export class accessdata{
  Barcode:string;
  CityName:string;
  ColorArea:string;
  EndDate:string;
  EventName:string;
  GateNo:string;
  Id:number;
  ImagePath:"/assets/imgs/dummy.jpg";
  Quantity:number;
  StartDate:string;
  TicketName:string;
  Venue:string;
  BarcodeImageP:"/assets/imgs/qrcode.jpg";
  Sheet_No :string;
  Name:string;
}

export class scanedList{
   Id :number;
   TicketName  :string;
   Name :string;
   Eventname :string;
   TicketbuyerEmail :string;
   CheckInStatus:boolean;
   Startdate:DateTime;
   Venuename :string;
}
export class LiveList{
  Id :number;
  TicketName  :string;
  Name :string;
  EventName :string;
  TicketbuyerEmail :string;
  CheckInStatus:boolean;
  Startdate:DateTime;
  Venuename :string;
  Email :string;
}

export class LocalData{
    Event_Id:number;
    Event_Name :string;
    City_Name :string;
    Start_Date :DateTime;
    End_Date :DateTime;
    Event_Venue :string;
    Ticket_Id :number;
    Ticket_User_name:string;
    ticket_Barcode:string; 
    ticket_Area :string;
    Ticket_Name:string;
    Gate_No :string;
    Quantity :number;
    IsCheckIn :Boolean;
    Email :string;
    Ticket_buyer_name :string;
    Table_No :string;
    Sheet_No :string;
    updateOn :number=0;
    updateindb :number=0;
}

export class CustomLoading{
  constructor(private loadingCtrl: LoadingController ) { 
    // this.segmentSelect = "t1";
    // this.showLoading();
  } 
  showLoading() {
    const loading =  this.loadingCtrl.create({
      content:"Loading",
      // duration: 2000,
    });

    loading.present();
  }

  dismissLoading() {
    // const dismis =  this.loadingCtrl.create({
    //   // message:"Loading",
    //   dismissOnPageChange: true,
    // });

    // dismis.dismiss();
  }
}


import { AlertController, LoadingController, Toast, ToastController } from 'ionic-angular';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

@Injectable()
export class GlobalFunctionProvider {
  public databaseLoaded=0;
  public tableLoaded=0;
  public loader=undefined;
  public user_detail=undefined;
  // public api_url = "https://www.ticketsandinvites.com/api/ScanNPassAPI/";
  public api_url = 'https://www.veetickets.com/api/ScanNPassAPI/';
  constructor(
    public http: HttpClient,
    public alertCtrl:AlertController,
    public loadingCtrl : LoadingController,
    public toastCtrl:ToastController,
    ) {
  }

  start_loading(title:String = "Loading data"):void{
    this.loader = this.loadingCtrl.create({
      content:""+title,
      duration: 2000000,
    });
    // this.loader.present();
  }

  stop_loading(after:number = 10):void{
    setTimeout(() => {
      try {
        this.loader.dismiss();
      } catch (error) {
        console.log("Error in dismiss");
      }
    }, after);
  }

  presentAlert( alertMessage:any, parseToJson=0 , alertTitle:string ="Alert") {
    let alert = this.alertCtrl.create({
      title: alertTitle,
      subTitle: (parseToJson)? JSON.stringify(alertMessage):alertMessage,
      buttons: ['Ok']
    });
    alert.present();
  }

  presentToast(title:String="Successful",dur:number = 200,) {
    let toast = this.toastCtrl.create({
      message: ""+title,
      duration: dur
    });
    // toast.present();
  }

}

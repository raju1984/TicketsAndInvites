import { LoadingOptions } from './../../../node_modules/ionic-angular/umd/components/loading/loading-options.d';
import { GlobalFunctionProvider } from './../global-function/global-function';
import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs/Observable";
import { Storage } from "@ionic/storage";
import "rxjs/add/operator/catch";
import "rxjs/add/operator/map";
import { Barcdoe } from "../custom-class/custom-class";
import { LoadingController } from 'ionic-angular';

/*
  Generated class for the AuthProvider provider.

  See https://angular.io/guide/dependency-injection for more info on providers
  and Angular DI.
*/
@Injectable()
export class AuthProvider {
  // Variable Declared in it
  // ApiUrl: string = "http://localhost:8080/adequate/blog/LaravelApi/public/api/";
  ApiUrl: string;
  authUser: object = {};
  rootPage: any;
  imageacces: any;
  loading: any;
  constructor(public http: HttpClient, private storage: Storage,
    private globalProvider : GlobalFunctionProvider,
    private loadingCtrl: LoadingController) {
    let env = this;

    // For LocalDatabase
    //  this.ApiUrl="http://localhost:62110/api/ScanNPassAPI/"
    //  this.imageacces="http://localhost:62110";

    // For Development Env
    //   this.ApiUrl="http://192.168.1.101:84/api/ScanNPassAPI/"
    //  this.imageacces="http://192.168.1.101:84";

    // for out side Link
    this.ApiUrl="http://151.106.41.94:88/api/ScanNPassAPI/";
     this.imageacces="http://151.106.41.94:88";

    // For live Server
    // this.ApiUrl = "https://www.veetickets.com/api/ScanNPassAPI/";
    // this.imageacces = "https://www.veetickets.com";

    // Check Store value in this in our gest users
    this.storage
      .get("guser")
      .then((rt) => {
        console.log(rt);
        env.authUser = rt;
      })
      .catch((e) => {
        // For Error Log When some Exception Occured
        let date = new Date().toISOString().slice(0, 19).replace("T", " ");
        let req = {
          err_text: JSON.stringify(e) + "Line No 58",
          file_path: "auth.ts",
          method: "constructor",
          parent_method: "0",
          error_time: date,
          type: "mobile",
        };
        this.errorLog(req).subscribe();
      });
  }

  // This code is used for error log on server
  errorLog(values: any) {
    return this.http
      .post(this.ApiUrl + "error_log", values)
      .map((response: any) => {});
  }

  // It is colling when user manual login
  doLogin(postData: object) {
    let env = this;
    return new Promise((resolve, reject) => {
      this.http.post(this.ApiUrl + "login", postData, {}).subscribe(
        (data) => {

          console.log("Login data : ",data);

          if (data["status"] == "error") {
            reject(data["data"]);
          } else {
            env.storage.set("guser", data["results"]);
            this.globalProvider.user_detail=data["results"];
            env.authUser = data["results"];
            resolve(data);
          }
        },
        (error) => {
          reject(error);
        }
      );
    });
  }

  doLoginOrganizer(postData: object) {
    let env = this;
    return new Promise((resolve, reject) => {
      this.http.post(this.ApiUrl + "loginOrganiser", postData, {}).subscribe(
        (data) => {

          console.log("Login data : ",data);

          if (data["status"] == "error") {
            reject(data["data"]);
          } else {
            env.storage.set("guser", data["results"]);
            this.globalProvider.user_detail=data["results"];
            env.authUser = data["results"];
            resolve(data);
          }
        },
        (error) => {
          reject(error);
        }
      );
    });
  }
  // Set PlayerId as User id In one Signal
  public setDateTime(Id: any) {
    this.storage.set("DateTime ", Id);
  }

  // Get PlayerId as User id In one Signal
  public getDateTime() {
    return this.storage.get("DateTime");
  }

  // Clear storage function when logout users
  public clear() {
    this.storage.clear().then(() => {
      this.authUser = null;
    });
  }

  // Get Event Detail
  GetEventsList(id: any, type: any,EventId:any): Observable<any> {
    // debugger;
    console.log(this.ApiUrl + "Events?id=" + id + "&Type=" + type +"&EventId"+EventId);
    return this.http
      .get(this.ApiUrl + "Events?id=" + id + "&Type=" + type +"&EventId="+EventId)
      .map((response: any) => {
        console.log(response);
        // this.globalProvider.presentAlert(response,1);
        return <string>response;
      });
  }

  GetLiveEventsList(email: any,type:any): Observable<any> {
    // debugger;
    console.log(this.ApiUrl + "LiveEvents?Email=" + email+"&&Type="+ type);
    return this.http
      .get(this.ApiUrl + "LiveEvents?Email=" + email +"&&Type="+ type)
      .map((response: any) => {
        console.log(response);
        // this.globalProvider.presentAlert(response,1);
        return <string>response;
      });
  }
  GetExistingUserList(id: any): Observable<any> {
    // debugger;
    console.log(this.ApiUrl + "GetScannerList?Eventid=" + id);
    return this.http
      .get(this.ApiUrl + "GetScannerList?Eventid=" + id )
      .map((response: any) => {
        console.log(response);
        // this.globalProvider.presentAlert(response,1);
        return <string>response;
      });
  }

  GetAssignedScannerList(id: any): Observable<any> {
    // debugger;
    console.log(this.ApiUrl + "GetAssignedScannerList?Eventid=" + id);
    return this.http
      .get(this.ApiUrl + "GetAssignedScannerList?Eventid=" + id )
      .map((response: any) => {
        console.log(response);
        // this.globalProvider.presentAlert(response,1);
        return <string>response;
      });
  }

  GetPastEventsList(email: any,type:any): Observable<any> {
    // debugger;
    console.log(this.ApiUrl + "PastEvents?Email=" + email+"&&Type="+ type);
    return this.http
      .get(this.ApiUrl + "PastEvents?Email=" + email+"&&Type="+ type )
      .map((response: any) => {
        console.log(response);
        // this.globalProvider.presentAlert(response,1);
        return <string>response;
      });
  }

  GetEventsListDetails(id: any): Observable<any> {
    // debugger;
    console.log(this.ApiUrl + "GetEventDetails?EventId=" + id);
    return this.http
      .get(this.ApiUrl + "GetEventDetails?EventId=" + id)
      .map((response: any) => {
        console.log(response);
        // this.globalProvider.presentAlert(response,1);
        return <string>response;
      });
  }

  GetScannerEventsListDetails(id: any,eventId:any): Observable<any> {
    // debugger;
    console.log(this.ApiUrl + "MyScanActivity?ScanId=" + id+"&&EventId="+eventId);
    return this.http
      .get(this.ApiUrl + "MyScanActivity?ScanId=" + id+"&&EventId="+eventId)
      .map((response: any) => {
        console.log(response);
        // this.globalProvider.presentAlert(response,1);
        return <string>response;
      });
  }

  GetCustomerList(id: any): Observable<any> {
    // debugger;
    console.log(this.ApiUrl + "GetCustomerList?EventId=" + id);
    return this.http
      .get(this.ApiUrl + "GetCustomerList?EventId=" + id)
      .map((response: any) => {
        console.log(response);
        // this.globalProvider.presentAlert(response,1);
        return <string>response;
      });
  }

  GetEventsListUserDetails(id: any, userid: any): Observable<any> {
    // debugger;
    console.log(this.ApiUrl + "GetUserDetails?EventId=" + id + "&&userid=" + userid);
    return this.http
      .get(this.ApiUrl + "GetUserDetails?EventId=" + id + "&&userid=" + userid)
      .map((response: any) => {
        console.log(response);
        // this.globalProvider.presentAlert(response,1);
        return <string>response;
      });
  }

  CheckedIn(id: any,scannerid:any) {
    // debugger;
    console.log(this.ApiUrl + "UpdateCheckInStatusManually?TicketmapId=" + id +"&&scannerid="+scannerid);
     let env = this;
    return new Promise((resolve, reject) => {
      this.http.post(this.ApiUrl + "UpdateCheckInStatusManually?TicketmapId=" + id +"&&scannerid="+scannerid, {}).subscribe(
        (data) => {

          console.log("Check data : ",data);
          resolve(data);

          // if (data["Code"] == '404') {
          //   console.log("if");
          //   reject(data["data"]);
          // } else {
          //   console.log("else");
          //   resolve(data);
          // }
        },
        (error) => {
          reject(error);
        }
      );
    });
  }

  assignExistingUser(postData: object) {
    // debugger;
    console.log(this.ApiUrl + "AssignedExistingUser");
     let env = this;
    return new Promise((resolve, reject) => {
      this.http.post(this.ApiUrl + "AssignedExistingUser",postData, {}).subscribe(
        (data) => {

          console.log("Check data : ",data);
          resolve(data);

          // if (data["Code"] == '404') {
          //   console.log("if");
          //   reject(data["data"]);
          // } else {
          //   console.log("else");
          //   resolve(data);
          // }
        },
        (error) => {
          reject(error);
        }
      );
    });
  }

  WebChannel(id: any): Observable<any> {
    // debugger;
    console.log(this.ApiUrl + "WebSupport?EventId=" + id);
    return this.http
      .get(this.ApiUrl + "WebSupport?EventId=" + id)
      .map((response: any) => {
        console.log(response);
        // this.globalProvider.presentAlert(response,1);
        return <string>response;
      });
  }

  UssdChannel(id: any): Observable<any> {
    // debugger; http://151.106.41.94:88/api/ScanNPassAPI/USSDSupport?Eventid=147
    console.log(this.ApiUrl + "USSDSupport?EventId=" + id);
    return this.http
      .get(this.ApiUrl + "USSDSupport?EventId=" + id)
      .map((response: any) => {
        console.log(response);
        // this.globalProvider.presentAlert(response,1);
        return <string>response;
      });
  }


  webSupportResend(postData: object) {
    // debugger;
    console.log(this.ApiUrl + "SendEmail");
     let env = this;
    return new Promise((resolve, reject) => {
      this.http.post(this.ApiUrl + "SendEmail",postData, {}).subscribe(
        (data) => {

          console.log("Check data : ",data);
          resolve(data);

          // if (data["Code"] == '404') {
          //   console.log("if");
          //   reject(data["data"]);
          // } else {
          //   console.log("else");
          //   resolve(data);
          // }
        },
        (error) => {
          reject(error);
        }
      );
    });
  }

  UssdSupportResendSMS(postData: object) {
    // debugger;
    console.log(this.ApiUrl + "ResendSMS");
     let env = this;
    return new Promise((resolve, reject) => {
      this.http.post(this.ApiUrl + "ResendSMS",postData, {}).subscribe(
        (data) => {

          console.log("Check data : ",data);
          resolve(data);

          // if (data["Code"] == '404') {
          //   console.log("if");
          //   reject(data["data"]);
          // } else {
          //   console.log("else");
          //   resolve(data);
          // }
        },
        (error) => {
          reject(error);
        }
      );
    });
  }

  GetOrgStatus(transactionId: any) {
    // debugger;
    console.log(this.ApiUrl + "GetOrgStatus"+"?reference="+transactionId);
     let env = this;
    return new Promise((resolve, reject) => {
      this.http.get(this.ApiUrl + "GetOrgStatus"+"?reference="+transactionId).subscribe(
        (data) => {

          console.log("Check data : ",data);
          resolve(data);
        },
        (error) => {
          reject(error);
        }
      );
    });
  }

  // get Event By Search
  GetEventSearchList(id: any, Prefix: any, type: any): Observable<any> {
    return this.http
      .get(
        this.ApiUrl + "Events?id=" + id + "&Event=" + Prefix + "&Type=" + type
      )
      .map((response: any) => {
        return <string>response;
      });
  }

  GetScanSummary(postData: object) {
    // debugger;
    console.log(this.ApiUrl + "ScanSummary");
     let env = this;
    return new Promise((resolve, reject) => {
      this.http.post(this.ApiUrl + "ScanSummary",postData, {}).subscribe(
        (data) => {

          console.log("Check data : ",data);
          resolve(data);

          // if (data["Code"] == '404') {
          //   console.log("if");
          //   reject(data["data"]);
          // } else {
          //   console.log("else");
          //   resolve(data);
          // }
        },
        (error) => {
          reject(error);
        }
      );
    });
  }
  WebSupportDetails(postData: object) {
    // debugger;
    console.log(this.ApiUrl + "WebSupportDetails");
     let env = this;
    return new Promise((resolve, reject) => {
      this.http.post(this.ApiUrl + "WebSupportDetails",postData, {}).subscribe(
        (data) => {

          console.log("Check data : ",data);
          resolve(data);

          // if (data["Code"] == '404') {
          //   console.log("if");
          //   reject(data["data"]);
          // } else {
          //   console.log("else");
          //   resolve(data);
          // }
        },
        (error) => {
          reject(error);
        }
      );
    });
  }

  // get Event By Search
  GetEventData(id: any): Observable<any> {
    return this.http
      .get(this.ApiUrl + "Geteventdetails?id=" + id)
      .map((response: any) => {
        return <string>response;
      });
  }

  GetTicketSales(id: any): Observable<any> {
    return this.http
      .get(this.ApiUrl + "GetTicketSales?EventId=" + id)
      .map((response: any) => {
        return <string>response;
      });
  }

  GetEventActivity(id: any): Observable<any> {
    return this.http
      .get(this.ApiUrl + "EventActivity?EventId=" + id)
      .map((response: any) => {
        return <string>response;
      });
  }

  // check validticket
  Getvalidticket(data: Barcdoe): Observable<any> {
    // this.globalProvider.presentAlert(data,1,"auth-143");
    debugger;
    return this.http
      .post(this.ApiUrl + "ValidateTicket", data)
      .map((response: any) => {
        return <string>response;
      });
  }

  // get Event By Search
  GetScanedList(id: any): Observable<any> {


    // alert(this.ApiUrl + "ScannedTicketList?Event_Id=" + id);
    return this.http
      .get(this.ApiUrl + "ScannedTicketList?Event_Id=" + id)
      .map((response: any) => {
        return <string>response;
      });
  }

  ScannedTicketListByScanner(id: any,ScannerId:any): Observable<any> {


    // alert(this.ApiUrl + "ScannedTicketList?Event_Id=" + id);
    return this.http
      .get(this.ApiUrl + "ScannedTicketListByScanner?Event_Id=" + id +"&Scannerid="+ScannerId)
      .map((response: any) => {
        return <string>response;
      });
  }

  // It is colling when user manual login
  doSocialLogin(postData: object) {
    let env = this;
    return new Promise((resolve, reject) => {
      this.http.post(this.ApiUrl + "Sociallogin", postData, {}).subscribe(
        (data) => {
          if (data["status"] == "error") {
            reject(data["data"]);
          } else {
            env.storage.set("guser", data["results"]);
            env.authUser = data["results"];
            resolve(data);
          }
        },
        (error) => {
          reject(error);
        }
      );
    });
  }

  // get Event By Search
  GetDataList(id: any, type: any, datetime: any): Observable<any> {
    // alert(this.ApiUrl +"getLocalData?id=" +id +"&type=" +type +"&time=" +datetime);
    return this.http.get(this.ApiUrl +"getLocalData?id=" +id +"&type=" +type +"&time=" +datetime).map((response: any) => {

      // alert("2");
        return <any>response;
      });
  }

  // get Event By Search
  postSaveDataList(Listdata: any): Observable<any> {
    return this.http
      .post(this.ApiUrl + "SavedataList", Listdata)
      .map((response: any) => {
        return <any>response;
      });
  }

  showLoading() {
    if(!this.loading){
      this.loading = this.loadingCtrl.create({
          content: 'Please Wait...'
      });
      this.loading.present();
  }
  }

  dismissLoading() {
    // const dismis =  this.loadingCtrl.create({
    //   // message:"Loading",
    //   dismissOnPageChange: true,
    // });

    if(this.loading){
      this.loading.dismiss();
      this.loading = null;
  }
    // this.loadingCtrl.
  }
}

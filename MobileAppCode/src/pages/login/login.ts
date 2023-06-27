import { GlobalFunctionProvider } from './../../providers/global-function/global-function';
import { Component, NgZone } from "@angular/core";
import {
  NavController,
  Platform,
  AlertController,
  ToastController,
} from "ionic-angular";
import { TabsPage } from "../tabs/tabs";
import { FormBuilder, FormControl, Validators } from "@angular/forms";
import { AuthProvider } from "../../providers/auth/auth";
import { GooglePlus } from "@ionic-native/google-plus";
import { Facebook, FacebookLoginResponse } from "@ionic-native/facebook";
//import { SQLite, SQLiteObject } from '@ionic-native/sqlite';
// Import Auth0Cordova and auth0.js
import Auth0Cordova from "@auth0/cordova";
import * as auth0 from "auth0-js";
import { Storage } from "@ionic/storage";
import { DataBaseProvider } from "../../providers/data-base/data-base";
import { Network } from "@ionic-native/network";
import { OrganizerHome } from '../organiser-home/organiser-home';
import { SignupPage } from '../signup/signup';
export const AUTH_CONFIG = {
  // Needed for Auth0 (capitalization: ID):
  clientID: "GaEgEcDQK4NJtHFutcFhXv3QGEWpr2EI",
  // Needed for Auth0Cordova (capitalization: Id):
  clientId: "GaEgEcDQK4NJtHFutcFhXv3QGEWpr2EI",
  domain: "dev-9lhjzdrf.auth0.com",
  packageIdentifier: "com.ticketsninvites.app", // config.xml widget ID, e.g., com.auth0.ionic
};

@Component({
  selector: "page-login",
  templateUrl: "login.html",
})
export class LoginPage {
  Auth0 = new auth0.WebAuth(AUTH_CONFIG);
  Client = new Auth0Cordova(AUTH_CONFIG);
  // Variable Declared in it
  myForm: any;
  userData: any;
  submitt: boolean = false;
  Toast: any;
  type: boolean = true;
  organizerChecked = true;
  scannerChecked=false;
  loginType:any;
  Type1:any;

  constructor(
    public navCtrl: NavController,
    public formBuilder: FormBuilder,
    public auth: AuthProvider,
    public platform: Platform,
    private alert: AlertController,
    private googlePlus: GooglePlus,
    private fb: Facebook,
    private storage: Storage,
    public zone: NgZone,
    private network: Network,
    private toastCtrl: ToastController,
    private database: DataBaseProvider,
    private globalprovider:GlobalFunctionProvider,

  ) {
    this.auth.clear();
    localStorage.setItem("Organizer","Organizer");
    this.loginType="Organizer";
    this.initializeForm();
  }

  ngOnInit(): void {
    //Called after the constructor, initializing input properties, and the first call to ngOnChanges.
    //Add 'implements OnInit' to the class.
  }

  organizerRadio($event){
    localStorage.setItem("Organizer","Organizer");
    this.loginType="Organizer";
    this.organizerChecked = !this.organizerChecked;
    console.log("loginType",this.loginType);
    this.scannerChecked=false;
   
  }
  scannerRadio($event){
    localStorage.setItem("Organizer","Event");
    this.loginType="Event";
    this.scannerChecked = !this.scannerChecked;
    console.log("loginTypescan",this.loginType);
    this.organizerChecked=false;
  }

  goToHome() {
    this.navCtrl.push(TabsPage);
  }

  // form(){
  //   if(this.loginType=="Organiser"){
  //     this.myForm = this.formBuilder.group({
  
  //       Type:new FormControl({value: 'Organizer', disabled: false}),
  //     });
  //   }
  //   else{
  //   this.myForm = this.formBuilder.group({
     
  //     Type:new FormControl({value: 'Event', disabled: false}),
  //   });
  // }
  // }
  // InitiallizeFrom validation and through form Builder
  private initializeForm() {
    //  Set From Validation
    this.myForm = this.formBuilder.group({
    password: new FormControl(
      "",
      Validators.compose([
        Validators.required,
        Validators.minLength(6),
        Validators.maxLength(15),
      ])
    ),
    email: new FormControl(
      "",
      Validators.compose([
        Validators.required,
        Validators.pattern("^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+.[a-zA-Z0-9-.]+$"),
      ])
    ),
    // Type:new FormControl(""),
  });
   
  }

  login(){
    if(this.loginType=="Organizer"){
      console.log("organizer login");
      this.loginAsOrganizer();
    }else{
      console.log("Event login");
      this.loginScanner();
    }

  }

     // Function for manual Login
   loginScanner() {
    let env = this;
   console.log("aaa",this.myForm.value.radioCheck=this.loginType)
  //  localStorage.setItem("Organizer","Event");
    // Check form validation
    if (this.myForm.valid) {
      // Coll do login in auth.ts function
      this.auth.showLoading();
      this.auth
        .doLogin(this.myForm.value)
        .then(
          res => {
            this.auth.dismissLoading();
            this.globalprovider.databaseLoaded=0;
            this.globalprovider.tableLoaded=0
            this.database.createDatabase();

            // env.navCtrl.setRoot(TabsPage, { Id: res["results"]["id"],type:res["results"]["type"] });
            env.navCtrl.setRoot(OrganizerHome, { Id: res["results"]["id"],type:res["results"]["type"] });
              //storage.set('name', 'Max');
              console.log("Login response",JSON.stringify(res["results"]));
              localStorage.setItem("LoginRes",JSON.stringify(res["results"]));

            },
          error => {
            this.auth.dismissLoading();
            if (error == "Login Email or Password are incorrect") {
              env.presentAlert(error);
            } else if (error == "Unable to login to your account") {
              env.presentAlert(error);
            }
            else {
              env.presentAlert("Login Email or Password are incorrect");
            }

            // Set Error In Error Log Table
            let date = new Date().toISOString().slice(0, 19).replace('T', ' ');
            let req = { 'err_text': JSON.stringify(error) + "Line No 160", 'file_path': 'login.ts', 'method': 'login', 'parent_method': "0", 'error_time': date, 'type': 'mobile' }
            this.auth.errorLog(req).subscribe();
          }
        )
        .catch(e => {
          this.auth.dismissLoading();
          // In case Of Exception
          let date = new Date().toISOString().slice(0, 19).replace('T', ' ');
          let req = { 'err_text': JSON.stringify(e) + "Line No 167", 'file_path': 'login.ts', 'method': 'login', 'parent_method': "0", 'error_time': date, 'type': 'mobile' }
          this.auth.errorLog(req).subscribe();
          env.presentAlert("Login Email or Password are incorrect");
        });
    }
    else {
      // when error on form show
      this.submitt = true;
    }
  }


  loginAsOrganizer() {
    let env = this;
    console.log("aaa00",this.myForm.value);
  //  localStorage.setItem("Organizer","Organizer");
   
  //  this.navCtrl.push(OrganizerHome);
    // Check form validation
    if (this.myForm.valid) {
      this.auth.showLoading();
      // Coll do login in auth.ts function
      this.auth
        .doLoginOrganizer(this.myForm.value)
        .then(
          res => {
            this.auth.dismissLoading();
            this.globalprovider.databaseLoaded=0;
            this.globalprovider.tableLoaded=0
            this.database.createDatabase();

            env.navCtrl.setRoot(OrganizerHome, { Id: res["results"]["id"],type:res["results"]["type"] });
              //storage.set('name', 'Max');
              console.log("Login response",JSON.stringify(res["results"]));
              localStorage.setItem("LoginRes",JSON.stringify(res["results"]));
            },
          error => {
            this.auth.dismissLoading();
            if (error == "Login Email or Password are incorrect") {
              env.presentAlert(error);
            } else if (error == "Unable to login to your account") {
              env.presentAlert(error);
            }
            else {
              env.presentAlert("Login Email or Password are incorrect");
            }

            // Set Error In Error Log Table
            let date = new Date().toISOString().slice(0, 19).replace('T', ' ');
            let req = { 'err_text': JSON.stringify(error) + "Line No 160", 'file_path': 'login.ts', 'method': 'login', 'parent_method': "0", 'error_time': date, 'type': 'mobile' }
            this.auth.errorLog(req).subscribe();
          }
        )
        .catch(e => {
          this.auth.dismissLoading();
          // In case Of Exception
          let date = new Date().toISOString().slice(0, 19).replace('T', ' ');
          let req = { 'err_text': JSON.stringify(e) + "Line No 167", 'file_path': 'login.ts', 'method': 'login', 'parent_method': "0", 'error_time': date, 'type': 'mobile' }
          this.auth.errorLog(req).subscribe();
          env.presentAlert("Login Email or Password are incorrect");
        });
    }
    else {
      // when error on form show
      this.submitt = true;
      console.log("Organizer",localStorage.getItem("Organizer"));
    }
  }

  signup(){
    this.navCtrl.push(SignupPage,);
  }

  // Alert any Error occured
  presentAlert(error: any) {
    let alert = this.alert.create({
      subTitle: error,
      buttons: ["Ok"],
      cssClass:"myalert1",
    });
    alert.present();
  }

  googleLogin() {
    this.googlePlus
      .login({})
      .then((res) => {
        this.userData = {
          email: res["email"],
          name: res["displayName"],
        };

        this.socailLogin(this.userData);
      })
      .catch((err) => console.log(err));
  }
  loginWithFB() {
    this.fb
      .login(["email", "public_profile"])
      .then((res: FacebookLoginResponse) => {
        console.log("Logged into Facebook!", res);
        if (res.status == "connected") {
          this.fb
            .api(
              "me?fields=id,name,email,first_name,picture.width(720).height(720).as(picture_large)",
              []
            )
            .then((profile) => {
              this.userData = {
                email: profile["email"],
                name: profile["name"],
              };

              console.log(profile);
              this.socailLogin(this.userData);
            })
            .catch((e) => {
              // Set Error Log when any Exception is occured
              let date = new Date()
                .toISOString()
                .slice(0, 19)
                .replace("T", " ");
              let req = {
                err_text: JSON.stringify(e) + "Line No 159",
                file_path: "login.ts",
                method: "loginWithFB",
                parent_method: "0",
                error_time: date,
                type: "mobile",
              };
              this.auth.errorLog(req).subscribe();
            });
        }
      })
      .catch((e) => console.log("Error logging into Facebook", e));
  }

  accessToken: any;
  InstaLogin() {
    console.log("instaEntry");
    //this.loading = true;
    const options = {
      scope: "openid profile offline_access",
    };

    this.Client.authorize(options, (err, authResult) => {
      if (err) {
        this.zone.run(() => console.log("error"));
        throw err;
      }
      // Set Access Token
      console.log(authResult.accessToken);
      //this.storage.set('access_token', authResult.accessToken);
      this.accessToken = authResult.accessToken;
      // Set Access Token expiration
      const expiresAt = JSON.stringify(
        authResult.expiresIn * 1000 + new Date().getTime()
      );
      console.log(expiresAt);

      //this.storage.set('expires_at', expiresAt);
      // Set logged in
      // this.loading = false;
      // this.loggedIn = true;
      // Fetch user's profile info
      this.Auth0.client.userInfo(this.accessToken, (err, profile) => {
        if (err) {
          throw err;
        }

        console.log(profile);
        this.storage
          .set("profile", profile)
          .then((val) => this.zone.run(() => console.log("data")));
        this.goToHome();
      });
    });
  }

  socailLogin(data: any) {
    let env = this;

    this.auth
      .doSocialLogin(data)
      .then(
        (res) => {
          console.log(res["results"]);
          env.navCtrl.setRoot(TabsPage, {
            Id: res["results"]["id"],
            type: res["results"]["type"],
          });
        },
        (error) => {
          if (error == "Login Email or Password are incorrect") {
            env.presentAlert(error);
          } else if (error == "Unable to login to your account") {
            env.presentAlert(error);
          } else {
            env.presentAlert("Something went wrong");
          }

          // Set Error In Error Log Table
          let date = new Date().toISOString().slice(0, 19).replace("T", " ");
          let req = {
            err_text: JSON.stringify(error) + "Line No 160",
            file_path: "login.ts",
            method: "login",
            parent_method: "0",
            error_time: date,
            type: "mobile",
          };
          this.auth.errorLog(req).subscribe();
        }
      )
      .catch((e) => {
        // In case Of Exception
        let date = new Date().toISOString().slice(0, 19).replace("T", " ");
        let req = {
          err_text: JSON.stringify(e) + "Line No 167",
          file_path: "login.ts",
          method: "login",
          parent_method: "0",
          error_time: date,
          type: "mobile",
        };
        this.auth.errorLog(req).subscribe();
        env.presentAlert("Something went wrong");
      });
  }

  insertDataafterlogin(user_id: any, type: any) {
    if (this.platform.is("android")) {
      console.log(this.network.type);
      if (this.network.type !== "none") {
        this.database.checktableExitDrop().then((data) => {
          if (data == null) {
            this.database.connectToDb();
            setTimeout(() => {
              let date = new Date();
              date = new Date("Wed, 08 Jan 2019 06:06:18 GMT");
              this.auth
                .GetDataList(user_id, type, date.toUTCString())
                .subscribe((x) => {
                  // debugger;
                  this.auth.setDateTime(date);
                  if (x.length > 0) {
                    this.database.addEventData(x).then((x) => {
                      if (x == true) {
                        this.presentToast("sync data successfully");
                      }
                    });
                  }
                });
            }, 4000);
          }
        });
      } else if (this.network.type === "none") {
        // this.presentToast('Please Check your network and try again');
      } else {
        // this.presentToast('Please Check your network and try again');
      }
    }

    // stop connect watch
    //connectSubscription.unsubscribe();
  }

  // Toast when Colled
  private presentToast(text) {
    let toast = this.toastCtrl.create({
      message: text,
      duration: 3000,
      position: "top",
    });
    toast.present();
  }
}

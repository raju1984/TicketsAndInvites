import { ScannerEventDetailsPage } from './../pages/scanner-event-details/scanner-event-details';
import { GlobalFunctionProvider } from './../providers/global-function/global-function';
import { Component, ViewChild } from "@angular/core";
import { Nav, Platform, ModalController, ToastController } from "ionic-angular";
import { StatusBar } from "@ionic-native/status-bar";
import { SplashScreen } from "@ionic-native/splash-screen";
import { LoginPage } from "../pages/login/login";
import { HomePage } from "../pages/home/home";
import { TabsPage } from "../pages/tabs/tabs";
import { Storage } from "@ionic/storage";
import { AuthProvider } from "../providers/auth/auth";
import Auth0Cordova from "@auth0/cordova";
import { SafariViewController } from "@ionic-native/safari-view-controller";
import { DataBaseProvider } from "../providers/data-base/data-base";
import { Network } from "@ionic-native/network";
import { OrganizerHome } from '../pages/organiser-home/organiser-home';
import { OrganizerEventDetails } from '../pages/organiser-event-details/organiser-event-details';
import { MenuPage } from '../pages/menu/menu';

export const AUTH_CONFIG = {
  // Needed for Auth0 (capitalization: ID):
  clientID: "GaEgEcDQK4NJtHFutcFhXv3QGEWpr2EI",
  // Needed for Auth0Cordova (capitalization: Id):
  clientId: "GaEgEcDQK4NJtHFutcFhXv3QGEWpr2EI",
  domain: "dev-9lhjzdrf.auth0.com",
  packageIdentifier: "com.ticketsninvites.app", // config.xml widget ID, e.g., com.auth0.ionic
};

@Component({
  templateUrl: "app.html",
})
export class MyApp {
  @ViewChild(Nav) nav: Nav;
  // Variable Declared in it
  rootPage: any;

  loc: any;

  pages: Array<{ title: string; component: any }>;
  loginRes:any;
  ParentuserName: string = "";
  EmailId:string="";
  name:any;
  email:any;
  
  constructor(
    public platform: Platform,
    public statusBar: StatusBar,
    public splashScreen: SplashScreen,
    private storage: Storage,
    public auth: AuthProvider,
    private modal: ModalController,
    private safariViewController: SafariViewController,
    private database: DataBaseProvider,
    private network: Network,
    private toastCtrl: ToastController,
    private globalProvider:GlobalFunctionProvider,

  ) {
    this.initializeApp();
    // if (localStorage.getItem("LoginRes")) {
    // this.loginRes=JSON.parse(localStorage.getItem("LoginRes"));
    // console.log("Home response app",this.loginRes);
    // console.log("Home EmailId",this.loginRes.EmailId);

    // this.name = this.loginRes.Name;
    // this.email = this.loginRes.EmailId;
    // }
    
    if(localStorage.getItem("Organizer")){
      console.log("localstorage if",localStorage.getItem("Organizer"));
    }else{
      console.log("localstorage else",localStorage.getItem("Organizer"));
    }
    this.loc=localStorage.getItem("Organizer");
    console.log("localstorage",this.loc);
    if(this.loc=="Organizer"){
   
    if(localStorage.getItem("LoginRes")){
    this.loginRes=JSON.parse(localStorage.getItem("LoginRes"));
    console.log("organizer login",this.loginRes);
    console.log("Home EmailId",this.loginRes.EmailId);

    this.name = this.loginRes.Name;
    this.email = this.loginRes.EmailId;
    this.rootPage = OrganizerHome; 
    this.pages = [
      { title: "Dashboard", component: OrganizerHome },
      { title: "Create Event", component: OrganizerHome },
      { title: "Create Ticket", component: OrganizerHome },
      { title: "Manage Event", component: OrganizerHome },
      { title: "Disable/Enable Events", component: OrganizerHome },
      // { title: "Guest Support", component: OrganizerEventDetails },
      { title: "Logout", component: LoginPage },
    ];
    }else{
      this.rootPage = LoginPage; 
    }
  }else{
    
    if(localStorage.getItem("LoginRes")){
      this.loginRes=JSON.parse(localStorage.getItem("LoginRes"));
      console.log("Home response app",this.loginRes);
      console.log("Home EmailId",this.loginRes.EmailId);
  
      this.name = this.loginRes.Name;
      this.email = this.loginRes.EmailId;
      this.rootPage = OrganizerHome; 
      this.pages = [
        { title: "Dashboard1", component: OrganizerHome },
        // { title: "Guest Support", component: OrganizerEventDetails },
        { title: "Logout", component: LoginPage },
      ];
      }else{
        this.rootPage = LoginPage; 
      }
  }

  // if(localStorage.getItem("LoginRes")){
  //         this.loginRes=JSON.parse(localStorage.getItem("LoginRes"));
  //         console.log("this.rootPage",this.loginRes);
  //         console.log("Home EmailId",this.loginRes.EmailId);
  //         this.rootPage = OrganizerHome; 
  //         }else{
  //           // console.log("Home elseee",this.loginRes.EmailId);
  //           this.rootPage = LoginPage; 
  //           this.pages =[];
  //         }
  
    // this.storage
    //   .get("guser")
    //   .then((rt) => {
    //     console.log("all data", this.auth.authUser['CompName']);
    //     // if(this.auth.authUser['CompName']){
    //     //   this.pages = [
    //     //     { title: "All Events", component: HomePage },
    //     //     { title: "Logout", component: LoginPage },
    //     //     // { title: "Organizer home", component: OrganizerHome },
    //     //   ];
    //     // }else{
    //     //   this.pages = [
    //     //     // { title: "All Events", component: HomePage },
    //     //     { title: "Logout", component: LoginPage },
    //     //     { title: "Organizer home", component: OrganizerHome },
    //     //   ];
    //     // }
    //     // this.loginRes=JSON.parse(localStorage.getItem("LoginRes"));
    //     this.auth.authUser = rt;
    //     this.ParentuserName = rt["name"];
    //     // this.ParentuserName=this.loginRes.Name;
    //     // this.EmailId = this.loginRes.EmailId;
    //     this.EmailId = rt["EmailId"];
    //     let user_id = rt["id"];
    //     let type = rt["type"];
    //     this.globalProvider.user_detail=rt;
    //     // console.log("all data ParentuserName", this.loginRes.Name);
    //     // alert(JSON.stringify(rt));

    //     if(!globalProvider.databaseLoaded){
    //       database.createDatabase();
    //       globalProvider.databaseLoaded=1;
    //      }

    //     // if (this.platform.is("android")) {
    //       // if ( this.network.type !== "none") {
    //         this.insertDataafterlogin(user_id, type);
    //     // }
    //     if(localStorage.getItem("LoginRes")){
    //       this.loginRes=JSON.parse(localStorage.getItem("LoginRes"));
    //       console.log("this.rootPage",this.loginRes);
    //       console.log("Home EmailId",this.loginRes.EmailId);
    //       this.rootPage = OrganizerHome; 
    //       }else{
    //         console.log("Home elseee andrr",);
    //         this.rootPage = LoginPage; 
    //       }
    //     // this.rootPage = OrganizerHome;     //TabsPage
    //   })
    //   .catch((e) => {
    //     this.rootPage = LoginPage;  //LoginPage
    //     this.database.connectToDb();
       
    //   });


    platform.ready().then(() => {
      statusBar.styleDefault();
      splashScreen.hide();
      (window as any).handleOpenURL = (url: string) => {
        Auth0Cordova.onRedirectUri(url);
      };
    });

    let disconnectSubscription = this.network.onDisconnect().subscribe(() => {
    });


    let connectSubscription = this.network.onConnect().subscribe(() => {
      setTimeout(() => {
      }, 3000);
    });


  }


  public refresh_data(e):void{
    // database.createDatabase();

    this.globalProvider.databaseLoaded=0;
    this.globalProvider.tableLoaded=0;
    this.database.loadTable('');
  }

  initializeApp() {
    this.platform.ready().then(() => {
      // Okay, so the platform is ready and our plugins are available.
      // Here you can do any higher level native things you might need.
      this.statusBar.styleDefault();
      this.splashScreen.hide();
    });
  }

  openPage(page) {
    // Reset the content nav to have just this page
    // we wouldn't want the back button to show in this scenario
    if (page.title == "All Events") {
      this.nav.setRoot(TabsPage);
    }else if(page.title == "Logout"){
      this.nav.setRoot(LoginPage);
      this.logout();
      // window.location.reload();
    }else if (page.title == "home") {
      console.log("home");
      this.nav.setRoot(OrganizerHome);
    }  else {
      this.nav.setRoot(page.component);
      // this.logout();Organisation Details
    }
  }

  logout() {
    this.storage.remove("profile");
    this.storage.remove("access_token");
    this.storage.remove("expires_at");
    this.globalProvider.databaseLoaded=0;
    this.globalProvider.tableLoaded=0;
    this.storage.clear().then(() => {
      this.auth.authUser = null;
    });
    // this.accessToken = null;
    // this.user = null;
    // this.loggedIn = false;

    // this.safariViewController.isAvailable().then((available: boolean) => {
    //   let url = `https://${AUTH_CONFIG.domain}/v2/logout?client_id=${AUTH_CONFIG.clientId}&returnTo=${AUTH_CONFIG.packageIdentifier}://${AUTH_CONFIG.domain}/cordova/${AUTH_CONFIG.packageIdentifier}/callback`;
    //   if (available) {
    //     this.safariViewController
    //       .show({
    //         url: url,
    //       })
    //       .subscribe(
    //         (result: any) => {
    //           if (result.event === "opened") console.log("Opened");
    //           else if (result.event === "loaded") console.log("Loaded");
    //           else if (result.event === "closed") console.log("Closed");
    //         },
    //         (error: any) => console.error(error)
    //       );
    //   } else {
    //     // use fallback browser
    //     //cordova.InAppBrowser.open(url, '_system');
    //   }
    // });
    localStorage.clear();
    location.reload();
    console.log("Logout local");
  }

  insertDataafterlogin(user_id: any, type: any) {
    if (this.platform.is("android")) {
      if (this.network.type !== "none") {
        this.database.connectToDb();
        setTimeout(() => {
          let date = new Date();
          date = new Date("Wed, 08 Jan 2019 06:06:18 GMT");
          this.auth
            .GetDataList(user_id, type, date.toUTCString())
            .subscribe((x) => {
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

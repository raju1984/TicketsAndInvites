import { Component, ViewChild } from '@angular/core';
import { IonicPage, Nav, NavController, NavParams } from 'ionic-angular';
import { OrganizerHome } from '../organiser-home/organiser-home';
import { LoginPage } from '../login/login';
import { TabsPage } from '../tabs/tabs';
import { AuthProvider } from '../../providers/auth/auth';
import { GlobalFunctionProvider } from '../../providers/global-function/global-function';

/**
 * Generated class for the MenuPage page.
 *
 * See https://ionicframework.com/docs/components/#navigation for more info on
 * Ionic pages and navigation.
 */

@IonicPage()
@Component({
  selector: 'page-menu',
  templateUrl: 'menu.html',
})
export class MenuPage {
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

  constructor(public navCtrl: NavController, public navParams: NavParams,
   
   ) {
  }

  ionViewDidLoad() {
    console.log('ionViewDidLoad MenuPage');
  }
  ionViewWillEnter(){
    // this.loc=localStorage.getItem("Organizer");
    // console.log("localstorage",this.loc);

    if(localStorage.getItem("LoginRes")){

    if(this.loc=="Organizer"){
      this.pages = [
        { title: "home", component: OrganizerHome },
        // { title: "Guest Support", component: OrganizerEventDetails },
        { title: "Logout", component: LoginPage },
      ];
    }else{
      this.pages = [
        // { title: "All Events", component: HomePage },
        { title: "home", component: OrganizerHome },
        // { title: "Guest Support", component: ScannerEventDetailsPage },
        { title: "Logout", component: LoginPage },
      ];
    
    }
  }else{
    this.rootPage = LoginPage; 
    
  }
   
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
    // this.storage.remove("profile");
    // this.storage.remove("access_token");
    // this.storage.remove("expires_at");
    // this.globalProvider.databaseLoaded=0;
    // this.globalProvider.tableLoaded=0;
    // this.storage.clear().then(() => {
    //   this.auth.authUser = null;
    // });
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
    console.log("Logout local");
  }


}

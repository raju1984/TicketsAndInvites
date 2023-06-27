import { Component, ViewChild } from '@angular/core';
import { Nav } from 'ionic-angular';
import { HomePage } from '../../pages/home/home';
import { LoginPage } from '../../pages/login/login';
import { OrganizerHome } from '../../pages/organiser-home/organiser-home';
import { TabsPage } from '../../pages/tabs/tabs';
import { DataBaseProvider } from '../../providers/data-base/data-base';
import { GlobalFunctionProvider } from '../../providers/global-function/global-function';

/**
 * Generated class for the SidebarComponent component.
 *
 * See https://angular.io/api/core/Component for more info on Angular
 * Components.
 */
@Component({
  selector: 'sidebar',
  templateUrl: 'sidebar.html'
})
export class SidebarComponent {
  @ViewChild(Nav) nav: Nav;
  text: string;
  pages: Array<{ title: string; component: any }>;

  constructor(private globalProvider:GlobalFunctionProvider, private database: DataBaseProvider,) {
    console.log('Hello SidebarComponent Component');
    this.text = 'Hello World';
    
    var loc=localStorage.getItem("Organizer");
    console.log("localstorage",loc);
    if(loc=="Organizer"){
    this.pages = [
      // { title: "All Events", component: HomePage },
      { title: "Logout", component: LoginPage },
      { title: "Organizer home", component: OrganizerHome },
    ];
  }else if(loc=="Event"){
    this.pages = [
      { title: "All Events", component: HomePage },
      { title: "Logout", component: LoginPage },
      // { title: "Organizer home", component: OrganizerHome },
    ];
  }

}
openPage(page) {
  // Reset the content nav to have just this page
  // we wouldn't want the back button to show in this scenario
  if (page.title == "All Events") {
    this.nav.setRoot(TabsPage);
  } else {
    this.nav.setRoot(page.component);
    // this.logout();
  }
}
public refresh_data(e):void{
  // database.createDatabase();

  this.globalProvider.databaseLoaded=0;
  this.globalProvider.tableLoaded=0;
  this.database.loadTable('');
}


}

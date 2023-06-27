import { TicketSalesPage } from './../pages/ticket-sales/ticket-sales';
import { AssignExistingUserPage } from './../pages/assign-existing-user/assign-existing-user';
import { TotalTicketAvailableTotalBoughtPage } from './../pages/total-ticket-available-total-bought/total-ticket-available-total-bought';
import { ScanUserListPage } from './../pages/scan-user-list/scan-user-list';
import { BrowserModule } from '@angular/platform-browser';
import { ErrorHandler, NgModule,CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { IonicApp, IonicErrorHandler, IonicModule, Menu } from 'ionic-angular';
import { MyApp } from './app.component';
import { HomePage } from '../pages/home/home';
import { StatusBar } from '@ionic-native/status-bar';
import { SplashScreen } from '@ionic-native/splash-screen';
import { LoginPage } from '../pages/login/login';
import { TicketsPage } from '../pages/tickets/tickets';
import { ScanTicketPage } from '../pages/scan-ticket/scan-ticket';
import { TabsPage } from '../pages/tabs/tabs';
import { AuthProvider } from '../providers/auth/auth';
import { HttpClientModule } from '@angular/common/http';
import { IonicStorageModule } from '@ionic/storage';
import { BarcodeScanner } from '@ionic-native/barcode-scanner';
// import { BarcodeScanner } from '@awesome-cordova-plugins/barcode-scanner/ngx';
import { ScannedTicketListPage } from '../pages/scanned-ticket-list/scanned-ticket-list';
import { GooglePlus } from '@ionic-native/google-plus';
import { Facebook } from '@ionic-native/facebook';
import { SafariViewController } from '@ionic-native/safari-view-controller';
import { SQLite } from '@ionic-native/sqlite';
import { Network } from '@ionic-native/network';
import { GlobalFunctionProvider } from '../providers/global-function/global-function';
import { DataBaseProvider } from '../providers/data-base/data-base';
import { OrganizerHome } from '../pages/organiser-home/organiser-home';
import { OrganizerEventDetails } from '../pages/organiser-event-details/organiser-event-details';
import { DoughnutChartComponent} from 'angular-d3-charts'; // this is needed!
import { MgxCircularProgressModule } from 'mgx-circular-progress-bar';
//or in alternative ViewAllCustomer
import { MgxCircularProgressBarModule, 
MgxCircularProgressFullBarModule, 
MgxCircularProgressPieModule } from'mgx-circular-progress-bar';
import { ViewAllCustomer } from '../pages/view-all-customer/view-all-customer';
import { CustomerListDetails } from '../pages/customer-list-details/customer-list-details';
import { AccessUserDashboard } from '../pages/access-user-dashboard/access-user-dashboard';
import { SidebarComponent } from '../components/sidebar/sidebar';
import { ScanUserListSummaryPage } from '../pages/scan-user-list-summary/scan-user-list-summary';
import { UssdCodeDetailsPage } from '../pages/ussd-code-details/ussd-code-details';
import { ScannerEventDetailsPage } from '../pages/scanner-event-details/scanner-event-details';
import { ModalpopupPageModule } from '../pages/modalpopup/modalpopup.module';
import { ModalpopupPage } from '../pages/modalpopup/modalpopup';
import { MyVisitorsPage } from '../pages/my-visitors/my-visitors';
import { WebChannelTicketListPage } from '../pages/web-channel-ticket-list/web-channel-ticket-list';
import { MenuPage } from '../pages/menu/menu';
import { SignupPage } from '../pages/signup/signup';



// import { AlphabeticalScrollBarModule } from 'alphabetical-scroll-bar';

@NgModule({
  declarations: [
    MyApp,
    HomePage,
    LoginPage,
    TicketsPage,
    ScanTicketPage,
    TabsPage,
    ScannedTicketListPage,
    OrganizerHome,
    OrganizerEventDetails,
    DoughnutChartComponent, 
    ViewAllCustomer,
    CustomerListDetails,
    AccessUserDashboard,
    ScanUserListPage,
    ScanUserListSummaryPage,
    TotalTicketAvailableTotalBoughtPage,
    AssignExistingUserPage,
    UssdCodeDetailsPage,
    ScannerEventDetailsPage,
    TicketSalesPage,
    SidebarComponent,
    MyVisitorsPage,
    WebChannelTicketListPage,
    MenuPage,
    SignupPage
  ],
  imports: [
    BrowserModule,
    IonicModule.forRoot(MyApp),
    HttpClientModule,
    IonicStorageModule.forRoot(),
    MgxCircularProgressModule,
    //or in alternative
    MgxCircularProgressBarModule,
    MgxCircularProgressFullBarModule,
    MgxCircularProgressPieModule,
    ModalpopupPageModule
    // AlphabeticalScrollBarModule 
  ],
  bootstrap: [IonicApp],
  entryComponents: [
    LoginPage,
    MyApp,
    HomePage,
    TicketsPage,
    ScanTicketPage,
    TabsPage,
    ScannedTicketListPage,
    OrganizerHome,
    OrganizerEventDetails,
    ViewAllCustomer,
    CustomerListDetails,
    AccessUserDashboard,
    ScanUserListPage,
    ScanUserListSummaryPage,
    TotalTicketAvailableTotalBoughtPage,
    AssignExistingUserPage,
    UssdCodeDetailsPage,
    ScannerEventDetailsPage,
    TicketSalesPage,
    ModalpopupPage,
    SidebarComponent,
    MyVisitorsPage,
    WebChannelTicketListPage,
    MenuPage,
    SignupPage
  ],
  providers: [
    GlobalFunctionProvider,
    StatusBar,
    SplashScreen,
    { provide: ErrorHandler, useClass: IonicErrorHandler },
    AuthProvider,
    BarcodeScanner,
    GooglePlus,
    Facebook,
    SafariViewController,
    SQLite,
    DataBaseProvider,
    Network,
    GlobalFunctionProvider,
    Storage,
  ],
  schemas:[CUSTOM_ELEMENTS_SCHEMA],
})
export class AppModule { }

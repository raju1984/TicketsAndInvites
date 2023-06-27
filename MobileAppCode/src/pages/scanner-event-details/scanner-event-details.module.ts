import { NgModule } from '@angular/core';
import { IonicPageModule } from 'ionic-angular';
import { ScannerEventDetailsPage } from './scanner-event-details';

@NgModule({
  declarations: [
    ScannerEventDetailsPage,
  ],
  imports: [
    IonicPageModule.forChild(ScannerEventDetailsPage),
  ],
})
export class ScannerEventDetailsPageModule {}

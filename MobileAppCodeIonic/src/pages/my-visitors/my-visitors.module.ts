import { NgModule } from '@angular/core';
import { IonicPageModule } from 'ionic-angular';
import { MyVisitorsPage } from './my-visitors';

@NgModule({
  declarations: [
    MyVisitorsPage,
  ],
  imports: [
    IonicPageModule.forChild(MyVisitorsPage),
  ],
})
export class MyVisitorsPageModule {}

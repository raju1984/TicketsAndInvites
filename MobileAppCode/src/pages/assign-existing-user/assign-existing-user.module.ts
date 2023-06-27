import { NgModule } from '@angular/core';
import { IonicPageModule } from 'ionic-angular';
import { AssignExistingUserPage } from './assign-existing-user';

@NgModule({
  declarations: [
    AssignExistingUserPage,
  ],
  imports: [
    IonicPageModule.forChild(AssignExistingUserPage),
  ],
})
export class AssignExistingUserPageModule {}

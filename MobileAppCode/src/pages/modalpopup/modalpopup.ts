import { Component } from '@angular/core';
import { IonicPage, NavController, NavParams } from 'ionic-angular';
import { ModalController } from 'ionic-angular';
/**
 * Generated class for the ModalpopupPage page.
 *
 * See https://ionicframework.com/docs/components/#navigation for more info on
 * Ionic pages and navigation.
 */

@IonicPage()
@Component({
  selector: 'page-modalpopup',
  templateUrl: 'modalpopup.html',
})
export class ModalpopupPage {

  constructor(public navCtrl: NavController, public navParams: NavParams,public modal:ModalController) {
  }

  ionViewDidLoad() {
    console.log('ionViewDidLoad ModalpopupPage');
  }
  CloseModel(){
    // this.modal.dismiss();
  }

}

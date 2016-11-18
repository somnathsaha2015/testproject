import { Component } from '@angular/core';
import { Subscription } from 'rxjs/subscription';
import { AppService } from '../../services/app.service';

@Component({
  templateUrl: 'app/components/order/order.component.html'
})
export class Order {
  email: string;
  staticTexts: {
    introText: string,
    holidayGift: string,
    minimumRequest: string,
    bottomNotes: string
  } = {
    introText: this.appService.getMessage('mess:order:intro:text'),
    holidayGift: this.appService.getMessage('mess:order:holiday:gift'),
    minimumRequest: this.appService.getMessage('mess:order:minimum:request'),
    bottomNotes: this.appService.getMessage('mess:order:bottom:notes')
  };
  currentOfferSubscription: Subscription;
  saveOrderSubscription:Subscription;
  orders: any[];
  constructor(private appService: AppService) {
    //this.staticTexts.introText = appService.getMessage('mess:order:intro:text');
    this.currentOfferSubscription = appService.filterOn('get:current:offer')
      .subscribe(d => {
        this.orders = JSON.parse(d.data).Table;
        console.log(d);
      });

    this.saveOrderSubscription = appService.filterOn('post:save:order') 
    .subscribe(d=>{
      console.log(d);
    });
  };
  request() {
    //console.log(this.orders);
    let finalOrder = this.orders.map(function (value,i) {
        return({OfferId:value.Id,OrderQty:value.OrderQty, WishList:value.WishList})      
    });
    let token = this.appService.getToken();
    this.appService.httpPost('post:save:order', {token:token, order: finalOrder });
   };
  ngOnInit() {
    let token = this.appService.getToken();
    this.appService.httpGet('get:current:offer', { token: token });
  };
  ngOnDestroy() {
    this.currentOfferSubscription.unsubscribe();
    this.saveOrderSubscription.unsubscribe();
  }
}
import { Component } from '@angular/core';
import { Subscription } from 'rxjs/subscription';
import { AppService } from '../../services/app.service';

@Component({
    templateUrl: 'app/components/orderHistory/orderHistory.component.html'
})
export class OrderHistory {
   
    subscription: Subscription;
    orderHeaders: [{}];
    constructor(private appService: AppService) {
        this.subscription = appService.filterOn('get:order:headers')
      .subscribe(d => {
        this.orderHeaders = JSON.parse(d.data).Table;
        console.log(d);
      });
    };
    showDetails(id) {
        console.log(id);
    };
    ngOnInit(){
        let token = this.appService.getToken();
        this.appService.httpGet('get:order:headers',{token:token})
    };
    ngOnDestroy() {
        this.subscription.unsubscribe();
    };
}
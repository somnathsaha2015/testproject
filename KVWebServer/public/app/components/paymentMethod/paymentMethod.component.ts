import { Component } from '@angular/core';
import { Subscription } from 'rxjs/subscription';
import { AppService } from '../../services/app.service';

@Component({
    templateUrl: 'app/components/paymentMethod/paymentMethod.component.html'
})
export class PaymentMethod {   
    getSubscription: Subscription;
    postSubscription: Subscription;
    deleteSubscription: Subscription;
    //isNewCard:boolean=false;
    cards:[any];
    constructor(private appService: AppService) {
        this.getSubscription = appService.filterOn("get:credit:card")
            .subscribe(d => {
                if (d.data.error) {
                    console.log(d);
                } else {
                    this.cards = JSON.parse(d.data).Table;
                }                
            });
        this.postSubscription = appService.filterOn("insert:credit:card")
            .subscribe(d => {
                if (d.data.error) {
                    console.log(d);
                }
                else {
                    this.cards[0].id = d.data.result.id;
                    d.body.card.isNew = false;
                }
            });
        this.deleteSubscription = appService.filterOn("delete:credit:card")
            .subscribe(d => {
                if (d.data.error) {
                    console.log("Error occured");
                } else {
                    this.cards.splice(d.index, 1);
                }
            });
    };
    addCard(){
        let card = {cardName:'',cardNumber:'',isNew:false};
        card.isNew=true;
        this.cards.unshift(card);
        
    };
    removeNew(index){
        this.cards.splice(index,1);
    };
    remove(card,index){
        //let token = this.appService.getToken();
        this.appService.httpDelete('delete:credit:card',{id:card.id,index:index});
    };
    save(card){
        this.appService.httpPost('insert:credit:card',{card:card});
    };
    ngOnInit(){
        let token = this.appService.getToken();
        this.appService.httpGet('get:credit:card', { token: token });
    }
    ngOnDestroy() {
        this.getSubscription.unsubscribe();
        this.postSubscription.unsubscribe();
    };
}